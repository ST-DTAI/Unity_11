using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkidClickHandler : MonoBehaviour
{
    public YardMap skidData; // 클릭된 스키드의 데이터

    public Material outlineMaterial; // 클릭 시 적용할 머티리얼
    public Material originalMaterial; // 원래 머티리얼

    private static SkidClickHandler lastClickedHandler;
    private Renderer myRenderer;

    [SerializeField] private Color selectedColor = Color.yellow;

    void Start()
    {
        myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
        {
            originalMaterial = myRenderer.material;
        }
        else
        {
            Debug.LogWarning($"SkidClickHandler: {gameObject.name}에 Renderer가 없습니다.");
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"[Skid Clicked] {skidData.Skid} (SkidNo: {skidData.SkidNo})");

        // 이전 선택된 Skid 색 복원
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null && lastClickedHandler.originalMaterial != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // 현재 Skid에 outline 머티리얼 적용
        if (myRenderer != null && outlineMaterial != null)
        {
            myRenderer.material = outlineMaterial;
        }

        // ButtonTaskController에 dx, dy 전달
        ButtonTaskController.Instance?.SetSelectedSkid(skidData);

        // 현재 핸들러 저장
        lastClickedHandler = this;
    }
}
