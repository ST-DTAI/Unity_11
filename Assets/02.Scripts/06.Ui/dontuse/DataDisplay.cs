using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour
{
    public Transform tableLayout; // ������ ǥ�ø� ���� �θ� Transform
    public GameObject rowPrefab; // Row ������
    public GameObject cellPrefab; // Cell ������
    public ScrollRect scrollRect; // ScrollRect ������Ʈ

    private void Start()
    {
        LoadData();
        //scrollRect.onValueChanged.AddListener(OnScroll);
    }

    private void LoadData()
    {
        SingletonDatabaseManager.Instance.LoadData(OnDataLoaded);
    }

    private void OnDataLoaded(List<string[]> data)
    {
        // ���� Row ����
        foreach (Transform child in tableLayout)
        {
            Destroy(child.gameObject);
        }

        foreach (var rowData in data)
        {
            // Row ������ �ν��Ͻ�ȭ
            GameObject row = Instantiate(rowPrefab, tableLayout);

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

}
