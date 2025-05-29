using System;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class SingletonDatabaseManager : MonoBehaviour
{
    public static SingletonDatabaseManager Instance { get; private set; }

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    private const int pageSize = 20; // 한 페이지에 표시할 데이터 수
    private int totalRecords = 0; // 총 레코드 수

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지
            InitializeDatabase();
            InitializeTotalRecords(); // 총 레코드 수 초기화
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeDatabase()
    {

        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();
    }

    public void LoadData(Action<List<string[]>> callback)
    {
        string query = $"SELECT Time, CrNo, DrvMode, WorkType, UpAddr, DnAddr, PdNo, State, Width, Outdia, India, Thick, Weight, Date FROM work_result";

        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        List<string[]> pageData = new List<string[]>();

        while (reader.Read())
        {
            string[] rowData = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                rowData[i] = reader[i].ToString();
            }
            pageData.Add(rowData);
        }

        reader.Close();
        callback(pageData); // 콜백으로 데이터 반환
    }

    public int GetTotalRecords()
    {
        return totalRecords;
    }

    private void InitializeTotalRecords()
    {
        string countQuery = "SELECT COUNT(*) FROM work_result";
        MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
        totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
    }
}
