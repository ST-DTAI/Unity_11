using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SkidClickHandler : MonoBehaviour
{
    public YardMap skidData; // Ŭ���� ��Ű���� ������

    public Material outlineMaterial; // Ŭ�� �� ������ ��Ƽ����
    public Material originalMaterial; // ���� ��Ƽ����

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
            Debug.LogWarning($"SkidClickHandler: {gameObject.name}�� Renderer�� �����ϴ�.");
        }
    }

    void OnMouseDown()
    {
        Debug.Log($"[Skid Clicked] {skidData.Skid} (SkidNo: {skidData.SkidNo})");

        // ���� ���õ� Skid �� ����
        if (lastClickedHandler != null && lastClickedHandler != this)
        {
            Renderer lastRenderer = lastClickedHandler.GetComponent<Renderer>();
            if (lastRenderer != null && lastClickedHandler.originalMaterial != null)
            {
                lastRenderer.material = lastClickedHandler.originalMaterial;
            }
        }

        // ���� Skid�� outline ��Ƽ���� ����
        if (myRenderer != null && outlineMaterial != null)
        {
            myRenderer.material = outlineMaterial;
        }

        // ButtonTaskController�� dx, dy ����
        ButtonTaskController.Instance?.SetSelectedSkid(skidData);

        // ���� �ڵ鷯 ����
        lastClickedHandler = this;
    }
}
