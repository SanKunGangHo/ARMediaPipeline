using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;

public class Mobile_MyDetail : MonoBehaviour
{
    public MyDataManager myData;
    private DataSnapshot _history;

    private Dictionary<string, List<DataSnapshot>> angles = new Dictionary<string, List<DataSnapshot>>();

    public TMP_Text dayText, notAvailableText;

    [Tooltip("Prefab")]
    public GameObject tileManager;

    public GameObject Before;
    public GameObject After;
    // private GameObject selectTile;
    private Dictionary<string, GameObject> tileManagers = new Dictionary<string, GameObject>();
    public Transform ResultPos;

    private PlayerData pd;

    string leftSide = "LeftSide";
    string rightSide = "RightSide";
    string leftForward = "LeftForward";
    string rightForward = "RightForward";

    private int dayCount = 0;

    private Dictionary<string, List<float>> anglesMaxData;
    private Dictionary<string, List<float>> anglesAvgData;
    
    private void OnEnable()
    {
        _history = myData._history;//히스토리
        pd = NetworkManager._instance._playerData;
        StartCoroutine(CallDatas());
    }
    
    private IEnumerator CallDatas()
    {
        angles.Clear();
        if(myData._user.Child("days_list").Exists){
            foreach (var child in myData._user.Child("days_list").Children)
            {
                string thatDay = child.Key.ToString();
                angles[thatDay] = new List<DataSnapshot>();
                foreach (var day in _history.Children)
                {
                    if (day.Key.Contains(thatDay))
                    {
                        angles[thatDay].Add(day);
                    }
                }
            }
        }
        
        // string firstDate = pd.firstDate;
        // DateTime firstDay = DateTime.Parse(firstDate);
        // for(int i = 0; i < 20; i++)
        // {
        //     string thatDay = firstDay.AddDays(i).ToString("yyyy-MM-dd");
        //     angles[thatDay] = new List<DataSnapshot>();
        //     Debug.Log("that day :" + thatDay);
        //     foreach (var day in _history.Children)
        //     {
        //         Debug.Log("day key :"+day.Key);
        //         if (day.Key.Contains(thatDay))
        //         {
        //             Debug.Log("day key add :"+day.Key);
        //             angles[thatDay].Add(day);
        //         }
        //     }
        // }
        UpcomingUpdater(pd.day);
        yield return null;
    }
    
    private void AddTileManager(string key, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, ResultPos);
        instance.name = key;
        tileManagers.Add(key, instance);
    }
    
    public void NextDayButton()
    {
        UpcomingUpdater(1);
    }

    public void PreviousDayButton()
    {
        UpcomingUpdater(-1);
    }
    
    public void UpcomingUpdater(int i)
    {
        foreach (var manager in tileManagers.Values)
        {
            Destroy(manager.gameObject);
        }
        
        List<float> nowMaxData = new List<float>();
        List<float> nowAveData = new List<float>();

        tileManagers.Clear();
        
        dayCount += i;
        if (dayCount <= 0)
        {
            dayCount = 0;
        }
        else if (dayCount >= 19)
        {
            dayCount = 19;
        }

        if (dayCount > pd.day)
        {
            dayCount = pd.day;
        }
        
        AddTileManager(leftSide, tileManager);
        AddTileManager(rightSide, tileManager);
        AddTileManager(leftForward, tileManager);
        AddTileManager(rightForward, tileManager);
        
        anglesMaxData = new Dictionary<string, List<float>>();
        anglesAvgData = new Dictionary<string, List<float>>();
        
        List<string> selectedTimes = TimeSelector();

        if (selectedTimes.Count <= 0)
        {
            foreach (var manager in tileManagers.Values)
            {
                Destroy(manager.gameObject);
            }
            tileManagers.Clear();
            notAvailableText.gameObject.SetActive(true);
            return;
        }
        else
        {
            notAvailableText.gameObject.SetActive(false);
        }
        
        bool isAfter = false;
        foreach (var times in selectedTimes)
        {
            foreach (var angle in angles)
            {
                if (angle.Key.Contains(dayCount.ToString()))
                {
                    StartCoroutine(AngleInputer(times, angle));
                }
            }

            if (anglesMaxData.ContainsKey(times))
            {
                notAvailableText.gameObject.SetActive(false);
                if (isAfter)
                {
                    nowMaxData = anglesMaxData[times];
                    nowAveData = anglesAvgData[times];
                    MakeTile(nowMaxData[4], nowAveData[4], "Left", "Side", isAfter);
                    MakeTile(nowMaxData[5], nowAveData[5], "Right", "Side", isAfter);
                    MakeTile(nowMaxData[6], nowAveData[6], "Left", "Forward", isAfter);
                    MakeTile(nowMaxData[7], nowAveData[7], "Right", "Forward", isAfter);
                }
                else
                {
                    nowMaxData = anglesMaxData[times];
                    nowAveData = anglesAvgData[times];
                    MakeTile(nowMaxData[0], nowAveData[0], "Left", "Side", isAfter);
                    MakeTile(nowMaxData[1], nowAveData[1], "Right", "Side", isAfter);
                    MakeTile(nowMaxData[2], nowAveData[2], "Left", "Forward", isAfter);
                    MakeTile(nowMaxData[3], nowAveData[3], "Right", "Forward", isAfter);
                }
                isAfter = true;
            }
            else
            {
                foreach (var manager in tileManagers.Values)
                {
                    Destroy(manager.gameObject);
                }
                tileManagers.Clear();
                notAvailableText.gameObject.SetActive(true);
            }
        }
        
        // foreach (var manager in tileManagers)
        // {
        //     StartCoroutine(MakeTile(manager.Value));
        // }
        //UpcomingActivityActivate(dayCount);
    }
    
    private void MakeTile(float nowMaxData, float nowAveData, string beforeName, string afterName, bool isAfter = false)
    {
        string name = beforeName;
        GameObject selectTile = SelectTile(isAfter);

        if (isAfter)
        {
            name = afterName;
        }
        
        Transform instance = Instantiate(selectTile, tileManagers[beforeName+afterName].transform).transform;
        instance.name = name;
        instance.GetChild(0).GetComponent<TMP_Text>().text = name;
    
        instance.GetComponent<ResultAngle>().ResultGraph_Mobile(nowMaxData, nowAveData, isAfter);
    }

    public GameObject SelectTile(bool isAfter)
    {
        GameObject selectTile = new GameObject();
        switch (isAfter)
        {
            case false :
                selectTile = Before;
                break;
            case true:
                selectTile = After;
                break;
        }

        return selectTile;
    }

    public string NameSelector(bool isAfter, int count)
    {
        string name = "";
        switch (isAfter)
        {
            case false://전
                switch (count)
                {
                    case 0:
                    case 2:
                        name = "Left";
                        break;
                    case 1:
                    case 3:
                        name = "Right"; 
                        break;
                    default:
                        name = "";
                        break;
                }
                break;
            case true://후
                switch (count)
                {
                    case 0:
                    case 1:
                        name = "Side";
                        break;
                    case 2:
                    case 3:
                        name = "Forward"; 
                        break;
                    default:
                        name = "";
                        break;
                }
                break;
        }

        return name;
    }

    public IEnumerator AngleInputer(string time_, KeyValuePair<string, List<DataSnapshot>> angle)
    {
        anglesMaxData[time_] = new List<float>();
        foreach (var day in angle.Value)
        {
            foreach (var direction in day.Child("Angles_Max_Before").Children)
            {
                if (direction.Key.Contains("dateTime")) continue;
                anglesMaxData[time_].Add(float.Parse(direction.Value.ToString()));
            }
        }
        
        anglesAvgData[time_] = new List<float>();
        foreach (var day in angle.Value)
        {
            foreach (var direction in day.Child("Angles_Ave_Before").Children)
            {
                if(direction.Key.Contains("dateTime")) continue;
                anglesAvgData[time_].Add(float.Parse(direction.Value.ToString()));
            }
        }
        yield return null;
    }

    private List<string> TimeSelector()
    {
        List<string> selectedTimes = new List<string>();
        DataSnapshot date = myData._user.Child("days_list").Child(dayCount.ToString());
        if (date.Exists)
        {
            string day = date.Value.ToString().Replace("day ", "");
            if (string.IsNullOrEmpty(day)) return new List<string>();
            DateTime startDate = DateTime.Parse(day);
            string dayKey = startDate.ToString("yyyy-MM-dd");

            dayText.text = startDate.ToString("MM/dd");

            // Collect all DataSnapshot keys for the given day
            List<DateTime> dateTimeList = new List<DateTime>();

            foreach (var dataSnap in _history.Children)
            {
                if (dataSnap.Key.Contains(dayKey))
                {
                    string keyChange = dataSnap.Key.Replace("day ", "");
                    DateTime snapDateTime = DateTime.Parse(keyChange);
                    dateTimeList.Add(snapDateTime);
                }
            }
            // Sort DateTime list descending
            dateTimeList.Sort((a, b) => b.CompareTo(a));

            // Return the latest and the next latest times
            
            if (dateTimeList.Count > 1)
            {
                selectedTimes.Add(dateTimeList[1].ToString("HH:mm"));
                selectedTimes.Add(dateTimeList[0].ToString("HH:mm"));
            }
            else if (dateTimeList.Count > 0)
            {
                selectedTimes.Add(dateTimeList[0].ToString("HH:mm"));
            }
        }
        else
        {
            DateTime startDate = DateTime.Parse(NetworkManager._instance._playerData.firstDate);
            selectedTimes.Add(startDate.ToString("HH:mm"));
        }
        
        return selectedTimes;
    }
}
