using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;



public class CoilClickHandler2 : MonoBehaviour, IPointerClickHandler
{

    public YardSkidCoil coilData2; // �� ������Ʈ�� �ش��ϴ� ���� ������
    public Material outlineMaterial; // Ŭ�� �� ������ ���ο� Material
    public Material originalMaterial; // ���� Material

    private YardSkidDbManager2 yardSkidDbManager2;
    private static CoilClickHandler2 lastClickedHandler;

    void Start()
    {
        yardSkidDbManager2 = FindObjectOfType<YardSkidDbManager2>();
        // ������ Material�� ����
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            originalMaterial = renderer.material;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Object Clicked!");

        // DataManager�� coilsList�� ����
        if (yardSkidDbManager2 != null)
        {
            List<YardSkidCoil> coilsList = yardSkidDbManager2.CoilsList;

        }

        // UiManager �ν��Ͻ� ã��
        UiManager2 uiManager2 = FindObjectOfType<UiManager2>();
        if (uiManager2 != null)
        {
            // coilData�� �⺻ ���� ������ Ȯ��
            if (!EqualityComparer<YardSkidCoil>.Default.Equals(coilData2, default(YardSkidCoil)))
            {
                Debug.Log("Coil Data: " + coilData2.PdNo);
                uiManager2.UpdateSkidUI(coilData2); // UiManager�� UI ������Ʈ �޼��� ȣ��
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

        // ������ Ŭ���� ������Ʈ�� Material�� ���� ���·� ����
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // ���� Ŭ���� ������Ʈ�� Material ����
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

        // ���������� Ŭ���� �ڵ鷯�� ���� �ڵ鷯�� ������Ʈ
        lastClickedHandler = this;
    }

}



