using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mobile
{
    public class ActivityGroup :MonoBehaviour
    {
        [SerializeField] private bool isClear = false;
        
        public Transform summonPoint;

        private Dictionary<int, List<ExerciseData>> setExercises = new Dictionary<int, List<ExerciseData>>();

        public TMP_Text dayText;
        private int showDay = 0;

        public DataSnapshot _snapshot;
        private Dictionary<int, DateTime> playDays = new Dictionary<int, DateTime>();

        public GameObject ActivatePrefab;
        public GameObject NormalPrefab;
        
        public Dictionary<int, List<ExerciseData>> exercisesByDay = new Dictionary<int, List<ExerciseData>>();
    
        public Dictionary<int, List<ExerciseData>> ShoulderImpingementSyndrome = new Dictionary<int, List<ExerciseData>>();

        public Dictionary<int, List<ExerciseData>> adhesiveCapsulitis = new Dictionary<int, List<ExerciseData>>();
        
        public IEnumerator ChainStart()
        {
            yield return StartCoroutine(ExerciseListSetting());
            showDay = NetworkManager._instance._playerData.day;
            dayText.text = DateTime.Today.ToString("MM/dd");

            yield return StartCoroutine(timeSet());
            
            foreach (Transform summonedObject in summonPoint.transform)
            {
                Destroy(summonedObject.gameObject);
            }

            switch (NetworkManager._instance._playerData.category)
            {
                case "Shoulder Impingement Syndrome":
                    yield return StartCoroutine(MakeActivity_myData(ShoulderImpingementSyndrome));
                    setExercises = ShoulderImpingementSyndrome;
                    break;
                case "Adhesive Capsulitis":
                    yield return StartCoroutine(MakeActivity_myData(adhesiveCapsulitis));
                    setExercises = adhesiveCapsulitis;
                    break;
                default:
                    yield return StartCoroutine(MakeActivity_myData(exercisesByDay));
                    setExercises = exercisesByDay;
                    break;
            }
        }

        public IEnumerator ExerciseListSetting()
        {
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

            yield return null;
        }
        
        

        public IEnumerator timeSet()
        {
            playDays.Clear();
            DataSnapshot playDate = _snapshot.Child(NetworkManager._instance._playerData.PatientID)
                .Child("days_list");
             playDays = new Dictionary<int, DateTime>();

            for (int i = 0; i < 20; i++) //20일간 돌아가야함
            {
                if(playDate.Child(i.ToString()).Exists){
                    playDays.Add(int.Parse(playDate.Child(i.ToString()).Key), DateTime.Parse(playDate.Child(i.ToString()).Value.ToString()));
                }
                else
                {
                    if (playDays.ContainsKey(i)) continue;
                    if (i == 0)
                    {
                        playDays.Add(i, DateTime.Parse(NetworkManager._instance._playerData.firstDate));
                    }
                    else
                    {
                        playDays.Add(i, playDays[i - 1].AddDays(1));
                    }
                }
            }
            
            yield return null;
        }

        IEnumerator MakeActivity_myData(Dictionary<int, List<ExerciseData>> curriculum)
        {
            //Todo : 가장 마지막 숫자
            foreach (var item in curriculum[NetworkManager._instance._playerData.day])
            {
                yield return StartCoroutine(SummonActivity_myData(item));
            }
        }

        public void BTN_MakeElseActivity(int day)
        {
            showDay += day;
            bool hasDisease;
            switch (NetworkManager._instance._playerData.category)
            { 
                case "Shoulder Impingement Syndrome":
                case "Adhesive Capsulitis":
                    if (showDay <= 0)
                    {
                        showDay = 0;
                    }
                    else if(showDay >= 19)
                    {
                        //showDay = 19;
                    }

                    hasDisease = true;
                    break;
                default:
                    if (showDay <= 0)
                    {
                        showDay = 0;
                    }
                    else if(showDay >= 19)
                    {
                        //showDay = 19;
                    }

                    hasDisease = false;
                    break;
            }
            dayText.text = playDays[showDay].ToString("MM/dd");
            StartCoroutine(MakeActivity_(showDay, hasDisease));
        }
        
        IEnumerator MakeActivity_(int day, bool hasDisease)
        {
            if (hasDisease)
            {
                day %= 10;
            }
            
            foreach (Transform summonedObject in summonPoint.transform)
            {
                Destroy(summonedObject.gameObject);
            }
            
            Debug.Log(setExercises[day].Count + " exercises found");
            foreach (var item in setExercises[day])
            {
                yield return StartCoroutine(SummonActivity_myData(item));
            }
        }

        IEnumerator SummonActivity_myData(ExerciseData data)
        {
            yield return StartCoroutine(CheckDayClear(data.Title));
            
            GameObject ActiveObject = Instantiate(ActivatePrefab, summonPoint);
            ActiveObject.GetComponent<Button>().enabled = false;
            if (isClear)
            {
                ActiveObject.transform.GetChild(5).gameObject.SetActive(true);
            }
            else
            {
                if (NetworkManager._instance._playerData.day != showDay)
                {
                    ActiveObject.transform.GetChild(4).gameObject.SetActive(true);
                    // var imageComponent = ActiveObject.transform.GetChild(0).GetComponent<Image>();
                    // var currentColor = imageComponent.color;
                    // imageComponent.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0.4f);
                    //
                    // var imageComponent_title = ActiveObject.transform.GetChild(1).GetComponent<Image>();
                    // var currentColor_Title = imageComponent_title.color;
                    // imageComponent_title.color = new Color(currentColor_Title.r, currentColor_Title.g,
                    //     currentColor_Title.b, 0.4f);
                }
            }
            isClear = false;

            // ActiveObject.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = data.Type_string;

            ActiveObject.GetComponent<Image>().sprite = data.Char_mini;
            // ActiveObject.transform.GetChild(2).GetComponent<TMP_Text>().text = data.Title;
            // ActiveObject.transform.GetChild(3).GetComponent<TMP_Text>().text = $"{data.time}sec·{data.set}set";
            
            yield return null;
        }

        IEnumerator CheckDayClear(string title)
        {
            DateTime showDate = playDays[showDay];
            
            DataSnapshot snapshot = _snapshot.Child(NetworkManager._instance._playerData.PatientID)
                .Child("ExerciseDatas").Child(title).Child("history");

            foreach (var day in snapshot.Children)
            {
                if (day.Key.Contains(showDate.ToString("yyyy-MM-dd")))
                {
                    isClear = true;
                }
            }

            yield return null;
        }
    }
}
