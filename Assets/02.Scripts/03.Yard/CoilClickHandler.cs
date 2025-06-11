using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static YardMapManager;



public class CoilClickHandler : MonoBehaviour
{

    public YardMap coilData; // �� ������Ʈ�� �ش��ϴ� ���� ������
    public Material clickedMaterial; // Ŭ�� �� ������ ���ο� Material
    public Material originalMaterial; // ���� Material

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
            Debug.LogWarning($"[CoilClickHandler] {gameObject.name} �� Renderer�� �����ϴ�.");
        }

    }

    private void OnMouseDown()
    {
        Debug.Log("Coil Clicked!");

        // UI ������Ʈ
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

        // ���� ���õ� ���� ��Ƽ���� ����
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null && lastClickedHandler.originalMaterial != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // ���� ���Ͽ� ���� ��Ƽ���� ����
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

        // ���� �ڵ鷯 ����
        lastClickedHandler = this;
    }

}


