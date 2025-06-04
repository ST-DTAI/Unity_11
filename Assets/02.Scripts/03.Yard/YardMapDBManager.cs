using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using UnityEngine;

public class YardMapDBManager : MonoBehaviour
{
    private MySqlConnection connection;

    public GameObject SkidObject;
    public GameObject CoilObject;

    public Material outlineMaterial;
    public TextMeshProUGUI dbConnectText;

    public GameObject coilObjectList;   
    public GameObject skidObjectList;   

    [SerializeField]
    private List<YardMap> skidsList = new List<YardMap>();
    public List<YardMap> SkidsList => skidsList;

    [SerializeField]
    private List<YardMap> coilsList = new List<YardMap>();
    public List<YardMap> CoilsList => coilsList;

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;


        if (connection != null)
        {
            dbConnectText.color = Color.green;

            // 🚀 초기 데이터 로딩 + 바로 화면 출력
            ReadSkidData(forceUpdate: true);
            FetchSkidData(skidsList);
            FetchCoilData(coilsList);

            // 이후 코루틴으로 데이터 감시
            StartCoroutine(UpdateSkidDataCoroutine());
        }
        else
        {
            dbConnectText.color = Color.red;
            Debug.LogError("DB 연결 실패!");
        }
    }


    private void ReadSkidData(bool forceUpdate = false)
    {
        string query = "SELECT SkidNo, Dong, Skid, Sect, DxNo, DyNo, DzNo, Addr, Dx, Dy, Dz, Dir, MaxWid, MaxDia, PdYN, Hold, CrRev, SupRev, OutRev, FwdYN, BwdYN, PdNo, State, Width, Outdia, India, Thick, Weight, Temp, Date, ToNo FROM clts.yard_map;";

        List<YardMap> newSkidsList = new List<YardMap>();
        List<YardMap> newCoilsList = new List<YardMap>();
        bool hasChanged = forceUpdate;

        if (connection != null)
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
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

                        if (skid.DzNo != 2)
                            newSkidsList.Add(skid);

                        if (skid.PdYN == "1")
                            newCoilsList.Add(skid);
                    }
                }
            }

            // 데이터 변화 감지
            if (!hasChanged)
            {
                if (newSkidsList.Count != skidsList.Count)
                {
                    hasChanged = true;
                }
                else
                {
                    for (int i = 0; i < newSkidsList.Count; i++)
                    {
                        if (newSkidsList[i].SkidNo != skidsList[i].SkidNo ||
                            newSkidsList[i].PdYN != skidsList[i].PdYN)
                        {
                            hasChanged = true;
                            break;
                        }
                    }
                }
            }

            // 리스트 갱신
            skidsList.Clear();
            skidsList.AddRange(newSkidsList);

            coilsList.Clear();
            coilsList.AddRange(newCoilsList);

            if (hasChanged)
            {
                Debug.Log("데이터 변경 감지 혹은 초기 로딩: FetchSkidData() 호출");
                FetchSkidData(skidsList);
                FetchCoilData(coilsList);
            }
        }
    }

    private IEnumerator UpdateSkidDataCoroutine()
    {
        while (true)
        {
            ReadSkidData(forceUpdate: false);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void FetchSkidData(List<YardMap> skids)
    {
        Debug.Log("FetchSkidData() 실행: skids.Count=" + skids.Count);

        foreach (YardMap skid in skids)
        {
            Vector3 position = new Vector3(skid.Dx * 0.001f, 0, skid.Dy * 0.001f);
            if (skid.Dong == 2)
            {
                position.z += 40f;
            }

            GameObject newObject = Instantiate(
                SkidObject,
                position,
                Quaternion.Euler(0, skid.Dir, 0)
            );

            newObject.transform.SetParent(skidObjectList.transform, false);
            newObject.name = $"Skid_D{skid.Dong}_{skid.SkidNo}";

            SkidClickHandler clickHandler = newObject.GetComponent<SkidClickHandler>();
            clickHandler.skidData = skid;
            clickHandler.outlineMaterial = outlineMaterial;
        }
    }

    private void FetchCoilData(List<YardMap> coils)
    {
        Debug.Log("FetchCoilData() 실행: coils.Count=" + coils.Count);

        foreach (GameObject existingCoil in GameObject.FindGameObjectsWithTag("YardCoil"))
        {
            Destroy(existingCoil);
        }

        foreach (YardMap coil in coils)
        {
            Vector3 position = new Vector3(coil.Dx * 0.001f, coil.Dz * 0.001f - 0.9f, coil.Dy * 0.001f);
            if (coil.Dong == 2)
            {
                position.z += 40f;
            }

            GameObject newObject = Instantiate(
                CoilObject,
                position,
                Quaternion.Euler(0, coil.Dir, 0)
            );

            newObject.transform.SetParent(coilObjectList.transform, false);

            newObject.tag = "YardCoil";
            newObject.name = $"Coil_D{coil.Dong}_{coil.PdNo}";
            

            newObject.transform.localScale = new Vector3(
                coil.Width / 1500f,
                coil.Outdia / 1800f,
                coil.Outdia / 1800f
            );

            CoilClickHandler clickHandler = newObject.AddComponent<CoilClickHandler>();
            clickHandler.coilData = coil;
            clickHandler.outlineMaterial = outlineMaterial;

            TextMeshPro tmp = newObject.GetComponentInChildren<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = coil.PdNo.ToString();
            }
            else
            {
                Debug.LogWarning("TextMeshPro component not found in Coil Object.");
            }
        }
    }
}
