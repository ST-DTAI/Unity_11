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
    public float mainColumnSpacing =42f; //dymax�� �޾ƿ���
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
            Debug.LogWarning("railRootInstance�� null�Դϴ�.");
            return;
        }


        Transform mainColumn = railRootInstance.transform.Find("MainColumn");
        Transform mainColumnE = railRootInstance.transform.Find("MainColumnE");

        if (mainColumn != null && mainColumnE != null)
        {
            // MainColumnE�� mainColumn �������� �̵�
            mainColumnE.localPosition = mainColumn.localPosition + Vector3.forward * mainColumnSpacing;
        }
        else
        {
            Debug.LogWarning("MainColumn �Ǵ� MainColumnE�� ã�� �� �����ϴ�.");
        }


        Transform railRoot1 = railRootInstance.transform.Find("RailRoot_1");
        if (railRoot1 != null)
        {
            Transform railE = railRoot1.transform.Find("RailE");
            Transform railS = railRoot1.transform.Find("RailS");

            if (railE != null && railS != null)
            {
                // RailS�� railE �������� �̵�
                railE.localPosition = railS.localPosition + Vector3.forward * railERailSSpacing;
            }
            else
            {
                Debug.LogWarning("RailE �Ǵ� RailS�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogWarning("RailRoot_1�� ã�� �� �����ϴ�.");
        }
    }

    void PlaceFloorPrefab()
    {
        float cumulativeZ = 0f;
        for (int i = 0; i < yardSetUp.Count; i++)
        {
            YardSetUp dongData = yardSetUp[i];

            // FloorPrefab ����
            GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);

            // Floor Scale ����
            float floorDx = dongData.DxMax * Global.UnityCorrectValue * 0.1f;
            float floorDy = dongData.DyMax * Global.UnityCorrectValue * 0.1f;
            floor.transform.localScale = new Vector3(floorDx, 1f, floorDy);

            // Floor Position ����
            //float floorPosX = i * (floorDx * 10f + 10f); // dong index * Floor ����(������ �����)
            //float floorPosZ = floorDy / 2f * 10f - 5f;
            float floorPosX = floorDx * 10f / 2f -5f;  // X�� ����
            float floorPosZ = cumulativeZ + (floorDy / 2f * 10f)-5f;
            floor.transform.position = new Vector3(floorPosX, 0, floorPosZ);

            cumulativeZ += floorDy * 10f;
        }
    }
}
