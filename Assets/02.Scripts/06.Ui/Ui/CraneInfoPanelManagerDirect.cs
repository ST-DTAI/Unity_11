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

    private MySqlConnection connection;

    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;
    }

    //ǥ�� ������ �ֱ�

    // �г��� �ݴ� �޼���
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }




}
