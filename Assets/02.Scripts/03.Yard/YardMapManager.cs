using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YardMapManager : MonoBehaviour
{
    public GameObject CoilnSkidPrefab;                          // 프리팹
    List<GameObject> CoilnSkidObjList = new List<GameObject>(); // 프리팹으로 만든 코일 스키드 오브젝트를 저장할 리스트

    List<YardMap> copyCoilnSkidList = new List<YardMap>(); // 복사본 리스트
    public float updateInterval = 1.0f; // 코루틴 업데이트 간격
    void Start()
    {
        InitializeCranePosition();

        StartCoroutine(UpdateCoilnSkidCoroutine());
    }
    private void InitializeCranePosition()
    {
        const string query = "SELECT SkidNo, Dong, Skid, Sect, DxNo, DyNo, DzNo, Addr, Dx, Dy, Dz, Dir, MaxWid, MaxDia, PdYN, Hold, CrRev, SupRev, OutRev, FwdYN, BwdYN, PdNo, State, Width, Outdia, India, Thick, Weight, Temp, Date, ToNo FROM yard_map ORDER BY Skid, SkidNo;";

        using (MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection.Instance.Connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    YardMap newYardMap = new YardMap(
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

                    GameObject obj = Instantiate(CoilnSkidPrefab);
                    obj.name = "CoilnSkid_" + newYardMap.Skid + "_" + newYardMap.SkidNo;
                    obj.transform.SetParent(transform); // YardMapManager의 자식으로 설정

                    obj.GetComponent<CoilnSkid>().InitializeYardMap(newYardMap);    // 초기 정보는 바로 대입
                    obj.GetComponent<CoilnSkid>().FetchDraw();

                    CoilnSkidObjList.Add(obj);          // 오브젝트 리스트에 추가
                    Global.YardMapList.Add(newYardMap); // YardMapList에 추가
                }
            }

        }
    }
    private IEnumerator UpdateCoilnSkidCoroutine()
    {
        while (true)
        {
            lock (Global.dbLocks[(int)Global.DbLockType.YARDMAP])
            {
                // YardMapList의 복사본을 생성
                copyCoilnSkidList.Clear();
                copyCoilnSkidList.AddRange(Global.YardMapList); // dbList의 현재 상태를 복사
            }

            for (int i = 0; i < CoilnSkidObjList.Count; i++)
            {
                CoilnSkidObjList[i].GetComponent<CoilnSkid>().FetchInfo(copyCoilnSkidList[i]);
                CoilnSkidObjList[i].GetComponent<CoilnSkid>().FetchDraw();
            }

            yield return new WaitForSeconds(updateInterval);    // updateInterval 초마다 데이터 갱신
        }
    }

}
