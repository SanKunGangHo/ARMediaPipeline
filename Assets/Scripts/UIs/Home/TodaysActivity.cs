//using System;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using Mediapipe;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Random = UnityEngine.Random;

public class TodaysActivity : MonoBehaviour
{
    [SerializeField] private Transform layoutGroup;
    [Space(10)]
    [SerializeField] private Transform AllActivities;
    [Space(10)]
    [SerializeField] private Transform upcomingActivities;

    public TMP_Text DayText;
    [Space(10)]

    public GameObject lockedPrefab;
    public GameObject ActivatePrefab;
    public  GameObject normalPrefab;
    
    private Color InteractableFalseColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1); 

    public int dayCount = 1;
    
    public List<ExerciseData> AllExercises;

    public Dictionary<int, List<ExerciseData>> exercisesByDay = new Dictionary<int, List<ExerciseData>>();
    
    public Dictionary<int, List<ExerciseData>> ShoulderImpingementSyndrome = new Dictionary<int, List<ExerciseData>>();

    public Dictionary<int, List<ExerciseData>> adhesiveCapsulitis = new Dictionary<int, List<ExerciseData>>();
    

    public int indexer;
    
    public Sprite finishedSprites;

    private void Awake()
    {
        if(NetworkManager._instance is null) return;
        exercisesByDay = new Dictionary<int, List<ExerciseData>>
        {
            { 0, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[1] } },
            { 1, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[2] } },
            { 2, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[3] } },
            { 3, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[2] } },
            { 4, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[3] } },
            { 5, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[5] } },
            { 6, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[6] } },
            { 7, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[2], NetworkManager._instance.exerciseDatas_all[4] } },
            { 8, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[2], NetworkManager._instance.exerciseDatas_all[4] } },
            { 9, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[2], NetworkManager._instance.exerciseDatas_all[3], NetworkManager._instance.exerciseDatas_all[4] } },
            { 10, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[7] } },
            { 11, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[8] } },
            { 12, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[9] } },
            { 13, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[10] } },
            { 14, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[7] } },
            { 15, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[7] } },
            { 16, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[2], NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[7] } },
            { 17, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[3], NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[7] } },
            { 18, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[5] } },
            { 19, new List<ExerciseData> { NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[6] } }
        };
        
        ShoulderImpingementSyndrome = new Dictionary<int, List<ExerciseData>>
        {
            { 0, new List<ExerciseData>{ NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[2]} },
            {1, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[6]}},
            {2, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[2], NetworkManager._instance.exerciseDatas_all[6]}},
            {3, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[8]}},
            {4, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[4], NetworkManager._instance.exerciseDatas_all[9]}},
            {5, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[8], NetworkManager._instance.exerciseDatas_all[9]}},
            {6, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[5], NetworkManager._instance.exerciseDatas_all[6]}},
            {7, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[9]}},
            {8, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[9]}},
            {9, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[7], NetworkManager._instance.exerciseDatas_all[5], NetworkManager._instance.exerciseDatas_all[10]}},
        };
        
        adhesiveCapsulitis = new Dictionary<int, List<ExerciseData>>
        {
            { 0, new List<ExerciseData>{ NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[1]} },
            {1, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[4]}},
            {2, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[4]}},
            {3, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[6]}},
            {4, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[6]}},
            {5, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[7]}},
            {6, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[7]}},
            {7, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[0], NetworkManager._instance.exerciseDatas_all[9]}},
            {8, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[1], NetworkManager._instance.exerciseDatas_all[8]}},
            {9, new List<ExerciseData>{NetworkManager._instance.exerciseDatas_all[9], NetworkManager._instance.exerciseDatas_all[8], NetworkManager._instance.exerciseDatas_all[7]}},
        };
    }

    IEnumerator Start()
    {
        NetworkManager._instance.ta = this;
        NetworkManager._instance.isNative_Activity = false;
        string id = NetworkManager._instance._playerData.PatientID;
        List<string> exercise = new List<string>();
        yield return StartCoroutine(NetworkManager._instance._firebaseLoader.TodaysRandomClearCheck());
        
        NetworkManager._instance.todaysRandom = new List<ExerciseData>();
        
        if(gameObject.name.Contains("acrtivity")) yield break;
        
        Start_native();
        if (!NetworkManager._instance.isMobile)
        {
            Start_alternative();
            UpcomingUpdater(NetworkManager._instance._playerData.day);
        }
    }

    public void Start_native()
    {
        if (NetworkManager._instance.todayActivity)
        {
            NetworkManager._instance.todaysCleared = new List<bool>(NetworkManager._instance.todaysRandom.Count);
            for (int index = 0; index < NetworkManager._instance.todaysRandom.Count; index++)
            {
                var count = NetworkManager._instance.todaysRandom[index];
                GameObject activatePrefab = Instantiate(ActivatePrefab, layoutGroup);
                MakeActivity_Already(activatePrefab, count, index);
            }
        }
        else
        {
            
            if (!NetworkManager._instance.isAppTutorialEnd)
            {
                for (int i = 0; i < Random.Range(2,4); i++) {
                    Instantiate(lockedPrefab, layoutGroup);
                }
            }
            else
            {
                List<ExerciseData> nowExercise = new List<ExerciseData>();
                
                nowExercise = DayChecker(nowExercise);
                
                foreach (var vExerciseData in nowExercise)
                {
                    GameObject activatePrefab = Instantiate(ActivatePrefab, layoutGroup);
                    MakeActivity(activatePrefab, vExerciseData, Array.IndexOf(nowExercise.ToArray(), vExerciseData), true);
                }
            }
        }
    }

    private List<ExerciseData> DayChecker(List<ExerciseData> nowExercise)
    {
        if (NetworkManager._instance._playerData.day > 19)
        {
            StartCoroutine(NetworkManager._instance._firebaseLoader.UpdateDays(0));
            NetworkManager._instance._playerData.day = 0;
        }
        int lastDigit = NetworkManager._instance._playerData.day % 10;
        if (NetworkManager._instance._playerData.category.Contains("Impingement"))
        {
            nowExercise = ShoulderImpingementSyndrome[lastDigit];
        }
        else if (NetworkManager._instance._playerData.category.Contains("Adhesive"))
        {
            nowExercise = adhesiveCapsulitis[lastDigit];
        }
        else //평범
        {
            nowExercise = exercisesByDay[NetworkManager._instance._playerData.day];
        }

        return nowExercise;
    }

    public void Start_alternative()
    {
        if (!NetworkManager._instance.isAppTutorialEnd)
        {
            for (int i = 0; i<12; i++) {
                Instantiate(lockedPrefab, AllActivities);
            }
        }
        else
        { 
            List<ExerciseData> nowExercise = new List<ExerciseData>();
   
            NetworkManager._instance.todaysCleared = new List<bool>(NetworkManager._instance.todaysRandom.Count);

            nowExercise = NetworkManager._instance.exerciseDatas_all.ToList();
            
            foreach (var vExerciseData in nowExercise)
            {
                GameObject activatePrefab = Instantiate(normalPrefab, AllActivities);
                MakeActivity_AllActivity(activatePrefab, vExerciseData, Array.IndexOf(NetworkManager._instance.exerciseDatas_all, vExerciseData), false);
            }
        }
    }


    void MakeActivity(GameObject activePrefab, ExerciseData nowExercise, int i, bool isNative)
    {
        // if (nowExercise.isClear)
        // {
        //     activePrefab.transform.GetChild(0).GetComponent<Image>().sprite =
        //         nowExercise.Type[3];
        // }
        // else
        // {
        //     activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = nowExercise.Type[1];
        // }
        //
        // activePrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = nowExercise.Type_string;
        //
        // activePrefab.transform.GetChild(1).GetComponent<Image>().sprite =
        //     nowExercise.Character;
        // activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
        //     nowExercise.Title;
        // activePrefab.transform.GetChild(3).GetComponent<TMP_Text>().text =
        //     $"{nowExercise.time}sec·{nowExercise.set + 1}set";

        activePrefab.GetComponent<Image>().sprite = nowExercise.Character;

        activePrefab.GetComponent<ActivityData>().ButtonExData = nowExercise;

        activePrefab.GetComponent<Button>().onClick.AddListener(() => firstCheck(activePrefab, i, isNative));
        if (isNative && !NetworkManager._instance.todayActivity)
        {
            NetworkManager._instance.todaysRandom.Add(activePrefab.GetComponent<ActivityData>().ButtonExData);
        }
        
    }
    
    void MakeActivity_Already(GameObject activePrefab, ExerciseData nowExercise, int i)
    {
        // if (nowExercise.isClear)
        // {
        //     activePrefab.transform.GetChild(0).GetComponent<Image>().sprite =
        //         nowExercise.Type[3];
        // }
        // else
        // {
        //     activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = nowExercise.Type[1];
        // }
        //
        // activePrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = nowExercise.Type_string;
        //
        // activePrefab.transform.GetChild(1).GetComponent<Image>().sprite =
        //     NetworkManager._instance.todaysRandom[i].Character;
        // activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
        //     NetworkManager._instance.todaysRandom[i].Title;
        // activePrefab.transform.GetChild(3).GetComponent<TMP_Text>().text =
        //     $"{nowExercise.time}sec·{nowExercise.set+1}set";
        
        activePrefab.GetComponent<Image>().sprite = nowExercise.Character;
        
        if (NetworkManager._instance.todayActivity)
        {
            activePrefab.GetComponent<ActivityData>().ButtonExData = NetworkManager._instance.todaysRandom[i];
        }
        else
        {
            activePrefab.GetComponent<ActivityData>().ButtonExData = NetworkManager._instance.exerciseDatas[i];
        }
        
        activePrefab.GetComponent<Button>().onClick.AddListener(()=> firstCheck(activePrefab, i, true));
    }

    void firstCheck(GameObject activePrefab, int i, bool isNative)
    {
        if (isNative && activePrefab.transform.GetSiblingIndex() != 0 && !NetworkManager._instance.isTestMode)
        {
            if (!layoutGroup.GetChild(activePrefab.transform.GetSiblingIndex() - 1).GetComponent<ActivityData>()
                    .ButtonExData.isClear)
            {
                SoundManager.instance.SEAudioPlay(4);
                StartCoroutine(RedRecycler(activePrefab));
                return;
            }
        }

        StartCoroutine(SceneConverter(i, isNative));
    }

    IEnumerator RedRecycler(GameObject activePrefab)
    {
        //activePrefab.GetComponent<Image>().color = new Color(1f, 0.9176f,0.9176f);
        activePrefab.GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.warningCharacter;
        // activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.Type[2];
        // activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().color = new Color(0.9411f, 0.2823f, 0.1333f);
        //TODO: 빨간색 이미지로 변경
        activePrefab.transform.GetChild(4).gameObject.SetActive(true);

        yield return new WaitForSeconds(1);
        
        //activePrefab.GetComponent<Image>().color = new Color(1f, 1f, 1f);
        activePrefab.GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.Character;
        // activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.Type[1];
        // activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().color = new Color(0.2627f, 0.7568f, 0.7803f);
        activePrefab.transform.GetChild(4).gameObject.SetActive(false);
    }

    public void UpcomingUpdater(int i)
    {
        dayCount += i;
        if (dayCount <= 0)
        {
            dayCount = 0;
        }
        else if (dayCount >= 19)
        {
            dayCount = 19;
        }
        DayText.text = $"Day {dayCount+1}";
        UpcomingActivityActivate(dayCount);
    }

    public void UpcomingActivityActivate(int Count)
    {

        if (NetworkManager._instance.isAppTutorialEnd)
        {

            foreach (var nowExercise in exercisesByDay[Count])
            {
                GameObject activePrefab = Instantiate(normalPrefab, upcomingActivities);
                
                //activePrefab.GetComponent<Image>().sprite = nowExercise.Character;
                activePrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = nowExercise.Type_string;
                activePrefab.transform.GetChild(0).GetComponent<Image>().color = InteractableFalseColor;
                
                activePrefab.transform.GetChild(1).GetComponent<TMP_Text>().text =
                    nowExercise.Title;
                
                activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
                    $"{nowExercise.time}sec·{nowExercise.set + 1}set";

                activePrefab.GetComponent<Button>().interactable = false;
            }
        }
        else
        {
            foreach (var lockedExercise in exercisesByDay[Count])
            {
                Instantiate(lockedPrefab, upcomingActivities);
            }
        }


    }
    
    void MakeActivity_AllActivity(GameObject activePrefab, ExerciseData nowExercise, int i, bool isNative)
    {
        
        activePrefab.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = nowExercise.Type_string;
       
        activePrefab.transform.GetChild(1).GetComponent<TMP_Text>().text =
            nowExercise.Title;
        activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
            $"{nowExercise.time}sec·{nowExercise.set + 1}set";

        if (NetworkManager._instance.todayActivity)
        {
            activePrefab.GetComponent<ActivityData>().ButtonExData = nowExercise;
        }
        else
        {
            activePrefab.GetComponent<ActivityData>().ButtonExData = nowExercise;
        }

        activePrefab.GetComponent<Button>().onClick.AddListener(() => firstCheck(activePrefab, i, isNative));
        if (isNative)
        {
            NetworkManager._instance.todaysRandom.Add(activePrefab.GetComponent<ActivityData>().ButtonExData);
        }
    }

    IEnumerator SceneConverter(int i, bool isNative)
    {
        SoundManager.instance.SEAudioPlay(0);
        SoundManager.instance.SEAudioPlay2(10);
        NetworkManager._instance.isNative_Activity = isNative;
        yield return new WaitForSeconds(1);
        NetworkManager._instance.choosedExercise = i;
        //NetworkManager._instance.exerciseDatas[i].isClear = true;
        if (NetworkManager._instance.isMobile)
        {
            SceneManager.LoadScene("#4 exercise");
        }
        else{
            SceneManager.LoadScene("ExerciseScene");
        }
    }
}
