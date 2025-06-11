using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneManager : MonoBehaviour
{
    public GameObject cranePrefab;                                  // 프리팹
    private List<GameObject> craneObject = new List<GameObject>();  // 프리팹으로 만든 크레인 오브젝트를 저장할 리스트

    List<CrStatus> copyCraneStatusList = new List<CrStatus>();      // 복사본 리스트
    public float updateInterval = 0.5f; // 코루틴 업데이트 간격
    private void Awake()
    {
        // 초기 크레인 위치 설정
        InitializeCranePosition();
    }
    void Start()
    {
        StartCoroutine(UpdateCraneCoroutine());
    }
    private void InitializeCranePosition()
    {
        string query = "SELECT a.CrNo, a.DxOffset, a.MinDx, a.MaxDx, a.DyOffset, a.MinDy, a.MaxDy" +
            " FROM cr_init a INNER JOIN cr_status b ON a.CrNo = b.CrNo ORDER BY a.CrNo;";

        using (MySqlCommand cmd = new MySqlCommand(query, DatabaseConnection.Instance.Connection))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int crNo  = reader.GetInt32("CrNo");
                    int dxOffset = reader.GetInt32("DxOffset");
                    int minDx = reader.GetInt32("MinDx");
                    int maxDx = reader.GetInt32("MaxDx");
                    int dyOffset = reader.GetInt32("DyOffset");
                    int minDy = reader.GetInt32("MinDy");
                    int maxDy = reader.GetInt32("MaxDy");

                    GameObject crane = Instantiate(cranePrefab);

                    crane.GetComponent<Crane>().crNo = crNo;
                    crane.GetComponent<Crane>().dxOffset = dxOffset ;
                    crane.GetComponent<Crane>().minDx = minDx ;
                    crane.GetComponent<Crane>().maxDx = maxDx ;
                    crane.GetComponent<Crane>().dyOffset = dyOffset ;
                    crane.GetComponent<Crane>().minDy = minDy ;
                    crane.GetComponent<Crane>().maxDy = maxDy;
                    ApplyCraneScale(crane);

                    crane.name = "Crane" + crNo;
                    crane.transform.SetParent(transform);
                    craneObject.Add(crane);

                    Global.CrStatusList.Add(new CrStatus()); // 초기화용 빈 CrStatus 객체 추가
                }

            }
        }
    }
    private IEnumerator UpdateCraneCoroutine()
    {
        while (true)
        {
            lock (Global.dbLocks[(int)Global.DbLockType.CRSTATUS])
            {
                copyCraneStatusList.Clear();
                copyCraneStatusList.AddRange(Global.CrStatusList); // dbList의 현재 상태를 복사
            }

            for (int crIdx = 0; crIdx < craneObject.Count; crIdx++)
            {
                craneObject[crIdx].GetComponent<Crane>().crStatus = copyCraneStatusList[crIdx];
                craneObject[crIdx].GetComponent<Crane>().FetchCraneData();
            }

            yield return new WaitForSeconds(updateInterval); // updateInterval 초마다 데이터 갱신
        }
    }

    private void ApplyCraneScale(GameObject crane)
    {
        YardSetUpManager yardManager = FindObjectOfType<YardSetUpManager>();
        if (yardManager == null)
        {
            Debug.LogWarning("YardSetUpManager가 씬에 존재하지 않습니다.");
            return;
        }
        if (Global.DongSpacing.Count == 0)
        {
            Debug.LogWarning("Global.DongSpacing이 초기화되지 않았습니다.");
            return;
        }

        int dong = crane.GetComponent<Crane>().crNo / 10;
        float spacing = Global.DongSpacing[dong - 1];
        float craneScale= 1f * spacing /32f;
        // spacing을 크레인 Z축 스케일에 반영 (예시)
        crane.transform.localScale = new Vector3(craneScale, 1f, craneScale);
    }
}
