using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class TestDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row 프리팹
    public GameObject cellPrefab; // Cell 프리팹
    public GameObject indexRow;

    public Transform tableLayout; // TableLayout의 Transform

    public Toggle[] checkboxes; // 체크박스 배열
    public Button loadButton; // 데이터를 로드할 버튼
    public Button exportButton; // CSV로 내보내기 버튼

  
    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {

        // 버튼 클릭 이벤트 추가
        loadButton.onClick.AddListener(LoadData); // 버튼 클릭 시 LoadData 메서드 호출
        exportButton.onClick.AddListener(ExportToCSV); // CSV 내보내기 버튼 클릭 시 ExportToCSV 호출


        // 모든 체크박스를 체크 해제 상태로 초기화
        foreach (Toggle toggle in checkboxes)
        {
            toggle.isOn = false; // 체크박스 초기 상태를 체크 해제
        }
        connection = DatabaseConnection.Instance.Connection;
        // 처음 데이터 로드 (체크박스 상태에 관계없이)
        LoadInitialData();
    }


    void LoadInitialData()
    {
        // 기존 UI 초기화
        for (int i = tableLayout.childCount - 1; i >= 0; i--)
        {
            Transform child = tableLayout.GetChild(i);

            // indexRow와 같은 GameObject는 삭제하지 않음
            if (child.gameObject != indexRow)
            {
                Destroy(child.gameObject);
            }
        }

        try
        {
            // 모든 데이터를 가져오는 기본 쿼리
            string query = "SELECT * FROM w1_wbsupply"; // 기본 쿼리

            Debug.Log("Initial Query: " + query);

            MySqlCommand cmd = new MySqlCommand(query, connection); // 쿼리 실행 전에 명령어 생성
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Row 프리팹 인스턴스화
                GameObject row = Instantiate(rowPrefab, tableLayout);

                // 각 데이터 필드를 Cell로 추가
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Cell 프리팹 인스턴스화
                    GameObject cell = Instantiate(cellPrefab, row.transform);
                    // tmpText에 데이터 설정
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                    textComponent.text = reader[i].ToString(); // 읽어온 데이터
                }
            }

            reader.Close(); // Reader를 닫아줍니다.
        }
        catch (Exception ex)
        {
            Debug.LogError("Database error: " + ex.Message);
        }
    }

    void LoadData()
    {
        // 기존 UI 초기화
        for (int i = tableLayout.childCount - 1; i >= 0; i--)
        {
            Transform child = tableLayout.GetChild(i);

            // indexRow와 같은 GameObject는 삭제하지 않음
            if (child.gameObject != indexRow)
            {
                Destroy(child.gameObject);
            }
        }

        try
        {
            // 기본 쿼리
            string query = "SELECT * FROM w1_wbsupply WHERE 1=1"; // 기본 쿼리

            // 체크박스 상태에 따라 조건 추가
            if (checkboxes[0].isOn) // 첫 번째 체크박스가 선택된 경우
            {
                query += " AND CrRev = '0'"; // 조건 추가
            }

            if (checkboxes[1].isOn) // 두 번째 체크박스가 선택된 경우
            {
                query += " AND OdNo = '1024'"; // 조건 추가
            }

            Debug.Log("Generated Query: " + query);

            MySqlCommand cmd = new MySqlCommand(query, connection); // 쿼리 실행 전에 명령어 생성
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Row 프리팹 인스턴스화
                GameObject row = Instantiate(rowPrefab, tableLayout);

                // 각 데이터 필드를 Cell로 추가
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Cell 프리팹 인스턴스화
                    GameObject cell = Instantiate(cellPrefab, row.transform);
                    // tmpText에 데이터 설정
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                    textComponent.text = reader[i].ToString(); // 읽어온 데이터
                }
            }

            reader.Close(); // Reader를 닫아줍니다.
        }
        catch (Exception ex)
        {
            Debug.LogError("Database error: " + ex.Message);
        }
    }

    void ExportToCSV()
    {
        try
        {
            // 데스크탑 경로 설정
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "csvtest"); //데탑의 폴더 경로
            //string folderPath = @"C:\Users\Sohyun\Desktop\csvtest" //경로 하드코딩방법

            string filePath = Path.Combine(folderPath, "data.csv"); //폴더에 data.csv 파일 경로 설정


            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // CSV 헤더 작성 (필요에 따라 수정)
                writer.WriteLine("w1_wbsupply"); // 여기서는 예시로 헤더를 작성합니다.

                // 모든 데이터를 가져오는 기본 쿼리
                string query = "SELECT * FROM w1_wbsupply";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // 데이터를 CSV 형식으로 작성
                    string line = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        line += reader[i].ToString();
                        if (i < reader.FieldCount - 1)
                        {
                            line += ","; // 필드 구분자
                        }
                    }
                    writer.WriteLine(line); // 한 줄씩 CSV에 작성
                }

                reader.Close(); // Reader를 닫아줍니다.
            }

            Debug.Log("CSV 파일이 저장되었습니다: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("CSV export error: " + ex.Message);
        }
    }
}
