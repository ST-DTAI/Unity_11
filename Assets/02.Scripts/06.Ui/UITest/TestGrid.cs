using UnityEngine;
using UnityEngine.UI; // UI ���� ���ӽ����̽�
using TMPro; // TextMeshPro ���ӽ����̽� ����
using MySql.Data.MySqlClient; // MySQL ���ӽ����̽� ����
using System.Collections.Generic; // List ����� ���� ���ӽ����̽�

public class TestGrid : MonoBehaviour
{
    private DatabaseConnection dbConnection;
    public GameObject cellPrefab; // �� ������ (TextMeshProUGUI�� ���Ե� ������Ʈ)
    public Transform contentPanel; // GridLayoutGroup�� �߰��� Panel�� Transform
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
        string query = "SELECT * FROM pd_info;"; // ����

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
                        // �� ���� ���� �� ����
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

