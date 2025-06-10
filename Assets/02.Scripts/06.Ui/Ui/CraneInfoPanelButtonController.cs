using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data.MySqlClient;
using System;// MySQL 라이브러리

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

        public bool isToggleButton = false;  //토글 버튼 여부
        public bool toggleState = false;      //현재 토글 상태 (false = 0, true = 1)
    }
    [Header("이 패널의 CrNo")]
    public int crNo;

    [Header("버튼과 DB 타겟 매핑")]
    public List<ButtonDBBinding> buttonBindings = new List<ButtonDBBinding>(9);


    private MySqlConnection connection;

    private const string TableName = "cr_command";


    // Start is called before the first frame update
    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        // 버튼 이벤트 등록
        foreach (var binding in buttonBindings)
        {
            if (binding.button != null && binding.target != null)
            {
                var bindingCopy = binding; // Closure 방지
                                           
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
        int valueToSend = binding.valueToWrite; // 기본값

        if (binding.isToggleButton)
        {
            // 토글 버튼이면 상태를 반전시킨다
            binding.toggleState = !binding.toggleState;
            valueToSend = binding.toggleState ? 1 : 0;
        }

        Debug.Log($"[버튼 클릭] {TableName}.{binding.target.columnName} 컬럼에 {binding.valueToWrite} 쓰기 (CrNo=11)");
        WriteToDatabase(binding.target, binding.valueToWrite);

        UpdateButtonColors(binding); // 버튼 색 갱신
    }

    private void WriteToDatabase(DBTarget target, int value)
    {
        if (connection == null)
        {
            Debug.LogError("DB 연결이 없습니다!");
            return;
        }

        try
        {
            // 여기서 테이블, 컬럼, 조건을 모두 세팅
            string query = $"UPDATE {TableName} SET {target.columnName} = @Value WHERE CrNo = @CrNo";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@Value", value);
                cmd.Parameters.AddWithValue("@CrNo", crNo);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    Debug.Log($"[DB 쓰기 성공] {TableName}.{target.columnName} = {value} (CrNo={crNo})");
                }
                else
                {
                    Debug.LogWarning($"[DB 쓰기 실패] 해당 조건에 맞는 행이 없습니다. (CrNo={crNo})");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"DB 쓰기 중 오류 발생: {ex.Message}");
        }
    }

    private void UpdateButtonColors(ButtonDBBinding clickedBinding)
    {
        Color selectedColor = new Color(0.3f, 0.6f, 1f); // 선택된 버튼 색 (파란 느낌)
        Color normalColor = Color.white; // 기본 버튼 색 (하얀색)

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
