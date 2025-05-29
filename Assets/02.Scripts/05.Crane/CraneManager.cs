using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class CraneManager : MonoBehaviour
{
    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    [SerializeField]
    private List<CrStatus> craneStatusList = new List<CrStatus>();

    public float updateInterval = 0.5f; // ������ ���� �ֱ�(0.5��)

    public GameObject craneRootObject;
    public GameObject hoistRootObject;
    public GameObject liftRootObject;

    public GameObject coilObject;

    public GameObject liftArm_L;
    public GameObject liftArm_R;

    public Transform newParentTransform;


    // ��ǥ ��ġ�� ������ Vector3 ���� ����
    private Vector3 targetPositionCrane;
    private Vector3 targetPositionHoist;
    private Vector3 targetPositionLift;

    public float moveSpeed = 1.0f;

    private bool isCoilAttached = false;

    /// <summary>
    /// ���⼭ ũ���� ��ȣ ����
    /// </summary>
    public int crNo = 13;
    

    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();
        if (connection != null)
        {
            // �ʱ� ũ���� ��ġ ����
            InitializeCranePosition();
            StartCoroutine(UpdateCraneCoroutine());
        }

        targetPositionCrane = craneRootObject.transform.position;
        targetPositionHoist = hoistRootObject.transform.position;
        targetPositionLift = liftRootObject.transform.position;

    }

    //private void InitializeCranePosition()
    //{
    //    string query = $"SELECT * FROM cr_status WHERE CrNo = {crNo};";

    //    if (connection != null)
    //    {
    //        using (MySqlCommand cmd = new MySqlCommand(query, connection))
    //        {
    //            using (MySqlDataReader reader = cmd.ExecuteReader())
    //            {
    //                if (reader.Read())
    //                {
    //                    // �����ͺ��̽����� dx, dy, dz ���� ������ ũ���� ��ġ�� ����
    //                    float dx = reader.GetFloat("dx") * 0.001f;
    //                    float dy = reader.GetFloat("dy") * 0.001f;
    //                    float dz = reader.GetFloat("dz") * 0.001f;


    //                    if(crNo.ToString().StartsWith("1"))
    //                    {
    //                        craneRootObject.transform.position = new Vector3(dx-5f, craneRootObject.transform.position.y, craneRootObject.transform.position.z);
    //                        hoistRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, hoistRootObject.transform.position.y, dy);
    //                        liftRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, dz, hoistRootObject.transform.position.z);
    //                    }

    //                    // CrNo�� 2�� �����ϴ� ��� ����Ƽ z ����(DB y��) ���� +40�� �߰�
    //                    if (crNo.ToString().StartsWith("2"))
    //                    {
    //                        craneRootObject.transform.position = new Vector3(dx-5f, craneRootObject.transform.position.y, craneRootObject.transform.position.z);
    //                        hoistRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, hoistRootObject.transform.position.y, 40f +4f);
    //                        liftRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, dz, hoistRootObject.transform.position.z);
    //                    }
    //                    //Debug.Log($"�ʱ� ũ���� ��ġ ����: x: {dx}, y: {dz}, z: {dy}");
    //                }
    //                else
    //                {
    //                    Debug.LogError("ũ���� �����Ͱ� �����ϴ�.");
    //                }
    //            }
    //        }
    //    }
    //}
    private void InitializeCranePosition()
    {
        string query = $"SELECT * FROM cr_status WHERE CrNo = {crNo};";

        if (connection != null)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        float dx = reader.GetFloat("dx") * 0.001f;
                        float dy = reader.GetFloat("dy") * 0.001f;
                        float dz = reader.GetFloat("dz") * 0.001f;

                        string crPrefix = crNo.ToString().Substring(0, 1);

                        float zOffset = 0f;
                        bool shouldPlace = true;

                        switch (crPrefix)
                        {
                            case "1":
                                zOffset = 0f;
                                break;
                            case "2":
                                zOffset = 40f;
                                break;
                            case "3":
                                zOffset = 80f;
                                break;
                            default:
                                shouldPlace = false;
                                Debug.LogWarning($"[CrNo={crNo}] : 1, 2, 3���� �������� �����Ƿ� ��ġ���� ����.");
                                break;
                        }

                        if (shouldPlace)
                        {
                            // ������ DB�� �����Ѵٴ� ���� reader.Read()�� Ȯ�ε����Ƿ� ��ġ ����
                            craneRootObject.transform.position = new Vector3(dx - 5f, craneRootObject.transform.position.y, craneRootObject.transform.position.z);
                            hoistRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, hoistRootObject.transform.position.y, dy + zOffset);
                            liftRootObject.transform.position = new Vector3(craneRootObject.transform.position.x, dz, hoistRootObject.transform.position.z);

                            Debug.Log($"[CrNo={crNo}] ��ġ �Ϸ�. Z ������: {zOffset}");
                        }
                    }
                    else
                    {
                        // DB�� �������� ������ � CrNo�� ��ġ �� ��
                        Debug.LogWarning($"[CrNo={crNo}] : �����ͺ��̽��� �������� �����Ƿ� ��ġ���� ����.");
                    }
                }
            }
        }
    }

    private void Update()
    {
        // MoveTowards�� ����Ͽ� ������Ʈ�� ��ǥ ��ġ�� �ε巴�� �̵�

        hoistRootObject.transform.position = Vector3.MoveTowards(hoistRootObject.transform.position, targetPositionHoist, Time.deltaTime * 0.32f);
        craneRootObject.transform.position = Vector3.MoveTowards(craneRootObject.transform.position, targetPositionCrane, Time.deltaTime * 1.0f);
        liftRootObject.transform.position = Vector3.MoveTowards(liftRootObject.transform.position, targetPositionLift, Time.deltaTime * 0.1f);

       
    }

    void OnDestroy()
    {
        dbConnection.CloseConnection();
    }

    private IEnumerator UpdateCraneCoroutine()
    {
        while (true)
        {
            string query = $"SELECT * FROM cr_status WHERE CrNo = {crNo};";
            craneStatusList.Clear();

            if (connection != null)
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
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

                            craneStatusList.Add(crStatus);
                        }
                    }
                }
            }

            // �� ��� ���ڿ��� ���� �Ľ̵� �����͸� ó���մϴ�.
            FetchCraneData(craneStatusList);
            //Debug.Log("������ ó�� �Ϸ� �� ũ���� ������Ʈ");

            yield return new WaitForSeconds(updateInterval); // updateInterval �ʸ��� ������ ����
        }
    }




    private void FetchCraneData(List<CrStatus> craneStatusList)
    {
        //Debug.Log("������ ũ���� ������ ����: " + craneStatusList.Count);

        if (craneStatusList.Count > 0)
        {
            CrStatus crane = craneStatusList[0];
            // CrNo�� 2�� �����ϴ� ��� z �������� +20�� ����
            float offsetZ = (crane.CrNo.ToString().StartsWith("2")) ? 40f : 0f;

            targetPositionCrane = new Vector3(crane.Dx * 0.001f, craneRootObject.transform.position.y, craneRootObject.transform.position.z);
            targetPositionHoist = new Vector3(craneRootObject.transform.position.x, hoistRootObject.transform.position.y, crane.Dy * 0.001f + offsetZ);
            targetPositionLift = new Vector3(craneRootObject.transform.position.x, crane.Dz * 0.001f+1.3f, hoistRootObject.transform.position.z);

            liftRootObject.transform.rotation = Quaternion.Euler(0, crane.SwivAng, 0);
            UpdateCoilObjectStatus(crane.Status, crane.PdNo, crane.SwivAng);

            UpdateLiftArmWidth(crane.ArmWid);
            //Debug.Log($"Updated Crane{crane.CrNo} position to x: {crane.Dx}, y: {crane.Dy}, z: {crane.Dz}, angle: {crane.SwivAng}");

        }

        }


    //���� ����� �� ���ѻ���
    //���� ����� �� ȣ�̽�Ʈ ����


    private void UpdateCoilObjectStatus(string status, string pdNo, int swivAng)
    {
        if (pdNo != "0" && !isCoilAttached)
        {
            AttachCoilObject(swivAng);
            isCoilAttached = true;
        }
        else if (pdNo == "0" && isCoilAttached)
        {
            // ������ ������ ��� �ʱ�ȭ
            isCoilAttached = false;
        }
    }



    private void AttachCoilObject(int swivAng)
    {
        GameObject coilObjectInstance;

        if (PrefabUtility.GetPrefabAssetType(coilObject) != PrefabAssetType.NotAPrefab)
        {
            coilObjectInstance = Instantiate(coilObject);
        }
        else
        {
            coilObjectInstance = coilObject;
        }

        coilObjectInstance.transform.SetParent(newParentTransform, false);
        coilObjectInstance.transform.localPosition = Vector3.zero;
        coilObjectInstance.SetActive(true);
        coilObjectInstance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        coilObjectInstance.transform.localRotation = Quaternion.Euler(0.0f, swivAng, 0.0f);

        //Debug.Log("Coil Object Attached to " + newParentTransform.name);
    }

    private void UpdateLiftArmWidth(int armWid)
    {
        float armWidth = armWid * 0.0001f; 
        float halfWidth = armWidth * 0.5f;

        liftArm_L.transform.localPosition = new Vector3(-halfWidth, liftArm_L.transform.localPosition.y, liftArm_L.transform.localPosition.z);
        liftArm_R.transform.localPosition = new Vector3(halfWidth, liftArm_R.transform.localPosition.y, liftArm_R.transform.localPosition.z);

        //Debug.Log($"Lift Arms updated: L({-halfWidth}), R({halfWidth})");
    }

}
