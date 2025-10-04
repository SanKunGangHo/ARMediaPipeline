using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Color = UnityEngine.Color;

namespace UIs.Home
{
    public class AllActivity : MonoBehaviour
    {
        [SerializeField] private Transform layoutGroup; 

        public GameObject lockedPrefab;
        public GameObject ActivatePrefab;
        private GameObject normalPrefab;

        public int indexer;
    
        public Sprite finishedSprites;
        IEnumerator Start()
        {
            //NetworkManager._instance.ta = this;
            string id = NetworkManager._instance._playerData.PatientID;
            List<string> exercise = new List<string>();
            yield return StartCoroutine(NetworkManager._instance._firebaseLoader.TodaysRandomClearCheck());
            Start_alternative();
            Debug.LogError(NetworkManager._instance._playerData.day);
            //yield return StartCoroutine(NetworkManager._instance._firebaseLoader.TodaysActivityChecker());
        }

        public void Start_alternative()
        {
            Debug.LogError("alternative start!");
        
            if (NetworkManager._instance.todayActivity)
            {
                Debug.LogError($"Already_TodaysActivity {NetworkManager._instance.todaysRandom.Count}");
                NetworkManager._instance.todaysCleared = new List<bool>(NetworkManager._instance.todaysRandom.Count);
                for (int index = 0; index < NetworkManager._instance.todaysRandom.Count; index++)
                {
                    var count = NetworkManager._instance.todaysRandom[index];
                    Debug.Log($"Instantiating prefab for count: {count}");
                    GameObject activatePrefab = Instantiate(ActivatePrefab, layoutGroup);
                    Debug.Log($"Prefab instantiated: {activatePrefab.transform.GetSiblingIndex()}");
                    MakeActivity_Already(activatePrefab, count, index);
                    Debug.Log($"MakeActivity_Already called for index: {index}");
                    //if (NetworkManager._instance.todaysRandom.Count == layoutGroup.childCount);
                }
            }
            else
            {
                NetworkManager._instance.todaysRandom.Clear();
                if (!NetworkManager._instance.isAppTutorialEnd)
                {
                    Debug.LogError("AppTuto");
                    for (int i = 0; i<5; i++) {
                        Instantiate(lockedPrefab, layoutGroup);
                    }
                }
                else
                { 
                    Debug.LogError("NotTuto");
                    PlayerData pd = NetworkManager._instance._playerData;
                    List<ExerciseData> cacheData = NetworkManager._instance.exerciseDatas_all.ToList();
                    List<ExerciseData> nowExercise = new List<ExerciseData>();
                
                    // var exercisesByDay = new Dictionary<int, List<ExerciseData>>
                    // {
                    //     { 0, new List<ExerciseData> { cacheData[0], cacheData[1] } },
                    //     { 1, new List<ExerciseData> { cacheData[0], cacheData[2] } },
                    //     { 2, new List<ExerciseData> { cacheData[0], cacheData[3] } },
                    //     { 3, new List<ExerciseData> { cacheData[1], cacheData[2] } },
                    //     { 4, new List<ExerciseData> { cacheData[1], cacheData[3] } },
                    //     { 5, new List<ExerciseData> { cacheData[4], cacheData[5] } },
                    //     { 6, new List<ExerciseData> { cacheData[4], cacheData[6] } },
                    //     { 7, new List<ExerciseData> { cacheData[0], cacheData[2], cacheData[4] } },
                    //     { 8, new List<ExerciseData> { cacheData[1], cacheData[2], cacheData[4] } },
                    //     { 9, new List<ExerciseData> { cacheData[2], cacheData[3], cacheData[4] } },
                    //     { 10, new List<ExerciseData> { cacheData[4], cacheData[7] } },
                    //     { 11, new List<ExerciseData> { cacheData[7], cacheData[8] } },
                    //     { 12, new List<ExerciseData> { cacheData[7], cacheData[9] } },
                    //     { 13, new List<ExerciseData> { cacheData[7], cacheData[10] } },
                    //     { 14, new List<ExerciseData> { cacheData[0], cacheData[4], cacheData[7] } },
                    //     { 15, new List<ExerciseData> { cacheData[1], cacheData[4], cacheData[7] } },
                    //     { 16, new List<ExerciseData> { cacheData[2], cacheData[4], cacheData[7] } },
                    //     { 17, new List<ExerciseData> { cacheData[3], cacheData[4], cacheData[7] } },
                    //     { 18, new List<ExerciseData> { cacheData[7], cacheData[5] } },
                    //     { 19, new List<ExerciseData> { cacheData[7], cacheData[6] } },
                    // };
                    NetworkManager._instance.todaysCleared = new List<bool>(NetworkManager._instance.todaysRandom.Count);

                    nowExercise = NetworkManager._instance.exerciseDatas_all.ToList();
                
                    //분류 노가다
                
                    foreach (var vExerciseData in nowExercise)
                    {
                        Debug.LogError(vExerciseData.Title);
                        GameObject activatePrefab = Instantiate(ActivatePrefab, layoutGroup);
                        MakeActivity(activatePrefab, vExerciseData, Array.IndexOf(NetworkManager._instance.exerciseDatas_all, vExerciseData));
                    }
                
                    //var lastTwoIndices = new[] { NetworkManager._instance.exerciseDatas.Length - 2 , NetworkManager._instance.exerciseDatas.Length - 1 };
                    //TODO: 이후 i를 타입별 랜덤화해서 작업 
                    // for (int i = 0; i < NetworkManager._instance.exerciseDatas.Length; i++)
                    // {
                    //     GameObject activePrefab = Instantiate(ActivatePrefab, layoutGroup);
                    //
                    //     ExerciseData nowExercise;
                    // if (i == 0)
                    // {
                    //     ///int j = Random.Range(0, 2);
                    //     //if(j < NetworkManager._instance.exerciseDatas.Length)
                    //     //{
                    //         nowExercise = NetworkManager._instance.exerciseDatas[j];
                    //         MakeActivity(activePrefab, nowExercise, j);
                    //         i = 1;
                    //     //}
                    // }
                    // else if(i == NetworkManager._instance.exerciseDatas.Length-2 && NetworkManager._instance._playerData.category != "Common" )
                    // {
                    //     //int j = lastTwoIndices[Random.Range(0, 2)];
                    //     //nowExercise = NetworkManager._instance.exerciseDatas[j];
                    //     //MakeActivity(activePrefab, nowExercise, j);
                    //     //i = NetworkManager._instance.exerciseDatas.Length-1;
                    // }
                    //     
                    //     nowExercise = NetworkManager._instance.exerciseDatas[i];
                    //     MakeActivity(activePrefab, nowExercise, i);
                    //
                    //     Debug.LogError("NowExerciseCheck");
                    // }
                    NetworkManager._instance.todayActivity = true;

                    if (NetworkManager._instance.todayActivity)
                    {//오늘의 운동 리스트 저장
                        //PlayerPrefs.SetString("todaysActivityDate", DateTime.Now.ToString("yyyy-MM-dd"));
                     
                        // List<string> _exercises = new List<string>();
                        // List<bool> clear = new List<bool>();
                        // int i = 0;
                        // foreach (var exerciseData in NetworkManager._instance.todaysRandom)
                        // {
                        //     _exercises.Add(exerciseData.Title);
                        //     clear.Add(exerciseData.isClear);
                        //     i++;
                        // }
                        // NetworkManager._instance._firebaseLoader.TodaysActivityCollector(exercises);
                    }
                }
            }
        }
    

        void MakeActivity(GameObject activePrefab, ExerciseData nowExercise, int i)
        {
            Debug.LogError("MakeActivity" + indexer++);
            Debug.LogError(nowExercise.Title);
        
            if (nowExercise.isClear)
            {
                Debug.LogError("Error1");
                activePrefab.transform.GetChild(0).GetComponent<Image>().sprite =
                    nowExercise.Type[3];
            }
            else
            {
                Debug.LogError("Error2");
                activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = nowExercise.Type[1];
            }
        
            ImageSizer(activePrefab, nowExercise.Type_string);
            Debug.LogError("Error3");
            activePrefab.transform.GetChild(1).GetComponent<Image>().sprite =
                nowExercise.Character;
            Debug.LogError("Error4");
            activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
                nowExercise.Title;
            Debug.LogError("Error5");
            activePrefab.transform.GetChild(3).GetComponent<TMP_Text>().text =
                $"{nowExercise.time}sec·{nowExercise.set+1}set";
        
            if (NetworkManager._instance.todayActivity)
            {
                Debug.LogError("today");
                activePrefab.GetComponent<ActivityData>().ButtonExData = nowExercise;
            }
            else
            {
                Debug.LogError("Exercise");
                activePrefab.GetComponent<ActivityData>().ButtonExData = nowExercise;
            }
        
            activePrefab.GetComponent<Button>().onClick.AddListener(()=> firstCheck(activePrefab, i));
            //if(NetworkManager._instance.todaysRandom.Count > 4) return;
            NetworkManager._instance.todaysRandom.Add(activePrefab.GetComponent<ActivityData>().ButtonExData);
            Debug.LogError("Error10");
        }
    
        void MakeActivity_Already(GameObject activePrefab, ExerciseData nowExercise, int i)
        {
            Debug.LogError("MakeActivity" + indexer++);
            Debug.LogError(nowExercise.Title);
            if (nowExercise.isClear)
            {
                Debug.LogError("Error1");
                activePrefab.transform.GetChild(0).GetComponent<Image>().sprite =
                    nowExercise.Type[3];
            }
            else
            {
                Debug.LogError("Error2");
                activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = nowExercise.Type[1];
            }
        
            ImageSizer(activePrefab, nowExercise.Type_string);
            Debug.LogError("Error3");
            activePrefab.transform.GetChild(1).GetComponent<Image>().sprite =
                NetworkManager._instance.todaysRandom[i].Character;
            Debug.LogError("Error4");
            activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().text =
                NetworkManager._instance.todaysRandom[i].Title;
            Debug.LogError("Error5");
            activePrefab.transform.GetChild(3).GetComponent<TMP_Text>().text =
                $"{nowExercise.time}sec·{nowExercise.set+1}set";
            if (NetworkManager._instance.todayActivity)
            {
                Debug.LogError("today");
                activePrefab.GetComponent<ActivityData>().ButtonExData = NetworkManager._instance.todaysRandom[i];
            }
            else
            {
                Debug.LogError("exercise");
                activePrefab.GetComponent<ActivityData>().ButtonExData = NetworkManager._instance.exerciseDatas[i];
            }
        
            activePrefab.GetComponent<Button>().onClick.AddListener(()=> firstCheck(activePrefab, i));
            if(NetworkManager._instance.todaysRandom.Count > 4) return;
            //NetworkManager._instance.todaysRandom.Add(activePrefab.GetComponent<ActivityData>().ButtonExData);
            Debug.LogError("Error10");
        }

        void firstCheck(GameObject activePrefab, int i)
        {
        
            // if (activePrefab.transform.GetSiblingIndex() != 0 && !NetworkManager._instance.isTestMode)
            // {
            //     if (!layoutGroup.GetChild(activePrefab.transform.GetSiblingIndex() - 1).GetComponent<ActivityData>()
            //             .ButtonExData.isClear)
            //     {
            //         SoundManager.instance.SEAudioPlay(4);
            //         //StartCoroutine(RedRecycler(activePrefab));
            //         return;
            //     }
            // }

            StartCoroutine(SceneConverter(i));
        }

        IEnumerator RedRecycler(GameObject activePrefab)
        {
            activePrefab.GetComponent<Image>().color = new Color(1f, 0.9176f,0.9176f);
            activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.Type[2];
            activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().color = new Color(0.9411f, 0.2823f, 0.1333f);
            activePrefab.transform.GetChild(4).gameObject.SetActive(true);

            yield return new WaitForSeconds(1);
        
            activePrefab.GetComponent<Image>().color = new Color(1f, 1f, 1f);
            activePrefab.transform.GetChild(0).GetComponent<Image>().sprite = activePrefab.GetComponent<ActivityData>().ButtonExData.Type[1];
            activePrefab.transform.GetChild(2).GetComponent<TMP_Text>().color = new Color(0.2627f, 0.7568f, 0.7803f);
            activePrefab.transform.GetChild(4).gameObject.SetActive(false);
        }

        void ImageSizer(GameObject activePrefab, string nowExerciseType)
        {
            Debug.LogError("ImageSizer");
            RectTransform rect = activePrefab.transform.GetChild(0).GetComponent<RectTransform>();
            Vector2 newSizeDelta;
            switch (nowExerciseType)
            {
                case "Stretching":
                    newSizeDelta = new Vector2(114, rect.sizeDelta.y);
                    rect.sizeDelta = newSizeDelta;
                    break;
                case "Pain Relief":
                    newSizeDelta = new Vector2(114, rect.sizeDelta.y);
                    rect.sizeDelta = newSizeDelta;
                    break;
                case "Stabilization":
                    newSizeDelta = new Vector2(129, rect.sizeDelta.y);
                    rect.sizeDelta = newSizeDelta;
                    break;
                case "ROM":
                    newSizeDelta = new Vector2(70, rect.sizeDelta.y);
                    rect.sizeDelta = newSizeDelta;
                    break;
                case "Strengthening":
                    newSizeDelta = new Vector2(145, rect.sizeDelta.y);
                    rect.sizeDelta = newSizeDelta;
                    break;
            }
        
        }

        IEnumerator SceneConverter(int i)
        {
            SoundManager.instance.SEAudioPlay(0);
            SoundManager.instance.SEAudioPlay2(10);
            yield return new WaitForSeconds(1);
            Debug.LogError(i +" "+ NetworkManager._instance.choosedExercise);
            NetworkManager._instance.choosedExercise = i;
            NetworkManager._instance.exerciseDatas[i].isClear = true;
            SceneManager.LoadScene("ExerciseScene");
        }
    }
}
