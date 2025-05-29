using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Sse4_2;

public class YardMapDBManager : MonoBehaviour
{

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    public GameObject SkidObject;
    public GameObject CoilObject;

    private UiManager uiManager;
    public Material outlineMaterial;

    public TextMeshProUGUI dbConnectText;

    [SerializeField]
    private List<YardMap> skidsList = new List<YardMap>();
    public List<YardMap> SkidsList => skidsList;

    [SerializeField]
    private List<YardMap> coilsList = new List<YardMap>();

    public List<YardMap> CoilsList => coilsList; // coilsList에 접근할 수 있는 프로퍼티

    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        dbConnectText.color = Color.green; //db통신 성공시 텍스트 색상 변경

        if (connection != null)
        {
            ReadSkidData(true); // 시작할 때 데이터를 한 번만 가져오고 처리
        }
    }

    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.CloseConnection();
        }
    }

    private void ReadSkidData(bool initialLoad) //1초에 한번씩 읽기***코루틴
    {
        string query = "SELECT SkidNo, Dong, Skid, Sect, DxNo, DyNo, DzNo, Addr, Dx, Dy, Dz, Dir, MaxWid, MaxDia, PdYN, Hold, CrRev, SupRev, OutRev, FwdYN, BwdYN, PdNo, State, Width, Outdia, India, Thick, Weight, Temp, Date, ToNo FROM clts.yard_map;";
        skidsList.Clear();
        coilsList.Clear();

        bool isPdYNChanged = initialLoad; // PdYN 변경 여부 확인 변수

        if (connection != null)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        //string pdNo = reader["pdNo"] != DBNull.Value ? reader.GetString("pdNo") : "0";


                        YardMap skid = new YardMap(
                          reader.GetInt32("skidNo"),
                          reader.GetInt32("dong"),
                          reader.GetString("skid"),
                          reader.GetInt32("sect"),
                          reader.GetInt32("dxNo"),
                          reader.GetInt32("dyNo"),
                          reader.GetInt32("dzNo"),
                          reader.GetString("addr"),

                          reader.GetFloat("dx"),
                          reader.GetFloat("dy"),
                          reader.GetFloat("dz"),
                          reader.GetFloat("dir"),
                          reader.GetInt32("maxWid"),
                          reader.GetInt32("maxDia"),
                          reader.GetString("pdYN"),
                          reader.GetString("hold"),
                          reader.GetInt32("crRev"),
                          reader.GetInt32("supRev"),
                          reader.GetInt32("outRev"),
                          reader.GetString("fwdYN"),
                          reader.GetString("bwdYN"),

                          reader.GetString("pdNo"),
                          reader.GetString("state"),
                          reader.GetInt32("width"),
                          reader.GetInt32("outdia"),
                          reader.GetInt32("india"),
                          reader.GetFloat("thick"),
                          reader.GetInt32("weight"),
                          reader.GetInt32("temp"),
                          reader.GetString("date"),
                          reader.GetString("toNo")
                        );
                        //Debug.Log(reader["crRev"]);

                        // DzNo 값이 2가 아닌 경우만 skidsList에 추가
                        if (skid.DzNo != 2)
                        {
                            skidsList.Add(skid);
                        }

                        // PdYN이 1이면 coilsList에 추가
                        if (skid.PdYN == "1")
                        {
                            coilsList.Add(skid);
                        }

                    }
                }
            }

            FetchSkidData(skidsList);

            // PdYN이 변경되었다면 FetchCoilData 호출
            if (isPdYNChanged)
            {
                FetchCoilData(coilsList);
            }
        }
    }

    private void FetchSkidData(List<YardMap> skids)
    {
        foreach (YardMap skid in skids)
        {
            if (skid.Dong == 1)
            {
                GameObject newObject = Instantiate(
                SkidObject,
                new Vector3(skid.Dx * 0.001f, 0, skid.Dy * 0.001f),
                Quaternion.Euler(0, skid.Dir, 0)
                );

                newObject.name = "Skid_D1_" + skid.SkidNo;

                SkidClickHandler clickHandler = newObject.GetComponent<SkidClickHandler>();
                clickHandler.skidData = skid;
                clickHandler.outlineMaterial = outlineMaterial;
            }

            if (skid.Dong == 2)
            {
                GameObject newObject = Instantiate(
                SkidObject,
                new Vector3(skid.Dx * 0.001f, 0, skid.Dy * 0.001f + 40),
                Quaternion.Euler(0, skid.Dir, 0)
                );

                newObject.name = "Skid_D2_" + skid.SkidNo;

                SkidClickHandler clickHandler = newObject.GetComponent<SkidClickHandler>();
                clickHandler.skidData = skid;
                clickHandler.outlineMaterial = outlineMaterial;
            }

        }


    }

    //private void FetchSkidData(List<YardMap> skids)
    //{

    //    foreach (YardMap skid in skids)
    //    {


    //        float adjustedDx = skid.Dx * 0.001f;
    //        float adjustedDy = skid.Dy * 0.001f;


    //        Dong 값에 따른 위치 조정
    //        switch (skid.Dong) // Dong이 int 또는 byte일 경우
    //        {
    //            case 1:
    //                Dong이 1일 때는 y 위치를 그대로 사용
    //                break;
    //            case 2:
    //                Dong이 2일 때 y 위치를 40만큼 증가
    //                adjustedDy += 40;
    //                break;
    //            default:
    //                Dong이 1이나 2가 아닐 경우는 건너뜀
    //                continue;
    //        }
    //        GameObject 생성
    //        GameObject newObject = Instantiate(
    //            SkidObject,
    //            new Vector3(adjustedDx, 0, adjustedDy),
    //            Quaternion.Euler(0, skid.Dir, 0)
    //        );

    //        newObject.name = $"Skid_D{skid.Dong}_{skid.SkidNo}";

    //    }
    //}

    private void FetchCoilData(List<YardMap> coils)
    {
        foreach (YardMap coil in coils)
        {
            if (coil.Dong == 1)
            {
                GameObject newObject = Instantiate(
                    CoilObject,
                    new Vector3(coil.Dx * 0.001f, coil.Dz * 0.001f - 0.9f, coil.Dy * 0.001f),
                    Quaternion.Euler(0, coil.Dir, 0)
                );

                newObject.name = "Coil_D1_" + coil.PdNo;
                // 코일 크기조절
                newObject.transform.localScale = new Vector3(coil.Width / 1500f, coil.Outdia / 1800f, coil.Outdia / 1800f);

                // Debug.Log($"Coil Prefab position set to x: {coil.Dx}, y: 0, z: {coil.Dy}");

                // 새로운 오브젝트 생성 후 CoilClickHandler 추가
                CoilClickHandler clicker = newObject.AddComponent<CoilClickHandler>();
                clicker.coilData = coil;
                clicker.outlineMaterial = outlineMaterial; // 여기서 outlineMaterial을 설정

                // TextMeshPro를 찾아서 pdno 설정
                TextMeshPro tmp = newObject.GetComponentInChildren<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = coil.PdNo.ToString();  // PdNo를 텍스트로 설정
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found in Coil Object.");
                }

            }

            if (coil.Dong == 2)
            {
                GameObject newObject = Instantiate(
                    CoilObject,
                    new Vector3(coil.Dx * 0.001f, coil.Dz * 0.001f - 0.9f, coil.Dy * 0.001f + 40),
                    Quaternion.Euler(0, coil.Dir, 0)
                );
                newObject.name = "Coil_D2_" + coil.PdNo;
                // 코일 크기조절
                newObject.transform.localScale = new Vector3(coil.Width / 1500f, coil.Outdia / 1800f, coil.Outdia / 1800f);

                // Debug.Log($"Coil Prefab position set to x: {coil.Dx}, y: 0, z: {coil.Dy}");

                // 새로운 오브젝트 생성 후 CoilClickHandler 추가
                CoilClickHandler clicker = newObject.AddComponent<CoilClickHandler>();
                clicker.coilData = coil;
                clicker.outlineMaterial = outlineMaterial; // 여기서 outlineMaterial을 설정

                // TextMeshPro를 찾아서 pdno 설정
                TextMeshPro tmp = newObject.GetComponentInChildren<TextMeshPro>();
                if (tmp != null)
                {
                    tmp.text = coil.PdNo.ToString();  // PdNo를 텍스트로 설정
                }
                else
                {
                    Debug.LogWarning("TextMeshPro component not found in Coil Object.");
                }
            }

        }
    }
}
