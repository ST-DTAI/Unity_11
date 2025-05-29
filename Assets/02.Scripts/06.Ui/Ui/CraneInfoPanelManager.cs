using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UI.Tables;
using UnityEngine;
using UnityEngine.UI;

public class CraneInfoPanelManager : MonoBehaviour
{

    public Button closeButton;
    public GameObject panel;

    public int crNo = 11;

    private DatabaseConnection dbConnection;
    private MySqlConnection connection;

    // Start is called before the first frame update
    void Start()
    {
        dbConnection = new DatabaseConnection();
        connection = dbConnection.OpenConnection();

        Loadworkorder();
        LoadCoilInfo();
    }


    void Loadworkorder()
    {
        {
            try
            {
                // 쿼리: Up% 및 Dn% 데이터 가져오기
                string query = $"SELECT UpAddr, UpDx, UpDy, UpDz, DnAddr, DnDx, DnDy, DnDz FROM clts.cr_view WHERE CrNo = {crNo}";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                // 결과를 저장할 리스트
                List<string[]> dataRows = new List<string[]>();

                // 모든 행을 리스트에 저장
                while (reader.Read())
                {
                    string[] rowData = new string[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        rowData[i] = reader[i]?.ToString() ?? "-";
                    }
                    dataRows.Add(rowData);
                }

                reader.Close(); // 리더 닫기

                // 첫 번째 행 (Up% 데이터)
                if (dataRows.Count > 0) // 데이터가 있는 경우
                {
                    // 예를 들어, row1이 이미 존재하는 경우
                    GameObject row1 = GameObject.Find("RowUpAddr"); // Row1의 이름으로 GameObject를 찾기
                    Debug.Log("Using existing Row1 for Up% data");

                    // Up% 데이터 채우기 (0~3 인덱스)
                    for (int i = 0; i < 4; i++) // 0번째 cell을 건너뛰고 1~3 인덱스
                    {
                        // Row1의 i번째 자식 cell에서 TMP_Text를 찾기
                        GameObject cell = row1.transform.GetChild(i+1).gameObject; // i번째 자식 cell
                        TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();  // TMP_Text 컴포넌트 가져오기

                        if (textComponent == null)
                        {
                            Debug.LogError("TMP_Text component not found in existing cell!"); // 텍스트 컴포넌트 확인
                        }

                        textComponent.text = dataRows[0][i]; // 읽어온 데이터
                        Debug.Log($"Setting cell text for Row1 column {i}: {textComponent.text}");
                    }
                }

                // 두 번째 행 (Dn% 데이터)
                if (dataRows.Count > 0) // 데이터가 있는 경우
                {
                    // 예를 들어, row2가 이미 존재하는 경우
                    GameObject row2 = GameObject.Find("RowDnAddr"); // Row2의 이름으로 GameObject를 찾기
                    Debug.Log("Using existing Row2 for Dn% data");

                    // Dn% 데이터 채우기 (4~7 인덱스)
                    for (int i = 4; i < 8; i++) // Dn% 데이터는 4~7 인덱스
                    {
                        // Row2의 (i - 4 + 1)번째 자식 cell에서 TMP_Text를 찾기
                        GameObject cell = row2.transform.GetChild(i - 4 + 1).gameObject; // 0번째 cell을 건너뛰기 위해 i - 4 + 1
                        TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>();  // TMP_Text 컴포넌트 가져오기

                        if (textComponent == null)
                        {
                            Debug.LogError("TMP_Text component not found in existing cell!"); // 텍스트 컴포넌트 확인
                        }

                        textComponent.text = dataRows[0][i]; // 읽어온 데이터
                        Debug.Log($"Setting cell text for Row2 column {i - 4 + 1}: {textComponent.text}");
                    }
                }
                else
                {
                    Debug.LogWarning("No data found in the query result."); // 데이터가 없을 경우 경고 로그
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"An error occurred: {ex.Message}"); // 에러 메시지 출력
            }
        }
    }

    void LoadCoilInfo()
    {
        try
        {
            // 쿼리: WorkPdNo, Width, Weight, Outdia 데이터 가져오기
            string query = $"SELECT WorkPdNo, Width, Weight, Outdia FROM clts.cr_view WHERE CrNo = {crNo}";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            MySqlDataReader reader = cmd.ExecuteReader();

            // 결과를 저장할 리스트
            List<string[]> dataRows = new List<string[]>();

            // 모든 행을 리스트에 저장
            while (reader.Read())
            {
                string[] rowData = new string[reader.FieldCount];
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    rowData[i] = reader[i].ToString();
                }
                dataRows.Add(rowData);
            }

            reader.Close(); // 리더 닫기

            // 첫 번째 행의 데이터가 있는지 확인
            if (dataRows.Count > 0)
            {
                // 데이터가 있는 경우, 첫 번째 행의 데이터 가져오기
                string workPdNo = dataRows[0][0]; // WorkPdNo
                string width = dataRows[0][1];     // Width
                string weight = dataRows[0][2];    // Weight
                string outdia = dataRows[0][3];    // Outdia

                // 첫 번째 행 (WorkPdNo 데이터)
                GameObject row1 = GameObject.Find("RowPdNo");
                GameObject cell1 = row1.transform.GetChild(1).gameObject;
                TMP_Text textComponent1 = cell1.GetComponentInChildren<TMP_Text>();

                if (textComponent1 != null)
                {
                    textComponent1.text = workPdNo; // WorkPdNo 데이터
                    Debug.Log($"Setting cell text for RowPdNo: {textComponent1.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowPdNo!");
                }

                // 두 번째 행 (Width 데이터)
                GameObject row2 = GameObject.Find("RowWidth");
                GameObject cell2 = row2.transform.GetChild(1).gameObject;
                TMP_Text textComponent2 = cell2.GetComponentInChildren<TMP_Text>();

                if (textComponent2 != null)
                {
                    textComponent2.text = width; // Width 데이터
                    Debug.Log($"Setting cell text for RowWidth: {textComponent2.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowWidth!");
                }

                // 세 번째 행 (Weight 데이터)
                GameObject row3 = GameObject.Find("RowWeight");
                GameObject cell3 = row3.transform.GetChild(1).gameObject;
                TMP_Text textComponent3 = cell3.GetComponentInChildren<TMP_Text>();

                if (textComponent3 != null)
                {
                    textComponent3.text = weight; // Weight 데이터
                    Debug.Log($"Setting cell text for RowWeight: {textComponent3.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowWeight!");
                }

                // 네 번째 행 (Outdia 데이터)
                GameObject row4 = GameObject.Find("RowOutdia");
                GameObject cell4 = row4.transform.GetChild(1).gameObject;
                TMP_Text textComponent4 = cell4.GetComponentInChildren<TMP_Text>();

                if (textComponent4 != null)
                {
                    textComponent4.text = outdia; // Outdia 데이터
                    Debug.Log($"Setting cell text for RowOutdia: {textComponent4.text}");
                }
                else
                {
                    Debug.LogError("TMP_Text component not found in RowOutdia!");
                }
            }
            else
            {
                Debug.LogWarning("No data found in the query result."); // 데이터가 없을 경우 경고 로그
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}"); // 에러 메시지 출력
        }
    }






    // 패널을 닫는 메서드
    public void OnCloseButtonClick()
    {
        panel.SetActive(false);
    }

}
