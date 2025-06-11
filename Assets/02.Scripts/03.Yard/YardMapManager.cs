using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YardMapManager : MonoBehaviour
{
    public GameObject CoilnSkidPrefab;                          // ������
    List<GameObject> CoilnSkidObjList = new List<GameObject>(); // ���������� ���� ���� ��Ű�� ������Ʈ�� ������ ����Ʈ

    List<YardMap> copyCoilnSkidList = new List<YardMap>(); // ���纻 ����Ʈ
    public float updateInterval = 1.0f; // �ڷ�ƾ ������Ʈ ����
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
                    obj.transform.SetParent(transform); // YardMapManager�� �ڽ����� ����

                    obj.GetComponent<CoilnSkid>().InitializeYardMap(newYardMap);    // �ʱ� ������ �ٷ� ����
                    obj.GetComponent<CoilnSkid>().FetchDraw();

                    CoilnSkidObjList.Add(obj);          // ������Ʈ ����Ʈ�� �߰�
                    Global.YardMapList.Add(newYardMap); // YardMapList�� �߰�
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
                // YardMapList�� ���纻�� ����
                copyCoilnSkidList.Clear();
                copyCoilnSkidList.AddRange(Global.YardMapList); // dbList�� ���� ���¸� ����
            }

            for (int i = 0; i < CoilnSkidObjList.Count; i++)
            {
                CoilnSkidObjList[i].GetComponent<CoilnSkid>().FetchInfo(copyCoilnSkidList[i]);
                CoilnSkidObjList[i].GetComponent<CoilnSkid>().FetchDraw();
            }

            yield return new WaitForSeconds(updateInterval);    // updateInterval �ʸ��� ������ ����
        }
    }

}
