using MySql.Data.MySqlClient;
using System;
using UnityEngine;

public class DatabaseConnection : MonoBehaviour
{
    public static DatabaseConnection Instance { get; private set; }
    private MySqlConnection connection;
    public string ConnStr { get => $"server={Global.host}; database={Global.database}; uid={Global.user}; pwd={Global.password}; charset={Global.charset}; pooling=true;"; }

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
            connection = new MySqlConnection(ConnStr);
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
