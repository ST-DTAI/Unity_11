using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YardSetUpManager : MonoBehaviour
{

    private MySqlConnection connection;
    List<YardSetUp> yardSetUp = new List<YardSetUp>();


    public GameObject railPrefab;
    public float railSpacing = 10f;


    public GameObject floorPrefab;

    [HideInInspector]
    public float mainColumnSpacing =42f; //dymax를 받아오자
    private float railERailSSpacing;

    private void OnValidate()
    {
        railERailSSpacing = mainColumnSpacing - 2f;
    }

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;
        ReadDongInit();

        PlaceRailPrefabs();
        PlaceFloorPrefab();
    }

    private void ReadDongInit()
    {
        string query = "SELECT Dong, DxOffset, DyOffset, DxMax, DyMax FROM unity_dong_init ORDER BY Dong;";


        if (connection != null)
        {
            try
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            //int Dong = reader.GetInt32("Dong");
                            //int DxOffset = reader.GetInt32("DxOffset");
                            //int DxMax = reader.GetInt32("DxMax");
                            //int DyOffset = reader.GetInt32("DyOffset");
                            //int DyMax = reader.GetInt32("DyMax");



                            YardSetUp tt = new YardSetUp(
                                reader.GetInt32("Dong"),
                                reader.GetInt32("DxOffset"),
                                reader.GetInt32("DxMax"),
                                reader.GetInt32("DyOffset"),
                                reader.GetInt32("DyMax")
                            );
                            yardSetUp.Add(tt);
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                Debug.LogError("MySQL query error: " + ex.Message);
            }
        }
        else
        {
            Debug.LogError("Database connection is null.");
        }
    }


    void PlaceRailPrefabs()
    {
        float tmpCount = yardSetUp[0].DxMax * Global.UnityCorrectValue / railSpacing;
        int railCount = (int)tmpCount;
        if (tmpCount % 1 != 0)
        {
            railCount += 1;
        }
        for (int i = 0; i < railCount; i++)
        {
            Vector3 position = new Vector3(i * railSpacing, 0, 0);
            GameObject newRail = Instantiate(railPrefab, position, Quaternion.identity);
            newRail.transform.SetParent(transform, false);

            AdjustRailRootObjects(newRail);
        }
    }


    void AdjustRailRootObjects(GameObject railRootInstance)
    {
        if (railRootInstance == null)
        {
            Debug.LogWarning("railRootInstance가 null입니다.");
            return;
        }


        Transform mainColumn = railRootInstance.transform.Find("MainColumn");
        Transform mainColumnE = railRootInstance.transform.Find("MainColumnE");

        if (mainColumn != null && mainColumnE != null)
        {
            // MainColumnE를 mainColumn 기준으로 이동
            mainColumnE.localPosition = mainColumn.localPosition + Vector3.forward * mainColumnSpacing;
        }
        else
        {
            Debug.LogWarning("MainColumn 또는 MainColumnE를 찾을 수 없습니다.");
        }


        Transform railRoot1 = railRootInstance.transform.Find("RailRoot_1");
        if (railRoot1 != null)
        {
            Transform railE = railRoot1.transform.Find("RailE");
            Transform railS = railRoot1.transform.Find("RailS");

            if (railE != null && railS != null)
            {
                // RailS를 railE 기준으로 이동
                railE.localPosition = railS.localPosition + Vector3.forward * railERailSSpacing;
            }
            else
            {
                Debug.LogWarning("RailE 또는 RailS를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("RailRoot_1을 찾을 수 없습니다.");
        }
    }

    void PlaceFloorPrefab()
    {
        float cumulativeZ = 0f;
        for (int i = 0; i < yardSetUp.Count; i++)
        {
            YardSetUp dongData = yardSetUp[i];

            // FloorPrefab 생성
            GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);

            // Floor Scale 설정
            float floorDx = dongData.DxMax * Global.UnityCorrectValue * 0.1f;
            float floorDy = dongData.DyMax * Global.UnityCorrectValue * 0.1f;
            floor.transform.localScale = new Vector3(floorDx, 1f, floorDy);

            // Floor Position 설정
            //float floorPosX = i * (floorDx * 10f + 10f); // dong index * Floor 간격(적당히 띄워줌)
            //float floorPosZ = floorDy / 2f * 10f - 5f;
            float floorPosX = floorDx * 10f / 2f -5f;  // X축 고정
            float floorPosZ = cumulativeZ + (floorDy / 2f * 10f)-5f;
            floor.transform.position = new Vector3(floorPosX, 0, floorPosZ);

            cumulativeZ += floorDy * 10f;
        }
    }
}
