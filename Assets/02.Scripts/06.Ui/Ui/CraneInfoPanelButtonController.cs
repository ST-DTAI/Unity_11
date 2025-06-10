using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;// MySQL ���̺귯��

[Serializable]
public class DBTarget
{
    
    public string columnName;
}

public class CraneInfoPanelButtonController : MonoBehaviour
{
    [Serializable]
    public class ButtonDBBinding
    {
        public Button button;
        public DBTarget target;
        public int valueToWrite;

        [HideInInspector]
        public Image buttonImage;

        public bool isToggleButton = false;  //��� ��ư ����
        public bool toggleState = false;      //���� ��� ���� (false = 0, true = 1)
    }
    [Header("�� �г��� CrNo")]
    public int crNo;

    [Header("��ư�� DB Ÿ�� ����")]
    public List<ButtonDBBinding> buttonBindings = new List<ButtonDBBinding>(9);


    private MySqlConnection connection;

    private const string TableName = "cr_command";


    // Start is called before the first frame update
    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        // ��ư �̺�Ʈ ���
        foreach (var binding in buttonBindings)
        {
            if (binding.button != null && binding.target != null)
            {
                var bindingCopy = binding; // Closure ����
                                           
                binding.buttonImage = binding.button.GetComponent<Image>();

                binding.button.onClick.AddListener(() => OnButtonClicked(bindingCopy));
            }
            else
            {
                Debug.LogWarning("Button or Target not assigned!");
            }
        }

    }

    private void OnButtonClicked(ButtonDBBinding binding)
    {
        int valueToSend = binding.valueToWrite; // �⺻��

        if (binding.isToggleButton)
        {
            // ��� ��ư�̸� ���¸� ������Ų��
            binding.toggleState = !binding.toggleState;
            valueToSend = binding.toggleState ? 1 : 0;
        }

        Debug.Log($"[��ư Ŭ��] {TableName}.{binding.target.columnName} �÷��� {binding.valueToWrite} ���� (CrNo=11)");
        WriteToDatabase(binding.target, binding.valueToWrite);

        UpdateButtonColors(binding); // ��ư �� ����
    }

    private void WriteToDatabase(DBTarget target, int value)
    {
        if (connection == null)
        {
            Debug.LogError("DB ������ �����ϴ�!");
            return;
        }

        try
        {
            // ���⼭ ���̺�, �÷�, ������ ��� ����
            string query = $"UPDATE {TableName} SET {target.columnName} = @Value WHERE CrNo = @CrNo";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Value", value);
                cmd.Parameters.AddWithValue("@CrNo", crNo);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Debug.Log($"[DB ���� ����] {TableName}.{target.columnName} = {value} (CrNo={crNo})");
                }
                else
                {
                    Debug.LogWarning($"[DB ���� ����] �ش� ���ǿ� �´� ���� �����ϴ�. (CrNo={crNo})");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"DB ���� �� ���� �߻�: {ex.Message}");
        }
    }

    private void UpdateButtonColors(ButtonDBBinding clickedBinding)
    {
        Color selectedColor = new Color(0.3f, 0.6f, 1f); // ���õ� ��ư �� (�Ķ� ����)
        Color normalColor = Color.white; // �⺻ ��ư �� (�Ͼ��)

        foreach (var binding in buttonBindings)
        {
            if (binding.buttonImage != null)
            {
                if (binding == clickedBinding)
                    binding.buttonImage.color = selectedColor;
                else
                    binding.buttonImage.color = normalColor;
            }
        }
    }


}
