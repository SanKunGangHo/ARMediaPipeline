using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Network;
using Newtonsoft.Json;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;
using DataSnapshot = Firebase.Database.DataSnapshot;

public class AdminManager : MonoBehaviour
{
    
    private bool isPatientOn = true;
    private List<GameObject> patients = new List<GameObject>();
    public GameObject onGraphic, offGraphic;
    
    public void PatientListOnOff()
    {
        isPatientOn = !isPatientOn;

        switch (isPatientOn)
        {
            case true:
                onGraphic.SetActive(true);
                offGraphic.SetActive(false);
                foreach (GameObject user in patients)
                {
                    user.SetActive(true);
                }
                break;
            case false:
                onGraphic.SetActive(false);
                offGraphic.SetActive(true);
                foreach (GameObject user in patients)
                {
                    user.SetActive(false);
                }
                break;
        }
        
    }
    
    public static AdminManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        FirebaseDatabase.DefaultInstance.SetPersistenceEnabled(false);
        _databaseReference = FirebaseDatabase.DefaultInstance.GetReference("/");
        StartCoroutine(Start_a());
        StartCoroutine(EscActivate());
    }
    
    public Transform grid;

    public List<string> userList;
    public List<DateTime> DateTimes = new List<DateTime>();

    public GameObject userPrefab;
    public GameObject userPrefab_Last;

    public Transform userListContent;

    public DatabaseReference _databaseReference;

    // private float stepAvg_L = 0f;
    // private float stepAvg_R = 0f;

    public TMP_InputField searchbar;

    public DataSnapshot _snap;

    // string AVG_AFTER = "Angles_Ave_After";
    // string AVE_BEFORE = "Angles_Ave_Before";
    // string MAX_AFTER = "Angles_Max_After";
    // string MAX_BEFORE = "Angles_Max_Before";

    public GameObject ExitPopUp;

    public GameObject OverView;
    public GameObject userView;

    public List<ExerciseData> ExerciseDatas;

    public PlayerData playerdata = new PlayerData(); 

    public class userAngleList
    {
        public string userId;

        public Dictionary<string, FireBaseLoader.angleData> ave_Before =
            new Dictionary<string, FireBaseLoader.angleData>();

        public Dictionary<string, FireBaseLoader.angleData> ave_After =
            new Dictionary<string, FireBaseLoader.angleData>();

        public Dictionary<string, FireBaseLoader.angleData> max_Before =
            new Dictionary<string, FireBaseLoader.angleData>();

        public Dictionary<string, FireBaseLoader.angleData> max_After =
            new Dictionary<string, FireBaseLoader.angleData>();
    }

    private List<userAngleList> alluserAngles;

    public List<userAngleList> GetAllUserAngles()
    {
        return alluserAngles;
    }

    IEnumerator Start_a()
    {
        alluserAngles = new List<userAngleList>();
        
        OverView.gameObject.SetActive(true);
        userView.SetActive(false);
        yield return StartCoroutine(UserListMaker());
    }

    public IEnumerator UserListMaker() //리스트 가져옴
    {
        patients.Clear();
        
        foreach (Transform gridChild in grid.transform)
        {
            Destroy(gridChild.gameObject);
        }
        
        var task = _databaseReference.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

        if (task.IsFaulted)
        {
            Debug.Log("Error getting user list from Firebase: " + task.Exception);
        }
        else
        {
            _snap = task.Result;
            userList = new List<string>();
            foreach (DataSnapshot childSnapshot in _snap.Children)
            {
                if(childSnapshot.Child("isAdmin").Exists) continue;
                string data = childSnapshot.Key.ToString();
                userList.Add(data);
            }

            userList = userList.OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();

            foreach (var name in userList)
            {
                GameObject user;
                // Instantiate user prefab
                user = Instantiate(name == userList.Last() ? userPrefab_Last : userPrefab, userListContent);

                // Set user name text
                user.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
    
                // Add button listener
                user.GetComponent<Button>().onClick.AddListener(() => UserBTN(_snap.Child(name)));
                
                patients.Add(user);
            }
            
        }
    }

    public void UserBTN(DataSnapshot childSnapshot)
    {
        OverView.SetActive(false);
        userView.SetActive(true);
        userView.GetComponent<AdminUserManager>().UserClicked(childSnapshot);
    }

    public void SearchID()
    {
        foreach (Transform user in userListContent) //전체 삭제
        {
            Destroy(user.gameObject);
        }
        
        foreach (DataSnapshot childSnapshot in _snap.Children)
        {
            if (childSnapshot.Exists)
            {
                Debug.Log(childSnapshot.Key);
                if (childSnapshot.Key.IndexOf(searchbar.text, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    GameObject user = Instantiate(userPrefab, userListContent);
                    user.transform.GetChild(0).GetComponent<TMP_Text>().text = childSnapshot.Key;
                    user.GetComponent<Button>().onClick
                        .AddListener(() => UserBTN(childSnapshot));
                    if (childSnapshot.Key.IndexOf("Admin", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        Destroy(user);
                    }
                }
            }
        }
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
}
