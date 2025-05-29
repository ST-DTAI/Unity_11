using System;
using MySql.Data.MySqlClient;
using TMPro;
using UnityEngine;

public class DatabaseConnection
{
    private string host = "192.168.0.31";
    private string database = "clts";
    private string user = "clts";
    private string password = "clts";
    private string charset = "utf8";

    private MySqlConnection connection;

    

    public MySqlConnection OpenConnection()
    {
        try
        {
            string connectionString = string.Format("server={0}; database={1}; uid={2}; pwd={3}; charset={4};",
                                                    host, database, user, password, charset);
            connection = new MySqlConnection(connectionString);
            connection.Open();
            Debug.Log("MySQL Connection Opened");
            return connection;
        }
        catch (Exception ex)
        {
            Debug.LogError("MySQL Connection Error: " + ex.Message);
            return null;
        }
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
