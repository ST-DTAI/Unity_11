using MySql.Data.MySqlClient;
using System;
using System.Threading;
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
                    ReadYardMap(connection);

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[Database_Thread Error]: " + ex.Message);
                //connection.Close();
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
        int idx = 0;

        try
        {
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

                        lock (Global.dbLocks[(int)Global.DbLockType.CRSTATUS])
                            Global.CrStatusList[idx++] = crStatus;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Thread ReadCraneStatus Error]: " + ex.Message);
        }
    }
    void ReadSafeDoor(MySqlConnection conn)
    {
        const string query = "SELECT Name, State FROM unity_safedoorstatus;";

        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() && isRunning)
                    {
                        string name = reader.GetString("Name");
                        int state = reader.GetInt32("State");

                        lock (Global.dbLocks[(int)Global.DbLockType.DOORSTATE])
                            Global.DoorStateDict[name] = state;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Thread ReadSafeDoor Error]: " + ex.Message);
        }
    }
    void ReadYardMap(MySqlConnection conn)
    {
        const string query = "SELECT SkidNo, Dong, Skid, Sect, DxNo, DyNo, DzNo, Addr, Dx, Dy, Dz, Dir, MaxWid, MaxDia, PdYN, Hold, CrRev, SupRev, OutRev, FwdYN, BwdYN, PdNo, State, Width, Outdia, India, Thick, Weight, Temp, Date, ToNo FROM yard_map ORDER BY Skid, SkidNo;";
        int idx = 0;

        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read() && isRunning)
                    {
                        YardMap newYardMap = new YardMap(
                            reader.GetInt32("skidNo"),
                            reader.GetInt32("dong"),
                            reader.GetString("skid"),
                            reader.GetInt32("sect"),
                            reader.GetInt32("dxNo"),
                            reader.GetInt32("dyNo"),
                            reader.GetInt32("dzNo"),
                            reader.GetString("addr"),

                            reader.GetFloat("dx"),
                            reader.GetFloat("dy"),
                            reader.GetFloat("dz"),
                            reader.GetFloat("dir"),
                            reader.GetInt32("maxWid"),
                            reader.GetInt32("maxDia"),
                            reader.GetString("pdYN"),
                            reader.GetString("hold"),
                            reader.GetInt32("crRev"),
                            reader.GetInt32("supRev"),
                            reader.GetInt32("outRev"),
                            reader.GetString("fwdYN"),
                            reader.GetString("bwdYN"),

                            reader.GetString("pdNo"),
                            reader.GetString("state"),
                            reader.GetInt32("width"),
                            reader.GetInt32("outdia"),
                            reader.GetInt32("india"),
                            reader.GetFloat("thick"),
                            reader.GetInt32("weight"),
                            reader.GetInt32("temp"),
                            reader.GetString("date"),
                            reader.GetString("toNo")
                        );

                        lock (Global.dbLocks[(int)Global.DbLockType.YARDMAP])
                            Global.YardMapList[idx++] = newYardMap;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Thread ReadYardMap Error]: " + ex.Message);
        }
    }
}
