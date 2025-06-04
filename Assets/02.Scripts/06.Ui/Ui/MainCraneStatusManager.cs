using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainCraneStatusManager : MonoBehaviour
{

    private MySqlConnection connection;
    // Start is called before the first frame update
    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        LoadCraneStatus();
    }


    void LoadCraneStatus()
    {
        try
        {
            // ����: CrPdNo, UpAddr, DnAddr, Status ������ ��������
            string query = $"SELECT PdNo, UpAddr,DnAddr FROM clts.work_order";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            // ����� ������ ����Ʈ
            List<string[]> dataRows = new List<string[]>();

            // ��� ���� ����Ʈ�� ����
            while (reader.Read())
            {
                string[] rowData = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowData[i] = reader[i].ToString();
                }
                dataRows.Add(rowData);
            }

            reader.Close(); // ���� �ݱ�

            // �����Ͱ� �ִ� ��� ó��
            for (int rowIndex = 0; rowIndex < dataRows.Count; rowIndex++)
            {
                // �� �࿡ ���� GameObject�� ã��
                string rowName = $"RowCrane1{rowIndex + 1}"; // ��: Row1, Row2, Row3, ...
                GameObject row = GameObject.Find(rowName);
                if (row == null)
                {
                    Debug.LogError($"GameObject {rowName} not found!");
                    continue; // ���� ������ �Ѿ�ϴ�.
                }

                // �� ���� �����͸� ����
                for (int colIndex = 0; colIndex < dataRows[rowIndex].Length; colIndex++)
                {
                    // Row�� (colIndex + 1)��° �ڽ� cell���� TMP_Text�� ã��
                    GameObject cell = row.transform.GetChild(colIndex+1).gameObject; // colIndex ��° �ڽ� cell
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>(); // TMP_Text ������Ʈ ��������

                    if (textComponent == null)
                    {
                        Debug.LogError($"TMP_Text component not found in {rowName} cell {colIndex}!"); // �ؽ�Ʈ ������Ʈ Ȯ��
                        continue; // ���� cell�� �Ѿ�ϴ�.
                    }

                    textComponent.text = dataRows[rowIndex][colIndex]; // �о�� ������
                    
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}"); // ���� �޽��� ���
        }
    }
}
