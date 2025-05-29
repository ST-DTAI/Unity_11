using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UI.Tables;
using UnityEngine;
using UnityEngine.UI;

public class CraneInfoPanelManager : MonoBehaviour
{

    public Button closeButton;
    public GameObject panel;

    public int crNo = 11;

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        Loadworkorder();
        LoadCoilInfo();
    }


    void Loadworkorder()
    {
        {
            try
            {
                // ����: Up% �� Dn% ������ ��������
                string query = $"SELECT UpAddr, UpDx, UpDy, UpDz, DnAddr, DnDx, DnDy, DnDz FROM clts.cr_view WHERE CrNo = {crNo}";
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
                        rowData[i] = reader[i]?.ToString() ?? "-";
                    }
                    dataRows.Add(rowData);
                }

                reader.Close(); // ���� �ݱ�

                // ù ��° �� (Up% ������)
                if (dataRows.Count > 0) // �����Ͱ� �ִ� ���
                {
                    // ���� ���, row1�� �̹� �����ϴ� ���
                    GameObject row1 = GameObject.Find("RowUpAddr"); // Row1�� �̸����� GameObject�� ã��
                    Debug.Log("Using existing Row1 for Up% data");

                    // Up% ������ ä��� (0~3 �ε���)
                    for (int i = 0; i < 4; i++) // 0��° cell�� �ǳʶٰ� 1~3 �ε���
                    {
                        // Row1�� i��° �ڽ� cell���� TMP_Text�� ã��
                        GameObject cell = row1.transform.GetChild(i+1).gameObject; // i��° �ڽ� cell
                        TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();  // TMP_Text ������Ʈ ��������

                        if (textComponent == null)
                        {
                            Debug.LogError("TMP_Text component not found in existing cell!"); // �ؽ�Ʈ ������Ʈ Ȯ��
                        }

                        textComponent.text = dataRows[0][i]; // �о�� ������
                        Debug.Log($"Setting cell text for Row1 column {i}: {textComponent.text}");
                    }
                }

                // �� ��° �� (Dn% ������)
                if (dataRows.Count > 0) // �����Ͱ� �ִ� ���
                {
                    // ���� ���, row2�� �̹� �����ϴ� ���
                    GameObject row2 = GameObject.Find("RowDnAddr"); // Row2�� �̸����� GameObject�� ã��
                    Debug.Log("Using existing Row2 for Dn% data");

                    // Dn% ������ ä��� (4~7 �ε���)
                    for (int i = 4; i < 8; i++) // Dn% �����ʹ� 4~7 �ε���
                    {
                        // Row2�� (i - 4 + 1)��° �ڽ� cell���� TMP_Text�� ã��
                        GameObject cell = row2.transform.GetChild(i - 4 + 1).gameObject; // 0��° cell�� �ǳʶٱ� ���� i - 4 + 1
                        TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();  // TMP_Text ������Ʈ ��������

                        if (textComponent == null)
                        {
                            Debug.LogError("TMP_Text component not found in existing cell!"); // �ؽ�Ʈ ������Ʈ Ȯ��
                        }

                        textComponent.text = dataRows[0][i]; // �о�� ������
                        Debug.Log($"Setting cell text for Row2 column {i - 4 + 1}: {textComponent.text}");
                    }
                }
                else
                {
                    Debug.LogWarning("No data found in the query result."); // �����Ͱ� ���� ��� ��� �α�
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}"); // ���� �޽��� ���
            }
        }
    }

    void LoadCoilInfo()
    {
        try
        {
            // ����: WorkPdNo, Width, Weight, Outdia ������ ��������
            string query = $"SELECT WorkPdNo, Width, Weight, Outdia FROM clts.cr_view WHERE CrNo = {crNo}";
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

            // ù ��° ���� �����Ͱ� �ִ��� Ȯ��
            if (dataRows.Count > 0)
            {
                // �����Ͱ� �ִ� ���, ù ��° ���� ������ ��������
                string workPdNo = dataRows[0][0]; // WorkPdNo
                string width = dataRows[0][1];     // Width
                string weight = dataRows[0][2];    // Weight
                string outdia = dataRows[0][3];    // Outdia

                // ù ��° �� (WorkPdNo ������)
                GameObject row1 = GameObject.Find("RowPdNo");
                GameObject cell1 = row1.transform.GetChild(1).gameObject;
                TMP_Text textComponent1 = cell1.GetComponentInChildren<TMP_Text>();

                if (textComponent1 != null)
                {
                    textComponent1.text = workPdNo; // WorkPdNo ������
                    Debug.Log($"Setting cell text for RowPdNo: {textComponent1.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowPdNo!");
                }

                // �� ��° �� (Width ������)
                GameObject row2 = GameObject.Find("RowWidth");
                GameObject cell2 = row2.transform.GetChild(1).gameObject;
                TMP_Text textComponent2 = cell2.GetComponentInChildren<TMP_Text>();

                if (textComponent2 != null)
                {
                    textComponent2.text = width; // Width ������
                    Debug.Log($"Setting cell text for RowWidth: {textComponent2.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowWidth!");
                }

                // �� ��° �� (Weight ������)
                GameObject row3 = GameObject.Find("RowWeight");
                GameObject cell3 = row3.transform.GetChild(1).gameObject;
                TMP_Text textComponent3 = cell3.GetComponentInChildren<TMP_Text>();

                if (textComponent3 != null)
                {
                    textComponent3.text = weight; // Weight ������
                    Debug.Log($"Setting cell text for RowWeight: {textComponent3.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowWeight!");
                }

                // �� ��° �� (Outdia ������)
                GameObject row4 = GameObject.Find("RowOutdia");
                GameObject cell4 = row4.transform.GetChild(1).gameObject;
                TMP_Text textComponent4 = cell4.GetComponentInChildren<TMP_Text>();

                if (textComponent4 != null)
                {
                    textComponent4.text = outdia; // Outdia ������
                    Debug.Log($"Setting cell text for RowOutdia: {textComponent4.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowOutdia!");
                }
            }
            else
            {
                Debug.LogWarning("No data found in the query result."); // �����Ͱ� ���� ��� ��� �α�
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}"); // ���� �޽��� ���
        }
    }






    // �г��� �ݴ� �޼���
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }

}
