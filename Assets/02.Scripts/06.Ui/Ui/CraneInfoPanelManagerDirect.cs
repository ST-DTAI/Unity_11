using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraneInfoPanelManagerDirect : MonoBehaviour
{
    public Button closeButton;
    public GameObject panel;

    public int crNo = 11;

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();
    }

    //표에 데이터 넣기

    // 패널을 닫는 메서드
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }




}
