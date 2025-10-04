using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class ExerciseManager : MonoBehaviour
{
    public ResultManager rm;
    public RotationLoading rotLoad;
    public VideoSources videoSources;
    public GameObject AnnotationLayer;
    public GameObject OperationUI;
    public GameObject StartUI;
    public GameObject StopUI;
    public GameObject StartOnUI;
    public MobileReport mobileReport;
    //스크립트 연결용

    public Transform resultPos;
    

    public static ExerciseManager instance;
    public NetworkManager nm;

    public GameObject ExitPopUp;

    private void Start()
    {
        nm = NetworkManager._instance;
        if(!nm.isAppTutorialEnd && nm.isMobile) return;
        StartCoroutine(EscActivate());
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

        if (nm.choosedExercise == 1000
            || nm.choosedExercise == 1001)
        {
            Debug.LogError("operation");
            switch (nm.choosedExercise)
            {
                case 1000:
                    SoundManager.instance.BGMAudioPlay(6);
                    break;
                case 1001:
                    SoundManager.instance.BGMAudioPlay(7);
                    break;
            }
            OperationUI.SetActive(true);
            return;
        }
        
        if (nm.choosedExercise == 0 && !nm.preMeasurement)
        {
            Debug.LogError("operation");
            SoundManager.instance.BGMAudioPlay(6);
            //nm.preMeasurement = true;
            OperationUI.SetActive(true);
            return;
        }
        else
        {
            Debug.LogError("No");
            SoundManager.instance.SEAudioPlay(13);
            StartUI.SetActive(true);
        }
    }

/// <summary>
/// 본 운동 시작, 가동률검사가 끝난 뒤에 StartUI가 나와서 이걸 진행하는 시스템.
/// </summary>
    public void StartExercise()
    {
        //AnnotationLayer.gameObject.SetActive(true);
        videoSources.CheckIndex(0);
        videoSources._video.Play();
        StartOnUI.SetActive(true);
        PoseManager.instance.reset();
       //timer.StartStopwatch();
    }

    public void xbutton()
    {
        if(nm.choosedExercise == 1000 || nm.choosedExercise == 1001) return;
        if (!nm.isNative_Activity)
        {
            nm.exerciseDatas_all[nm.choosedExercise].isClear = false;
        }
        nm.todaysRandom[nm.choosedExercise].isClear = false;
    }
    
    IEnumerator EscActivate()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPopUp.SetActive(true);
            }
            yield return null;  // This will wait until the next frame, preventing the editor from freezing
        }
    }

    public void AppQuit()
    {
        Application.Quit();
    }

    public void AngleSummon(string dt, List<float> max, List<float> avg, int count)
    {
       ResultManager resultManage = Instantiate(rm, resultPos);
       resultManage.ResultSort(dt, max, avg, count);
    }
}
