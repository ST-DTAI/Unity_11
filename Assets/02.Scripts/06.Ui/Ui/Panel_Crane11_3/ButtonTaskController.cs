using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using MySql.Data.MySqlClient;
using DG.Tweening;

public class ButtonTaskController : MonoBehaviour
{
    [Header("크레인 번호")]
    public int crNo = 11;

    [Header("모드 전환 버튼")]
    public Button manualButton;
    public Button autoButton;
    public Button remoteButton;

    [Header("기능 버튼")]
    public Button autoScheduleButton;
    public Button emergencyStopButton;

    private MySqlConnection connection;

    private string currentMode = "Auto";
    private bool isEmergencyActive = false;
    private bool isAutoScheduleActive = false;
    private Tween emergencyBlinkTween;

    private readonly Color defaultColor = new Color32(24, 24, 27, 255);
    private readonly Color selectedColor = new Color32(70, 130, 255, 255);
    private readonly Color defaultOutlineColor = new Color32(100, 100, 100, 255);
    private readonly Color selectedOutlineColor = new Color32(255, 255, 255, 255);

    public static ButtonTaskController Instance { get; private set; }
    [Header("Skid 좌표 표시용 InputField")]
    public TMP_InputField dxInputField;
    public TMP_InputField dyInputField;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        InitializeModeFromDB();
        InitializeEmergencyStateFromDB();
    }


    // --------------------- initialize ---------------------

    void InitializeModeFromDB()
    {
        string query = $"SELECT Status FROM cr_status WHERE CrNo = {crNo}";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        string status = cmd.ExecuteScalar()?.ToString();

        if (!string.IsNullOrEmpty(status) && status.Length >= 5)
        {
            char modeChar = status[4]; // 5번째 문자
            switch (modeChar)
            {
                case '0': ApplyModeUI("Manual"); break;
                case '1': ApplyModeUI("Auto"); break;
                case '2': ApplyModeUI("Remote"); break;
                default: ApplyModeUI("Auto"); break;
            }
        }
    }

    void InitializeEmergencyStateFromDB()
    {
        string query = $"SELECT EmgStop FROM cr_command WHERE CrNo = {crNo}";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        string result = cmd.ExecuteScalar()?.ToString();

        isEmergencyActive = result == "1";

        if (isEmergencyActive)
            StartEmergencyBlink();
        else
            StopEmergencyBlink();

        UpdateEmergencyButtonVisual();
    }

    // --------------------- UI ---------------------

    void ApplyModeUI(string mode)
    {
        currentMode = mode;

        SetButtonVisual(manualButton, mode == "Manual");
        SetButtonVisual(autoButton, mode == "Auto");
        SetButtonVisual(remoteButton, mode == "Remote");
        SetButtonVisual(autoScheduleButton, isAutoScheduleActive);
    }

    void SetButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedColor : defaultColor;

        var outline = button.GetComponent<Outline>();
        if (outline != null)
            outline.effectColor = isSelected ? selectedOutlineColor : defaultOutlineColor;
    }

    void UpdateEmergencyButtonVisual()
    {
        emergencyStopButton.image.color = isEmergencyActive ? selectedColor : defaultColor;
    }

    // --------------------- 모드 전환 ---------------------

    public void OnClickManualMode() => ChangeMode("Manual", 0);
    public void OnClickAutoMode() => ChangeMode("Auto", 1);
    public void OnClickRemoteMode() => ChangeMode("Remote", 2);

    void ChangeMode(string mode, int dbValue)
    {
        ApplyModeUI(mode);
        UpdateValueInDB("cr_init", "DrvMode", dbValue);
    }

    // --------------------- 작업 제어 ---------------------

    public void OnClickStart() => UpdateValueInDB("cr_command", "OpCmd", 1);
    public void OnClickPause() => UpdateValueInDB("cr_command", "OpCmd", 2);
    public void OnClickCancel() => UpdateValueInDB("cr_command", "OpCmd", 3);

    // --------------------- 특수 명령 ---------------------

    public void OnClickAutoSchedule()
    {
        isAutoScheduleActive = !isAutoScheduleActive;
        int value = isAutoScheduleActive ? 1 : 0;

        UpdateValueInDB("cr_command", "AutoSch", value);
        ApplyModeUI(currentMode);
    }

    public void OnClickEmergencyStop()
    {
        isEmergencyActive = !isEmergencyActive;
        int value = isEmergencyActive ? 1 : 0;

        UpdateValueInDB("cr_command", "EmgStop", value);

        if (isEmergencyActive)
            StartEmergencyBlink();
        else
            StopEmergencyBlink();

        UpdateEmergencyButtonVisual();
    }

    void StartEmergencyBlink()
    {
        emergencyBlinkTween?.Kill();

        emergencyBlinkTween = emergencyStopButton.image
            .DOFade(0.3f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    void StopEmergencyBlink()
    {
        emergencyBlinkTween?.Kill();
        emergencyStopButton.image.DOFade(1f, 0.2f);
    }

    // --------------------- DB 전송 ---------------------

    void UpdateValueInDB(string table, string column, int value)
    {
        string query = $"UPDATE {table} SET {column} = {value} WHERE CrNo = {crNo}";
        try
        {
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
                Debug.Log($"[DB] {table}.{column} ← {value} (CrNo: {crNo})");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[DB 오류] {column} 업데이트 실패: {ex.Message}");
        }
    }

    // --------------------- 수동 지시---------------------

    public void OnClickMoveCommand()
    {
        
    }

    public void OnClickUpCommand()
    {
      
    }

    public void OnClickDownCommand()
    {
    }

    public void SetSelectedSkid(YardMap selectedSkid)
    {
        dxInputField.text = selectedSkid.Dx.ToString("F0");
        dyInputField.text = selectedSkid.Dy.ToString("F0");

        Debug.Log($"Skid 선택됨 → Dx: {selectedSkid.Dx}, Dy: {selectedSkid.Dy}");
    }
}
