using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraneManager : MonoBehaviour
{
    public GameObject cranePrefab;                                  // ������
    private List<GameObject> craneObject = new List<GameObject>();  // ���������� ���� ũ���� ������Ʈ�� ������ ����Ʈ

    List<CrStatus> copyCraneStatusList = new List<CrStatus>();      // ���纻 ����Ʈ
    public float updateInterval = 0.5f; // �ڷ�ƾ ������Ʈ ����
    private void Awake()
    {
        // �ʱ� ũ���� ��ġ ����
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

                    Global.CrStatusList.Add(new CrStatus()); // �ʱ�ȭ�� �� CrStatus ��ü �߰�
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
                copyCraneStatusList.AddRange(Global.CrStatusList); // dbList�� ���� ���¸� ����
            }

            for (int crIdx = 0; crIdx < craneObject.Count; crIdx++)
            {
                craneObject[crIdx].GetComponent<Crane>().crStatus = copyCraneStatusList[crIdx];
                craneObject[crIdx].GetComponent<Crane>().FetchCraneData();
            }

            yield return new WaitForSeconds(updateInterval); // updateInterval �ʸ��� ������ ����
        }
    }

    private void ApplyCraneScale(GameObject crane)
    {
        YardSetUpManager yardManager = FindObjectOfType<YardSetUpManager>();
        if (yardManager == null)
        {
            Debug.LogWarning("YardSetUpManager�� ���� �������� �ʽ��ϴ�.");
            return;
        }
        if (Global.DongSpacing.Count == 0)
        {
            Debug.LogWarning("Global.DongSpacing�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
            return;
        }

        int dong = crane.GetComponent<Crane>().crNo / 10;
        float spacing = Global.DongSpacing[dong - 1];
        float craneScale= 1f * spacing /32f;
        // spacing�� ũ���� Z�� �����Ͽ� �ݿ� (����)
        crane.transform.localScale = new Vector3(craneScale, 1f, craneScale);
    }
}
