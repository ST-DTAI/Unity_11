using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataDisplayManager : MonoBehaviour
{
    public Transform tableLayout; // 데이터 표시를 위한 부모 Transform
    public GameObject rowPrefab; // Row 프리팹
    public GameObject cellPrefab; // Cell 프리팹
    public ScrollRect scrollRect; // ScrollRect 컴포넌트

    public float threshold = 0.1f; // 스크롤 하단에 도달하는 임계값
    private int currentOffset = 0; // 현재 데이터 오프셋
    private List<GameObject> activeRows = new List<GameObject>(); // 현재 활성화된 Row 리스트

    private void Start()
    {
        LoadData(); // 초기 데이터 로드
        GetComponent<ScrollRect>().onValueChanged.AddListener(OnScroll); // 스크롤 이벤트 리스너 추가
    }

    private void LoadData()
    {
        SingletonDatabaseManager.Instance.LoadData(OnDataLoaded); // 데이터 로드
    }

    private void OnDataLoaded(List<string[]> data)
    {
        foreach (var rowData in data)
        {
            // Row 프리팹 인스턴스화
            GameObject row = Instantiate(rowPrefab, tableLayout);
            activeRows.Add(row); // 현재 Row 리스트에 추가

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

    private void OnScroll(Vector2 scrollPosition)
    {
        if (scrollPosition.y <= threshold) // 하단에 가까운 위치
        {
            currentOffset += SingletonDatabaseManager.Instance.GetTotalRecords(); // 다음 데이터 오프셋으로 이동
            LoadData(); // 새로운 데이터 로드
        }
        else if (scrollPosition.y >= 1 - threshold) // 상단에 가까운 위치
        {
            if (currentOffset > 0)
            {
                currentOffset -= SingletonDatabaseManager.Instance.GetTotalRecords(); // 이전 데이터 오프셋으로 이동
                LoadData(); // 이전 데이터 로드
            }
        }
    }

}
