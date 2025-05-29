using System;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class DataDisplay : MonoBehaviour
{
    public Transform tableLayout; // 데이터 표시를 위한 부모 Transform
    public GameObject rowPrefab; // Row 프리팹
    public GameObject cellPrefab; // Cell 프리팹
    public ScrollRect scrollRect; // ScrollRect 컴포넌트

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
        // 기존 Row 삭제
        foreach (Transform child in tableLayout)
        {
            Destroy(child.gameObject);
        }

        foreach (var rowData in data)
        {
            // Row 프리팹 인스턴스화
            GameObject row = Instantiate(rowPrefab, tableLayout);

            // 각 데이터 필드를 Cell로 추가
            for (int i = 0; i < rowData.Length; i++)
            {
                // Cell 프리팹 인스턴스화
                GameObject cell = Instantiate(cellPrefab, row.transform);
                TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();
                textComponent.text = rowData[i]; // 읽어온 데이터
            }
        }

        // Layout Group과 Content Size Fitter 업데이트
        LayoutRebuilder.ForceRebuildLayoutImmediate(tableLayout.GetComponent<RectTransform>());
    }

}
