using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using XCharts.Runtime;

public class OverviewManager : MonoBehaviour
{
    private DataSnapshot _snap;
    private string formatWithDay = "yyyy-MM-dd";
    
    public TMP_Text todayUser;
    [Space(10)] public TMP_Text newCommerw;
    public TMP_Text totalUsers;
    [Space(10)] public AveragePainReduction avgPainReduction;
    
    [Space(10)] 
    public BaseChart genderChart;
    public BaseChart ageChart;
    public BaseChart categoryChart;

    [Space(10)] 
    [Tooltip("prefab")]
    public GameObject jobChart_iso;
    [Tooltip("Transform")]
    public Transform jobChart_transform;
    private Dictionary<string, BarChart> jobCharts = new Dictionary<string, BarChart>(8);
    
    [Space(10)]
    public Transform requestTransform;
    public GameObject request_iso;
    public List<Sprite> request_userIcons;
    
    private Dictionary<string, Dictionary<string, float>> daysDict = new Dictionary<string, Dictionary<string, float>>();
    private Dictionary<string, Dictionary<string, float>> countDict = new Dictionary<string, Dictionary<string, float>>();
    
    private List<string> timeList = new List<string>();
    private float[,] _historySum = new float[8, 3];

    public void Start()
    {
        OverviewBTN();
    }
    
    public void OverviewBTN()
    {
        DateTime today = DateTime.Today;
        timeList = new List<string>
        {
            today.AddDays(-2).ToString(formatWithDay),
            today.AddDays(-1).ToString(formatWithDay),
            today.AddDays(-15).ToString(formatWithDay),
            today.ToString(formatWithDay)
        };
        AdminManager.Instance.userView.SetActive(false);
        AdminManager.Instance.OverView.SetActive(true);
        StartCoroutine(OverviewSet());
    }

    // Start is called before the first frame update
    IEnumerator OverviewSet()
    {
        yield return StartCoroutine(RefreshOverview());

        yield return StartCoroutine(StartUser());
        yield return StartCoroutine(TotalUserCount());
        yield return StartCoroutine(RequestSet());

        yield return StartCoroutine(XChartSet());

    }

    IEnumerator RequestSet()
    {
        foreach (Transform request in requestTransform)
        {
            Destroy(request.gameObject);
        }
        
        DataSnapshot _request = _snap.Child("Admin001").Child("PatientCall");

        foreach (var day in _request.Children)
        {
            foreach (var value in day.Children)
            {
                GameObject instance = Instantiate(request_iso, requestTransform);
                if (_snap.Child(value.Key).Exists)
                {
                    switch (_snap.Child(value.Key).Child("gender").Value.ToString())
                    {
                        case "Man":
                            instance.transform.GetChild(0).GetComponent<Image>().sprite = request_userIcons[0];
                            break;
                        case "Woman":
                            instance.transform.GetChild(0).GetComponent<Image>().sprite = request_userIcons[1];
                            break;
                        case "RatherNotSay":
                        case "Rather Not Say":
                            instance.transform.GetChild(0).GetComponent<Image>().sprite = request_userIcons[2];
                            break;
                    }

                    instance.transform.GetChild(1).GetComponent<TMP_Text>().text = value.Key;
                }
            }

        }

        yield return null;
    }
    
    IEnumerator RefreshOverview()
    {
        var task = AdminManager.Instance._databaseReference.GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

        _snap = task.Result;
        if (!_snap.Exists)
        {
            _snap = AdminManager.Instance._snap;
        }

        avgPainReduction._snapshot = _snap;
        avgPainReduction.main();
    }

    IEnumerator StartUser() //ok
    {
        int userCount = 0;
        foreach (var user in _snap.Children)
        {
            if (user.Child("lastDate").ToString().Contains(DateTime.Today.ToString("yyyy-MM-dd")))
            {
                userCount++;
            }
        }

        newCommerw.text = userCount.ToString();
        yield return null;
    }


    IEnumerator TotalUserCount() //ok
    {
        int userCount = 0;

        foreach (var user in _snap.Children)
        {
            if (user.Exists)
            {
                userCount++;
            }

            if (user.Child("isAdmin").Exists || !user.Child("lastDate").Exists)
            {
                userCount--;
            }
        }
        
        totalUsers.text = "/"+userCount.ToString();
        yield return null;
    }

    IEnumerator XChartSet()
    {
        yield return StartCoroutine(findGender());
        yield return StartCoroutine(findOld());
        yield return StartCoroutine(findDisease());
        
        //카테고리로 비교
        
        yield return StartCoroutine( findJob());
    }

    IEnumerator findGender()
    {
        foreach (var user in _snap.Children)
        {
            if (user.Child("gender").Exists && !string.IsNullOrEmpty(user.Child("gender").Value.ToString()))
            {
                string gen = user.Child("gender").Value.ToString();
                switch (gen)
                {
                    case "Man":
                        yield return DataSet(user, gen);
                        break;
                    case "Woman":
                        yield return DataSet(user, gen);
                        break;
                    case "RatherNotSay":
                    case "Rather Not Say":
                        yield return DataSet(user, "RatherNotSay");
                        break;
                }
            }
        }

        genderChart.GetChartComponent<XAxis>().ClearData();
        
        Serie serieM = genderChart.GetSerie("M");
        Serie serieF = genderChart.GetSerie("F");
        Serie serieOther = genderChart.GetSerie("Other");
        
        serieM.ClearData();
        serieF.ClearData();
        serieOther.ClearData();
        
        foreach (var timeString in timeList)
        {
            DateTime changeTime = DateTime.Parse(timeString); 
            genderChart.GetChartComponent<XAxis>().AddData(changeTime.ToString("MM/dd/yyyy"));
            
            if (daysDict.ContainsKey(timeString))
            {
                if (daysDict[timeString].ContainsKey("Man"))
                {
                    float man = daysDict[timeString]["Man"] / countDict[timeString]["Man"];
                    serieM.AddData(man);
                }
                if (daysDict[timeString].ContainsKey("Woman"))
                {
                    float woman = daysDict[timeString]["Woman"] / countDict[timeString]["Woman"];
                    serieF.AddData(woman);
                }
                if (daysDict[timeString].ContainsKey("RatherNotSay"))
                {
                    float other = daysDict[timeString]["RatherNotSay"] / countDict[timeString]["RatherNotSay"];
                    serieM.AddData(other);
                }
            }
            else
            {
                serieM.AddData(0);
                serieF.AddData(0);
                serieOther.AddData(0);
            }
        }
        yield return null;
    }

    IEnumerator findOld()
    {
        foreach (var user in _snap.Children)
        {
            if (user.Child("dateOfBirth").Exists && !string.IsNullOrEmpty(user.Child("dateOfBirth").Value.ToString()))
            {
                if (DateTime.TryParse(user.Child("dateOfBirth").Value.ToString(), out var dateOfBirth))
                {
                    int age = CalculateAge(dateOfBirth);
                    string ageGroup = GetAgeGroup(age);
                    
                    switch (ageGroup)
                    {
                        case "10대":
                        case "10세 미만":
                            yield return StartCoroutine(DataSet(user, "10대"));
                            break;
                        case "20대":
                            yield return StartCoroutine(DataSet(user, ageGroup));
                            break;
                        case "30대":
                            yield return StartCoroutine(DataSet(user, ageGroup));
                            break;
                        case "40대":
                            yield return StartCoroutine(DataSet(user, ageGroup));
                            break;
                        case "50대 이상":
                            yield return StartCoroutine(DataSet(user, ageGroup));
                            break;
                    }
                }
                else
                {
                    Debug.LogWarning($"Invalid dateOfBirth: {user.Child("dateOfBirth").Value.ToString()}");
                }
            }
        }
        
        ageChart.GetChartComponent<XAxis>().ClearData();
        
        Serie serie10 = ageChart.GetSerie("10");
        Serie serie20 = ageChart.GetSerie("20");
        Serie serie30 = ageChart.GetSerie("30");
        Serie serie40 = ageChart.GetSerie("40");
        Serie serie50 = ageChart.GetSerie("50");
        
        serie10.ClearData();
        serie20.ClearData();
        serie30.ClearData();
        serie40.ClearData();
        serie50.ClearData();
        
        foreach (var timeString in timeList)
        {
            DateTime changeTime = DateTime.Parse(timeString);
            ageChart.GetChartComponent<XAxis>().AddData(changeTime.ToString("MM/dd/yyyy"));

            if (daysDict.ContainsKey(timeString))
            {
                if (daysDict[timeString].ContainsKey("10대"))
                {
                    float teen = daysDict[timeString]["10대"] / countDict[timeString]["10대"];
                    serie10.AddData(teen);
                }

                if (daysDict[timeString].ContainsKey("20대"))
                {
                    float twenty = daysDict[timeString]["20대"] / countDict[timeString]["20대"];
                    serie20.AddData(twenty);
                }
                
                if (daysDict[timeString].ContainsKey("30대"))
                {
                    float thirty = daysDict[timeString]["30대"] / countDict[timeString]["30대"];
                    serie30.AddData(thirty);
                }

                if (daysDict[timeString].ContainsKey("40대"))
                {
                    float forty = daysDict[timeString]["40대"] / daysDict[timeString]["40대"];
                    serie40.AddData(forty);
                }
                
                if (daysDict[timeString].ContainsKey("50대 이상"))
                {
                    float fiftyOver = daysDict[timeString]["50대 이상"] / daysDict[timeString]["50대 이상"];
                    serie50.AddData(fiftyOver);
                }
            }
            else
            {
                serie10.AddData(0);
                serie20.AddData(0);
                serie30.AddData(0);
                serie40.AddData(0);
                serie50.AddData(0);
            }
        }

        yield break;
    }

    IEnumerator findDisease()
    {
        foreach (var user in _snap.Children)
        {
            if (user.Child("category").Exists && !string.IsNullOrEmpty(user.Child("category").Value.ToString()))
            {
                string category = user.Child("category").Value.ToString();
                switch (category)
                {
                    case "Shoulder Impingement Syndrome":
                        yield return StartCoroutine(DataSet(user, category));
                        break;
                    case "Adhesive Capsulitis":
                        yield return StartCoroutine(DataSet(user, category));
                        break;
                    case "Common":
                        yield return StartCoroutine(DataSet(user, category));
                        break;
                    default:
                        yield return StartCoroutine(DataSet(user, "Common"));
                        break;
                }
            }
            
            categoryChart.GetChartComponent<XAxis>().ClearData();

            Serie serieCommon = categoryChart.GetSerie("Common");
            Serie serieImpingement = categoryChart.GetSerie("Impingement");
            Serie serieAdhesive = categoryChart.GetSerie("Adhesive");

            serieCommon.ClearData();
            serieImpingement.ClearData();
            serieAdhesive.ClearData();
            
            foreach (var timeString in timeList)
            {
                DateTime changeTime = DateTime.Parse(timeString);
                categoryChart.GetChartComponent<XAxis>().AddData(changeTime.ToString("MM/dd/yyyy"));

                if (daysDict.ContainsKey(timeString))
                {
                    if (daysDict[timeString].ContainsKey("Common"))
                    {
                        float common = daysDict[timeString]["Common"] / countDict[timeString]["Common"];
                        serieCommon.AddData(common);
                    }

                    if (daysDict[timeString].ContainsKey("Shoulder Impingement Syndrome"))
                    {
                        float impingement = daysDict[timeString]["Shoulder Impingement Syndrome"] / countDict[timeString]["Shoulder Impingement Syndrome"];
                        serieImpingement.AddData(impingement);
                    }

                    if (daysDict[timeString].ContainsKey("Adhesive Capsulitis"))
                    {
                        float adhesive = daysDict[timeString]["Adhesive Capsulitis"] / countDict[timeString]["Adhesive Capsulitis"];
                        serieAdhesive.AddData(adhesive);
                    }
                }
                else
                {
                    serieCommon.AddData(0);
                    serieImpingement.AddData(0);
                    serieAdhesive.AddData(0);
                }
            }
        }
        yield break;
    }
    
    // 나이를 계산하는 헬퍼 함수
    int CalculateAge(DateTime dateOfBirth)
    {
        DateTime today = DateTime.Today;
        int age = today.Year - dateOfBirth.Year;

        if (dateOfBirth.Date > today.AddYears(-age)) age--;

        return age;
    }

    // 나이대를 구하는 헬퍼 함수
    string GetAgeGroup(int age)
    {
        if (age >= 10 && age < 20)
        {
            return "10대";
        }
        else if (age >= 20 && age < 30)
        {
            return "20대";
        }
        else if (age >= 30 && age < 40)
        {
            return "30대";
        }
        else if (age >= 40 && age < 50)
        {
            return "40대";
        }
        else if (age >= 50)
        {
            return "50대 이상";
        }
        else
        {
            return "10세 미만";
        }
    }

    IEnumerator DataSet(DataSnapshot user, string data)
    {
        DataSnapshot _history = user.Child("ExerciseDatas").Child("Measurement").Child("history");
        
        foreach (var day in _history.Children)
        {
            string playTime = "";
            foreach (var time in timeList)
            {
                if (day.Key.Contains(time))
                {
                    playTime = time;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(playTime))
            {
                continue;
            }
            
            float categorySum = 0;
            float count = 0;
            
            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if(direction.Key.Contains("dateTime")) continue;
                categorySum += float.Parse(direction.Value.ToString());
                count++;
            }
            
            // 키가 존재하는지 확인합니다.
            if (!daysDict.TryGetValue(playTime, out var innerDict))
            {
                // 존재하지 않는다면 새로운 사전을 추가합니다.
                innerDict = new Dictionary<string, float>();
                daysDict[playTime] = innerDict;
            }
            
            if (!countDict.TryGetValue(playTime, out var innerCountDict))
            {
                // 존재하지 않는다면 새로운 사전을 추가합니다.
                innerCountDict = new Dictionary<string, float>();
                countDict[playTime] = innerCountDict;
            }

            // 이제 innerDict에서 data 키가 존재하는지 확인하고 값을 업데이트합니다.
            if (innerDict.ContainsKey(data))
            {
                innerDict[data] += categorySum;
            }
            else
            {
                innerDict[data] = categorySum;
            }

            if (innerCountDict.ContainsKey(data))
            {
                innerCountDict[data] += count;
            }
            else
            {
                innerCountDict[data] = count;
            }
        }
        
        yield return null;
    }
    
    IEnumerator DataSet_job(DataSnapshot user, string data, string symptoms, int index)
    {
        DataSnapshot _history = user.Child("ExerciseDatas").Child("Measurement").Child("history");
        
        foreach (var day in _history.Children)
        {
            string playTime = "";
            foreach (var time in timeList)
            {
                if (day.Key.Contains(time))
                {
                    playTime = time;
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(playTime))
            {
                continue;
            }
            
            float categorySum = 0;
            float count = 0;
            
            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if(direction.Key.Contains("dateTime")) continue;
                categorySum += float.Parse(direction.Value.ToString());
                count++;
            }

            switch (symptoms)
            {
                case "Common":
                    _historySum[index, 0] = categorySum / count;
                    break;
                case "Shoulder Impingement Syndrome":
                    _historySum[index, 1] = categorySum / count;
                    break;
                case "Adhesive Capsulitis":
                    _historySum[index, 2] = categorySum / count;
                    break;
                default:
                    break;
            }

            // 키가 존재하는지 확인합니다.
            if (!daysDict.TryGetValue(playTime, out var innerDict))
            {
                // 존재하지 않는다면 새로운 사전을 추가합니다.
                innerDict = new Dictionary<string, float>();
                daysDict[playTime] = innerDict;
            }
            
            if (!countDict.TryGetValue(playTime, out var innerCountDict))
            {
                // 존재하지 않는다면 새로운 사전을 추가합니다.
                innerCountDict = new Dictionary<string, float>();
                countDict[playTime] = innerCountDict;
            }

            // 이제 innerDict에서 data 키가 존재하는지 확인하고 값을 업데이트합니다.
            if (innerDict.ContainsKey(data))
            {
                innerDict[data] += categorySum;
            }
            else
            {
                innerDict[data] = categorySum;
            }

            if (innerCountDict.ContainsKey(data))
            {
                innerCountDict[data] += count;
            }
            else
            {
                innerCountDict[data] = count;
            }
        }
        
        yield return null;
    }

    IEnumerator findJob()
    {
        foreach (var user in _snap.Children)
        {
            if (user.Child("job").Exists && !string.IsNullOrEmpty(user.Child("job").Value.ToString()))
            {
                string job = user.Child("job").Value.ToString();
                string symptoms = "Common";

                if (user.Child("category").Exists && !string.IsNullOrEmpty(user.Child("category").Value.ToString()))
                {
                    symptoms = user.Child("category").Value.ToString();
                }

                switch (job)
                {
                    case "Office Worker":
                        yield return DataSet_job(user, job, symptoms, 0);
                        break;
                    case "Self-employed":
                        yield return DataSet_job(user, job, symptoms, 1);
                        break;
                    case "Logistics Worker":
                        yield return DataSet_job(user, job, symptoms, 2);
                        break;
                    case "Healthcare Worker":
                        yield return DataSet_job(user, job, symptoms, 3);
                        break;
                    case "Athlete":
                        yield return DataSet_job(user, job, symptoms, 4);
                        break;
                    case "Military and Police Officer":
                        yield return DataSet_job(user, job, symptoms, 5);
                        break;
                    case "Homemaker":
                        yield return DataSet_job(user, job, symptoms, 6);
                        break;
                    case "Others/Other Professions":
                        yield return DataSet_job(user, job, symptoms, 7);
                        break;
                    default:
                        yield return DataSet_job(user, "Others/Other Professions", symptoms, 7);
                        break;
                }
            }
        }
        
        jobCharts = new Dictionary<string, BarChart>()
        {
            {"Office Worker", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Self-employed", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Logistics Worker", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Healthcare Worker", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Athlete", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Military and Police Officer", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Homemaker", CreateAndGetBarChart(jobChart_iso, jobChart_transform)},
            {"Others/Other Professions", CreateAndGetBarChart(jobChart_iso, jobChart_transform)}
        };

        foreach (var charts in jobCharts)
        {
            StartCoroutine(findJob_chartMaker(charts));
        }
        
        yield return null;
    }

    IEnumerator findJob_chartMaker(KeyValuePair<string, BarChart> chart)
    {
        chart.Value.transform.parent.GetChild(0).GetComponent<TMP_Text>().text = chart.Key;
        chart.Value.ClearData();
        
        Serie ordinary = chart.Value.GetSerie("Ordinary");
        Serie impinge = chart.Value.GetSerie("Impingement");
        Serie adhesive = chart.Value.GetSerie("Adhesive");
        
        ordinary.ClearData();
        impinge.ClearData();
        adhesive.ClearData();

        int index = jobChartInputer(chart.Key);

        ordinary.AddData(Math.Floor(_historySum[index, 0]));
        impinge.AddData(Math.Floor(_historySum[index, 1]));
        adhesive.AddData(Math.Floor(_historySum[index, 2]));

        yield return null;
    }

    int jobChartInputer(string job)
    {
        int index = 0;
        
        switch (job)
        {
            case "Office Worker":
                index = 0;
                break;
            case "Self-employed":
                index = 1;
                break;
            case "Logistics Worker":
                index = 2;
                break;
            case "Healthcare Worker":
                index = 3;
                break;
            case "Athlete":
                index = 4;
                break;
            case "Military and Police Officer":
                index = 5;
                break;
            case "Homemaker":
                index = 6;
                break; 
            case "Others/Other Professions":
                index = 7;
                break;
        }
        
        return index;
    }
    
    private BarChart CreateAndGetBarChart(GameObject prefab, Transform parent)
    {
        GameObject instance = Instantiate(prefab, parent);
    
        if (instance.transform.childCount > 1)
        {
            return instance.transform.GetChild(1).GetComponent<BarChart>();
        }
        else
        {
            Debug.LogError("인스턴스화된 객체에 자식 요소가 충분하지 않습니다.");
            return null;
        }
    }
}
