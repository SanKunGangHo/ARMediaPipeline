using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StepLayOutMaker : MonoBehaviour
{
    public ExerciseData nowExercize;
    private string[] _steps;

    [Header("SpawnPoint")] public Transform stepLayout;
    
    [Header("Prefabs")]
    public GameObject nowStep;
    public GameObject deactivatedStep;

    public Sprite Cleared;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TestScene")
        {
            nowExercize = TutorialManager.instance.TutorialExerciseData;
        }
        else
        {
            if (NetworkManager._instance.isNative_Activity)
            {
                nowExercize = NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise];
            }
            else
            {
                nowExercize = NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise];
            }
        }
        _steps = nowExercize.steps;

        for (int i = 0; i < _steps.Length; i++)
        {
            GameObject step;

            switch (i)
            {
                case 0:
                    step = Instantiate(nowStep, stepLayout);
                    step.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{i+1}. {_steps[0]}";
                    break;
                default:
                    step = Instantiate(deactivatedStep, stepLayout);
                    break;
            }
        }
    }

    public void ChangeStep(int index)
    {
        if (index < 0 || index >= stepLayout.childCount)
        {
            return;
        }
        
        GameObject nowStepObject;
        if (stepLayout.childCount > 0 && stepLayout.GetChild(0).childCount != 0 && index == 0)
        {
            nowStepObject = stepLayout.GetChild(index).gameObject;
        }
        else if(index == -1)
        {
            nowStepObject = stepLayout.GetChild(index + 1).gameObject;
        }
        else
        {
            if (index != 0)
            {
                nowStepObject = stepLayout.GetChild(index - 1).gameObject;
            }
            else
            {
                nowStepObject = stepLayout.GetChild(index).gameObject;
            }
        }

        if (index != 0 && SceneManager.GetActiveScene().name != "TestScene")
        {
            nowStepObject.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{index+1}. {_steps[index]}";
        }
        else if (index == 0 && SceneManager.GetActiveScene().name == "TestScene")
        {
            nowStepObject.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{index+2}. {_steps[1]}";
            nowStepObject.transform.SetSiblingIndex(index+1);
            stepLayout.GetChild(index).GetComponent<Image>().sprite = Cleared;
            return;
        }
        nowStepObject.transform.SetSiblingIndex(index);
        if (index > 0) // 이 부분을 추가하였습니다.
        {
            if (nowStepObject.transform.GetSiblingIndex() != 0)
            {
                stepLayout.GetChild(index - 1).GetComponent<Image>().sprite = Cleared;
            }
        }
    }

    public void Restart(int index)
    {
        foreach (Transform steps in stepLayout.transform)
        {
            Destroy(steps.gameObject);
        }
        
        for (int i = 0; i < _steps.Length; i++)
        {
            GameObject step;

            switch (i)
            {
                case 0:
                    step = Instantiate(nowStep, stepLayout);
                    step.transform.GetChild(0).GetComponent<TMP_Text>().text = $"{1}.{_steps[0]}";
                    break;
                default:
                    step = Instantiate(deactivatedStep, stepLayout);
                    break;
            }
        }
        ChangeStep(index);
    }

}
