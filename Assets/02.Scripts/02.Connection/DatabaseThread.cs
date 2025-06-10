using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Ocsp;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.MemoryProfiler;
using UnityEditor.Search;
using UnityEngine;

public class DatabaseThread : MonoBehaviour
{
    Thread thread_Database;
    bool isRunning = false;
    void Start()
    {
        ThreadStart();
    }
    private void OnDestroy()
    {
        if (isRunning)
        {
            isRunning = false;
            if (thread_Database != null)
            {
                thread_Database.Join(5000); // ±â´Ù·È´Ù°¡
                thread_Database = null;
                Debug.Log("==DatabaseThread destroyed and thread stopped==");
            }
        }
    }
    void ThreadStart()
    {
        if (thread_Database != null)
        {
            isRunning = false;
            thread_Database.Join(); // ±â´Ù·È´Ù°¡
            thread_Database = null;
            return;
        }

        isRunning = true;
        thread_Database = new Thread(Database_Thread);
        thread_Database.IsBackground = true;
        thread_Database.Start();
    }
    void Database_Thread()
    {
        while (true)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(DatabaseConnection.Instance.ConnStr))
                {
                    connection.Open();

                    ReadCraneStatus(connection);
                    ReadSafeDoor(connection);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[Database_Thread Error]: " + ex.Message);
            }

            Thread.Sleep(1000);
        }
    }

    void ReadCraneStatus(MySqlConnection conn)
    {
        if (Global.CrStatusList.Count == 0)
            return;

        string query = $"SELECT CrNo, `Status`, Locus, GoalDx, GoalDy, GoalDz, Addr, PdNo, " +
                           $"Dx, Dy, Dz, SwivAng, ArmWid, LdWeight, TLSway, TSSway, Temp, ErrCode, Input, Output, " +
                           $"ComChk, CycleTime FROM cr_status ORDER BY CrNo;";
        int crIdx = 0;

        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read() && isRunning)
                {
                    CrStatus crStatus = new CrStatus(
                        reader.GetInt32("crNo"),
                        reader.GetString("status"),
                        reader.GetInt32("locus"),
                        reader.GetFloat("goaldx"),
                        reader.GetFloat("goaldy"),
                        reader.GetFloat("goaldz"),
                        reader.GetString("addr"),
                        reader.GetString("pdNo"),
                        reader.GetFloat("dx"),
                        reader.GetFloat("dy"),
                        reader.GetFloat("dz"),
                        reader.GetInt32("swivAng"),
                        reader.GetInt32("armWid"),
                        reader.GetInt32("ldWeight"),
                        reader.GetInt32("tlsway"),
                        reader.GetInt32("tssway"),
                        reader.GetInt32("temp"),
                        reader.GetInt32("errCode"),
                        reader.GetInt32("input"),
                        reader.GetInt32("output"),
                        reader.GetInt32("comChk"),
                        reader.GetInt32("cycleTime")
                    );

                    Global.CrStatusList[crIdx++] = crStatus;
                }
            }
        }
    }
    void ReadSafeDoor(MySqlConnection conn)
    {
        const string query = "SELECT Name, State FROM unity_safedoorstatus;";

        using (MySqlCommand cmd = new MySqlCommand(query, conn))
        {
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read() && isRunning)
                {
                    string name = reader.GetString("Name");
                    int state = reader.GetInt32("State");

                    Global.DoorStateDict[name] = state;
                }
            }
        }
    }
}
