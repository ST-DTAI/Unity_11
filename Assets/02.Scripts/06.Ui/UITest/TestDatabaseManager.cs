using System.Collections;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;

public class TestDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row ������
    public GameObject cellPrefab; // Cell ������
    public GameObject indexRow;

    public Transform tableLayout; // TableLayout�� Transform

    public Toggle[] checkboxes; // üũ�ڽ� �迭
    public Button loadButton; // �����͸� �ε��� ��ư
    public Button exportButton; // CSV�� �������� ��ư

  
    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {

        // ��ư Ŭ�� �̺�Ʈ �߰�
        loadButton.onClick.AddListener(LoadData); // ��ư Ŭ�� �� LoadData �޼��� ȣ��
        exportButton.onClick.AddListener(ExportToCSV); // CSV �������� ��ư Ŭ�� �� ExportToCSV ȣ��


        // ��� üũ�ڽ��� üũ ���� ���·� �ʱ�ȭ
        foreach (Toggle toggle in checkboxes)
        {
            toggle.isOn = false; // üũ�ڽ� �ʱ� ���¸� üũ ����
        }
        connection = DatabaseConnection.Instance.Connection;
        // ó�� ������ �ε� (üũ�ڽ� ���¿� �������)
        LoadInitialData();
    }


    void LoadInitialData()
    {
        // ���� UI �ʱ�ȭ
        for (int i = tableLayout.childCount - 1; i >= 0; i--)
        {
            Transform child = tableLayout.GetChild(i);

            // indexRow�� ���� GameObject�� �������� ����
            if (child.gameObject != indexRow)
            {
                Destroy(child.gameObject);
            }
        }

        try
        {
            // ��� �����͸� �������� �⺻ ����
            string query = "SELECT * FROM w1_wbsupply"; // �⺻ ����

            Debug.Log("Initial Query: " + query);

            MySqlCommand cmd = new MySqlCommand(query, connection); // ���� ���� ���� ��ɾ� ����
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Row ������ �ν��Ͻ�ȭ
                GameObject row = Instantiate(rowPrefab, tableLayout);

                // �� ������ �ʵ带 Cell�� �߰�
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Cell ������ �ν��Ͻ�ȭ
                    GameObject cell = Instantiate(cellPrefab, row.transform);
                    // tmpText�� ������ ����
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                    textComponent.text = reader[i].ToString(); // �о�� ������
                }
            }

            reader.Close(); // Reader�� �ݾ��ݴϴ�.
        }
        catch (Exception ex)
        {
            Debug.LogError("Database error: " + ex.Message);
        }
    }

    void LoadData()
    {
        // ���� UI �ʱ�ȭ
        for (int i = tableLayout.childCount - 1; i >= 0; i--)
        {
            Transform child = tableLayout.GetChild(i);

            // indexRow�� ���� GameObject�� �������� ����
            if (child.gameObject != indexRow)
            {
                Destroy(child.gameObject);
            }
        }

        try
        {
            // �⺻ ����
            string query = "SELECT * FROM w1_wbsupply WHERE 1=1"; // �⺻ ����

            // üũ�ڽ� ���¿� ���� ���� �߰�
            if (checkboxes[0].isOn) // ù ��° üũ�ڽ��� ���õ� ���
            {
                query += " AND CrRev = '0'"; // ���� �߰�
            }

            if (checkboxes[1].isOn) // �� ��° üũ�ڽ��� ���õ� ���
            {
                query += " AND OdNo = '1024'"; // ���� �߰�
            }

            Debug.Log("Generated Query: " + query);

            MySqlCommand cmd = new MySqlCommand(query, connection); // ���� ���� ���� ��ɾ� ����
            MySqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Row ������ �ν��Ͻ�ȭ
                GameObject row = Instantiate(rowPrefab, tableLayout);

                // �� ������ �ʵ带 Cell�� �߰�
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Cell ������ �ν��Ͻ�ȭ
                    GameObject cell = Instantiate(cellPrefab, row.transform);
                    // tmpText�� ������ ����
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                    textComponent.text = reader[i].ToString(); // �о�� ������
                }
            }

            reader.Close(); // Reader�� �ݾ��ݴϴ�.
        }
        catch (Exception ex)
        {
            Debug.LogError("Database error: " + ex.Message);
        }
    }

    void ExportToCSV()
    {
        try
        {
            // ����ũž ��� ����
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string folderPath = Path.Combine(desktopPath, "csvtest"); //��ž�� ���� ���
            //string folderPath = @"C:\Users\Sohyun\Desktop\csvtest" //��� �ϵ��ڵ����

            string filePath = Path.Combine(folderPath, "data.csv"); //������ data.csv ���� ��� ����


            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // CSV ��� �ۼ� (�ʿ信 ���� ����)
                writer.WriteLine("w1_wbsupply"); // ���⼭�� ���÷� ����� �ۼ��մϴ�.

                // ��� �����͸� �������� �⺻ ����
                string query = "SELECT * FROM w1_wbsupply";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    // �����͸� CSV �������� �ۼ�
                    string line = "";
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        line += reader[i].ToString();
                        if (i < reader.FieldCount - 1)
                        {
                            line += ","; // �ʵ� ������
                        }
                    }
                    writer.WriteLine(line); // �� �پ� CSV�� �ۼ�
                }

                reader.Close(); // Reader�� �ݾ��ݴϴ�.
            }

            Debug.Log("CSV ������ ����Ǿ����ϴ�: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("CSV export error: " + ex.Message);
        }
    }
}
