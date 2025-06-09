using MySql.Data.MySqlClient;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class YardMapDBManager : MonoBehaviour
{
    private bool isFirst = true;

    public GameObject SkidObject;
    public GameObject CoilObject;

    public Material clickedMaterial;
    public TextMeshProUGUI dbConnectText;

    public GameObject coilObjectList;   
    public GameObject skidObjectList;   

    [SerializeField]
    private List<YardMap> skidsList = new List<YardMap>();
    public List<YardMap> SkidsList => skidsList;

    [SerializeField]
    private List<YardMap> coilsList = new List<YardMap>();
    public List<YardMap> CoilsList => coilsList;


    private Thread thread_yardmap;
    private volatile bool isRunning = true;

    void Start()
    {

        dbConnectText.color = Color.green;

        // 🚀 초기 데이터 로딩 + 바로 화면 출력
        //ReadSkidData(forceUpdate: true);
        //FetchSkidData(skidsList);
        //FetchCoilData(coilsList);

        ThreadStart();

    }
    private void OnDestroy()
    {
        isRunning = false; // 스레드 종료 플래그 설정
        if (thread_yardmap != null)
        {
            thread_yardmap.Join(5000); // 스레드가 종료될 때까지 대기
            thread_yardmap = null;
            Debug.Log("==YardMapDBManager destroyed and thread stopped==");
        }
    }
    void ThreadStart()
    {
        if (thread_yardmap != null)
        {
            isRunning = false;
            thread_yardmap.Join(); // 기다렸다가
            thread_yardmap = null;
            return;
        }

        isRunning = true;
        thread_yardmap = new Thread(Yardmap_Thread);
        thread_yardmap.IsBackground = true;
        thread_yardmap.Start();
    }
    private void Yardmap_Thread()
    {
        while (isRunning)
        {
            try
            {
                string query = "SELECT SkidNo, Dong, Skid, Sect, DxNo, DyNo, DzNo, Addr, Dx, Dy, Dz, Dir, MaxWid, MaxDia, PdYN, Hold, CrRev, SupRev, OutRev, FwdYN, BwdYN, PdNo, State, Width, Outdia, India, Thick, Weight, Temp, Date, ToNo FROM clts.yard_map_b;";

                List<YardMap> newSkidsList = new List<YardMap>();
                List<YardMap> newCoilsList = new List<YardMap>();
                bool hasChanged = isFirst;

                using (MySqlConnection connection = new MySqlConnection(DatabaseConnection.Instance.ConnStr))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read() && isRunning)
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
                                if (newSkidsList[i].SkidNo != skidsList[i].SkidNo)
                                {
                                    hasChanged = true;
                                    break;
                                }
                            }
                        }


                        if (newCoilsList.Count != coilsList.Count || hasChanged == true)
                        {
                            hasChanged = true;
                        }
                        else
                        {
                            for (int i = 0; i < newCoilsList.Count; i++)
                            {
                                if (newCoilsList[i].PdYN != coilsList[i].PdYN)
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

                    if (hasChanged && UnityMainThreadDispatcher.Instance() != null)
                    {
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            Debug.Log("FetchSkidData() / FetchCoilData() 호출");
                            FetchSkidData();
                            FetchCoilData();
                            Debug.Log("Yardmap_Thread: Data fetched and updated on main thread.");
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                if (isRunning)
                    Debug.LogError("Yardmap_Thread error: " + ex.Message);
            }
            Thread.Sleep(1000);

            isFirst = false;
        }
    }
    private void FetchSkidData()
    {
        Debug.Log("FetchSkidData() 실행: skids.Count=" + skidsList.Count);

        foreach (YardMap skid in skidsList.ToList())
        {
            Vector3 position = new Vector3(skid.Dx * Global.UnityCorrectValue, 0, skid.Dy * Global.UnityCorrectValue);
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
            clickHandler.clickedMaterial = clickedMaterial;
        }
    }

    private void FetchCoilData()
    {
        Debug.Log("FetchCoilData() 실행: coils.Count=" + coilsList.Count);

        foreach (GameObject existingCoil in GameObject.FindGameObjectsWithTag("YardCoil"))
        {
            Destroy(existingCoil);
        }

        foreach (YardMap coil in coilsList.ToList())
        {
            Vector3 position = new Vector3(coil.Dx * Global.UnityCorrectValue, coil.Dz * Global.UnityCorrectValue - 0.9f, coil.Dy * Global.UnityCorrectValue);
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
            clickHandler.clickedMaterial = clickedMaterial;

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
