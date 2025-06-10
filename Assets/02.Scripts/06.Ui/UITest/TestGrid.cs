using UnityEngine;
using UnityEngine.UI; // UI 관련 네임스페이스
using TMPro; // TextMeshPro 네임스페이스 포함
using MySql.Data.MySqlClient; // MySQL 네임스페이스 포함
using System.Collections.Generic; // List 사용을 위한 네임스페이스

public class TestGrid : MonoBehaviour
{
    private DatabaseConnection dbConnection;
    public GameObject cellPrefab; // 셀 프리팹 (TextMeshProUGUI가 포함된 오브젝트)
    public Transform contentPanel; // GridLayoutGroup이 추가된 Panel의 Transform
    public int[] CellWidth;
    public int[] CellHeight;
    public int RowHeight;
    public int RowWidth;


    private void Start()
    {
        LoadDataFromDatabase();
    }

    private void LoadDataFromDatabase()
    {
        if (cellPrefab == null || contentPanel == null)
        {
            Debug.LogError("cellPrefab or contentPanel is not assigned in the inspector.");
            return;
        }

        string connectionString = string.Format("server=192.168.0.4; database=clts_unity; uid=clts; pwd=clts; charset=utf8;");
        string query = "SELECT * FROM pd_info;"; // 쿼리

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            Debug.Log("MySQL Connection Opened");


            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    int RowCount = 0;
                    while (reader.Read())
                    {
                        // 각 열에 대한 셀 생성
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            GameObject cell = Instantiate(cellPrefab, contentPanel);
                            TextMeshProUGUI cellText = cell.GetComponentInChildren<TextMeshProUGUI>();
                            if (cellText != null)
                            {
                                cell.name = "cell :" + i.ToString() +","+ RowCount.ToString();
                                cellText.text = reader[i].ToString();
                            }
                            else
                            {
                                Debug.LogError("TextMeshProUGUI component not found in cell prefab.");
                            }
                        }
                        RowCount++;
                    }
                }
            }
        }
    }
}

