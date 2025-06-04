using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using TMPro;
using UI.Tables;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class HistorySDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row ������
    public GameObject cellPrefab; // Cell ������
    public Transform content; // TableLayout�� Transform
    public ScrollRect scrollRect; // ScrollRect ������Ʈ
    public float scrollAmount = 0.4f; // ��ũ�� ��

    private List<string[]> dataRows = new List<string[]>();

    public Toggle[] checkboxes;
    public TMP_InputField pdNoInputField;

    public TMP_Text startDateInput; // ���� ��¥ �Է� �ʵ�
    public TMP_InputField startTimeInput; // ���� �ð� �Է� �ʵ�
    public TMP_Text endDateInput; // �� ��¥ �Է� �ʵ�
    public TMP_InputField endTimeInput; // �� �ð� �Է� �ʵ�
    public Button queryButton; // ��ȸ ��ư
    public TMP_Text rowCountText;

    public Button closeButton;
    public GameObject panel;

    public Button CSVExportButton;

    private MySqlConnection connection;

    private Queue<GameObject> rowPool = new Queue<GameObject>(); // Row ������Ʈ Ǯ
    private List<GameObject> activeRows = new List<GameObject>(); // ���� Ȱ��ȭ�� Row ���

    private const int maxRows = 20; // ������ �ִ� Row ��
    private int currentRowCount = 0; // ���� Row ��
    private int totalRecords = 0; // �� ���ڵ� ��
    private int offset = 0; // ������ ������

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        // �� ���ڵ� �� �ʱ�ȭ
        //InitializeTotalRecords();

        // Row ������Ʈ �ʱ�ȭ
        for (int i = 0; i < maxRows; i++)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false); // �θ� ����
            row.SetActive(false); // ��Ȱ��ȭ
            rowPool.Enqueue(row); // Ǯ�� �߰�
        }

        scrollRect.onValueChanged.AddListener(OnScroll); // ��ũ�� �̺�Ʈ ������ �߰�

    }


    void InitializeTotalRecords()
    {
        string countQuery = "SELECT COUNT(*) FROM work_result";
        MySqlCommand countCmd = new MySqlCommand(countQuery, connection);
        totalRecords = Convert.ToInt32(countCmd.ExecuteScalar());
    }

    void LoadInitialData()
    {
        // �ʱ� ������ �ε�
        LoadData(offset);
    }

    void LoadData(int dataOffset)
    {

        // ������ �ε�
        string query = $"SELECT Time, CrNo, DrvMode, WorkType, UpAddr, DnAddr, PdNo, State, Width, Outdia, India, Thick, Weight, Date FROM work_result LIMIT {dataOffset}, {maxRows}";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        MySqlDataReader reader = cmd.ExecuteReader();

        int rowCount = 0; // ���� ǥ�õ� Row ��

        while (reader.Read() && rowCount < maxRows - 1)
        {
            // Row�� Ǯ���� �����ͼ� Ȱ��ȭ
            GameObject row = GetRowFromPool();

            // 0��° ���� �ε��� ��ȣ �߰�
            GameObject indexCell = Instantiate(cellPrefab, row.transform);
            TMP_Text indexText = indexCell.GetComponentInChildren<TMP_Text>();
            indexText.text = (dataOffset + rowCount + 1).ToString(); // 1���� �����ϴ� ��ȣ �ű��

            // 1��° ������ ������ �ʵ带 �߰�
            for (int i = 0; i < reader.FieldCount; i++)
            {
                // Cell ������ �ν��Ͻ�ȭ
                GameObject cell = Instantiate(cellPrefab, row.transform);
                TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                textComponent.text = reader[i].ToString(); // �о�� ������
            }

            rowCount++;
        }

        reader.Close(); // Reader�� �ݾ��ݴϴ�.
        currentRowCount += rowCount; // ���� Row �� ������Ʈ
    }

    public void OnAllDataButtonClick()
    {
        // ó�� ������ �ε�
        LoadInitialData();
    }

    public void OnQueryButtonClick()
    {
        // �Էµ� ��¥�� �ð��� ��������
        string startDate = startDateInput.text;
        string startTime = startTimeInput.text;
        string endDate = endDateInput.text;
        string endTime = endTimeInput.text;

        // ������ ����
        string formattedStartDate = startDate.Replace("-", ":");
        string formattedEndDate = endDate.Replace("-", ":");

        // ���۰� �� ��¥/�ð��� �����Ͽ� DateTime ��ü ����
        DateTime startDateTime = DateTime.Parse($"{startDate} {startTime}");
        DateTime endDateTime = DateTime.Parse($"{endDate} {endTime}");

        // ������ �ε�
        LoadDataByDateRange(startDateTime, endDateTime);
        Debug.Log("LoadDataByDateRange ȣ���");
    }

    void LoadDataByDateRange(DateTime startDateTime, DateTime endDateTime)
    {
        ClearRows(); // ���� Row�� ���ֱ�
        dataRows.Clear(); // ���� ������ ����

        // SQL ���� �ۼ�
        string query = "SELECT Time, CrNo, DrvMode, WorkType, UpAddr, DnAddr, PdNo, State, Width, Outdia, India, Thick, Weight, Date FROM work_result WHERE SUBSTRING(Time, 1, 14) >= @startDateTime AND SUBSTRING(Time, 1, 14) <= @endDateTime";


        // üũ�ڽ� ���� �߰�
        List<string> crNoConditions = new List<string>();

        if (checkboxes[0].isOn) // ù ��° üũ�ڽ��� ���õ� ���
        {
            crNoConditions.Add("CrNo = '11'"); // ���� �߰�
        }

        if (checkboxes[1].isOn) // �� ��° üũ�ڽ��� ���õ� ���
        {
            crNoConditions.Add("CrNo = '12'"); // ���� �߰�
        }

        if (checkboxes[2].isOn) // �� ��° üũ�ڽ��� ���õ� ���
        {
            crNoConditions.Add("CrNo = '13'"); // ���� �߰�
        }


        // üũ�ڽ��� ���õ� ��쿡�� ���� ����
        if (crNoConditions.Count > 0)
        {
            query += " AND (" + string.Join(" OR ", crNoConditions) + ")";
        }

        string pdNoValue = pdNoInputField.text.Trim(); // ���� ����

        if (!string.IsNullOrEmpty(pdNoValue)) // InputField�� ������� ���� ���
        {
            query += " AND PdNo LIKE @pdNo"; // ���� �߰�
        }

        using (MySqlCommand cmd = new MySqlCommand(query, connection))
        {

            cmd.Parameters.AddWithValue("@startDateTime", startDateTime);
            cmd.Parameters.AddWithValue("@endDateTime", endDateTime);

            if (!string.IsNullOrEmpty(pdNoValue))
            {
                cmd.Parameters.AddWithValue("@pdNo", "%" + pdNoValue + "%");
            }

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                // 0��° ���� �����ϰ� ����
                GameObject emptyRow = Instantiate(rowPrefab, content);
                for (int i = 0; i < 15; i++) // numberOfColumns�� �������� �� ��
                {
                    GameObject emptyCell = Instantiate(cellPrefab, emptyRow.transform);
                    TMP_Text emptyTextComponent = emptyCell.GetComponentInChildren<TMP_Text>();
                    emptyTextComponent.text = ""; // �ؽ�Ʈ ����
                }

                int rowCount = 0; // ���� ǥ�õ� Row ��

                while (reader.Read())
                {
                    // ������ ����
                    string[] rowData = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowData[i] = reader[i].ToString();
                    }
                    dataRows.Add(rowData); // ����Ʈ�� ������ �߰�


                    // Row�� Ǯ���� �����ͼ� Ȱ��ȭ
                    //GameObject row = GetRowFromPool();
                    GameObject row = Instantiate(rowPrefab, content);
                    // 0��° ���� �ε��� ��ȣ �߰�
                    GameObject indexCell = Instantiate(cellPrefab, row.transform);
                    TMP_Text indexText = indexCell.GetComponentInChildren<TMP_Text>();
                    indexText.text = (rowCount + 1).ToString(); // 1���� �����ϴ� ��ȣ �ű��

                    // 1��° ������ ������ �ʵ带 �߰�
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        // Cell ������ �ν��Ͻ�ȭ
                        GameObject cell = Instantiate(cellPrefab, row.transform);

                        TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                        textComponent.text = reader[i].ToString(); // �о�� ������
                    }

                    rowCount++;
                    
                }

                rowCountText.text = $"{rowCount}���� �����Ͱ� ��ȸ�Ǿ����ϴ�";
            }
        }
    }

    private void ClearFirstRow()
    {
        // 0��° ���� ����
        if (rowPool.Count > 0)
        {
            GameObject firstRow = rowPool.Dequeue(); // 0��° �� ��������
            firstRow.SetActive(false); // ��Ȱ��ȭ
            activeRows.Add(firstRow); // Ȱ��ȭ�� Row ��Ͽ� �߰�
        }
    }



    private void ClearRows()
    {
        foreach (Transform child in content)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    private GameObject GetRowFromPool()
    {
        if (rowPool.Count > 0)
        {
            GameObject row = rowPool.Dequeue(); // Ǯ���� Row ��������
            row.SetActive(true); // Ȱ��ȭ
            activeRows.Add(row); // Ȱ��ȭ�� Row ��Ͽ� �߰�
            return row;
        }
        else
        {
            // �߰� Row�� �ʿ��� ��� ���� ����
            GameObject newRow = Instantiate(rowPrefab);
            newRow.transform.SetParent(content, false);
            activeRows.Add(newRow); // Ȱ��ȭ�� Row ��Ͽ� �߰�
            return newRow;
        }
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        // ��ũ���� �ϴܿ� �����ߴ��� Ȯ��
        if (scrollPosition.y <= 0.1f) // �ϴܿ� ����� ��ġ
        {
            if (currentRowCount < totalRecords) // �� ���ڵ� ���� �ʰ����� �ʵ���
            {
                offset += maxRows; // ���� ������ ���������� �̵�
                LoadData(offset); // ���ο� ������ �ε�
            }
        }

    }


    // �г��� �ݴ� �޼���
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }


    void ExportToCSV()
    {
        // ����ȭ�� ��� ��������
        string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "data.csv");

        StringBuilder csvContent = new StringBuilder();

        // ��� �߰�
        csvContent.AppendLine("Time,CrNo,DrvMode,WorkType,UpAddr,DnAddr,PdNo,State,Width,Outdia,India,Thick,Weight,Date");

        foreach (var row in dataRows)
        {
            csvContent.AppendLine(string.Join(",", row)); // ������ �߰�
        }

        // CSV ���� ����
        File.WriteAllText(filePath, csvContent.ToString());
        Debug.Log("CSV ������ ����ȭ�鿡 ����Ǿ����ϴ�: " + filePath);
    }


    public void OnExportButtonClick()
    {
        
        ExportToCSV();
        
    }

    // �� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnClickScrollUp()
    {
        // ���� ��ũ�� ��ġ�� ����
        scrollRect.verticalNormalizedPosition += scrollAmount;
    }

    // �Ʒ� ��ư Ŭ�� �� ȣ��Ǵ� �޼���
    public void OnClickScrollDown()
    {
        // ���� ��ũ�� ��ġ�� ����
        scrollRect.verticalNormalizedPosition -= scrollAmount;
    }
}