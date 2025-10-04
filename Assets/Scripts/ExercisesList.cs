using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExercisesList : MonoBehaviour
{
    public Transform ExercisePos;

    public GameObject ExerciseList_exercise;
    public List<string> ExerciseList_Name;


    private void Start()
    {
        OnFirstListUp();
    }

    public void OnFirstListUp()
    {
        int i = 0;
        foreach (var data in NetworkManager._instance.exerciseDatas_else)
        {
            ListUp(data, i, "else");
            i++;
        }

        i = 0;
        foreach (var data in NetworkManager._instance.exerciseDatas_shoulder)
        {
            ListUp(data, i, "shoulder");
            i++;
        }

        i = 0;
        foreach (var data in NetworkManager._instance.exerciseDatas_Rotator)
        {
            ListUp(data, i, "rotator");
            i++;
        }

        i = 0;
        foreach (var data in NetworkManager._instance.exerciseDatas_all)
        {
            ListUp(data, i, "");
            i++;
        }
    }

    public void OnTypeListUp()
    {
        
    }

    public void OnForListUp()
    {
        
    }

    public void OnDifficultyListUp()
    {
        
    }

    public void ListUp(ExerciseData data, int i, string type)
    {
        foreach (var name in ExerciseList_Name)
        {
            if (name == data.Title) return;
        }
        
        GameObject ExerciseList_Content = Instantiate(ExerciseList_exercise, ExercisePos);
        ExerciseList_Content.name = data.Title;//이름 변경
        ExerciseList_Content.GetComponent<Button>().onClick.AddListener(()=>SceneMove(i, type));
        ExerciseList_Content.transform.GetChild(0).GetComponent<Image>().sprite = data.Type[1]; //TODO:나중에 멈춰야하는 것만 고르기.
        ExerciseList_Content.transform.GetChild(1).GetComponent<Image>().sprite = data.Character;
        ExerciseList_Content.transform.GetChild(2).GetComponent<TMP_Text>().text = data.Title;
        ExerciseList_Content.transform.GetChild(3).GetComponent<TMP_Text>().text = $"{data.set}set · {data.time}time";
        //나중에 난이도는 추가 예정
            
        ExerciseList_Name.Add(data.Title);//중복 확인용 이름 추가
    }

    public void SceneMove(int i, string type)
    {
        switch (type)
        {
            // case "else":
            //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_else;
            //     NetworkManager._instance.choosedExercise = i;
            //     SceneManager.LoadScene("ExerciseScene");
            //     break;
            // case "shoulder":
            //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_shoulder;
            //     NetworkManager._instance.choosedExercise = i;
            //     SceneManager.LoadScene("ExerciseScene");
            //     break;
            // case "rotator":
            //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_Rotator;
            //     NetworkManager._instance.choosedExercise = i;
            //     SceneManager.LoadScene("ExerciseScene");
            //     break;
            default:
                NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_all;
                NetworkManager._instance.choosedExercise = i;
                SceneManager.LoadScene("ExerciseScene");
                break;
        }
    }
}
