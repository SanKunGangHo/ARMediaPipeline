using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : MonoBehaviour
{
    public Button onBording;
    public Sprite onBoardingEndSprite;
    public Button appTutorial;
    public Sprite appTutorialEndSprite;

    public Button BTN_PreMeasurement;
    public Sprite BTN_PreMeasurement_sprite;
    public Button BTN_PostMeasurement;
    public Sprite BTN_PostMeasurement_sprite;
    
    public GameObject[] onBoardingFalseThings;
    public GameObject[] appTutorialFalseThings;

    public GameObject[] PreFalseThings;
    public GameObject[] PostFalseThings;

    public GameObject EndExamination;

    public GameObject ExitPopUp;
    public TMP_Text[] playerID;
    public Sprite[] profileSprites;
    public Image[] playerProfile;
    public TMP_Text dayDisplayText;

    public MobileReport mobileReport;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        if(!NetworkManager._instance.isMobile)
        {
            StartCoroutine(EscActivate());
        }

        SoundManager.instance.bgm_audio.loop = true;
        SoundManager.instance.BGMAudioPlay(0);
        NetworkManager._instance.hm = this;
        //yield return StartCoroutine(NetworkManager._instance._firebaseLoader.PlayerDataLoad(id));
        PendingCheck();
        if(NetworkManager._instance.isMobile)
        {
            var p = NetworkManager._instance._playerData;
            dayDisplayText.text = (p.day+1).ToString();

            foreach(var t in playerID)
            {
                t.text = p.PatientID;
            }
            foreach(var i in playerProfile)
            {
                if(p.gender == "Man")
                {
                    i.sprite = profileSprites[0];
                }
                else if(p.gender == "Woman")
                {
                    i.sprite = profileSprites[1];                
                }
                else
                {
                    i.sprite = profileSprites[2];                  
                }

            }

        }


        if (NetworkManager._instance.isAppTutorialEnd && NetworkManager._instance.isOnBoardingEnd)
        {
            onBording.gameObject.SetActive(false);
            appTutorial.gameObject.SetActive(false);
            BTN_PreMeasurement.gameObject.SetActive(true);
            BTN_PostMeasurement.gameObject.SetActive(true);
        }
        else
        { 
            if(!NetworkManager._instance.isOnBoardingEnd)
            {
                foreach (var thing in onBoardingFalseThings)
                {
                    thing.SetActive(true);
                }
            }
            if (NetworkManager._instance.isOnBoardingEnd)
            {
                onBording.interactable = false;
                onBording.GetComponent<Image>().sprite = onBoardingEndSprite;
                appTutorial.interactable = true;

                foreach (var thing in onBoardingFalseThings)
                {
                    thing.SetActive(false);
                }
                
                foreach (var thing in appTutorialFalseThings)
                {
                    thing.SetActive(true);
                }
            }

            if (NetworkManager._instance.isAppTutorialEnd)
            {
                appTutorial.interactable = false;
                var state = appTutorial.spriteState;
                state.disabledSprite = appTutorialEndSprite;
                appTutorial.spriteState = state;
            
                foreach (var thing in appTutorialFalseThings)
                {
                    thing.SetActive(false);
                }
            }
        }
        
        if (NetworkManager._instance.preMeasurement)
        {
            BTN_PreMeasurement.interactable = false;
            var state = BTN_PreMeasurement.spriteState;
            state.disabledSprite = BTN_PreMeasurement_sprite;
            BTN_PreMeasurement.spriteState = state;
            
            foreach (var thing in PreFalseThings)
            {
                thing.SetActive(false);
            }
            
            //if(NetworkManager._instance.todaysRandom[^1].isClear != true) yield break;
            
            BTN_PostMeasurement.interactable = true;
            
            foreach (var thing in PostFalseThings)
            {
                thing.SetActive(true);
            }
        }

        if (NetworkManager._instance.postMeasurement)
        {
            
            BTN_PostMeasurement.interactable = false;
            var state = BTN_PostMeasurement.spriteState;
            state.disabledSprite = BTN_PostMeasurement_sprite;
            BTN_PostMeasurement.spriteState = state;
            
            foreach (var thing in PostFalseThings)
            {
                thing.SetActive(false);
            }
        }

        yield break;
    }

    public void OnBoardingEnd()
    {
        if (NetworkManager._instance.isOnBoardingEnd)
        {
            onBording.interactable = false;
            onBording.GetComponent<Image>().sprite = onBoardingEndSprite;
            appTutorial.interactable = true;

            foreach (var thing in onBoardingFalseThings)
            {
                thing.SetActive(false);
            }
        }
    }

    public void AppTutorialEnd()
    {
        if (NetworkManager._instance.isAppTutorialEnd)
        {
            appTutorial.interactable = false;
            var state = appTutorial.spriteState;
            state.disabledSprite = appTutorialEndSprite;
            appTutorial.spriteState = state;
            
            foreach (var thing in appTutorialFalseThings)
            {
                thing.SetActive(false);
            }
        }
    }

    public void Examination()
    {
        NetworkManager._instance.isOnBoardingEnd = true;
    }
    
    public void LogOut()
    {
        //NetworkManager._instance._firebaseLoader.Logout();
    }

    public void NowCategoryDone()
    {
            switch (NetworkManager._instance._playerData.category)
            {
                // case "ShoulderImpingementSyndrome":
                //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_shoulder;
                //     break;
                // case "RotatorCuffDisorder":
                //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_Rotator;
                //     break;
                // case "Common":
                //     NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_else;
                //     break;
                default:
                    NetworkManager._instance.exerciseDatas = NetworkManager._instance.exerciseDatas_all;
                    break;
            }
    }

    public void PreMeasurement()
    {
        NetworkManager._instance.preMeasurement = true;
        NetworkManager._instance.choosedExercise = 1000;
        StartCoroutine(NetworkManager._instance._firebaseLoader.PreMeasurement_Input());
    }

    public void PostMeasurement()
    {
        //NetworkManager._instance._firebaseLoader.PostMeasurement_Input();
        NetworkManager._instance.postMeasurement = true;
        NetworkManager._instance.choosedExercise = 1001;
    }

    public void PendingCheck()
    {
        if (NetworkManager._instance._playerData.category == "Pending")
        {
            EndExamination.SetActive(true);
        }
    }

    public void AppQuit()
    {
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        //TODO:PlayerPrefs로 아이디와 비밀번호등등 넣어두기.
    }
    
    IEnumerator EscActivate()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPopUp.gameObject.SetActive(true);
            }

            yield return null;  // This will wait until the next frame, preventing the editor from freezing
        }
    }

    public void TutorialClicked()
    {
        NetworkManager._instance.isAppTutorialEnd = true;
    }

    public void IsTutorialCheck()
    {
        if(!NetworkManager._instance.isAppTutorialEnd)
            foreach (var thing in appTutorialFalseThings)
            {
                thing.SetActive(true);
            }
    }
}


