using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using Unity.VisualScripting;

public class HistoryDatabaseManager : MonoBehaviour
{
    public GameObject rowPrefab; // Row ������
    public GameObject cellPrefab; // Cell ������
    

    public Transform content; // TableLayout�� Transform
    public ScrollRect scrollRect; // ScrollRect ������Ʈ

    //public Button exportButton; // CSV�� �������� ��ư

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    private Queue<GameObject> rowPool = new Queue<GameObject>(); // Row ������Ʈ Ǯ
    private List<GameObject> activeRows = new List<GameObject>(); // ���� Ȱ��ȭ�� Row ���

    private const int maxRows = 20; // ������ �ִ� Row ��
    private int currentRowCount = 0; // ���� Row ��
    private int totalRecords = 0; // �� ���ڵ� ��
    private int offset = 0; // ������ ������

    // Start is called before the first frame update
    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        // �� ���ڵ� �� �ʱ�ȭ
        InitializeTotalRecords();

        // Row ������Ʈ �ʱ�ȭ
        for (int i = 0; i < maxRows; i++)
        {
            GameObject row = Instantiate(rowPrefab);
            row.transform.SetParent(content, false); // �θ� ����
            row.SetActive(false); // ��Ȱ��ȭ
            rowPool.Enqueue(row); // Ǯ�� �߰�
        }

        // 0��° ���� ����
        ClearFirstRow();

        // ó�� ������ �ε�
        LoadInitialData();
        scrollRect.onValueChanged.AddListener(OnScroll); // ��ũ�� �̺�Ʈ ������ �߰�

    }

    void OnDestroy()
    {
        if (dbConnection != null)
        {
            dbConnection.CloseConnection();
        }
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

        while (reader.Read() && rowCount < maxRows-1)
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
        // ���� Row�� ��Ȱ��ȭ�ϰ� Ǯ�� ��ȯ
        foreach (GameObject row in activeRows)
        {
            row.SetActive(false);
            rowPool.Enqueue(row); // Ǯ�� ��ȯ
        }
        activeRows.Clear(); // ����Ʈ �ʱ�ȭ
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
        // ��� ��ũ�� ���� (�ɼ�)
        else if (scrollPosition.y >= 0.9f) // ��ܿ� ����� ��ġ
        {
            if (offset > 0)
            {
                offset = Mathf.Max(0, offset - maxRows); // ���� ������ ���������� �̵�
                //ClearRows(); // ���� Row ��Ȱ��ȭ
                LoadData(offset); // ���� ������ �ε�
            }
        }
    }


}
