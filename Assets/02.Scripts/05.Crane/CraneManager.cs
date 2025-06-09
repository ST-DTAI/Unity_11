using MySql.Data.MySqlClient;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using TMPro;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CraneManager : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> craneObject = new List<GameObject>();

    public GameObject cranePrefab;

    public float updateInterval = 0.5f; // 데이터 갱신 주기(0.5초)

    private Thread thread_crane;
    private volatile bool isRunning = true;

    void Start()
    {
        // 초기 크레인 위치 설정
        InitializeCranePosition();

        ThreadStart();
        //StartCoroutine(UpdateCraneCoroutine());

        //targetPositionCrane = craneRootObject.transform.position;
        //targetPositionHoist = hoistRootObject.transform.position;
        //targetPositionLift = liftRootObject.transform.position;
    }
    private void OnDestroy()
    {
        if (isRunning)
        {
            isRunning = false;

            if (thread_crane != null)
            {
                thread_crane.Join(5000); // 기다렸다가
                thread_crane = null;
                Debug.Log("==CraneManager destroyed and thread stopped==");
            }
        }
    }
    private void InitializeCranePosition()
    {
        string query = "SELECT a.CrNo, a.DxOffset, a.MinDx, a.MaxDx, a.DyOffset, a.MinDy, a.MaxDy " +
            "FROM cr_init a INNER JOIN cr_status b ON a.CrNo = b.CrNo ORDER BY CrNo;";

        using (MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection.Instance.Connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int crNo  = reader.GetInt32("CrNo");
                    int dxOffset = reader.GetInt32("DxOffset");
                    int minDx = reader.GetInt32("MinDx");
                    int maxDx = reader.GetInt32("MaxDx");
                    int dyOffset = reader.GetInt32("DyOffset");
                    int minDy = reader.GetInt32("MinDy");
                    int maxDy = reader.GetInt32("MaxDy");

                    GameObject crane = Instantiate(cranePrefab);
                    ApplyCraneScale(crane);

                    crane.GetComponent<Crane>().crNo = crNo;
                    crane.GetComponent<Crane>().dxOffset = dxOffset ;
                    crane.GetComponent<Crane>().minDx = minDx ;
                    crane.GetComponent<Crane>().maxDx = maxDx ;
                    crane.GetComponent<Crane>().dyOffset = dyOffset ;
                    crane.GetComponent<Crane>().minDy = minDy ;
                    crane.GetComponent<Crane>().maxDy = maxDy;
                    crane.name = "Crane" + crNo;

                    crane.transform.SetParent(transform);
                    craneObject.Add(crane);
                }

            }
        }
    }
    void ThreadStart()
    {
        if (thread_crane != null)
        {
            isRunning = false;
            thread_crane.Join(); // 기다렸다가
            thread_crane = null;
            return;
        }

        isRunning = true;
        thread_crane = new Thread(CraneManager_Thread);
        thread_crane.IsBackground = true;
        thread_crane.Start();
    }
    private void CraneManager_Thread()
    {
        while (true)
        {
            try
            {
                string query = $"SELECT CrNo, `Status`, Locus, GoalDx, GoalDy, GoalDz, Addr, PdNo, " +
                           $"Dx, Dy, Dz, SwivAng, ArmWid, LdWeight, TLSway, TSSway, Temp, ErrCode, Input, Output, " +
                           $"ComChk, CycleTime FROM cr_status ORDER BY CrNo;";
                int crIdx = 0;
                using (MySqlConnection connection = new MySqlConnection(DatabaseConnection.Instance.ConnStr))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read() && isRunning)
                            {
                                CrStatus crStatus = new CrStatus(
                                    reader.GetInt32("crNo"),
                                    reader.GetString("status"),
                                    reader.GetInt32("locus"),
                                    reader.GetFloat("goaldx"),
                                    reader.GetFloat("goaldy"),
                                    reader.GetFloat("goaldz"),
                                    reader.GetString("addr"),
                                    reader.GetString("pdNo"),
                                    reader.GetFloat("dx"),
                                    reader.GetFloat("dy"),
                                    reader.GetFloat("dz"),
                                    reader.GetInt32("swivAng"),
                                    reader.GetInt32("armWid"),
                                    reader.GetInt32("ldWeight"),
                                    reader.GetInt32("tlsway"),
                                    reader.GetInt32("tssway"),
                                    reader.GetInt32("temp"),
                                    reader.GetInt32("errCode"),
                                    reader.GetInt32("input"),
                                    reader.GetInt32("output"),
                                    reader.GetInt32("comChk"),
                                    reader.GetInt32("cycleTime")
                                );

                                if (UnityMainThreadDispatcher.Instance() != null)
                                {
                                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                                    {
                                        craneObject[crIdx].GetComponent<Crane>().crStatus = crStatus;
                                        craneObject[crIdx].GetComponent<Crane>().FetchCraneData();
                                        crIdx++;
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (isRunning)
                    Debug.LogError("CraneManager_Thread Error: " + ex.Message);
            }
        }
    }

    //private IEnumerator UpdateCraneCoroutine()
    //{
    //    while (true)
    //    {
    //        string query = $"SELECT CrNo, `Status`, Locus, GoalDx, GoalDy, GoalDz, Addr, PdNo, " +
    //                       $"Dx, Dy, Dz, SwivAng, ArmWid, LdWeight, TLSway, TSSway, Temp, ErrCode, Input, Output, " +
    //                       $"ComChk, CycleTime FROM cr_status ORDER BY CrNo;";

    //        int crIdx = 0;
    //        if (connection != null)
    //        {
    //            using (MySqlCommand cmd = new MySqlCommand(query, connection))
    //            {
    //                using (MySqlDataReader reader = cmd.ExecuteReader())
    //                {
    //                    while (reader.Read())
    //                    {
    //                        CrStatus crStatus = new CrStatus(
    //                            reader.GetInt32("crNo"),
    //                            reader.GetString("status"),
    //                            reader.GetInt32("locus"),
    //                            reader.GetFloat("goaldx"),
    //                            reader.GetFloat("goaldy"),
    //                            reader.GetFloat("goaldz"),
    //                            reader.GetString("addr"),
    //                            reader.GetString("pdNo"),
    //                            reader.GetFloat("dx"),
    //                            reader.GetFloat("dy"),
    //                            reader.GetFloat("dz"),
    //                            reader.GetInt32("swivAng"),
    //                            reader.GetInt32("armWid"),
    //                            reader.GetInt32("ldWeight"),
    //                            reader.GetInt32("tlsway"),
    //                            reader.GetInt32("tssway"),
    //                            reader.GetInt32("temp"),
    //                            reader.GetInt32("errCode"),
    //                            reader.GetInt32("input"),
    //                            reader.GetInt32("output"),
    //                            reader.GetInt32("comChk"),
    //                            reader.GetInt32("cycleTime")
    //                        );


    //                        craneObject[crIdx].GetComponent<Crane>().crStatus = crStatus;
    //                        craneObject[crIdx].GetComponent<Crane>().FetchCraneData();
    //                        crIdx++;
    //                    }
    //                }
    //            }
    //        }

    //        yield return new WaitForSeconds(updateInterval); // updateInterval 초마다 데이터 갱신
    //    }
    //}

    private void ApplyCraneScale(GameObject crane)
    {
        // YardSetUpManager에서 mainColumnSpacing 가져오기
        YardSetUpManager yardManager = FindObjectOfType<YardSetUpManager>();
        if (yardManager == null)
        {
            Debug.LogWarning("YardSetUpManager가 씬에 존재하지 않습니다.");
            return;
        }

        float spacing = yardManager.mainColumnSpacing;
        float craneScale= 1f * spacing /32f;
        // spacing을 크레인 Z축 스케일에 반영 (예시)
        crane.transform.localScale = new Vector3(craneScale, 1f, craneScale);


    }
}
