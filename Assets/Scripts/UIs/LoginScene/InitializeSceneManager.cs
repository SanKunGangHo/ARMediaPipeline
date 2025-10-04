using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeSceneManager : MonoBehaviour
{
    public GameObject[] hashtags;

    public GameObject exitpopup;
    
    private void Start()
    {
        StartCoroutine(EscActivate());

        foreach (var VARIABLE in NetworkManager._instance.exerciseDatas_all)
        {
            VARIABLE.isClear = false;
        }

        NetworkManager._instance.ResetData();
    }

    public void QuitApp()
    {
        Application.Quit();
    }
    
    IEnumerator EscActivate()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                exitpopup.SetActive(true);
            }

            yield return null;  // This will wait until the next frame, preventing the editor from freezing
        }
    }
}
