using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OperationRate_Admin : MonoBehaviour
{
    private AdminManager am;
    private DataSnapshot _snap;
    
    [SerializeField] private List<Color> Impingement_colors = new List<Color>()
    {
        new(0.2f, 0.5529f, 0.7176f, 1f),
        new(0.3529f, 0.7921f, 0.9686f, 1f),
        new(0.4862f, 0.8431f, 0.9725f, 1f),
        new(0.6509f, 0.8941f, 0.9843f, 1f),
        new(0.6509f, 0.8941f, 0.9843f, 1f)
    };
    
    [SerializeField] private List<Color> Adhesive_colors = new List<Color>()
    {
        new(0.7058f, 0.3921f, 0.19215f, 1f),
        new(0.9372f, 0.5254f, 0.2588f, 1f),
        new(0.9490f, 0.6196f, 0.3960f, 1f),
        new(0.9607f, 0.7137f, 0.5450f, 1f),
        new(0.9607f, 0.7137f, 0.5450f, 1f)
    };
    
    List<DataSnapshot> impingementList = new List<DataSnapshot>();
    List<DataSnapshot> adhesiveList = new List<DataSnapshot>();
    
    public Transform impingementParent;
    public Transform adhesiveParent;
    
    public GameObject impingementPrefab;
    public GameObject adhesivePrefab;

    private List<KeyValuePair<string, float>> impingement_max = new List<KeyValuePair<string, float>>();
    private List<KeyValuePair<string, float>> adhesive_max = new List<KeyValuePair<string, float>>();

    private void Start()
    {
        am = AdminManager.Instance;
        Refresh_BTN();
    }

    public void Refresh_BTN()
    {
        foreach (Transform child in impingementParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in adhesiveParent)
        {
            Destroy(child.gameObject);
        }
        
        StartCoroutine(Refresh());
    }

    IEnumerator Refresh()
    {
        var task = am._databaseReference.GetValueAsync();
        yield return new WaitUntil( ()=> task.IsCompleted | task.IsFaulted);
        
        _snap = task.Result;

        yield return StartCoroutine(makeList());
        yield return StartCoroutine(Adhesive());
        yield return StartCoroutine(Impingement());
        yield return StartCoroutine(makeRate());
        
        yield return StartCoroutine(showList(impingementPrefab, impingementParent, Impingement_colors, impingement_max));
        yield return StartCoroutine(showList(adhesivePrefab, adhesiveParent, Adhesive_colors, adhesive_max));
    }

    IEnumerator makeList()
    {
        foreach (var user in _snap.Children)
        {
            if (user.Child("category").ToString().Contains("Adhesive"))
            {
                adhesiveList.Add(user);
            }
            else if (user.Child("category").ToString().Contains("Impingement"))
            {
                impingementList.Add(user);
            }
        }
        yield return null;
    }

    IEnumerator Adhesive()
    {
        foreach (var adhesiveUser in adhesiveList)
        {
            var measureSnap = adhesiveUser.Child("ExerciseDatas").Child("Measurement").Child("history");
            StartCoroutine(makeMaxVariance(adhesiveUser.Child("id").Value.ToString(), measureSnap, adhesive_max));
        }

        yield return null;
    }
    
    IEnumerator Impingement()
    {
        foreach (var ImpingementUser in impingementList)
        {
            var measureSnap = ImpingementUser.Child("ExerciseDatas").Child("Measurement").Child("history");
            yield return StartCoroutine(makeMaxVariance(ImpingementUser.Child("id").Value.ToString(), measureSnap, impingement_max));
        }

        yield return null;
    }

    IEnumerator makeMaxVariance(string id, DataSnapshot measureSnap, List<KeyValuePair<string, float>> MaxDict)
    {
        float todaysMaxValue = 0f;
        float yesterdayMaxValue = 0f;
        DateTime today = DateTime.Today;
        DateTime yesterday = today.AddDays(-1);
        foreach (var day in measureSnap.Children)
        {
            if (day.Child("Angles_Max_Before").Exists)
            {
                if (day.Key.Contains(today.ToString("yyyy-MM-dd"))) //오늘자 모든 max값의 평균
                {
                    foreach (var angles in day.Child("Angles_Max_Before").Children)
                    {
                        if (angles.Key.Contains("dateTime")) continue;
                        float maxAngle = float.Parse(angles.Value.ToString());
                        todaysMaxValue += maxAngle;
                    }

                    todaysMaxValue /= day.Child("Angles_Max_Before").ChildrenCount;
                }
                
                //DateTime firstDay = 
                if (day.Key.Contains(yesterday.ToString("yyyy-MM-dd"))) //어제자 모든 max값의 평균
                {
                    foreach (var angles in day.Child("Angles_Max_Before").Children)
                    {
                        if (angles.Key.Contains("dateTime")) continue;
                        float maxAngle = float.Parse(angles.Value.ToString());
                        yesterdayMaxValue += maxAngle;
                    }

                    yesterdayMaxValue /= day.Child("Angles_Max_Before").ChildrenCount;
                }
            }
        }
        
        float percent = (todaysMaxValue - yesterdayMaxValue) / yesterdayMaxValue * 100;
        MaxDict.Add(new KeyValuePair<string, float>(id, percent));

        yield return null;
    }

    IEnumerator makeRate()
    {
        
        adhesive_max.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        adhesive_max = adhesive_max.Take(5).ToList();
        
        impingement_max.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
        impingement_max = impingement_max.Take(5).ToList();
        
        yield return null;
    }

    IEnumerator showList(GameObject setPrefab, Transform setPos, List<Color> colors, List<KeyValuePair<string, float>> percent)
    {
        int count = 0;
        foreach (var per in percent)
        {
            GameObject prefab = Instantiate(setPrefab, setPos);
            prefab.GetComponent<Image>().color = colors[count];
            prefab.transform.GetChild(0).GetComponent<TMP_Text>().text = $"No.{count+1}";
            prefab.transform.GetChild(1).GetComponent<TMP_Text>().text = per.Key;
            
            if (float.IsNaN(per.Value))
            {
                prefab.transform.GetChild(2).GetComponent<TMP_Text>().text = "---";
            }
            // else if (per.Value < 0)
            // {
            //     prefab.transform.GetChild(2).GetComponent<TMP_Text>().text = "";
            // }
            else
            {
                prefab.transform.GetChild(2).GetComponent<TMP_Text>().text = $"{per.Value}";
            }
            count++;
        }
        
        

        yield return null;
    }
}
