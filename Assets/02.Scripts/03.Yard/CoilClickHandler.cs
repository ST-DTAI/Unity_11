using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static YardMapManager;



public class CoilClickHandler : MonoBehaviour
{

    public YardMap coilData; // 이 오브젝트에 해당하는 코일 데이터
    public Material clickedMaterial; // 클릭 시 적용할 새로운 Material
    public Material originalMaterial; // 원래 Material

    private YardMapManager YardMapManager;
    private static CoilClickHandler lastClickedHandler;
    private Renderer coilRenderer;

    void Start()
    {
        coilRenderer = GetComponent<Renderer>();
        if (coilRenderer != null)
        {
            originalMaterial = coilRenderer.material;
        }
        else
        {
            Debug.LogWarning($"[CoilClickHandler] {gameObject.name} 에 Renderer가 없습니다.");
        }

    }

    private void OnMouseDown()
    {
        Debug.Log("Coil Clicked!");

        // UI 업데이트
        UiManager uiManager = FindObjectOfType<UiManager>();
        if (uiManager != null)
        {
            if (!EqualityComparer<YardMap>.Default.Equals(coilData, default(YardMap)))
            {
                Debug.Log("Coil Data: " + coilData.PdNo);
                uiManager.UpdateSkidUI(coilData);
            }
            else
            {
                Debug.LogWarning("coilData is default value.");
            }
        }
        else
        {
            Debug.LogWarning("UiManager not found.");
        }

        // 이전 선택된 코일 머티리얼 복원
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null && lastClickedHandler.originalMaterial != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // 현재 코일에 선택 머티리얼 적용
        if (coilRenderer != null && clickedMaterial != null)
        {
            coilRenderer.material = clickedMaterial;

            Debug.Log("New Material set: " + clickedMaterial.name);
            if (clickedMaterial.shader != null)
                Debug.Log("Shader used: " + clickedMaterial.shader.name);
            else
                Debug.LogWarning("New Material does not have a valid shader.");
        }
        else
        {
            Debug.LogWarning("Outline material or renderer is null.");
        }

        // 현재 핸들러 저장
        lastClickedHandler = this;
    }

}


