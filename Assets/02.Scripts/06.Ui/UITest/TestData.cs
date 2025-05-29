using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestData : MonoBehaviour
{
    private DatabaseConnection dbConnection;
    private MySqlConnection connection;
    // Start is called before the first frame update
    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        if (connection != null)
        {
            ReadData(true);
        }
    }

    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.CloseConnection();
        }
    }

    private void ReadData(bool initialLoad)
    {
        string query = "";

        if (connection != null) 
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection)) 
            {
                using (MySqlDataReader reader = cmd.ExecuteReader()) 
                {
                    while (reader.Read()) 
                    {
                        
                    }
                }

            }
        }


    }
}
