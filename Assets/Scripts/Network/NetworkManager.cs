using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using Unity.VisualScripting;
using System.Linq;
using Firebase.Database;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public PlayerData _playerData;//자기자신만

    public ExerciseData[] exerciseDatas_Rotator, exerciseDatas_shoulder, exerciseDatas_else, exerciseDatas_all, exerciseDatas;
    //public AppScriptReceiver ASR;
    public FireBaseLoader _firebaseLoader;
    public static NetworkManager _instance;

    public GameObject ExitPopup;

    public bool isServer;
    public bool isHoming;

    public List<ExerciseData> todaysRandom;
    public bool todayActivity;
    public List<bool> todaysCleared;

    public int choosedExercise = 0;

    public static NetworkManager Instance => _instance;
    public ReportsManager rm;
    public HomeManager hm;
    public TodaysActivity ta;

    public bool preMeasurement, postMeasurement;

    public bool isNative_Activity;
    
    public bool isOnBoardingEnd;
    public bool isAppTutorialEnd;

    public bool isTestMode, isPlaying;

    public float time_play;

    public bool isMobile = false;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        _playerData = new PlayerData();
        _playerData.operationResult_Max_Before = new List<float>();
        _playerData.operationResult_Ave_Before = new List<float>();
        if(SceneManager.GetActiveScene().name == "#2 LogIn")
        {
            isMobile = true;
        }
        else isMobile = false;
        Debug.Log($"isMobileMode : {isMobile}");

        // if (PlayerPrefs.HasKey("CATEGORY"))
        // {
        //     CategorySetter(PlayerPrefs.GetString("CATEGORY"));
        // }
        //ASR = GetComponent<AppScriptReceiver>();

        //서버 연결시 삭제 임시 데이터 저장소
        //LoadData();
    }

    public void timerStart()
    {
        StartCoroutine(timer());
    }
    
    IEnumerator timer()
    {
        while (isPlaying)
        {
            time_play += Time.deltaTime;
            yield return null;
        }
    }

    // public void SaveData()
    // {
    //     if (isServer)
    //     {
    //         return;
    //     }
    //     PlayerPrefs.SetString("ID", _playerData.PatientID);
    //     PlayerPrefs.SetString("PW", _playerData.PW);
    //     PlayerPrefs.SetString("DATEOFBIRTH", _playerData.dateOfBirth.ToString("yyyy-mm-dd"));
    //     PlayerPrefs.SetString("JOB", _playerData.job);
    //     PlayerPrefs.SetString("GENDER", _playerData.gender);
    // }
    //
    // public void LoadData()
    // {
    //     if (isServer && !PlayerPrefs.HasKey("ID"))
    //     {
    //         return;
    //     }
    //     _playerData.PatientID = PlayerPrefs.GetString("ID");
    //     _playerData.PW = PlayerPrefs.GetString("PW");
    //     _playerData.dateOfBirth = DateTime.Parse(PlayerPrefs.GetString("DATEOFBIRTH"));
    //     _playerData.job = PlayerPrefs.GetString("JOB");
    //     _playerData.gender = PlayerPrefs.GetString("GENDER");
    // }

    public void CategorySetter(string _category)
    {
        exerciseDatas = exerciseDatas_all;

        switch (_category)
        {
            case "Shoulder Impingement Syndrome":
                exerciseDatas = exerciseDatas_Rotator;
                _playerData.category = _category;
                break;
            case "Adhesive Capsulitis":
                exerciseDatas = exerciseDatas_shoulder;
                _playerData.category = _category;
                break;
            case "Pending":
                exerciseDatas = Array.Empty<ExerciseData>();
                _playerData.category = _category;
                break;
            case "Common":
                exerciseDatas = exerciseDatas_else;
                _playerData.category = _category;
                break;
            default:
                exerciseDatas = exerciseDatas_else;
                _playerData.category = "Common";
                Debug.LogError("CategorySetter Error");
                break;
        }
         _firebaseLoader.CategoryInput(_category);
    }

    public void ExaminationData(string _title)
    {
        _firebaseLoader.ExamInput(_title);
    }

    public void BriefData(string _title)
    {
        _firebaseLoader.ExamInput(_title);
    }
    

    public void ResetData()
    {
        _playerData = new PlayerData();
        exerciseDatas = null;
        todayActivity = false;
        todaysRandom.Clear();
        choosedExercise = 0;
        isHoming = false;
        todaysCleared.Clear();
        isOnBoardingEnd = false;
        isAppTutorialEnd = false;
        preMeasurement = false;
        postMeasurement = false;
    }
}
