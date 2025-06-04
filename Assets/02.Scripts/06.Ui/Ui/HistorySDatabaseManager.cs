using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TMPro;
using UI.Tables;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class HistorySDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row 프리팹
    public GameObject cellPrefab; // Cell 프리팹
    public Transform content; // TableLayout의 Transform
    public ScrollRect scrollRect; // ScrollRect 컴포넌트
    public float scrollAmount = 0.4f; // 스크롤 양

    private List<string[]> dataRows = new List<string[]>();

    public Toggle[] checkboxes;
    public TMP_InputField pdNoInputField;

    public TMP_Text startDateInput; // 시작 날짜 입력 필드
    public TMP_InputField startTimeInput; // 시작 시간 입력 필드
    public TMP_Text endDateInput; // 끝 날짜 입력 필드
    public TMP_InputField endTimeInput; // 끝 시간 입력 필드
    public Button queryButton; // 조회 버튼
    public TMP_Text rowCountText;

    public Button closeButton;
    public GameObject panel;

    public Button CSVExportButton;

    private MySqlConnection connection;

    private Queue<GameObject> rowPool = new Queue<GameObject>(); // Row 오브젝트 풀
    private List<GameObject> activeRows = new List<GameObject>(); // 현재 활성화된 Row 목록

    private const int maxRows = 20; // 유지할 최대 Row 수
    private int currentRowCount = 0; // 현재 Row 수
    private int totalRecords = 0; // 총 레코드 수
    private int offset = 0; // 데이터 오프셋

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        // 총 레코드 수 초기화
        //InitializeTotalRecords();

        // Row 오브젝트 초기화
        for (int i = 0; i < maxRows; i++)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false); // 부모 설정
            row.SetActive(false); // 비활성화
            rowPool.Enqueue(row); // 풀에 추가
        }

        scrollRect.onValueChanged.AddListener(OnScroll); // 스크롤 이벤트 리스너 추가

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

        while (reader.Read() && rowCount < maxRows - 1)
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

    public void OnAllDataButtonClick()
    {
        // 처음 데이터 로드
        LoadInitialData();
    }

    public void OnQueryButtonClick()
    {
        // 입력된 날짜와 시간을 가져오기
        string startDate = startDateInput.text;
        string startTime = startTimeInput.text;
        string endDate = endDateInput.text;
        string endTime = endTimeInput.text;

        // 하이픈 제거
        string formattedStartDate = startDate.Replace("-", ":");
        string formattedEndDate = endDate.Replace("-", ":");

        // 시작과 끝 날짜/시간을 결합하여 DateTime 객체 생성
        DateTime startDateTime = DateTime.Parse($"{startDate} {startTime}");
        DateTime endDateTime = DateTime.Parse($"{endDate} {endTime}");

        // 데이터 로드
        LoadDataByDateRange(startDateTime, endDateTime);
        Debug.Log("LoadDataByDateRange 호출됨");
    }

    void LoadDataByDateRange(DateTime startDateTime, DateTime endDateTime)
    {
        ClearRows(); // 기존 Row를 없애기
        dataRows.Clear(); // 이전 데이터 비우기

        // SQL 쿼리 작성
        string query = "SELECT Time, CrNo, DrvMode, WorkType, UpAddr, DnAddr, PdNo, State, Width, Outdia, India, Thick, Weight, Date FROM work_result WHERE SUBSTRING(Time, 1, 14) >= @startDateTime AND SUBSTRING(Time, 1, 14) <= @endDateTime";


        // 체크박스 조건 추가
        List<string> crNoConditions = new List<string>();

        if (checkboxes[0].isOn) // 첫 번째 체크박스가 선택된 경우
        {
            crNoConditions.Add("CrNo = '11'"); // 조건 추가
        }

        if (checkboxes[1].isOn) // 두 번째 체크박스가 선택된 경우
        {
            crNoConditions.Add("CrNo = '12'"); // 조건 추가
        }

        if (checkboxes[2].isOn) // 세 번째 체크박스가 선택된 경우
        {
            crNoConditions.Add("CrNo = '13'"); // 조건 추가
        }


        // 체크박스가 선택된 경우에만 쿼리 수정
        if (crNoConditions.Count > 0)
        {
            query += " AND (" + string.Join(" OR ", crNoConditions) + ")";
        }

        string pdNoValue = pdNoInputField.text.Trim(); // 공백 제거

        if (!string.IsNullOrEmpty(pdNoValue)) // InputField가 비어있지 않은 경우
        {
            query += " AND PdNo LIKE @pdNo"; // 조건 추가
        }

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {

            cmd.Parameters.AddWithValue("@startDateTime", startDateTime);
            cmd.Parameters.AddWithValue("@endDateTime", endDateTime);

            if (!string.IsNullOrEmpty(pdNoValue))
            {
                cmd.Parameters.AddWithValue("@pdNo", "%" + pdNoValue + "%");
            }

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                // 0번째 행을 생성하고 비우기
                GameObject emptyRow = Instantiate(rowPrefab, content);
                for (int i = 0; i < 15; i++) // numberOfColumns는 데이터의 열 수
                {
                    GameObject emptyCell = Instantiate(cellPrefab, emptyRow.transform);
                    TMP_Text emptyTextComponent = emptyCell.GetComponentInChildren<TMP_Text>();
                    emptyTextComponent.text = ""; // 텍스트 비우기
                }

                int rowCount = 0; // 현재 표시된 Row 수

                while (reader.Read())
                {
                    // 데이터 저장
                    string[] rowData = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowData[i] = reader[i].ToString();
                    }
                    dataRows.Add(rowData); // 리스트에 데이터 추가


                    // Row를 풀에서 가져와서 활성화
                    //GameObject row = GetRowFromPool();
                    GameObject row = Instantiate(rowPrefab, content);
                    // 0번째 셀에 인덱스 번호 추가
                    GameObject indexCell = Instantiate(cellPrefab, row.transform);
                    TMP_Text indexText = indexCell.GetComponentInChildren<TMP_Text>();
                    indexText.text = (rowCount + 1).ToString(); // 1부터 시작하는 번호 매기기

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

                rowCountText.text = $"{rowCount}개의 데이터가 조회되었습니다";
            }
        }
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
        foreach (Transform child in content)
        {
            GameObject.Destroy(child.gameObject);
        }
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

    }


    // 패널을 닫는 메서드
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }


    void ExportToCSV()
    {
        // 바탕화면 경로 가져오기
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "data.csv");

        StringBuilder csvContent = new StringBuilder();

        // 헤더 추가
        csvContent.AppendLine("Time,CrNo,DrvMode,WorkType,UpAddr,DnAddr,PdNo,State,Width,Outdia,India,Thick,Weight,Date");

        foreach (var row in dataRows)
        {
            csvContent.AppendLine(string.Join(",", row)); // 데이터 추가
        }

        // CSV 파일 저장
        File.WriteAllText(filePath, csvContent.ToString());
        Debug.Log("CSV 파일이 바탕화면에 저장되었습니다: " + filePath);
    }


    public void OnExportButtonClick()
    {
        
        ExportToCSV();
        
    }

    // 위 버튼 클릭 시 호출되는 메서드
    public void OnClickScrollUp()
    {
        // 현재 스크롤 위치를 조정
        scrollRect.verticalNormalizedPosition += scrollAmount;
    }

    // 아래 버튼 클릭 시 호출되는 메서드
    public void OnClickScrollDown()
    {
        // 현재 스크롤 위치를 조정
        scrollRect.verticalNormalizedPosition -= scrollAmount;
    }
}