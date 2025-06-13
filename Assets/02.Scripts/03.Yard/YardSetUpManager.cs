using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class YardSetUpManager : MonoBehaviour
{
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
                            reader.GetFloat("DxOffset") * Global.UnityCorrectValue,
                            reader.GetFloat("DxMax") * Global.UnityCorrectValue,
                            reader.GetFloat("DyOffset") * Global.UnityCorrectValue,
                            reader.GetFloat("DyMax") * Global.UnityCorrectValue,
                            reader.GetFloat("Height") * Global.UnityCorrectValue,
                            reader.GetFloat("DxSpacing") * Global.UnityCorrectValue
                        );

                        Global.YardSetUpList.Add(yard);
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
        for (int dong = 0; dong < Global.YardSetUpList.Count; dong++)
        {
            int railCount = (int)(Global.YardSetUpList[dong].DxMax / Global.YardSetUpList[dong].DxSpacing) + 1;
            float railSpacing = Global.YardSetUpList[dong].DxSpacing;
            float height = Global.YardSetUpList[dong].Height;
            
            if (dong > 0)
            {
                dongSpacing += Global.YardSetUpList[dong - 1].DyMax;    // 이전 동의 간격을 누적
            }

            for (int iRail = 0; iRail < railCount; iRail++)
            {
                Vector3 position = new Vector3(iRail * railSpacing, 0, dongSpacing);
                GameObject newRail = Instantiate(railPrefab, position, Quaternion.identity);
                newRail.transform.SetParent(transform, false);
                newRail.name = $"Rail_{dong}_{iRail + 1}";

                Transform rail = newRail.transform.Find("Rail");
                Transform railRev = newRail.transform.Find("RailRev");
                if (rail != null && railRev != null)
                {
                    railRev.localPosition = rail.localPosition + Vector3.forward * Global.YardSetUpList[dong].DyMax;
                    rail.GetComponent<RailSetting>().SetRailTransform(height, railSpacing);
                    railRev.GetComponent<RailSetting>().SetRailTransform(height, railSpacing);

                    if (iRail == 0 || iRail == railCount -1)
                    {
                        float dx = -railSpacing * 0.5f;
                        int rotY = 0;
                        if (iRail != 0)
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
        for (int i = 0; i < Global.YardSetUpList.Count; i++)
        {
            // FloorPrefab 생성
            GameObject floor = Instantiate(floorPrefab, Vector3.zero, Quaternion.identity);

            float margin = 0f;

            // Floor Scale 설정
            float floorDx = Global.YardSetUpList[i].DxMax * 0.1f;
            float floorDy = Global.YardSetUpList[i].DyMax * 0.1f;
            floor.transform.localScale = new Vector3(floorDx + margin * 2, 1f, floorDy + margin * 2);

            // Floor Position 설정
            float floorPosX = floorDx * 10f / 2f - margin;  // X축 고정
            float floorPosZ = cumulativeZ + (floorDy * 10f / 2f) - margin;
            floor.transform.position = new Vector3(floorPosX, 0, floorPosZ);

            cumulativeZ += floorDy * 10f;
        }
    }
}
