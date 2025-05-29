using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using Unity.VisualScripting;

public class HistoryDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row 프리팹
    public GameObject cellPrefab; // Cell 프리팹
    

    public Transform content; // TableLayout의 Transform
    public ScrollRect scrollRect; // ScrollRect 컴포넌트

    //public Button exportButton; // CSV로 내보내기 버튼

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    private Queue<GameObject> rowPool = new Queue<GameObject>(); // Row 오브젝트 풀
    private List<GameObject> activeRows = new List<GameObject>(); // 현재 활성화된 Row 목록

    private const int maxRows = 20; // 유지할 최대 Row 수
    private int currentRowCount = 0; // 현재 Row 수
    private int totalRecords = 0; // 총 레코드 수
    private int offset = 0; // 데이터 오프셋

    // Start is called before the first frame update
    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        // 총 레코드 수 초기화
        InitializeTotalRecords();

        // Row 오브젝트 초기화
        for (int i = 0; i < maxRows; i++)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false); // 부모 설정
            row.SetActive(false); // 비활성화
            rowPool.Enqueue(row); // 풀에 추가
        }

        // 0번째 행을 비우기
        ClearFirstRow();

        // 처음 데이터 로드
        LoadInitialData();
        scrollRect.onValueChanged.AddListener(OnScroll); // 스크롤 이벤트 리스너 추가

    }

    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.CloseConnection();
        }
    }

    void InitializeTotalRecords()
    {
        string countQuery = "SELECT COUNT(*) FROM work_result";
        MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
        totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
    }

    void LoadInitialData()
    {
        // 초기 데이터 로드
        LoadData(offset);
    }

    void LoadData(int dataOffset)
    {
        // 데이터 로드
        string query = $"SELECT Time, CrNo, DrvMode, WorkType, UpAddr, DnAddr, PdNo, State, Width, Outdia, India, Thick, Weight, Date FROM work_result LIMIT {dataOffset}, {maxRows}";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        int rowCount = 0; // 현재 표시된 Row 수

        while (reader.Read() && rowCount < maxRows-1)
        {
            // Row를 풀에서 가져와서 활성화
            GameObject row = GetRowFromPool();

            // 0번째 셀에 인덱스 번호 추가
            GameObject indexCell = Instantiate(cellPrefab, row.transform);
            TMP_Text indexText = indexCell.GetComponentInChildren<TMP_Text>();
            indexText.text = (dataOffset + rowCount + 1).ToString(); // 1부터 시작하는 번호 매기기

            // 1번째 셀부터 데이터 필드를 추가
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // Cell 프리팹 인스턴스화
                GameObject cell = Instantiate(cellPrefab, row.transform);
                TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                textComponent.text = reader[i].ToString(); // 읽어온 데이터
            }

            rowCount++;
        }


        reader.Close(); // Reader를 닫아줍니다.
        currentRowCount += rowCount; // 현재 Row 수 업데이트


    }

    private void ClearFirstRow()
    {
        // 0번째 행을 비우기
        if (rowPool.Count > 0)
        {
            GameObject firstRow = rowPool.Dequeue(); // 0번째 행 가져오기
            firstRow.SetActive(false); // 비활성화
            activeRows.Add(firstRow); // 활성화된 Row 목록에 추가
        }
    }

    private void ClearRows()
    {
        // 기존 Row를 비활성화하고 풀로 반환
        foreach (GameObject row in activeRows)
        {
            row.SetActive(false);
            rowPool.Enqueue(row); // 풀에 반환
        }
        activeRows.Clear(); // 리스트 초기화
    }

    private GameObject GetRowFromPool()
    {
        if (rowPool.Count > 0)
        {
            GameObject row = rowPool.Dequeue(); // 풀에서 Row 가져오기
            row.SetActive(true); // 활성화
            activeRows.Add(row); // 활성화된 Row 목록에 추가
            return row;
        }
        else
        {
            // 추가 Row가 필요할 경우 새로 생성
            GameObject newRow = Instantiate(rowPrefab);
            newRow.transform.SetParent(content, false);
            activeRows.Add(newRow); // 활성화된 Row 목록에 추가
            return newRow;
        }
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        // 스크롤이 하단에 도달했는지 확인
        if (scrollPosition.y <= 0.1f) // 하단에 가까운 위치
        {
            if (currentRowCount < totalRecords) // 총 레코드 수를 초과하지 않도록
            {
                offset += maxRows; // 다음 데이터 오프셋으로 이동
                LoadData(offset); // 새로운 데이터 로드
            }
        }
        // 상단 스크롤 감지 (옵션)
        else if (scrollPosition.y >= 0.9f) // 상단에 가까운 위치
        {
            if (offset > 0)
            {
                offset = Mathf.Max(0, offset - maxRows); // 이전 데이터 오프셋으로 이동
                //ClearRows(); // 기존 Row 비활성화
                LoadData(offset); // 이전 데이터 로드
            }
        }
    }


}
