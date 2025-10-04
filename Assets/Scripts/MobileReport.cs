using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MobileReport : MonoBehaviour
{
    public GameObject tileManager; //먼저 소환할 프리팹
    public GameObject tile_Before, tile_After; //들어가도록 소환할 프리팹

    public Transform ResultPos; //소환할 곳

    public Dictionary<string, GameObject> tileManagers = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> tiles_before = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> tiles_after = new Dictionary<string, GameObject>();
    
    public List<float> operationResult_Max_Before = new List<float>();
    public List<float> operationResult_Ave_Before = new List<float>();
    
    public List<float> operationResult_Max_After = new List<float>();
    public List<float> operationResult_Ave_After = new List<float>();

    public int nowSee;

    IEnumerator Start()
    {
        
        AddTileManager("LeftSide", tileManager);
        AddTileManager("RightSide", tileManager);
        AddTileManager("LeftForward", tileManager);
        AddTileManager("RightForward", tileManager);
        yield return StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult_Mobile("Angles_Max_Before",
            "Angles_Ave_Before"));
        
        // if(SceneManager.GetActiveScene().name == "#4 exercise"){
        //
        //     yield return StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult_Mobile("Angles_Max_Before",
        //         "Angles_Ave_Before"));
        // }
        // else if (SceneManager.GetActiveScene().name == "#3 homeMain")
        // {
        //     yield return StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult_Mobile("Angles_Max_Before",
        //         "Angles_Ave_Before", DateTime.Today));
        // }
    }

    public void NextDayButton()
    {
        nowSee = nowSee + 1;
        StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult_Mobile("Angles_Max_Before",
            "Angles_Ave_Before", DateTime.Today.AddDays(nowSee)));
    }

    public void PreviousDayButton()
    {
        nowSee = nowSee-1;
        StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult_Mobile("Angles_Max_Before",
            "Angles_Ave_Before", DateTime.Today.AddDays(nowSee)));
    }

    private void AddTileManager(string key, GameObject prefab)
    {
        GameObject instance = Instantiate(prefab, ResultPos);
        instance.name = key;
        tileManagers.Add(key, instance);
    }

    public void AddTiles(bool isAfter = false)
    {
        List<float> nowMaxData = new List<float>();
        List<float> nowAveData = new List<float>();
        
        if (isAfter)
        {
            nowMaxData = operationResult_Max_After;
            nowAveData = operationResult_Ave_After;
        }
        else
        {
            nowMaxData = operationResult_Max_Before;
            nowAveData = operationResult_Ave_Before;
        }
        
        MakeTiles(nowMaxData[0], nowAveData[0],"Left", "Side", isAfter);
        MakeTiles(nowMaxData[1], nowAveData[1],"Right", "Side", isAfter);
        MakeTiles(nowMaxData[2], nowAveData[2],"Left", "Forward", isAfter);
        MakeTiles(nowMaxData[3], nowAveData[3],"Right", "Forward", isAfter);
    }

    private void MakeTiles(float nowMaxData, float nowAveData, string beforeName, string afterName, bool isAfter = false)
    {
        GameObject selectTile = tile_Before;
        string name = beforeName;
        switch (isAfter)
        {
            case true:
                selectTile = tile_After;
                name = afterName;
                break;
            case false:
                selectTile = tile_Before;
                name = beforeName;
                break;
        }
        
        Transform instance = Instantiate(selectTile, tileManagers[beforeName+afterName].transform).transform;
        instance.name = name;
        instance.GetChild(0).GetComponent<TMP_Text>().text = name;
    
        instance.GetComponent<ResultAngle>().ResultGraph_Mobile(nowMaxData, nowAveData, isAfter);
    }

}
