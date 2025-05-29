using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class CoilClickHandler2 : MonoBehaviour, IPointerClickHandler
{

    public YardSkidCoil coilData2; // 이 오브젝트에 해당하는 코일 데이터
    public Material outlineMaterial; // 클릭 시 적용할 새로운 Material
    public Material originalMaterial; // 원래 Material

    private YardSkidDbManager2 yardSkidDbManager2;
    private static CoilClickHandler2 lastClickedHandler;

    void Start()
    {
        yardSkidDbManager2 = FindObjectOfType<YardSkidDbManager2>();
        // 원래의 Material을 저장
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Object Clicked!");

        // DataManager의 coilsList에 접근
        if (yardSkidDbManager2 != null)
        {
            List<YardSkidCoil> coilsList = yardSkidDbManager2.CoilsList;

        }

        // UiManager 인스턴스 찾기
        UiManager2 uiManager2 = FindObjectOfType<UiManager2>();
        if (uiManager2 != null)
        {
            // coilData가 기본 값과 같은지 확인
            if (!EqualityComparer<YardSkidCoil>.Default.Equals(coilData2, default(YardSkidCoil)))
            {
                Debug.Log("Coil Data: " + coilData2.PdNo);
                uiManager2.UpdateSkidUI(coilData2); // UiManager의 UI 업데이트 메서드 호출
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

        // 이전에 클릭된 오브젝트의 Material을 원래 상태로 복원
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // 현재 클릭된 오브젝트의 Material 변경
        Renderer currentRenderer = GetComponent<Renderer>();
        if (currentRenderer != null)
        {
            currentRenderer.material = outlineMaterial;
        }

        if (outlineMaterial != null)
        {
            Debug.Log("New Material set: " + outlineMaterial.name);
            if (outlineMaterial.shader != null)
            {
                Debug.Log("Shader used: " + outlineMaterial.shader.name);
            }
            else
            {
                Debug.LogWarning("New Material does not have a valid shader.");
            }
        }
        else
        {
            Debug.LogWarning("New Material is null.");
        }

        // 마지막으로 클릭된 핸들러를 현재 핸들러로 업데이트
        lastClickedHandler = this;
    }

}



