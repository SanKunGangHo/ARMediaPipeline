using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using XCharts.Runtime;

public class plusDataEnabled : MonoBehaviour
{
    public MyDataManager mdm;
    public TMP_Text totalOpe, maxOpe, taskComp, avgOpe, MaxInc, AvgInc;
    private List<DateTime> increase = new List<DateTime>();
    
    public LineChart lineChart_Max, lineChart_Ave;
    
    private void OnEnable()
    {
        TotalMaxOpeSet();
        TaskCompletionSet();
        MaximumIncreaseRateSet();
        MaxGraphInput();
        AverageIncreaseRateSet();
        AveGraphInput();
    }

    private void TotalMaxOpeSet()
    {
        float totalSum = 0;
        float totalAvg = 0;
        float maxOpeValue = float.MinValue;
        int count = 0;
        foreach (var day in mdm._history.Children)
        {
            foreach (var direction in day.Child("Angles_Max_Before").Children)
            {
                if(direction.Key.Contains("dateTime")) continue;
                float value = float.Parse(direction.Value.ToString());
                totalSum += value;
                count++;
                
                if (value > maxOpeValue)
                {
                    maxOpeValue = value;
                }
            }
        }
        
        totalAvg = totalSum / count;
        
        totalOpe.text = totalAvg.ToString("000");
        maxOpe.text = maxOpeValue.ToString("000.00");
        
        
        totalSum = 0;
        totalAvg = 0;
        count = 0;
        
        foreach (var day in mdm._history.Children)
        {
            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if(direction.Key.Contains("dateTime")) continue;
                float value = float.Parse(direction.Value.ToString());
                totalSum += value;
                count++;
            }
        }

        totalAvg = totalSum / count;
        
        avgOpe.text = totalAvg.ToString("000.00");
    }

    private void TaskCompletionSet()
    {
        Dictionary<int, List<ExerciseData>> checkExercise = new Dictionary<int, List<ExerciseData>>();
        switch (NetworkManager._instance._playerData.category)
        {
            case "Shoulder Impingement Syndrome":
                checkExercise = NetworkManager._instance.ta.ShoulderImpingementSyndrome;
                break;
            case "Adhesive Capsulitis":
                checkExercise = NetworkManager._instance.ta.adhesiveCapsulitis;
                break;
            default:
                checkExercise = NetworkManager._instance.ta.exercisesByDay;
                break;
        }
        
        Dictionary<int, List<bool>> checkTasks = new Dictionary<int, List<bool>>();

        for (int i = 0; i <= NetworkManager._instance._playerData.day; i++) //
        {
            // 미리 리스트를 초기화 해줍니다.
            if (!checkTasks.ContainsKey(i))
            {
                checkTasks[i] = new List<bool>();
            }
            
            DateTime day = DateTime.Parse(NetworkManager._instance._playerData.firstDate).AddDays(i);
            foreach (var exercises in checkExercise.Values)
            {
                foreach (var aExercise in exercises)
                {
                    foreach (var fbExercise in mdm._user.Child("ExerciseDatas").Child(aExercise.Title).Child("history").Children)//딕셔너리에 있는 운동 날짜
                    {
                        if (fbExercise.Key.Contains(day.ToString("yyyy-MM-dd")))
                        {
                            checkTasks[i].Add(true);
                        }
                    }
                }
            }
        }
        
        float tasksCompleted = 0;
        foreach (var task in checkTasks.Values)
        {
            if (task.Count != 0)
            {
                tasksCompleted++;
                Debug.Log(tasksCompleted+"/" +checkTasks.Count);
            }
        }
        
        Debug.Log((tasksCompleted/checkTasks.Count)*100);
        taskComp.text = (tasksCompleted/checkTasks.Count)*100 + "%";
    }

    public void MaximumIncreaseRateSet()
    {
        foreach (var day in mdm._history.Children)
        {
            increase.Add(DateTime.Parse(day.Key.Replace("day ", "")));
        }
        increase.Sort();


        float firstDay = 0;
        float lastDay = 0;
        foreach (var day in mdm._history.Children)
        {
            if (day.Key.Contains(increase[0].ToString("yyyy-MM-dd")))
            {
                foreach (var direction in day.Child("Angles_Max_Before").Children)
                {
                    if(direction.Key.Contains("dateTime")) continue;
                    firstDay += float.Parse(direction.Value.ToString());
                }
                firstDay /= day.Child("Angles_Max_Before").ChildrenCount;
            }
            else if(day.Key.Contains(increase[^1].ToString("yyyy-MM-dd")))
            {
                foreach (var direction in day.Child("Angles_Max_Before").Children)
                {
                    if(direction.Key.Contains("dateTime")) continue;
                    lastDay += float.Parse(direction.Value.ToString());
                }
                lastDay /= day.Child("Angles_Max_Before").ChildrenCount;
            }
        }
        
        MaxInc.text = lastDay.ToString("000.00");
    }

    public void MaxGraphInput()
    {
        Dictionary<string, float> maxValuesByDate = new Dictionary<string, float>();

        foreach (var day in mdm._history.Children)
        {
            float maxOfDay = float.MinValue;
            foreach (var direction in day.Child("Angles_Max_Before").Children)
            {
                if (direction.Key.Contains("dateTime")) continue;
                float value = float.Parse(direction.Value.ToString());
                if (value > maxOfDay)
                {
                    maxOfDay = value;
                }
            }

            // 동일 날짜가 존재하면 기존 값과 비교하여 더 큰 값을 저장
            if (maxValuesByDate.ContainsKey(day.Key))
            {
                if (maxOfDay > maxValuesByDate[day.Key])
                {
                    maxValuesByDate[day.Key] = maxOfDay;
                }
            }
            else
            {
                maxValuesByDate[day.Key] = maxOfDay;
            }
        }

        // Optional: If you have a way to display this data, for example:
        int count = 0;
        lineChart_Max.ClearData();
        foreach (var entry in maxValuesByDate)
        {
            lineChart_Max.AddData("serie0", count, entry.Value);
            Debug.Log($"Date: {entry.Key}, Max Value: {entry.Value}");
            count++;
        }
    }
    
    public void AverageIncreaseRateSet()
    {
        foreach (var day in mdm._history.Children)
        {
            increase.Add(DateTime.Parse(day.Key.Replace("day ", "")));
        }
        increase.Sort();
        
        float firstDay = 0;
        float lastDay = 0;
        foreach (var day in mdm._history.Children)
        {
            if (day.Key.Contains(increase[0].ToString("yyyy-MM-dd")))
            {
                foreach (var direction in day.Child("Angles_Ave_Before").Children)
                {
                    if(direction.Key.Contains("dateTime")) continue;
                    firstDay += float.Parse(direction.Value.ToString());
                }
                firstDay /= day.Child("Angles_Ave_Before").ChildrenCount;
            }
            else if(day.Key.Contains(increase[^1].ToString("yyyy-MM-dd")))
            {
                foreach (var direction in day.Child("Angles_Ave_Before").Children)
                {
                    if(direction.Key.Contains("dateTime")) continue;
                    lastDay += float.Parse(direction.Value.ToString());
                }
                lastDay /= day.Child("Angles_Ave_Before").ChildrenCount;
            }
        }

        float plusplus = (lastDay - firstDay) / firstDay * 100;
        AvgInc.text = $"{plusplus:0}%";
    }
    
    List<DateTime> DateTimes = new List<DateTime>();
    
    public void AveGraphInput()
    {
        Dictionary<string, float> aveValuesByDate = new Dictionary<string, float>();

        foreach (var day in mdm._history.Children)
        {
            float aveOfDay = float.MinValue;
            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if (direction.Key.Contains("dateTime")) continue;
                float value = float.Parse(direction.Value.ToString());
                if (value > aveOfDay)
                {
                    aveOfDay = value;
                }
            }

            // 동일 날짜가 존재하면 기존 값과 비교하여 더 큰 값을 저장
            if (aveValuesByDate.ContainsKey(day.Key))
            {
                if (aveOfDay > aveValuesByDate[day.Key])
                {
                    aveValuesByDate[day.Key] = aveOfDay;
                }
            }
            else
            {
                aveValuesByDate[day.Key] = aveOfDay;
            }
        }

        // Optional: If you have a way to display this data, for example:
        int count = 0;
        lineChart_Ave.ClearData();
        foreach (var entry in aveValuesByDate)
        {
            lineChart_Ave.AddData("serie0", count, entry.Value);
            Debug.Log($"Date: {entry.Key}, Average Value: {entry.Value}");
            count++;
        }
    }
    
    // public IEnumerator AngleResult()
    // {
    //
    //     AdminManager.Instance.playerdata = new PlayerData();
    //
    //     yield return GetClosestTimeEntry();
    //
    //     string angleType_Max_Before = "Angles_Max_Before";
    //     string angleType_Ave_Before = "Angles_Ave_Before";
    //     string angleType_Max_After = "Angles_Max_After";
    //     string angleType_Ave_After = "Angles_Ave_After";
    //
    //     if (DateTimes.Count <= 0)
    //     {
    //         ResultManager rm = Instantiate(AdminManager.Instance.reports, exerciseReportsList.transform);
    //         rm.No_ResultSort();
    //         yield break;
    //     }
    //     
    //     foreach (var dt in DateTimes)
    //     {
    //         List<float> angle_float_Max_Before = new List<float>();
    //         List<float> angle_float_Ave_Before = new List<float>();
    //
    //         DataSnapshot snapshot = mdm._user.Child("ExerciseDatas").Child("Measurement").Child("history").Child("day " + dt.ToString("yyyy-MM-dd HH:mm"));
    //         
    //         bool beforeProcessed = ProcessAndSendAnalytics(snapshot.Child(angleType_Max_Before),
    //                                    angle_float_Max_Before)
    //                                && ProcessAndSendAnalytics(snapshot.Child(angleType_Ave_Before),
    //                                    angle_float_Ave_Before);
    //
    //         if (beforeProcessed)
    //         {
    //             //_databaseReference.GetValueAsync().ContinueWithOnMainThread(task => {
    //             AdminManager.Instance.playerdata.operationResult_Max_Before = angle_float_Max_Before;
    //             AdminManager.Instance.playerdata.operationResult_Ave_Before = angle_float_Ave_Before;
    //
    //         }
    //
    //         ResultManager rm = Instantiate(AdminManager.Instance.reports, exerciseReportsList.transform);
    //         rm.ResultSort(dt.ToString("yyyy-MM-dd HH:mm"));
    //     }
    // }
    //
    // private bool ProcessAndSendAnalytics(DataSnapshot snapshot, List<float> outputList)
    // {
    //     if (snapshot != null && snapshot.Value != null)
    //     {
    //         IDictionary anglesDictionary = (IDictionary)snapshot.Value;
    //
    //         if (anglesDictionary != null)
    //         {
    //             outputList.Add(float.Parse(anglesDictionary["LeftSide"].ToString()));
    //             outputList.Add(float.Parse(anglesDictionary["RightSide"].ToString()));
    //             outputList.Add(float.Parse(anglesDictionary["LeftForward"].ToString()));
    //             outputList.Add(float.Parse(anglesDictionary["RightForward"].ToString()));
    //
    //             return true;
    //         }
    //         else
    //         {
    //             Debug.LogError("Snapshot is null!");
    //         }
    //
    //
    //     }
    //     else
    //     {
    //         Debug.LogError("There is no snapshot data.");
    //     }
    //
    //     return false;
    // }
    //
    // private IEnumerator GetClosestTimeEntry()
    // {
    //     DataSnapshot snapshot = mdm._history;
    //     List<DateTime> dateTimes = new List<DateTime>();
    //
    //     foreach (var childSnapshot in snapshot.Children)
    //     {
    //         string timestamp = childSnapshot.Key;
    //         timestamp = timestamp.Replace("day ", "");
    //         if (DateTime.TryParse(timestamp, out DateTime dateTime))
    //         {
    //             Debug.Log("parse Success");
    //             dateTimes.Add(dateTime);
    //         }
    //         else
    //         {
    //             Debug.LogError("Invalid date format: " + timestamp);
    //         }
    //     }
    //
    //     dateTimes.Reverse();
    //     DateTimes = dateTimes;
    //
    //     yield return null;
    // }
    //
}
