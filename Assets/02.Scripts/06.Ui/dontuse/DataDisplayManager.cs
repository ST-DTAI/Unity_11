using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataDisplayManager : MonoBehaviour
{
    public Transform tableLayout; // ������ ǥ�ø� ���� �θ� Transform
    public GameObject rowPrefab; // Row ������
    public GameObject cellPrefab; // Cell ������
    public ScrollRect scrollRect; // ScrollRect ������Ʈ

    public float threshold = 0.1f; // ��ũ�� �ϴܿ� �����ϴ� �Ӱ谪
    private int currentOffset = 0; // ���� ������ ������
    private List<GameObject> activeRows = new List<GameObject>(); // ���� Ȱ��ȭ�� Row ����Ʈ

    private void Start()
    {
        LoadData(); // �ʱ� ������ �ε�
        GetComponent<ScrollRect>().onValueChanged.AddListener(OnScroll); // ��ũ�� �̺�Ʈ ������ �߰�
    }

    private void LoadData()
    {
        SingletonDatabaseManager.Instance.LoadData(OnDataLoaded); // ������ �ε�
    }

    private void OnDataLoaded(List<string[]> data)
    {
        foreach (var rowData in data)
        {
            // Row ������ �ν��Ͻ�ȭ
            GameObject row = Instantiate(rowPrefab, tableLayout);
            activeRows.Add(row); // ���� Row ����Ʈ�� �߰�

            // �� ������ �ʵ带 Cell�� �߰�
            for (int i = 0; i < rowData.Length; i++)
            {
                // Cell ������ �ν��Ͻ�ȭ
                GameObject cell = Instantiate(cellPrefab, row.transform);
                TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                textComponent.text = rowData[i]; // �о�� ������
            }
        }

        // Layout Group�� Content Size Fitter ������Ʈ
        LayoutRebuilder.ForceRebuildLayoutImmediate(tableLayout.GetComponent<RectTransform>());
    }

    private void OnScroll(Vector2 scrollPosition)
    {
        if (scrollPosition.y <= threshold) // �ϴܿ� ����� ��ġ
        {
            currentOffset += SingletonDatabaseManager.Instance.GetTotalRecords(); // ���� ������ ���������� �̵�
            LoadData(); // ���ο� ������ �ε�
        }
        else if (scrollPosition.y >= 1 - threshold) // ��ܿ� ����� ��ġ
        {
            if (currentOffset > 0)
            {
                currentOffset -= SingletonDatabaseManager.Instance.GetTotalRecords(); // ���� ������ ���������� �̵�
                LoadData(); // ���� ������ �ε�
            }
        }
    }

}
