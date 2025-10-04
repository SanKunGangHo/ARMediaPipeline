using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StepLayOutMaker_ForMobile: MonoBehaviour
{
    public ExerciseData nowExercize;
    private string[] _steps;

    [Header("SpawnPoint")] public Transform stepLayout;
    
    [Header("Prefabs")]
    public GameObject nowStep;
    public GameObject deactivatedStep;
    public GameObject nowStepObject;
    
    // Start is called before the first frame update
    void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TestScene" || (NetworkManager._instance.isMobile && !NetworkManager._instance.isAppTutorialEnd))
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
    }

    public void ChangeStep(int index)
    {
        nowStepObject.GetComponent<TMP_Text>().text = $"{index+1}. {_steps[index]}";
    }

    public void Restart(int index)
    {
        nowStepObject.GetComponent<TMP_Text>().text = $"{index+1}. {_steps[index]}";
        
        ChangeStep(index);
    }

}
