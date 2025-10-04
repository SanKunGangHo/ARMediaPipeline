using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public ExerciseData TutorialExerciseData;

    public GameObject ExitPopUp;
    public GameObject StartUI;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(EscActivate());
    }

    public void Start()
    {
        if (NetworkManager._instance.isMobile && !NetworkManager._instance.isAppTutorialEnd)
        {
            StartUI.SetActive(true);
        }
    }

    public void xbutton()
    {
        NetworkManager._instance.isAppTutorialEnd = false;
    }
    
    IEnumerator EscActivate()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPopUp.SetActive(true);
            }

            yield return null;  
        }
    }
    
    public void AppQuit()
    {
        Application.Quit();
    }
}