using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainCraneStatusManager : MonoBehaviour
{

    private MySqlConnection connection;
    // Start is called before the first frame update
    void Start()
    {
        connection = DatabaseConnection.Instance.Connection;

        LoadCraneStatus();
    }


    void LoadCraneStatus()
    {
        try
        {
            // 쿼리: CrPdNo, UpAddr, DnAddr, Status 데이터 가져오기
            string query = $"SELECT PdNo, UpAddr,DnAddr FROM clts.work_order";
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

            // 데이터가 있는 경우 처리
            for (int rowIndex = 0; rowIndex < dataRows.Count; rowIndex++)
            {
                // 각 행에 대해 GameObject를 찾기
                string rowName = $"RowCrane1{rowIndex + 1}"; // 예: Row1, Row2, Row3, ...
                GameObject row = GameObject.Find(rowName);
                if (row == null)
                {
                    Debug.LogError($"GameObject {rowName} not found!");
                    continue; // 다음 행으로 넘어갑니다.
                }

                // 각 행의 데이터를 설정
                for (int colIndex = 0; colIndex < dataRows[rowIndex].Length; colIndex++)
                {
                    // Row의 (colIndex + 1)번째 자식 cell에서 TMP_Text를 찾기
                    GameObject cell = row.transform.GetChild(colIndex+1).gameObject; // colIndex 번째 자식 cell
                    TMP_Text textComponent = cell.GetComponentInChildren<TMP_Text>(); // TMP_Text 컴포넌트 가져오기

                    if (textComponent == null)
                    {
                        Debug.LogError($"TMP_Text component not found in {rowName} cell {colIndex}!"); // 텍스트 컴포넌트 확인
                        continue; // 다음 cell로 넘어갑니다.
                    }

                    textComponent.text = dataRows[rowIndex][colIndex]; // 읽어온 데이터
                    
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"An error occurred: {ex.Message}"); // 에러 메시지 출력
        }
    }
}
