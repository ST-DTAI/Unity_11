using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YardSetUpManager : MonoBehaviour
{
    List<YardSetUp> yardSetUp = new List<YardSetUp>();


    public GameObject railPrefab;
    public GameObject floorPrefab;
    public GameObject stopperPrefab;
    private void Awake()
    {
        ReadDongInit();
    }
    void Start()
    {
        PlaceRailPrefabs();
        PlaceFloorPrefab();
    }

    private void ReadDongInit()
    {
        const string query = "SELECT Dong, DxOffset, DyOffset, DxMax, DyMax, Height, DxSpacing FROM unity_dong_init ORDER BY Dong;";

        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection.Instance.Connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        YardSetUp yard = new YardSetUp(
                            reader.GetInt32("Dong"),
                            reader.GetInt32("DxOffset"),
                            reader.GetInt32("DxMax"),
                            reader.GetInt32("DyOffset"),
                            reader.GetInt32("DyMax"),
                            reader.GetInt32("Height"),
                            reader.GetInt32("DxSpacing")
                        );
                        yardSetUp.Add(yard);

                        Global.DongSpacing.Add(yard.DyMax * Global.UnityCorrectValue);
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Debug.LogError("ReadDongInit: " + ex.Message);
        }
    }


    void PlaceRailPrefabs()
    {
        float dongSpacing = 0;
        for (int dong = 0; dong < yardSetUp.Count; dong++)
        {
            float railCount = yardSetUp[dong].DxMax / yardSetUp[dong].DxSpacing + 1;
            float railSpacing = yardSetUp[dong].DxSpacing * Global.UnityCorrectValue;
            float height = yardSetUp[dong].Height * Global.UnityCorrectValue;
            
            if (dong > 0)
            {
                dongSpacing += Global.DongSpacing[dong - 1]; // 이전 동의 간격을 누적
            }

            for (int i = 0; i < railCount; i++)
            {
                Vector3 position = new Vector3(i * railSpacing, 0, dongSpacing);
                GameObject newRail = Instantiate(railPrefab, position, Quaternion.identity);
                newRail.transform.SetParent(transform, false);
                newRail.name = "Rail_" + dong;


                Transform rail = newRail.transform.Find("Rail");
                Transform railRev = newRail.transform.Find("RailRev");
                if (rail != null && railRev != null)
                {
                    railRev.localPosition = rail.localPosition + Vector3.forward * Global.DongSpacing[dong];
                    rail.GetComponent<RailSetting>().SetRailTransform(height, railSpacing);
                    railRev.GetComponent<RailSetting>().SetRailTransform(height, railSpacing);

                    if (i != 0 && i != railCount -1)
                    {
                        continue;
                    }
                    else
                    {
                        float dx = -railSpacing * 0.5f;
                        int rotY = 0;
                        if (i != 0)
                        {
                            dx = railSpacing * 0.5f;
                            rotY = 180;
                        }
                        Quaternion rotation = Quaternion.Euler(0, rotY, 0);

                        Vector3 stopperPosition = new Vector3(rail.localPosition.x + dx, height, rail.localPosition.z + 0.7f);
                        GameObject stopper = Instantiate(stopperPrefab, stopperPosition, rotation);
                        stopper.transform.SetParent(newRail.transform, false);
                        stopper.name = "Stopper";

                        Vector3 stopperRevPosition = new Vector3(railRev.localPosition.x + dx, height, railRev.localPosition.z - 0.7f);
                        GameObject stopperRev = Instantiate(stopperPrefab, stopperRevPosition, rotation);
                        stopperRev.transform.SetParent(newRail.transform, false);
                        stopperRev.name = "StopperRev";
                    }
                }
                else
                {
                    Debug.LogWarning("MainColumn 또는 MainColumnE를 찾을 수 없습니다.");
                }
            }
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

            float margin = 0f;

            // Floor Scale 설정
            float floorDx = dongData.DxMax * Global.UnityCorrectValue * 0.1f;
            float floorDy = dongData.DyMax * Global.UnityCorrectValue * 0.1f;
            floor.transform.localScale = new Vector3(floorDx + margin * 2, 1f, floorDy + margin * 2);

            // Floor Position 설정

            //float floorPosX = floorDx * 10f / 2f - 5f;  // X축 고정
            //float floorPosZ = cumulativeZ + (floorDy * 10f / 2f) - 5f;
            float floorPosX = floorDx * 10f / 2f - margin;  // X축 고정
            float floorPosZ = cumulativeZ + (floorDy * 10f / 2f) - margin;
            floor.transform.position = new Vector3(floorPosX, 0, floorPosZ);

            cumulativeZ += floorDy * 10f;
        }
    }
}
