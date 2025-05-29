using System;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;

public class SingletonDatabaseManager : MonoBehaviour
{
    public static SingletonDatabaseManager Instance { get; private set; }

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    private const int pageSize = 20; // �� �������� ǥ���� ������ ��
    private int totalRecords = 0; // �� ���ڵ� ��

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� ��ȯ �ÿ��� ����
            InitializeDatabase();
            InitializeTotalRecords(); // �� ���ڵ� �� �ʱ�ȭ
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
        callback(pageData); // �ݹ����� ������ ��ȯ
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
