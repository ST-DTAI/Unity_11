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

    //ǥ�� ������ �ֱ�

    // �г��� �ݴ� �޼���
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }




}
