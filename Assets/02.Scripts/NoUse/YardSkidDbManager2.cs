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

public class YardSkidDbManager2 : MonoBehaviour
{

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    public GameObject SkidObject;
    public GameObject CoilObject;

    private UiManager2 uiManager2;
    public Material outlineMaterial;

    public TextMeshProUGUI dbConnectText;

    [SerializeField]
    private List<YardSkidCoil> skidsList = new List<YardSkidCoil>();

    [SerializeField]
    private List<YardSkidCoil> coilsList = new List<YardSkidCoil>();

    public List<YardSkidCoil> CoilsList => coilsList; // coilsList에 접근할 수 있는 프로퍼티

    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = DatabaseConnection.Instance.Connection;

        dbConnectText.color = Color.green; //db통신 성공시 텍스트 색상 변경

        if (connection != null)
        {
            ReadSkidData(true); // 시작할 때 데이터를 한 번만 가져오고 처리
        }
    }


    private void ReadSkidData(bool initialLoad)
    {
        string query = "SELECT COALESCE(ym.SkidNo, 0) AS SkidNo, " +
       "COALESCE(ym.Dong, 0) AS Dong,                    " +
       "COALESCE(ym.Skid, '0') AS Skid,                  " +
       "COALESCE(ym.Sect, 0) AS Sect,                    " +
       "COALESCE(ym.DxNo, 0) AS DxNo,                    " +
       " COALESCE(ym.DyNo, 0) AS DyNo,                   " +
       "COALESCE(ym.DzNo, 0) AS DzNo,                    " +
       "COALESCE(ym.Addr, 0) AS Addr,                    " +
       "COALESCE(ym.PdNo, '0') AS PdNo,                  " +
       "COALESCE(ym.Dx, 0) AS Dx,                        " +
       "COALESCE(ym.Dy, 0) AS Dy,                        " +
       "COALESCE(ym.Dz, 0) AS Dz,                        " +
       " COALESCE(ym.Dir, 0) AS Dir,                     " +
       "COALESCE(ym.MaxWid, 0) AS MaxWid,                " +
       " COALESCE(ym.MaxDia, 0) AS MaxDia,               " +
       "COALESCE(ym.PdYN, 0) AS PdYN,                    " +
       "COALESCE(ym.Hold, 0) AS Hold,                    " +
       "COALESCE(ym.CrRev, 0) AS CrRev,                  " +
       "COALESCE(ym.SupRev, 0) AS SupRev,                " +
       "COALESCE(ym.OutRev, 0) AS OutRev,                " +
       "COALESCE(ym.FwdYN, 0) AS FwdYN,                  " +
       "COALESCE(ym.BwdYN, 0) AS BwdYN,                  " +
       "COALESCE(pi.State, 0) AS State,                  " +
       "COALESCE(pi.Width, 0) AS Width,                  " +
       "COALESCE(pi.Outdia, 0) AS Outdia,                " +
       "COALESCE(pi.India, 0) AS India,                  " +
       "COALESCE(pi.Thick, 0) AS Thick,                  " +
       "COALESCE(pi.Weight, 0) AS Weight,                " +
       "COALESCE(pi.Temp, 0) AS Temp,                    " +
       "COALESCE(pi.Date, '1970-01-01') AS Date,         " +
       "COALESCE(pi.ToNo, 0) AS ToNo FROM clts.yard_map ym LEFT JOIN clts.pd_info pi ON ym.PdNo = pi.PdNo;" ;
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


                        YardSkidCoil skid = new YardSkidCoil(
                          reader.GetInt32("skidNo"),
                          reader.GetString("dong"),
                          reader.GetString("skid"),
                          reader.GetString("sect"),
                          reader.GetInt32("dxNo"),
                          reader.GetInt32("dyNo"),
                          reader.GetInt32("dzNo"),
                          reader.GetString("addr"),
                          reader.GetString("pdNo"),
                          reader.GetInt32("dx"),
                          reader.GetInt32("dy"),
                          reader.GetInt32("dz"),
                          reader.GetInt32("dir"),
                          reader.GetInt32("maxWid"),
                          reader.GetInt32("maxDia"),
                          reader.GetString("pdYN"),
                          reader.GetString("hold"),
                          //crRev,
                          //reader.GetString("crRev"),
                          reader.GetInt32("crRev"),
                          reader.GetString("supRev"),
                          reader.GetString("outRev"),
                          reader.GetString("fwdYN"),
                          reader.GetString("bwdYN"),
                          reader.GetString("state"),
                          reader.GetFloat("width"),
                          reader.GetFloat("outdia"),
                          reader.GetFloat("india"),
                          reader.GetFloat("thick"),
                          reader.GetFloat("weight"),
                          reader.GetFloat("temp"),
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

    private void FetchSkidData(List<YardSkidCoil> skids)
    {
        foreach (YardSkidCoil skid in skids)
        {
            if(skid.Dong == "1")
            {
                GameObject newObject = Instantiate(
                SkidObject,
                new Vector3(skid.Dx * Global.UnityCorrectValue, 0, skid.Dy * Global.UnityCorrectValue),
                Quaternion.Euler(0, skid.Dir, 0)
                );

                newObject.name = "Skid_D1_" + skid.SkidNo;
            }

            if (skid.Dong == "2")
            {
                GameObject newObject = Instantiate(
                SkidObject,
                new Vector3(skid.Dx * Global.UnityCorrectValue , 0, skid.Dy * Global.UnityCorrectValue + 40),
                Quaternion.Euler(0, skid.Dir, 0)
                );

                newObject.name = "Skid_D2_" + skid.SkidNo;
            }

        }
    }
    private void FetchCoilData(List<YardSkidCoil> coils)
    {
        foreach (YardSkidCoil coil in coils)
        {
            if (coil.Dong == "1")
            {
                GameObject newObject = Instantiate(
                    CoilObject,
                    new Vector3(coil.Dx * Global.UnityCorrectValue, coil.Dz * Global.UnityCorrectValue - 0.9f, coil.Dy * Global.UnityCorrectValue),
                    Quaternion.Euler(0, coil.Dir, 0)
                );

                newObject.name = "Coil_D1_" + coil.PdNo;
                // 코일 크기조절
                newObject.transform.localScale = new Vector3(coil.Width / 1500f, coil.Outdia / 1800f, coil.Outdia / 1800f);

                // Debug.Log($"Coil Prefab position set to x: {coil.Dx}, y: 0, z: {coil.Dy}");

                // 새로운 오브젝트 생성 후 CoilClickHandler 추가
                CoilClickHandler2 clicker = newObject.AddComponent<CoilClickHandler2>();
                clicker.coilData2 = coil;
                clicker.outlineMaterial = outlineMaterial; // 여기서 outlineMaterial을 설정
            }

            if (coil.Dong == "2")
            {
                GameObject newObject = Instantiate(
                    CoilObject,
                    new Vector3(coil.Dx * Global.UnityCorrectValue, coil.Dz * Global.UnityCorrectValue - 0.9f, coil.Dy * Global.UnityCorrectValue+40),
                    Quaternion.Euler(0, coil.Dir, 0)
                );
                newObject.name = "Coil_D2_" + coil.PdNo;
                // 코일 크기조절
                newObject.transform.localScale = new Vector3(coil.Width / 1500f, coil.Outdia / 1800f, coil.Outdia / 1800f);

                // Debug.Log($"Coil Prefab position set to x: {coil.Dx}, y: 0, z: {coil.Dy}");

                // 새로운 오브젝트 생성 후 CoilClickHandler 추가
                CoilClickHandler2 clicker = newObject.AddComponent<CoilClickHandler2>();
                clicker.coilData2 = coil;
                clicker.outlineMaterial = outlineMaterial; // 여기서 outlineMaterial을 설정
            }

        }
    }
}
