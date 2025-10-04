using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.PlayerLoop;
using XCharts.Runtime;

public class AveragePainReduction : MonoBehaviour
{
    public DataSnapshot _snapshot;
    public LineChart lineChart;

    private List<DataSnapshot> patientImpingement = new List<DataSnapshot>();
    private List<DataSnapshot> patientAdhesive = new List<DataSnapshot>();

    public Dictionary<string, float> ImpingementSum = new Dictionary<string, float>();
    public Dictionary<string, float> AdhesiveSum = new Dictionary<string, float>();
    public Dictionary<string, float> TimeCount_im = new Dictionary<string, float>();
    public Dictionary<string, float> TimeCount_ad = new Dictionary<string, float>();
    
    DateTime today = DateTime.Today;
    List<string> dayList = new List<string>();

    private string formatWithDay = "yyyy-MM-dd";

    public void main()
    {
        dayList = new List<string>
        {
            today.AddDays(-6).ToString(formatWithDay),
            today.AddDays(-5).ToString(formatWithDay),
            today.AddDays(-4).ToString(formatWithDay),
            today.AddDays(-3).ToString(formatWithDay),
            today.AddDays(-2).ToString(formatWithDay),
            today.AddDays(-1).ToString(formatWithDay),
            today.ToString(formatWithDay)
        };
        StartCoroutine(lineMaker());
    }

    IEnumerator lineMaker()
    {
        yield return StartCoroutine(DataSet());
        
        lineChart.GetChartComponent<XAxis>().data.Clear();
        foreach (var dayString in dayList)
        {
            DateTime day = DateTime.Parse(dayString);
            lineChart.GetChartComponent<XAxis>().data.Add(day.ToString("MM/dd/yyyy"));
        }

        lineChart.GetChartComponent<XAxis>().axisLabel.show = false;
        yield return null;
        lineChart.GetChartComponent<XAxis>().axisLabel.show = true;
        lineChart.RefreshChart();
        
        Serie imp = lineChart.GetSerie("Impingement");
        yield return StartCoroutine(SerieMaker(imp));
        Serie adh = lineChart.GetSerie("Adhesive");
        yield return StartCoroutine(SerieMaker(adh));
    }

    IEnumerator SerieMaker(Serie serie)
    {
        serie.ClearData();
        
        Dictionary<string, float> DicSum = new Dictionary<string, float>();
        Dictionary<string, float> count = new Dictionary<string, float>();
        switch (serie.serieName)
        {
            case "Impingement":
                DicSum = ImpingementSum;
                count = TimeCount_im;
                break;
            case "Adhesive":
                DicSum = AdhesiveSum;
                count = TimeCount_ad;
                break;
        }
        
        foreach (var day in dayList)
        {
            if (DicSum.ContainsKey(day))
            {
                float sum = 0;
                foreach (var dicfloat in DicSum)
                {
                    if (dicfloat.Key.Contains(day))
                    {
                        sum += dicfloat.Value;
                    }
                }
                DicSum[day] = sum / count[day];
            }
        }

        foreach (var day in dayList)
        {
            if (DicSum.ContainsKey(day))
            {
                serie.AddData(DicSum[day]);
            }
            else
            {
                serie.AddData(0);
            }
        }
        yield return null;
    }

    IEnumerator DataSet()
    {
        foreach (var user in _snapshot.Children)
        {
            yield return StartCoroutine(UserSet(user));
        }
        

        foreach (var patient in patientImpingement)
        {
            yield return StartCoroutine(PatientSet(patient, "Impingement"));
        }

        foreach (var patient in patientAdhesive)
        {
            yield return StartCoroutine(PatientSet(patient, "Adhesive"));
        }
    }

    IEnumerator UserSet(DataSnapshot user)
    {
        if (user.Child("category").Exists)
        {
            if (user.Child("category").Value.ToString().Contains("Impingement"))
            {
                patientImpingement.Add(user);
            }
            else if (user.Child("category").Value.ToString().Contains("Adhesive"))
            {
                patientAdhesive.Add(user);
            }
        }
        //남은 건 건강한 사람들
        yield break;
    }

    IEnumerator PatientSet(DataSnapshot patient, string category)
    {
        DataSnapshot _history = patient.Child("ExerciseDatas").Child("Measurement").Child("history");
        switch (category)
        {
            case "Impingement":
                yield return StartCoroutine(AVGMaker(_history, category));
                break;
            case "Adhesive":
                yield return StartCoroutine(AVGMaker(_history, category));
                break;
        }

        yield return null;
    }
    
    IEnumerator AVGMaker(DataSnapshot _history, string category)
    {
        foreach (var day in _history.Children)
        {
            yield return StartCoroutine(DaySet(day, category));
        }
        
        yield return null;
    }
    
    IEnumerator DaySet(DataSnapshot day, string category)
    {
        string playTime = "";
        foreach (var time in dayList)
        {
            if (day.Key.Contains(time))
            {
                playTime = time;
                break;
            }
        }

        if (string.IsNullOrEmpty(playTime))
        {
            yield break;
        }
        
        float categorySum = 0;
        float count = 0;
        
        foreach (var direction in day.Child("Angles_Ave_Before").Children)
        {
            if(direction.Key.Contains("dateTime")) continue;
            categorySum += float.Parse(direction.Value.ToString());
            count++;
        }

        switch (category)
        {
            case "Impingement":
                if (!ImpingementSum.TryAdd(playTime, categorySum))
                {
                    ImpingementSum[playTime] += categorySum;
                }
                if (!TimeCount_im.TryAdd(playTime, count))
                {
                    TimeCount_im[playTime] += count;
                }
                break;
            case "Adhesive":
                if (!AdhesiveSum.TryAdd(playTime, categorySum))
                {
                    AdhesiveSum[playTime] += categorySum;
                }
                if (!TimeCount_ad.TryAdd(playTime, count))
                {
                    TimeCount_ad[playTime] += count;
                }
                break;
            default:
                break;
        }
        //ImpingementSum += categorySum;
    }
    
    
}
