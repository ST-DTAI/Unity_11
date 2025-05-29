using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CalendarController : MonoBehaviour
{
    public GameObject calendarPanel; // 캘린더 패널
    public TextMeshProUGUI yearText; // 연도 표시 텍스트
    public TextMeshProUGUI monthText; // 월 표시 텍스트

    public GameObject dateItemPrefab; // 날짜 아이템 프리팹

    public List<GameObject> dateItemList = new List<GameObject>(); // 날짜 아이템 리스트
    const int totalDateCount = 42; // 총 날짜 수

    private DateTime currentDate; // 현재 날짜
    private CalendarController calendarInstance; // 싱글톤 인스턴스

    public TextMeshProUGUI selectedDateText; // 선택된 날짜 텍스트 필드

    void Start()
    {
        calendarInstance = this;
        Vector3 startPosition = dateItemPrefab.transform.localPosition; // 초기 위치
        dateItemList.Clear();
        dateItemList.Add(dateItemPrefab); // 프리팹 추가

        for (int i = 1; i < totalDateCount; i++)
        {
            GameObject newItem = GameObject.Instantiate(dateItemPrefab) as GameObject; // 새로운 날짜 아이템 생성
            newItem.name = "DateItem" + (i + 1).ToString(); // 이름 설정
            newItem.transform.SetParent(dateItemPrefab.transform.parent); // 부모 설정
            newItem.transform.localScale = Vector3.one; // 스케일 초기화
            newItem.transform.localRotation = Quaternion.identity; // 회전 초기화
            newItem.transform.localPosition = new Vector3((i % 7) * 36 + startPosition.x, startPosition.y - (i / 7) * 30, startPosition.z); // 위치 설정

            dateItemList.Add(newItem); // 리스트에 추가
        }

        currentDate = DateTime.Now; // 현재 날짜 초기화

        CreateCalendar(); // 캘린더 생성

        calendarPanel.SetActive(false); // 캘린더 패널 비활성화
    }

    void CreateCalendar()
    {
        DateTime firstDayOfMonth = currentDate.AddDays(-(currentDate.Day - 1)); // 해당 월의 첫 번째 날
        int dayIndex = GetDayIndex(firstDayOfMonth.DayOfWeek); // 요일 인덱스

        int dayCounter = 0; // 날짜 카운터
        for (int i = 0; i < totalDateCount; i++)
        {
            TextMeshProUGUI dateLabel = dateItemList[i].GetComponentInChildren<TextMeshProUGUI>(); // 날짜 레이블
            dateItemList[i].SetActive(false); // 초기 상태에서 비활성화

            if (i >= dayIndex)
            {
                DateTime currentDay = firstDayOfMonth.AddDays(dayCounter); // 현재 날짜 계산
                if (currentDay.Month == firstDayOfMonth.Month)
                {
                    dateItemList[i].SetActive(true); // 해당 날짜에 대해 활성화
                    dateLabel.text = (dayCounter + 1).ToString(); // 날짜 텍스트 설정
                    dayCounter++; // 날짜 카운터 증가
                }
            }
        }
        yearText.text = currentDate.Year.ToString(); // 연도 텍스트 설정
        monthText.text = currentDate.Month.ToString("D2"); // 월 텍스트 설정
    }

    int GetDayIndex(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }

    public void ShowPreviousYear()
    {
        currentDate = currentDate.AddYears(-1); // 이전 연도로 이동
        CreateCalendar(); // 캘린더 재생성
    }

    public void ShowNextYear()
    {
        currentDate = currentDate.AddYears(1); // 다음 연도로 이동
        CreateCalendar(); // 캘린더 재생성
    }

    public void ShowPreviousMonth()
    {
        currentDate = currentDate.AddMonths(-1); // 이전 월로 이동
        CreateCalendar(); // 캘린더 재생성
    }

    public void ShowNextMonth()
    {
        currentDate = currentDate.AddMonths(1); // 다음 월로 이동
        CreateCalendar(); // 캘린더 재생성
    }

    public void ShowCalendar(TextMeshProUGUI selectedDateText)
    {
        calendarPanel.SetActive(true); // 캘린더 패널 활성화
        this.selectedDateText = selectedDateText; // 선택된 날짜 텍스트 저장
    }

    

    // 날짜 아이템 클릭 시 텍스트에 날짜 표시
    public void OnDateItemClick(string day)
    {
        selectedDateText.text = yearText.text + "-" + monthText.text + "-" + int.Parse(day).ToString("D2"); // 날짜 포맷 설정
        calendarPanel.SetActive(false); // 캘린더 패널 비활성화
    }
}
