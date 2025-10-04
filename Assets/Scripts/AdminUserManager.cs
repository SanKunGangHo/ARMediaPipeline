using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Firebase.Database;
using TMPro;
using UI.Dates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class AdminUserManager : MonoBehaviour
{
    public TMP_Text IDCodeText;
    public TMP_Text dobText;
    public TMP_Text genderText;
    public TMP_Text recentJobText;
    public TMP_Text lastDateText;
    [Space(10f)]
    public Transform exerciseList;
    public GameObject exercisePrefab;
    [Space(10f)]
    public List<DateTime> DateTimes = new List<DateTime>();

    //private float stepAvg_L = 0f;
    //private float stepAvg_R = 0f;

    private DataSnapshot _snap;

    private Dictionary<string, int> exerciseCountDict = new Dictionary<string, int>();

    // string AVG_AFTER = "Angles_Ave_After";
    // string AVE_BEFORE = "Angles_Ave_Before";
    // string MAX_AFTER = "Angles_Max_After";
    string MAX_BEFORE = "Angles_Max_Before";

    Dictionary<string, float> BestAngle = new Dictionary<string, float>();

    [Space(10f)]
    [Header("추가 작업_progress")]
    public TMP_Text categoryText;
    public LineChart categoryChart;
    public Dictionary<string, float> graphFloats = new Dictionary<string, float>();
    public TMP_Text AVG_Low, AVG_High;

    [Space(10f)]
    [Header("추가 작업_record")]
    public DatePicker dayPicker;
    public TMP_Text MonthText, dayText, LowAngle, HighAngle;

    [Space(10f)]
    [Header("추가 작업_뭔가 정보량 많은 그래프")]
    public BarChart exerciseChart;
    private List<string> timeList = new List<string>();
    private string formatWithDay = "yyyy-MM-dd";

    string[] monthNames = new string[]
{
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
};

    private void Start()
    {

        dayPicker.Config.Events.OnDaySelected.AddListener(UpdateDateValue);

    }

    /// <summary>
    /// 유저 버튼 클릭되었을 때
    /// </summary>
    /// <param name="snapshot">AdminManager에서 유저 데이터 가져와야함</param>
    public void UserClicked(DataSnapshot snapshot)
    {
        _snap = snapshot;

        IDCodeText.text = snapshot.Child("id").Value.ToString();
        dobText.text = snapshot.Child("dateOfBirth").Value.ToString();
        genderText.text = snapshot.Child("gender").Value.ToString();
        recentJobText.text = snapshot.Child("job").Value.ToString();
        lastDateText.text = snapshot.Child("lastDate").Value.ToString();

        DateTime today = DateTime.Today;
        timeList = new List<string>
        {
            today.AddDays(-6).ToString(formatWithDay),
            today.AddDays(-5).ToString(formatWithDay),
            today.AddDays(-4).ToString(formatWithDay),
            today.AddDays(-3).ToString(formatWithDay),
            today.AddDays(-2).ToString(formatWithDay),
            today.AddDays(-1).ToString(formatWithDay),
            today.ToString(formatWithDay)
        };

        graphFloats = new Dictionary<string, float>();

        AVG_Low.text = "00.00";
        AVG_High.text = "00.00";

        // test
        // SetBtnInteractable(snapshot);

        StartCoroutine(ShoulderAngleRotation(snapshot));
        StartCoroutine(ExerciseListMaker());
        StartCoroutine(RemoveProhibitedDates(DateTime.Today.Year, DateTime.Today.Month));
        // day 선택했을 때 호출
        // DayPickerSet(snapshot);
    }

    IEnumerator ShoulderAngleRotation(DataSnapshot snapshot)
    {
        _snap = snapshot;
        if (_snap.Child("category").Exists)
        {
            categoryText.text = _snap.Child("category").Value.ToString();
        }

        DataSnapshot history = _snap.Child("ExerciseDatas").Child("Measurement").Child("history");

        foreach (var day in history.Children)
        {
            string playtime = "";
            foreach (var timeString in timeList)
            {
                if (day.Key.Contains(timeString))
                {
                    playtime = timeString;
                    break;
                }
            }

            if (string.IsNullOrEmpty(playtime))
            {
                continue;
            }

            float categorySum = 0;
            float count = 0;

            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if (direction.Key.Contains("dateTime")) continue;
                categorySum += float.Parse(direction.Value.ToString());
                count++;
            }

            if (graphFloats.ContainsKey(playtime))
            {
                graphFloats[playtime] += categorySum / count;
            }
            else
            {
                graphFloats.Add(playtime, categorySum / count);
            }
        }

        categoryChart.GetChartComponent<XAxis>().ClearData();

        Serie angleSeries = categoryChart.GetSerie("serie0");

        angleSeries.ClearData();
        
        foreach (var timeString in timeList)
        {
            DateTime changeTime = DateTime.Parse(timeString);
            categoryChart.GetChartComponent<XAxis>().AddData(changeTime.ToString("MM/dd/yyyy"));

            if (graphFloats.ContainsKey(timeString))
            {
                angleSeries.AddData(graphFloats[timeString]);
            }
            else
            {
                angleSeries.AddData(0);
            }
        }

        if (graphFloats.Count > 0)
        {
            float maxValue = float.MinValue;
            float minValue = float.MaxValue;

            foreach (var value in graphFloats)
            {
                if (value.Value > maxValue)
                {
                    maxValue = value.Value;
                }
                if (value.Value < minValue)
                {
                    minValue = value.Value;
                }
                if (graphFloats.Count < 7)
                {
                    minValue = 0;
                }
            }

            AVG_Low.text = minValue.ToString("0.##");
            AVG_High.text = maxValue.ToString("0.##");
        }
        yield break;
    }

    private void AddProhibitedDates(int year, int month)
    {
        List<DateTime> dateTimes = GetDatesInMonth(year, month);
        for (int i = 0; i < dateTimes.Count; ++i)
        {
            if (!dayPicker.Config.DateRange.ProhibitedDates.Contains(dateTimes[i]))
            {
                if (dateTimes[i] == DateTime.Today)
                {
                    continue;
                }

                dayPicker.Config.DateRange.ProhibitedDates.Add(dateTimes[i]);
            }
        }

        dayPicker.UpdateDisplay();
    }

    /// <summary>
    /// 현재 달력에서 활동한 날짜 활성화
    /// </summary>
    public IEnumerator RemoveProhibitedDates(int year, int month)
    {
        yield return null;

        AddProhibitedDates(year,month);

        foreach (var day in _snap.Child("ExerciseDatas").Child("Measurement").Child("history").Children)
        {
            string date = day.Key.ToString().Replace("day ", "");
            DateTime parseDay = DateTime.Parse(date);
            DateTime resetDay = new DateTime(parseDay.Year, parseDay.Month, parseDay.Day);
            if (dayPicker.Config.DateRange.ProhibitedDates.Contains(resetDay))
            {
                dayPicker.Config.DateRange.ProhibitedDates.Remove(resetDay);
                dayPicker.UpdateDisplay();
            }
        }

    }

    public IEnumerator ExerciseListMaker()
    {
        foreach (Transform child in exerciseList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var ed in AdminManager.Instance.ExerciseDatas)
        {
            GameObject exercise = Instantiate(exercisePrefab, exerciseList);
            exercise.transform.GetChild(0).GetComponent<TMP_Text>().text = ed.Title;
            exercise.AddComponent<Button>().onClick.AddListener(() => StartCoroutine(StepGraphMaker(ed.Title)));
        }

        yield return null;
    }

    public List<DateTime> GetDatesInMonth(int year, int month)
    {
        DateTime firstDayOfMonth = new DateTime(year, month, 1);
        DateTime firstDayOfNextMonth = firstDayOfMonth.AddMonths(1);
        DateTime lastDayOfMonth = firstDayOfNextMonth.AddDays(-1);
        List<DateTime> dates = new List<DateTime>();
        for (DateTime date = firstDayOfMonth; date <= lastDayOfMonth; date = date.AddDays(1))
        {
            dates.Add(date);
        }

        return dates;
    }
    
    public IEnumerator StepGraphMaker(string Exercise)
    {
        //TODO : 그래프작업
        DataSnapshot snapshot = _snap.Child("ExerciseDatas").Child(Exercise).Child("history");

        Dictionary<string, Dictionary<string, float>> daysExercise_L = new Dictionary<string, Dictionary<string, float>>();
        Dictionary<string, Dictionary<string, float>> daysExercise_R = new Dictionary<string, Dictionary<string, float>>();

        int count_L = 0;
        int count_R = 0;
        foreach (var timeString in timeList)
        {
            foreach (var day in snapshot.Children)
            {
                if (day.Key.Contains(timeString))
                {
                    foreach (var angles_L in day.Children)
                    {
                        if (angles_L.Key.Contains("left"))
                        {
                            if (daysExercise_L.Keys.Contains(timeString))
                            {
                                if (!daysExercise_L[timeString].ContainsKey(Exercise + " Left"))
                                {
                                    daysExercise_L[timeString].Add(Exercise + " Left", 0);
                                }

                                daysExercise_L[timeString][Exercise + " Left"] +=
                                    float.Parse(angles_L.Value.ToString());
                                count_L++;
                            }
                            else
                            {
                                daysExercise_L.Add(timeString, new Dictionary<string, float>());
                            }
                        }
                    }

                    Debug.Log(timeString + " " + daysExercise_L[timeString].Count);

                    foreach (var angles_R in day.Children)
                    {
                        if (angles_R.Key.Contains("right"))
                        {
                            if (daysExercise_R.ContainsKey(timeString))
                            {
                                if (!daysExercise_R[timeString].ContainsKey(Exercise + " Right"))
                                {
                                    daysExercise_R[timeString].Add(Exercise + " Right", 0);
                                }

                                daysExercise_R[timeString][Exercise + " Right"] +=
                                    float.Parse(angles_R.Value.ToString());
                                count_R++;
                            }
                            else
                            {
                                daysExercise_R.Add(timeString, new Dictionary<string, float>());
                            }
                        }
                    }
                }
            }
        }

        exerciseChart.GetChartComponent<XAxis>().ClearData();
        Serie angleSeries_L = exerciseChart.GetSerie("Left average");
        Serie angleSeries_R = exerciseChart.GetSerie("Right average");
        angleSeries_L.ClearData();
        angleSeries_R.ClearData();

        //라인그래프부터
        foreach (var timeString in timeList)
        {
            DateTime time = DateTime.Parse(timeString);
            exerciseChart.GetChartComponent<XAxis>().AddData(time.ToString("MM/dd/yyyy"));

            if (daysExercise_L.ContainsKey(timeString))
            {
                if (daysExercise_L[timeString].ContainsKey(Exercise + " Left"))
                {
                    angleSeries_L.AddData(daysExercise_L[timeString][Exercise + " Left"] / count_L);
                }
            }
            else
            {
                angleSeries_L.AddData(0);
            }

            if (daysExercise_R.ContainsKey(timeString))
            {
                if (daysExercise_R[timeString].ContainsKey(Exercise + " Right"))
                {
                    angleSeries_R.AddData(daysExercise_R[timeString][Exercise + " Right"] / count_R);
                }
            }else
            {
                angleSeries_R.AddData(0);
            }
        }

        //--------------------------------------------큰거 비교

        Dictionary<string, float> days_L = new Dictionary<string, float>();
        Dictionary<string, float> days_R = new Dictionary<string, float>();

        foreach (var timeString in timeList)
        {
            foreach (var day in snapshot.Children)
            {
                if (day.Key.Contains(timeString))
                {
                    foreach (var angle in day.Children)
                    {
                        float angleValue = float.Parse(angle.Value.ToString());
                        if (angle.Key.Contains("left"))
                        {
                            if (!days_L.ContainsKey(timeString))
                            {
                                days_L.Add(timeString, angleValue);
                            }
                            else if (days_L[timeString] < angleValue)
                            {
                                days_L[timeString] = angleValue;
                            }
                        }
                        if(angle.Key.Contains("right"))
                        {
                            if (!days_R.ContainsKey(timeString))
                            {
                                days_R.Add(timeString, angleValue);
                            }
                            else if (days_R[timeString] < angleValue)
                            {
                                days_R[timeString] = angleValue;
                            }
                        }

                        if (days_L.ContainsKey(timeString))
                        {
                            Debug.Log(days_L[timeString]); 
                        }

                        if (days_R.ContainsKey(timeString))
                        {
                            Debug.Log(days_R[timeString]);
                        }
                    }
                }
                else
                {
                    Debug.Log("continue");
                    continue;
                }
            }
        }
        
        Serie leftHighSeries = exerciseChart.GetSerie("LeftHigh");
        Serie rightHighSeries = exerciseChart.GetSerie("RightHigh");
        
        leftHighSeries.ClearData();
        rightHighSeries.ClearData();

        foreach (var timeString in timeList)
        {
            if (days_L.ContainsKey(timeString))
            {
                leftHighSeries.AddData(days_L[timeString]);
            }
            else
            {
                leftHighSeries.AddData(0);
            }

            if (days_R.ContainsKey(timeString))
            {
                rightHighSeries.AddData(days_R[timeString]);
            }
            else
            {
                rightHighSeries.AddData(0);
            }
        }
        yield return null;
    }
    
    void UpdateDateValue(DateTime _dt)
    {
        _dt = dayPicker.SelectedDate;

        string dtToString = "";

        dtToString = _dt.ToString("yyyy-MM-dd");

        string monthInEnglish = monthNames[_dt.Month - 1];
        MonthText.text = monthInEnglish;
        dayText.text = _dt.Day.ToString();

        DataSnapshot history = _snap.Child("ExerciseDatas").Child("Measurement").Child("history");
        bool isExist = DateTime.Today.ToString("yyyy-MM-dd HH:mm").Contains(dtToString);

        foreach (var day in history.Children)
        {
            string dateString = day.Key.Replace("day ", "");
            dateString = dateString.Split(' ')[0];

            float lowvalue = float.MaxValue;
            float highvalue = float.MinValue;

            if (dateString == dtToString)
            {
                foreach (var value in day.Child(MAX_BEFORE).Children)
                {
                    isExist = false;
                    if (value.Key.Contains("dateTime")) continue;
                    float currentValue = float.Parse(value.Value.ToString());
                    // 최소값과 최대값 업데이트
                    if (currentValue < lowvalue)
                    {
                        lowvalue = currentValue; // 최소값 갱신
                    }

                    if (currentValue > highvalue)
                    {
                        highvalue = currentValue; // 최대값 갱신
                    }

                    LowAngle.text = string.Format("{0:N1}", lowvalue);
                    HighAngle.text = string.Format("{0:N1}", highvalue);
                }
            }
        }
        if (isExist)
        {
            LowAngle.text = "00";
            HighAngle.text = "00";
        }
    }
}
