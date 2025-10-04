using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using Mobile;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class MyDataManager : MonoBehaviour
{
    public DataSnapshot _user, _history;
    private string _withDay = "yyyy-MM-dd";
    public float mdmAllAvg; 
    
    public GameObject announcementText; //NotAvailableNow

    public ActivityGroup _activityGroup;
    public Mobile_MyDetail mobileExer;

    private void Start()
    {
        plusDataEnabled.mdm = this;
    }

    /// <summary>
    /// 창이 켜질 때마다
    /// </summary>
    private void OnEnable()
    {
        AllDelete();//데이터 삭제
        StartCoroutine(EnableCoroutine());
        //TODO: 켤 때마다 최신 데이터 갱신해야함 
    }

    /// <summary>
    /// 스냅샷 가져오기
    /// </summary>
    /// <returns></returns>
    IEnumerator EnableCoroutine()
    {
        var task = FirebaseDatabase.DefaultInstance.RootReference.GetValueAsync();
        yield return new WaitUntil(()=>task.IsCompleted||task.IsFaulted);
        //yield return StartCoroutine(_activityGroup.timeSet());

        if (task.IsCompleted)
        {
            _user = task.Result.Child(NetworkManager._instance._playerData.PatientID);
            _history = _user.Child("ExerciseDatas").Child("Measurement").Child("history"); //날짜값까지 진입
            RecentBTN(); //처음 화면이니까.
        }

        _activityGroup._snapshot = task.Result;
        yield return StartCoroutine(_activityGroup.ChainStart());
    }

    public void AllDelete()
    {
        _user = null;
        _history = null;
    }

    #region Overall

    [Header("Overall")] 
    public Image weekGraph_overAll;
    public Transform overAllTransform;
    public TMP_Text bestCondition;
    public TMP_Text averageChangeScore;
    public TMP_Text maxText, minText;
    private float allAverage = 0;
        
    public void OverallBTN()
    {
        bestCondition.text = "";
        averageChangeScore.text = "";
        maxText.text = "";
        minText.text = "";
        CalculateOverall();
    }

       private void CalculateOverall()
        {
            foreach (Transform child in overAllTransform)
            {
                Destroy(child.gameObject);
            }
            
            DateTime startDate = DateTime.Parse(_user.Child("startDate").Value.ToString());
            Dictionary<int, Dictionary<string, float>> weeklyDailyMaxAngles = new Dictionary<int, Dictionary<string, float>>();

            float week1Average = CalculateWeeklyAverage(startDate, 0, 5, 1, weeklyDailyMaxAngles);
            Image week1Graph = Instantiate(weekGraph_overAll, overAllTransform);
            week1Graph.transform.GetChild(0).GetComponent<TMP_Text>().text = "1st Week";
            week1Graph.fillAmount = week1Average/180;
            
            
            if (weeklyDailyMaxAngles.ContainsKey(1))
            {
                Debug.Log($"Week 1:");
                LineChart lineChart = week1Graph.transform.GetChild(1).GetComponent<LineChart>();
                
                lineChart.RemoveAllSerie();
                lineChart.AddSerie<Line>("week1");
                int count = 0;
                foreach (var dailyMax in weeklyDailyMaxAngles[1])//날의 최대각도
                {
                    lineChart.AddData("week1", count, dailyMax.Value);
                    count++;
                }
                Serie serie = lineChart.GetSerie("week1");
                serie.EnsureComponent<AreaStyle>();
                serie.clip = true;
                serie.symbol.type = SymbolType.None;
                serie.lineType = LineType.Smooth;
            }

            float week2Average = CalculateWeeklyAverage(startDate, 5, 10, 2, weeklyDailyMaxAngles);
            Image week2Graph = Instantiate(weekGraph_overAll, overAllTransform);
            week2Graph.transform.GetChild(0).GetComponent<TMP_Text>().text = "2nd Week";
            week2Graph.fillAmount = week2Average/weeklyDailyMaxAngles.Count;
            
            if (weeklyDailyMaxAngles.ContainsKey(2))
            {
                Debug.Log($"Week 2:");
                LineChart lineChart = week2Graph.transform.GetChild(1).GetComponent<LineChart>();
                lineChart.RemoveAllSerie();
                lineChart.AddSerie<Line>("week2");
                int count = 0;
                foreach (var dailyMax in weeklyDailyMaxAngles[2])//날의 최대각도
                {
                    lineChart.AddData("week2", count, dailyMax.Value);
                    count++;
                }
                Serie serie = lineChart.GetSerie("week2");
                serie.EnsureComponent<AreaStyle>();
                serie.symbol.type = SymbolType.None;
                serie.clip = true;
                serie.lineType = LineType.Smooth;
            }

            float week3Average = CalculateWeeklyAverage(startDate, 10, 15, 3, weeklyDailyMaxAngles);
            Image week3Graph = Instantiate(weekGraph_overAll, overAllTransform);
            week3Graph.transform.GetChild(0).GetComponent<TMP_Text>().text = "3rd Week";
            week3Graph.fillAmount = week3Average/weeklyDailyMaxAngles.Count;
            
            if (weeklyDailyMaxAngles.ContainsKey(3))
            {
                Debug.Log($"Week 3:");
                LineChart lineChart = week3Graph.transform.GetChild(1).GetComponent<LineChart>();
                lineChart.RemoveAllSerie();
                lineChart.AddSerie<Line>("week3");
                int count = 0;
                foreach (var dailyMax in weeklyDailyMaxAngles[3])//날의 최대각도
                {
                    lineChart.AddData("week3", count, dailyMax.Value);
                    count++;
                }
                Serie serie = lineChart.GetSerie("week3");
                serie.clip = true;
                serie.symbol.type = SymbolType.None;
                serie.EnsureComponent<AreaStyle>();
                serie.lineType = LineType.Smooth;
            }

            float week4Average = CalculateWeeklyAverage(startDate, 15, 20, 4, weeklyDailyMaxAngles);
            Image week4Graph = Instantiate(weekGraph_overAll, overAllTransform);
            week4Graph.transform.GetChild(0).GetComponent<TMP_Text>().text = "4th Week";
            week4Graph.fillAmount = week4Average/weeklyDailyMaxAngles.Count;
            
            if (weeklyDailyMaxAngles.ContainsKey(4))
            {
                Debug.Log($"Week 4:");
                LineChart lineChart = week4Graph.transform.GetChild(1).GetComponent<LineChart>();
                lineChart.RemoveAllSerie();
                lineChart.AddSerie<Line>("week4");
                int count = 0;
                foreach (var dailyMax in weeklyDailyMaxAngles[4])//날의 최대각도
                {
                    lineChart.AddData("week4", count, dailyMax.Value);
                    count++;
                }
                Serie serie = lineChart.GetSerie("week4");
                serie.clip = true;
                serie.symbol.type = SymbolType.None;
                serie.EnsureComponent<AreaStyle>();
                serie.lineType = LineType.Smooth;
            }

            float firstAngle = 0;
            float weekMax = float.MinValue;
            float weekMin = float.MaxValue;
            
            // 주별 최대 각도
            for (int week = 1; week <= 4; week++)
            {
                if (week == 1)
                {
                    firstAngle = weeklyDailyMaxAngles[week].First().Value;
                }
                if (weeklyDailyMaxAngles.ContainsKey(week))
                {
                    Debug.Log($"Week {week}:");
                    
                    foreach (var dailyMax in weeklyDailyMaxAngles[week])
                    {
                        float maxAngle = dailyMax.Value;
            
                        if (maxAngle > weekMax) weekMax = maxAngle;
                        if (maxAngle < weekMin) weekMin = maxAngle;
                    }
                }
            }
            bestCondition.text = (Math.Truncate(weekMax * 100) / 100).ToString("000.00");
            averageChangeScore.text = $"{Math.Truncate(((weekMax - firstAngle) / firstAngle * 100) * 100) / 100}%";
            maxText.text = (Math.Truncate(weekMax * 10) / 10).ToString("000.0");
            minText.text = (Math.Truncate(weekMin * 10) / 10).ToString("000.0");
        }

        private float CalculateWeeklyAverage(DateTime startDate, int startDay, int endDay, int weekNumber, Dictionary<int, Dictionary<string, float>> weeklyDailyMaxAngles)
        {
            List<float> weeklyAngles = new List<float>();
            float weeklyAverage = 0f;
            allAverage = 0f;

            foreach (var day in _history.Children)
            {
                if (IsWithinWeek(day.Key, startDate, startDay, endDay))
                {
                    float dailyMax = float.MinValue;
                    foreach (var direction in day.Child("Angles_Max_Before").Children)
                    {
                        if (direction.Key.Contains("dateTime")) continue;
                        float angle = float.Parse(direction.Value.ToString());
                        weeklyAngles.Add(angle);
                        weeklyAverage += angle;
                        if (angle > dailyMax) dailyMax = angle;
                    }

                    if (!weeklyDailyMaxAngles.ContainsKey(weekNumber))
                    {
                        weeklyDailyMaxAngles[weekNumber] = new Dictionary<string, float>();
                    }
                    weeklyDailyMaxAngles[weekNumber][day.Key] = dailyMax; // 주별 날짜별 최대 각도 저장
                    
                    weeklyAverage /= day.Child("Angles_Max_Before").ChildrenCount-1;
                    allAverage += weeklyAverage;
                    weeklyAverage = 0;
                }
                //Debug.Log("@" + weeklyAverage);
                // string afterWeek = "day " + startDate.AddDays(endDay).ToString(_withDay);
                // if (day.Key != afterWeek) return 0;
            }
            
            allAverage /= _history.ChildrenCount;
            
            Debug.Log("@" + weeklyAverage + "$$" + allAverage);
            return allAverage;
        }

        private bool IsWithinWeek(string dayKey, DateTime startDate, int startDay, int endDay)
        {
            for (int i = startDay; i < endDay; i++)
            {
                if (dayKey.Contains(startDate.AddDays(i).ToString(_withDay)))
                {
                    return true;
                }
            }
            return false;
        }
        
    #endregion

    #region Recent
    [Header("Recent")]
    public LineChart recentLineChart;
    public TMP_Text bestScore;
    public TMP_Text averageScore;
    public TMP_Text todayScore;
    private Serie line;

    Dictionary<DateTime, List<float>> days = new Dictionary<DateTime, List<float>>();
    public Dictionary<DateTime, float> daysAve = new Dictionary<DateTime, float>();
    
    //string == week, DateTime == 날짜, float == 값
    
    public void RecentBTN()
    {
        line = recentLineChart.GetSerie("line");
        recentLineChart.ClearData();
        XAxisChange_Recent();
    }

    public void XAxisChange_Recent()
    {
        List<DateTime> lastSevenDays = new List<DateTime>();
        for (int i = 0; i < 7; i++)
        {
            lastSevenDays.Add(DateTime.Today.AddDays(-i));
        }
        lastSevenDays.Reverse();

        XAxis xAxis = recentLineChart.GetChartComponent<XAxis>();
        xAxis.data.Clear(); //비우고
        
        foreach (var day in lastSevenDays)
        {
            xAxis.data.Add(day.ToString("MM/dd"));//추가하기
        }
        
        
        for (int i = 0; i < 7; i++)
        {
            days.Add(lastSevenDays[i], new List<float>());
            Debug.Log(lastSevenDays[i]);
            foreach (var day in _history.Children)
            {
                Debug.Log("Found"+lastSevenDays[i]);
                days.Add(lastSevenDays[i], new List<float>());
                
                if (day.Key.Contains(lastSevenDays[i].ToString(_withDay)))
                {
                    foreach (var direction in day.Child("Angles_Max_Before").Children)
                    {
                        if(direction.Key.Contains("dateTime")) continue;
                        days[lastSevenDays[i]].Add(float.Parse(direction.Value.ToString()));
                    }
                } 
            }
        }
        

        float aDayAvg = 0;
        foreach (var t in lastSevenDays)
        {
            aDayAvg += days[t].Sum();
            daysAve[t] = aDayAvg / days[t].Count;
            aDayAvg = 0;
        }


        int count = 0;
        foreach (var data in daysAve)
        {
            float setValue = data.Value;
            if (float.IsNaN(data.Value))
            {
                setValue = 0;
            }
            recentLineChart.AddData("serie0", count, yValue: setValue);
            //테스트 필요
            count++;
        }
        
        CalcBestScore();
        CalcAverageScore();
        CalcTodayScore();
    }
    
    public void CalcBestScore()
    {
        float best = float.MinValue; 
        foreach(var day in days)
        {
            foreach (var score in day.Value)
            {
                if (score > best)
                {
                    best = score;
                }
            }
        }

        if (best > 0)
        {
            bestScore.text = best.ToString("000.00");
        }
        else
        {
            bestScore.text = "No data";
        }
    }
    
    public void CalcAverageScore()
    {
        float totalScore = 0.0f;
        int scoreCount = 0;

        foreach (var day in days)
        {
            foreach (var score in day.Value)
            {
                totalScore += score;
                scoreCount++;
            }
        }

        if (scoreCount > 0)
        {
            float average = totalScore / scoreCount;
            averageScore.text = average.ToString("000.00");
        }
        else
        {
            averageScore.text = "No data";
        }
    }
    
    public void CalcTodayScore()
    {
        DateTime today = DateTime.Today;
        if (days.ContainsKey(today))
        {
            float totalScoreToday = 0.0f;
            int scoreCountToday = 0;
            foreach (var score in days[today])
            {
                totalScoreToday += score;
                scoreCountToday++;    
            }

            if (scoreCountToday > 0)
            {
                float averageToday = totalScoreToday / scoreCountToday;
                todayScore.text = averageToday.ToString("000.00");
            }
            else
            {
                todayScore.text = "No data";
            }
        }
        else
        {
            todayScore.text = "No data";
        }
    }
    #endregion

    #region Compare
    [Header("Compare")]
    public BarChart CompareChart;
    //큰 표가 하나 있으니 이 하나에 값을 넣어야함.
    
    // Calculate the average of Angles_Max_Data for the current and previous week.
        public void CompareAngleAverages()
        {
            // 오늘을 주의 마지막 날로 설정
            DateTime endDateCurrentWeek = DateTime.Today.Date;
            DateTime startDateCurrentWeek = endDateCurrentWeek.AddDays(-6);

            // 이전 주는 현재 주 시작일 바로 전까지 7일간
            DateTime endDatePreviousWeek = startDateCurrentWeek.AddDays(-1);
            DateTime startDatePreviousWeek = endDatePreviousWeek.AddDays(-6);

            float currentWeekSum = 0;
            int currentWeekCount = 0;
            float previousWeekSum = 0;
            int previousWeekCount = 0;

            List<float> currentWeekAverage = new List<float>(7);
            List<float> previousWeekAverage = new List<float>(7);

            // Dictionary to track the maximum value for each date
            Dictionary<DateTime, float> dailyMaxAngles = new Dictionary<DateTime, float>();

            foreach (var day in _history.Children)
            {
                DateTime dayDate = DateTime.Parse(day.Key.Replace("day ", "")).Date; // assuming day.Key is in a parseable date string format
                foreach (var direction in day.Child("Angles_Max_Before").Children)
                {
                    if(direction.Key.Contains("dateTime")) continue;
                    float angle = float.Parse(direction.Value.ToString());

                    // Update the maximum value for the current day
                    if (!dailyMaxAngles.ContainsKey(dayDate))
                    {
                        dailyMaxAngles[dayDate] = angle;
                        Debug.Log($"New entry: {dayDate} => {angle}");
                    }
                    else
                    {
                        if (angle > dailyMaxAngles[dayDate])
                        {
                            dailyMaxAngles[dayDate] = angle;
                            Debug.Log($"Updated entry: {dayDate} => {angle}");
                        }
                    }
                }
            }

            // Fill current week averages with 0 if no data
            for (int i = 0; i < 7; i++)
            {
                DateTime dayDate = startDateCurrentWeek.AddDays(i);
                Debug.Log($"Checking current week date: {dayDate}");
                if (dailyMaxAngles.ContainsKey(dayDate))
                {
                    float maxAngle = dailyMaxAngles[dayDate];
                    Debug.Log($"Adding to current week: {maxAngle}");
                    currentWeekAverage.Add(maxAngle);
                    currentWeekSum += maxAngle;
                    currentWeekCount++;
                }
                else
                {
                    Debug.Log($"No data for current week date: {dayDate}, adding 0");
                    currentWeekAverage.Add(0); // No data for this day
                }
            }

            // Fill previous week averages with 0 if no data
            for (int i = 0; i < 7; i++)
            {
                DateTime dayDate = startDatePreviousWeek.AddDays(i);
                Debug.Log($"Checking previous week date: {dayDate}");
                if (dailyMaxAngles.ContainsKey(dayDate))
                {
                    float maxAngle = dailyMaxAngles[dayDate];
                    Debug.Log($"Adding to previous week: {maxAngle}");
                    previousWeekAverage.Add(maxAngle);
                    previousWeekSum += maxAngle;
                    previousWeekCount++;
                }
                else
                {
                    Debug.Log($"No data for previous week date: {dayDate}, adding 0");
                    previousWeekAverage.Add(0); // No data for this day
                }
            }

            // Display the averages
            Debug.Log("Current Week Average: " + (currentWeekCount > 0 ? currentWeekSum / currentWeekCount : 0));
            Debug.Log("Previous Week Average: " + (previousWeekCount > 0 ? previousWeekSum / previousWeekCount : 0));
            Debug.Log("Current Week Count: " + currentWeekAverage.Count);
            Debug.Log("Previous Week Count: " + previousWeekAverage.Count);

            foreach (var a in currentWeekAverage)
            {
                Debug.Log("Current: " + a);   
            }

            foreach (var b in previousWeekAverage)
            {
                Debug.Log("Previous: " + b);
            }
            
            SetChart_Compare(currentWeekAverage, previousWeekAverage);
        }

    // User CompareSide() method calls the new function
    public void CompareSide()
    {
        CompareAngleAverages();
    }

    public void SetChart_Compare(List<float> current, List<float> previous)
    {
        var Current = CompareChart.GetSerie("Current");
        var Previous = CompareChart.GetSerie("Previous");
        
        Current.ClearData();
        Previous.ClearData();
        

        for (int i = 0; i < current.Count; i++)
        {
            Current.AddData(current[i]);
            Previous.AddData(previous[i]);
        }
    }

    #endregion

    [Header("PlusData")] public plusDataEnabled plusDataEnabled;
}
