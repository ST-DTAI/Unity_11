using System;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine;

public class DatabaseConnection : MonoBehaviour
{
    public static DatabaseConnection Instance { get; private set; }
    private MySqlConnection connection;
    public string ConnStr { get => $"server={host}; database={database}; uid={user}; pwd={password}; charset={charset};"; }

    [Header("DB Config")]
    public string host = "192.168.0.31";
    public string database = "clts";
    public string user = "clts";
    public string password = "clts";
    public string charset = "utf8";

    public MySqlConnection Connection => connection;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            OpenConnection();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OpenConnection()
    {
        try
        {
            string connStr = $"server={host}; database={database}; uid={user}; pwd={password}; charset={charset};";
            connection = new MySqlConnection(connStr);
            connection.Open();
            Debug.Log("MySQL Connection Opened");
        }
        catch (Exception ex)
        {
            Debug.LogError("MySQL Connection Error: " + ex.Message);
        }
    }

    private void OnDestroy()
    {
        CloseConnection();
    }

    public void CloseConnection()
    {
        if (connection != null)
        {
            connection.Close();
            Debug.Log("MySQL Connection Closed");
        }
    }
}
