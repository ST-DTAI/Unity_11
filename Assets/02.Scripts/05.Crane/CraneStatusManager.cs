using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEditor;
using UnityEngine;

public class CraneStatusManager : MonoBehaviour
{
    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    [SerializeField]
    private List<CrStatus> craneStatusList = new List<CrStatus>();

    public float updateInterval = 0.5f; // 데이터 갱신 주기(0.5초)

    public GameObject craneRootObject1;
    public GameObject craneRootObject2;
    public GameObject hoistRootObject1;
    public GameObject hoistRootObject2;
    public GameObject liftRootObject1;
    public GameObject liftRootObject2;

    public GameObject coilObject;

    public Transform newParentTransform1;
    public Transform newParentTransform2;

    // 목표 위치를 저장할 Vector3 변수 선언
    private Vector3 targetPositionCrane1;
    private Vector3 targetPositionHoist1;
    private Vector3 targetPositionLift1;

    private Vector3 targetPositionCrane2;
    private Vector3 targetPositionHoist2;
    private Vector3 targetPositionLift2;

    public float moveSpeed = 1.0f;

   
    

    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();
        if (connection != null)
        {
            //dataReader = new DatabaseReader(connection);
            StartCoroutine(UpdateCraneCoroutine());
        }

        targetPositionCrane1 = craneRootObject1.transform.position;
        targetPositionHoist1 = hoistRootObject1.transform.position;
        targetPositionLift1 = liftRootObject1.transform.position;

        targetPositionCrane2 = craneRootObject2.transform.position;
        targetPositionHoist2 = hoistRootObject2.transform.position;
        targetPositionLift2 = liftRootObject2.transform.position;
    }

    private void Update()
    {
        // MoveTowards를 사용하여 오브젝트를 목표 위치로 부드럽게 이동
        
        hoistRootObject1.transform.position = Vector3.MoveTowards(hoistRootObject1.transform.position, targetPositionHoist1, Time.deltaTime * 0.32f);
        craneRootObject1.transform.position = Vector3.MoveTowards(craneRootObject1.transform.position, targetPositionCrane1, Time.deltaTime * 1.0f);
        liftRootObject1.transform.position = Vector3.MoveTowards(liftRootObject1.transform.position, targetPositionLift1, Time.deltaTime * 0.1f);

        hoistRootObject2.transform.position = Vector3.MoveTowards(hoistRootObject2.transform.position, targetPositionHoist2, Time.deltaTime * 0.32f);
        craneRootObject2.transform.position = Vector3.MoveTowards(craneRootObject2.transform.position, targetPositionCrane2, Time.deltaTime * 1.0f);
        liftRootObject2.transform.position = Vector3.MoveTowards(liftRootObject2.transform.position, targetPositionLift2, Time.deltaTime * 0.10f);
    }

    void OnDestroy()
    {
        dbConnection.CloseConnection();
    }

    private IEnumerator UpdateCraneCoroutine()
    {
        while (true)
        {
            string query = "SELECT * FROM cr_status ORDER BY CrNo;";
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

            // 각 결과 문자열에 대한 파싱된 데이터를 처리합니다.
            FetchCraneData(craneStatusList);
            Debug.Log("데이터 처리 완료 및 크레인 업데이트");

            yield return new WaitForSeconds(updateInterval); // updateInterval 초마다 데이터 갱신
        }
    }




    private void FetchCraneData(List<CrStatus> craneStatusList)
    {
        Debug.Log("가져온 크레인 데이터 개수: " + craneStatusList.Count);

        if (craneStatusList.Count > 0)
        {
            CrStatus crane = craneStatusList[0];
            targetPositionCrane1 = new Vector3(crane.Dx * 0.001f, craneRootObject1.transform.position.y, craneRootObject1.transform.position.z);
            targetPositionHoist1 = new Vector3(craneRootObject1.transform.position.x, hoistRootObject1.transform.position.y, crane.Dy * 0.001f);
            targetPositionLift1 = new Vector3(craneRootObject1.transform.position.x, crane.Dz * 0.001f, hoistRootObject1.transform.position.z);

            liftRootObject1.transform.rotation = Quaternion.Euler(0, crane.SwivAng, 0);
            UpdateCoilObjectStatus(coilObject, crane.CrNo, crane.SwivAng, newParentTransform1, newParentTransform2, crane.Status);

            Debug.Log($"Updated Crane01 position to x: {crane.Dx}, y:{crane.Dy} z: {crane.Dz} angle :{crane.SwivAng}");
        }

        if (craneStatusList.Count > 1)
        {
            CrStatus crane = craneStatusList[1];
            targetPositionCrane2 = new Vector3(crane.Dx * 0.001f, craneRootObject2.transform.position.y, craneRootObject2.transform.position.z);
            targetPositionHoist2 = new Vector3(craneRootObject2.transform.position.x, hoistRootObject2.transform.position.y, crane.Dy * 0.001f);
            targetPositionLift2 = new Vector3(craneRootObject2.transform.position.x, crane.Dz * 0.001f, hoistRootObject2.transform.position.z);

            liftRootObject2.transform.rotation = Quaternion.Euler(0, crane.SwivAng, 0);
            UpdateCoilObjectStatus(coilObject, crane.CrNo, crane.SwivAng, newParentTransform1, newParentTransform2, crane.Status);


            Debug.Log($"Updated Crane02 position to x: {crane.Dx}, y:{crane.Dy} z: {crane.Dz} angle :{crane.SwivAng}");
        }

    }

    private void UpdateCoilObjectStatus(GameObject coilObject, int CrNo, int swivAng, Transform newParentTransform1, Transform newParentTransform2, string status)
    {
        if (status.Length == 10)
        {
            if (status[9] == '1')
            {
                AttachCoilObject(coilObject, CrNo, swivAng, newParentTransform1, newParentTransform2);
            }
        }
    }

    private void AttachCoilObject(GameObject coilObject, int CrNo,int swivAng, Transform newParentTransform1, Transform newParentTransform2)
    {
        Transform selectedParentTransform;

        // CrNo 값에 따라 newParentTransform 선택
        if (CrNo == 1)
        {
            selectedParentTransform = newParentTransform1;
        }
        else if (CrNo == 2)
        {
            selectedParentTransform = newParentTransform2;
        }
        else
        {
            Debug.LogError("Invalid CrNo value: " + CrNo);
            return; 
        }


        // 프리팹 인스턴스 생성
        GameObject coilObjectInstance = Instantiate(coilObject);
        if (PrefabUtility.GetPrefabAssetType(coilObject) != PrefabAssetType.NotAPrefab)
        {
            coilObjectInstance = Instantiate(coilObject);
        }
        else
        {
            coilObjectInstance = coilObject;
        }

        // coilObjectInstance를 선택된 newParentTransform에 붙이기
        coilObjectInstance.transform.SetParent(selectedParentTransform, false);
        coilObjectInstance.transform.localPosition = Vector3.zero; // 필요한 위치로 이동
        coilObjectInstance.SetActive(true);

        // 크기와 각도를 조절
        coilObjectInstance.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        coilObjectInstance.transform.localRotation = Quaternion.Euler(0.0f, swivAng, 0.0f);


        Debug.Log("Coil Object Attached to " + selectedParentTransform.name);
    }

}
