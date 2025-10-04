using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    public TMP_Text id_Text;
    public TMP_Text dateText;
    public ResultAngle[] _resultAngles_before; //결과값들

    public ResultAngle[] _ResultAngles_after;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().name == "ExerciseScene")
        {
            SoundManager.instance.BGMAudioPlay(5);
            //id_Text.text = NetworkManager._instance._playerData.PatientID;
        }
        
        yield return null;
    }
    
    public void ResultSort(string date)
    {
        PlayerData pd = new PlayerData();
        if (SceneManager.GetActiveScene().name == "ExerciseScene")
        {
            pd = NetworkManager._instance._playerData;
        }
        else if(SceneManager.GetActiveScene().name == "AdminScene")
        {
            pd = AdminManager.Instance.playerdata;
        }

        dateText.text = date;
        
        
        for (int index = 0; index < _resultAngles_before.Length; index++)
        {
            ResultAngle vResultAngle = _resultAngles_before[index];
            
            vResultAngle.ResultGraph(pd.operationResult_Max_Before[index], pd.operationResult_Ave_Before[index]);
        }
        
    }
    
    public void ResultSort(string date, List<float> max, List<float> avg, int count)
    {
        dateText.text = date;
        
        for (int index = 0; index < _resultAngles_before.Length; index++)
        {
            ResultAngle vResultAngle = _resultAngles_before[index];
            
            vResultAngle.ResultGraph(max[index], avg[index], count);
        }
        
    }
    
    public void No_ResultSort()
    {
        for (int index = 0; index < _resultAngles_before.Length; index++)
        {
            ResultAngle vResultAngle = _resultAngles_before[index];
            
            vResultAngle.ResultGraph(0, 0);
        }
        
        
        for (var index = 0; index < _ResultAngles_after.Length; index++)
        {
            var vResultAngle = _ResultAngles_after[index];
            vResultAngle.ResultGraph(0, 0);
        }
    }
}
