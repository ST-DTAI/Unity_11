using UnityEngine;
using TMPro;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown dropdown; // TMP_Dropdown ������Ʈ
    public GameObject[] panels;   // �г� GameObjects �迭

    void Start()
    {
        // ��Ӵٿ� ���� ����� �� ȣ��� �޼��� ���
        dropdown.onValueChanged.AddListener(DropdownValueChanged);

        // ��� �г��� ��Ȱ��ȭ ���·� ����
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }
        // ��Ӵٿ��� �⺻���� �ӽ÷� �ٸ� ������ ����
        dropdown.value = dropdown.options.Count - 1;

        // ��Ӵٿ� �� ���� �̺�Ʈ�� �������� ȣ���Ͽ� �⺻���� ����Ǿ����� �˸�
        DropdownValueChanged(dropdown.value);
    }

    void DropdownValueChanged(int value)
    {
        // ��� �г��� ��Ȱ��ȭ
        foreach (var panel in panels)
        {
            panel.SetActive(false);
        }

        // ���õ� ���� �ش��ϴ� �г��� Ȱ��ȭ
        if (value >= 0 && value < panels.Length)
        {
            panels[value].SetActive(true);
        }
    }
}
