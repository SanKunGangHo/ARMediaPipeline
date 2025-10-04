using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class StopUI : MonoBehaviour
{
    Button stopButton;

    private void OnEnable()
    {
        if (ExerciseManager.instance.OperationUI.gameObject.activeInHierarchy)
        {
            StartCoroutine(NetworkManager._instance._firebaseLoader.StopExercise("Shoulder Angle Operation"));
        }

        if (!NetworkManager._instance.isAppTutorialEnd)
        {
            StartCoroutine(NetworkManager._instance._firebaseLoader.StopExercise("Tutorial"));
        }
        
        StartCoroutine(NetworkManager._instance._firebaseLoader.StopExercise(PoseManager.instance.NowExercise.Title));
    }
}
