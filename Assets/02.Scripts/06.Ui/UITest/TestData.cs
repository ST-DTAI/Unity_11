using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestData : MonoBehaviour
{
    private MySqlConnection connection;
    // Start is called before the first frame update
    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        if (connection != null)
        {
            ReadData(true);
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
