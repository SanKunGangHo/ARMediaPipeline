using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DreamTeamMobile;
using UnityEngine;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Unity.VisualScripting;

namespace Network
{
    public class FireBaseLoader : MonoBehaviour
    {
        public static readonly string DateTimeFormat = "yyyy-MM-dd";
        //private static readonly string DateTimeFormatWithHours = "MM월 dd일 HH시";
        private static readonly string DateTimeFormatWithMinutes = "yyyy-MM-dd HH:mm";
        public bool isAdmin = false;

        [Header("LoginScene")] public PatientIDCheck patientIDCheck;
        public LoginManager loginManager;
        private NetworkManager nm;

        private List<DateTime> _dateTimes = new List<DateTime>();

        /// <summary>
        /// 회원가입용 데이터셋
        /// </summary>
        public class RegisterData
        {
            public string id;
            public string pw;
            public int days;
            public string dateOfBirth;
            public string job;
            public string gender;
            public string lastDate;
            public string startDate;
            public bool isTutorial;
            public List<string> days_list;


            public RegisterData(string id, string pw, string dateOfBirth, string job, string gender)
            {
                this.id = id;
                this.pw = pw;
                days = 0;
                this.gender = gender;
                this.dateOfBirth = dateOfBirth;
                this.lastDate = DateTime.Now.ToString(DateTimeFormat);
                this.startDate = DateTime.Now.ToString(DateTimeFormatWithMinutes);
                days_list = new List< string>
                {
                    DateTime.Now.ToString(DateTimeFormat)
                };
                this.job = job;
                isTutorial = false;
            }

        }

        /// <summary>
        /// bool 데이터
        /// </summary>
        private class BooleanData
        {
            public Boolean booler;

            public BooleanData(bool boolean)
            {
                booler = boolean;
            }
        }

        /// <summary>
        /// 튜토리얼 데이터 셋
        /// </summary>
        private class TutorialData
        {
            public bool isTutorial;

            public TutorialData(bool _tutorial)
            {
                this.isTutorial = _tutorial;
            }
        }

        /// <summary>
        /// 사전 문진표 정답용 데이터 셋
        /// </summary>
        public class ExamData
        {
            public string dateTime;
            public int Q01, Q02, Q03, Q04, Q04_1, Q05, Q06, Q07, Q08, Q09, Q10, Q10_1, Q10_2, Q10_3;

            public ExamData(int[] answerList)
            {

                if (answerList.Length < 14)
                {
                    throw new ArgumentException("Insufficient elements in the array", nameof(answerList));
                }

                dateTime = DateTime.Today.ToString();
                Q01 = answerList[0];
                Q02 = answerList[1];
                Q03 = answerList[2];
                Q04 = answerList[3];
                Q04_1 = answerList[4];
                Q05 = answerList[5];
                Q06 = answerList[6];
                Q07 = answerList[7];
                Q08 = answerList[8];
                Q09 = answerList[9];
                Q10 = answerList[10];
                Q10_1 = answerList[11];
                Q10_2 = answerList[12];
                Q10_3 = answerList[13];
            }
        }

        /// <summary>
        /// 중기 중증도 검사용 데이터 셋
        /// </summary>
        public class BriefData
        {
            public string dateTime;
            public int Q1, Q2, Q3, Q4;

            public BriefData(int Q1, int Q2, int Q3, int Q4)
            {
                dateTime = DateTime.Today.ToString();
                this.Q1 = Q1;
                this.Q2 = Q2;
                this.Q3 = Q3;
                this.Q4 = Q4;
            }
        }

        /// <summary>
        /// 일일 운동 세팅용 데이터 셋
        /// </summary>
        public class ExerciseData_Fire
        {
            public string fireExercise;
            public bool isClear;

            public ExerciseData_Fire(string FireExercise, bool isClear)
            {
                fireExercise = FireExercise;
                this.isClear = isClear;
            }
        }

        /// <summary>
        /// 어깨 각도 저장용 데이터 셋
        /// </summary>
        public class angleData
        {
            public string dateTime;
            public float LeftSide, LeftForward, RightSide, RightForward;

            public angleData(float[] angles)
            {
                dateTime = DateTime.Now.ToString();
                Debug.Log(angles.Length +" "+ angles[0]);
                LeftSide = angles[0];
                RightSide = angles[1];
                LeftForward = angles[2];
                RightForward = angles[3];
            }
        }

        private DatabaseReference _databaseReference;

        public string ComputeSha256Hash(string input)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        private void Start()
        {
            //CheckFirebaseDependencies();
            nm = NetworkManager._instance;
            nm._firebaseLoader = this; //싱글톤인 네트워크 매니저에 자신 등록
            _databaseReference = FirebaseDatabase.DefaultInstance.RootReference; //파이어베이스 레퍼런스 캐싱
        }
        
        public FirebaseApp app;
        private void CheckFirebaseDependencies()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                    Debug.Log("Firebase is ready to use.");
                }
                else
                {
                    Debug.LogError(System.String.Format(
                        "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }
            });
        }

        /// <summary>
        /// Registration
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_pw"></param>
        /// <param name="_dateOfBirth"></param>
        /// <param name="_job"></param>
        /// <param name="_gender"></param>
        public void RegisterInput(string _id, string _pw, string _dateOfBirth, string _job, string _gender)
        {
            Debug.LogError("InputFireBase");
            //비밀번호 암호화

            Debug.LogError($"{_id}, {_pw}, {_dateOfBirth}, {_job}, {_gender}");

            //string ComputedPW = ComputeSha256Hash(_pw);

            var data = new RegisterData(_id, _pw, _dateOfBirth, _job, _gender);
            string jsonData = JsonConvert.SerializeObject(data);

            _databaseReference.Child(_id).SetRawJsonValueAsync(jsonData);
        }

        /// <summary>
        /// Data Load
        /// </summary>
        /// <param name="_id"></param>
        public IEnumerator PlayerDataLoad(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot;
            IDictionary playerData = (IDictionary)snapshot.Value;
            nm._playerData.PatientID = playerData["id"].ToString();
            nm._playerData.PW = playerData["pw"].ToString();
            nm._playerData.dateOfBirth =
                DateTime.Parse(playerData["dateOfBirth"].ToString());
            nm._playerData.job = playerData["job"].ToString();
            nm._playerData.gender = playerData["gender"].ToString();
            nm._playerData.lastDate = playerData["lastDate"].ToString();
            nm._playerData.firstDate = playerData["startDate"].ToString();
            nm._playerData.day = int.Parse(playerData["days"].ToString());
            nm._playerData.category = playerData["category"].ToString();

            nm._playerData.days_list = new Dictionary<int, string>();
            foreach (var day in snapshot.Child("days_list").Children)
            {
                nm._playerData.days_list.Add(int.Parse(day.Key.ToString()), day.Value.ToString());
            }
            yield return null;
        }

        IEnumerator MeasurementChecker(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot;

            if (snapshot.Children.Count() != 0)
            {
                nm.preMeasurement = true;
            }

            if (snapshot.Children.Any())
            {
                nm.postMeasurement = true;
            }

            yield return null;
        }

        /// <summary>
        /// Log-In
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_pw"></param>
        public IEnumerator Login(string _id, string _pw)
        {
            var task = _databaseReference.Child(_id).GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);
            if (task.IsFaulted)
            {
                Debug.Log("Login Failed");
                
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists)
                {
                    loginManager.warningText.text =
                        "Login failed. \n Please check your userID and password and try again.";
                    yield break;
                }

                IDictionary data = (IDictionary)snapshot.Value;

                if (data["id"].ToString() == _id && data["pw"].ToString() == _pw)
                {
                    if (snapshot.Child("isAdmin").Exists)
                    {
                        if(NetworkManager._instance.isMobile)
                        {
                            loginManager.warningText.text =
                            "Login failed. \n  Cannot log in with this account ";
                            yield break;
                        }
                        isAdmin = true;
                        SceneManager.LoadScene("AdminScene");
                        yield break;
                    }
                    yield return StartCoroutine(PlayerDataLoad(snapshot));
                    yield return StartCoroutine(UpdateDays(snapshot));
                    yield return StartCoroutine(PreMeasurement_Check(snapshot));
                    yield return StartCoroutine(PostMeasurement_Check(snapshot));
                    yield return StartCoroutine(ExamCheck(snapshot));
                    yield return StartCoroutine(lastDateSetter(snapshot));
                    yield return StartCoroutine(TutorialCheck(snapshot));
                    
                    //yield return StartCoroutine(MeasurementChecker(snapshot));
                    PlayerPrefs.SetString("ID", _id);
                    PlayerPrefs.SetString("PW", _pw);
                    if (NetworkManager._instance.isMobile)
                        SceneManager.LoadScene("#3 homeMain");
                    else
                        SceneManager.LoadScene("Home");
                }
                else
                {
                    loginManager.warningText.text =
                        "Login failed. \n Please check your userID and password and try again.";
                }
            }
        }

        public IEnumerator lastDateSetter(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot;
            // 스냅샷이 null이 아닌지 확인
            if (_snapshot.Exists)
            {
                DateTime now = DateTime.Now;
                string nowFormatted = now.ToString(DateTimeFormat);
                nm._playerData.lastDate = nowFormatted;

                // _snapshot.Child("lastDate")의 값을 DateTime.Now.ToString(DateTimeFormat)으로 변경
                _databaseReference.Child(nm._playerData.PatientID).Child("lastDate")
                    .SetValueAsync(nowFormatted);

                Debug.Log("lastDate updated to: " + nowFormatted);
            }
            yield return null;
        }

        /// <summary>
        /// 아이디 체크
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public IEnumerator PlayerCheck(string _id)
        {
            var task = _databaseReference.Child(_id).GetValueAsync();

            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    patientIDCheck.login.SetActive(true);
                    patientIDCheck.gameObject.SetActive(false);
                    yield return new WaitForSeconds(2f);
                    patientIDCheck.gameObject.SetActive(true);
                }
                else
                {
                    patientIDCheck.IDCheck_Collected();
                    patientIDCheck.gameObject.SetActive(true);

                }
            }
        }

        /// <summary>
        /// 사전 문진표 체크
        /// </summary>
        /// <param name="_id"></param>
        public IEnumerator ExamCheck(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot.Child("Surveys").Child("On-Boarding Survey");

            if (snapshot.Exists)
            {
                nm.isOnBoardingEnd = true;
            }
            else
            {
                nm.isOnBoardingEnd = false;
            }
            
            if (SceneManager.GetActiveScene().name == "Home")
            {
                nm.hm.OnBoardingEnd();
            }
            
            yield return null;
        }

        /// <summary>
        /// 튜토리얼 수행여부 체크
        /// </summary>
        /// <param name="_id"></param>
        /// <returns></returns>
        public IEnumerator TutorialCheck(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot;
            IDictionary tutoData = (IDictionary)snapshot.Value;

            if (!snapshot.Exists)
            {
                yield break;
            }

            if (tutoData["isTutorial"] is bool booleanValue)
            {
                nm.isAppTutorialEnd = booleanValue;
            }
            else if (bool.TryParse(tutoData["isTutorial"].ToString(), out booleanValue))
            {
                nm.isAppTutorialEnd = booleanValue;
            }

            if (SceneManager.GetActiveScene().name == "Home")
            {
                nm.hm.AppTutorialEnd();
            }
        }

        /// <summary>
        /// 튜토리얼 완료
        /// </summary>
        public void TutorialInput()
        {
            _databaseReference.Child(nm._playerData.PatientID).Child("isTutorial")
                .SetValueAsync(true);
        }

        /// <summary>
        /// 사전문진표 작성
        /// </summary>
        /// <param name="values"></param>
        public void ExamInput(string _title)
        {
            //var data = new ExamData(values);
            var data = nm._playerData.ExaminationAnswerList;
            //string jsonData = JsonUtility.ToJson(data);
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            _databaseReference.Child(nm._playerData.PatientID).Child("Surveys").Child(_title).Child(DateTime.Now.ToString(DateTimeFormatWithMinutes))
                .SetRawJsonValueAsync(jsonData);
            nm._playerData.ExaminationAnswerList.Clear();
        }

        /// <summary>
        /// 중증도평가 작성
        /// </summary>
        /// <param name="values"></param>
        public void BriefInput(params int[] values)
        {
            var data = new BriefData(values[0], values[1], values[2], values[3]);
            string jsonData = JsonUtility.ToJson(data);

            _databaseReference.Child(nm._playerData.PatientID).Child("BriefSurvey")
                .Child(DateTimeFormatWithMinutes)
                .SetRawJsonValueAsync(jsonData);
        }

        /// <summary>
        /// 사전문진표 있는지 체크
        /// </summary>
        public IEnumerator ExamOutput()
        {
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("Surveys").Child("On-Boarding Survey").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);
            if (task.IsFaulted)
            {
                Debug.Log("TaskError - FireBaseLoader.ExamLoad");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result; //온보딩 - 날짜 - 답변
                if (snapshot.Exists)
                {
                    IDictionary examData = (IDictionary)snapshot.Value;
                    _databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        foreach (var date in examData.Keys)
                        {
                            nm.rm.MakeExamDone(DateTime.Parse((string)date));
                        }
                    });
                }
                else
                {
                    _databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        nm.rm.MakeExamStart();
                    });
                }
            }
        }

        /// <summary>
        /// 중증도평가 있는지 체크
        /// </summary>
        public IEnumerator BriefOutput()
        {
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("Surveys").Child("Brief Survey").GetValueAsync();
            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);
            
            if (task.IsFaulted)
            {
                Debug.Log("Brief Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result; //서베이 - 중증도 - 
                if (snapshot.Exists)
                {
                    IDictionary briefData = (IDictionary)snapshot.Value;
                    _databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        nm.rm.MakeBriefDone(briefData);
                        nm.rm.MakeBriefStart();
                    });
                }
                else
                {
                    _databaseReference.GetValueAsync().ContinueWithOnMainThread(task =>
                    {
                        nm.rm.MakeBriefStart();
                    });
                }
            }
        }

        // /// <summary>
        // /// 사후 가동률검사 최대각도, 평균각도 등록
        // /// </summary>
        // /// <param name="Angles_Max_After"></param>
        // /// <param name="Angles_Ave_After"></param>
        // /// <returns></returns>
        // public IEnumerator AngleCollector_After(float[] Angles_Max_After, float[] Angles_Ave_After)
        // {
        //     yield return StartCoroutine(AngleLoader(Angles_Max_After, nameof(Angles_Max_After), 0));
        //     yield return StartCoroutine(AngleLoader(Angles_Ave_After, nameof(Angles_Ave_After), 0));
        // }

        /// <summary>
        /// 사전 가동률검사 최대각도, 평균각도 등록
        /// </summary>
        /// <param name="Angles_Max_Before"></param>
        /// <param name="Angles_Ave_Before"></param>
        /// <returns></returns>
        public IEnumerator AngleCollector_Before(float[] Angles_Max_Before, float[] Angles_Ave_Before)
        {
            yield return StartCoroutine(AngleLoader(Angles_Max_Before, nameof(Angles_Max_Before), 1));
            yield return StartCoroutine(AngleLoader(Angles_Ave_Before, nameof(Angles_Ave_Before), 1));
        }

        /// <summary>
        /// 가동률 검사 데이터 집어넣기
        /// </summary>
        /// <param name="input"></param>
        /// <param name="_name"></param>
        /// <param name="beforeAfter"></param>
        /// <returns></returns>
        private IEnumerator AngleLoader(float[] input, string _name, int beforeAfter)
        {
            Debug.LogError(input.Length + " " + input[0]);
            var data = new angleData(input);
            string jsonData = JsonUtility.ToJson(data);
            _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                .Child("Measurement").Child("history").Child("day " + DateTime.Now.ToString(DateTimeFormatWithMinutes))
                .Child(_name).SetRawJsonValueAsync(jsonData);

            yield break;
        }

        /// <summary>
        /// 가동률 검사 결과값 불러오기
        /// </summary>
        /// <param name="angleType_Max_Before"></param>
        /// <param name="angleType_Ave_Before"></param>
        /// <param name="angleType_Max_After"></param>
        /// <param name="angleType_Ave_After"></param>
        /// <returns></returns>
        public IEnumerator AngleResult(string angleType_Max_Before, string angleType_Ave_Before)
        {
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                .Child("Measurement").Child("history").GetValueAsync();
            
            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                yield return StartCoroutine(GetClosestTimeEntry(snapshot));


                int count = 0;
                foreach (var dt in _dateTimes)
                {
                    Debug.Log("Result Count : "+ _dateTimes.Count);
                    List<float> angle_float_Max_Before = new List<float>();
                    List<float> angle_float_Ave_Before = new List<float>();

                    DataSnapshot snapshot2 = snapshot.Child("day " + dt.ToString(DateTimeFormatWithMinutes));

                    bool beforeProcessed = ProcessAndSendAnalytics(snapshot2.Child(angleType_Max_Before),
                                               angle_float_Max_Before)
                                           && ProcessAndSendAnalytics(snapshot2.Child(angleType_Ave_Before),
                                               angle_float_Ave_Before);

                    if (beforeProcessed)
                    {
                        //_databaseReference.GetValueAsync().ContinueWithOnMainThread(task => {
                        nm._playerData.operationResult_Max_Before = angle_float_Max_Before;
                        nm._playerData.operationResult_Ave_Before = angle_float_Ave_Before;
                    }
                    
                    ExerciseManager.instance.AngleSummon(dt.ToString(DateTimeFormatWithMinutes), angle_float_Max_Before, angle_float_Ave_Before, count);
                    count++;
                }
            }
            
            _dateTimes.Clear();
        }
        
        public IEnumerator AngleResult_Mobile(string angleType_Max_Before, string angleType_Ave_Before)
        {
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                .Child("Measurement").Child("history").GetValueAsync();
            // var task = FirebaseDatabase.DefaultInstance.RootReference.Child("1q2w3e4r").Child("ExerciseDatas")
            //     .Child("Measurement").Child("history").GetValueAsync();
            
            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                yield return StartCoroutine(GetClosestTimeEntry(snapshot));
                
                foreach (var dt in _dateTimes)
                {
                    Debug.Log("Result Count : "+ _dateTimes.Count);
                    List<float> angle_float_Max_Before = new List<float>();
                    List<float> angle_float_Ave_Before = new List<float>();

                    DataSnapshot snapshot2 = snapshot.Child("day " + dt.ToString(DateTimeFormatWithMinutes));

                    bool beforeProcessed = ProcessAndSendAnalytics(snapshot2.Child(angleType_Max_Before),
                                               angle_float_Max_Before)
                                           && ProcessAndSendAnalytics(snapshot2.Child(angleType_Ave_Before),
                                               angle_float_Ave_Before);

                    AngleSceneSelector(beforeProcessed, dt, angle_float_Max_Before, angle_float_Ave_Before);
                }
            }
            _dateTimes.Clear();
        }
        
        public IEnumerator AngleResult_Mobile(string angleType_Max_Before, string angleType_Ave_Before, DateTime thatDay)
        {
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                .Child("Measurement").Child("history").GetValueAsync();
    
            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                yield return StartCoroutine(GetClosestTimeEntry_Mobile(snapshot, thatDay.ToString("yyyy-MM-dd")));

                foreach (var dt in _dateTimes)
                {
                    Debug.Log("Result Count : "+ _dateTimes.Count);
                    List<float> angle_float_Max_Before = new List<float>();
                    List<float> angle_float_Ave_Before = new List<float>();

                    DataSnapshot snapshot2 = snapshot.Child("day " + dt.ToString("yyyy-MM-dd"));
                    bool beforeProcessed = ProcessAndSendAnalytics(snapshot2.Child(angleType_Max_Before), angle_float_Max_Before) &&
                                           ProcessAndSendAnalytics(snapshot2.Child(angleType_Ave_Before), angle_float_Ave_Before);

                    AngleSceneSelector(beforeProcessed, dt, angle_float_Max_Before, angle_float_Ave_Before);
                }
            }
    
            _dateTimes.Clear();
        }

        private void AngleSceneSelector(bool beforeProcessed, DateTime dt, List<float> max, List<float> average)
        {
            switch (SceneManager.GetActiveScene().name)
            {
                case "#4 exercise":
                    if (beforeProcessed && dt == _dateTimes.First())
                    {
                        Debug.Log(dt.ToString(DateTimeFormatWithMinutes));
                        ExerciseManager.instance.mobileReport.operationResult_Ave_Before = average;
                        ExerciseManager.instance.mobileReport.operationResult_Max_Before = max;
                        ExerciseManager.instance.mobileReport.AddTiles(false);
                    }
                    else if(beforeProcessed)
                    {
                        Debug.Log(dt.ToString(DateTimeFormatWithMinutes));
                        ExerciseManager.instance.mobileReport.operationResult_Max_After = max;
                        ExerciseManager.instance.mobileReport.operationResult_Ave_After = average;
                        ExerciseManager.instance.mobileReport.AddTiles(true);
                    }
                    break;
                case "#3 homeMain":
                    if (beforeProcessed && dt == _dateTimes.First())
                    {
                        NetworkManager._instance.hm.mobileReport.operationResult_Ave_Before = average;
                        NetworkManager._instance.hm.mobileReport.operationResult_Max_Before = max;
                        NetworkManager._instance.hm.mobileReport.AddTiles(false);
                    }
                    else if(beforeProcessed)
                    {
                        NetworkManager._instance.hm.mobileReport.operationResult_Ave_After = average;
                        NetworkManager._instance.hm.mobileReport.operationResult_Max_After = max;
                        NetworkManager._instance.hm.mobileReport.AddTiles(true);
                    }
                    break;
            }
            
        }
        
        private IEnumerator GetClosestTimeEntry(DataSnapshot _snap)
        {
            DataSnapshot snapshot = _snap;
            List<DateTime> dateTimes = new List<DateTime>();

            foreach (var childSnapshot in snapshot.Children)
            {
                string timestamp = childSnapshot.Key;
                timestamp = timestamp.Replace("day ", "");
                // if (SceneManager.GetActiveScene().name == "#3 homeMain" && string.IsNullOrEmpty(thatDay))
                // {
                //     if (thatDay != null && timestamp.Contains(thatDay))
                //     {
                //         continue;
                //     }
                // }
                if (DateTime.TryParse(timestamp, out DateTime dateTime))
                {
                    dateTimes.Add(dateTime);
                }
                else
                {
                    Debug.LogError("Invalid date format: " + timestamp);
                }
            }
    
            _dateTimes.Add(dateTimes[^2]);
            _dateTimes.Add(dateTimes[^1]);
            if (_dateTimes.Count >= 3)
            {
                _dateTimes.RemoveAt(0);
            }

            yield return null;
        }
        
        private IEnumerator GetClosestTimeEntry_Mobile(DataSnapshot snapshot, string thatDay)
        {
            List<DateTime> dateTimes = new List<DateTime>();

            foreach (var childSnapshot in snapshot.Children)
            {
                string timestamp = childSnapshot.Key;
                timestamp = timestamp.Replace("day ", "");

                if (DateTime.TryParse(timestamp, out DateTime dateTime))
                {
                    dateTimes.Add(dateTime);
                }
                else
                {
                    Debug.LogError("Invalid date format: " + timestamp);
                }
            }

            // Ensure _dateTimes only contains the specified day
            _dateTimes.Clear();
            DateTime parsedThatDay;

            if (DateTime.TryParse(thatDay, out parsedThatDay))
            {
                foreach (var dateTime in dateTimes)
                {
                    if (dateTime.Date == parsedThatDay.Date)
                    {
                        _dateTimes.Add(dateTime);
                    }
                }
            }
            else
            {
                Debug.LogError("Invalid thatDay format: " + thatDay);
            }

            // Also add the last two dates from the sorted list if they are not already included
            dateTimes.Sort();
            for (int i = dateTimes.Count - 2; i < dateTimes.Count; i++)
            {
                if (i >= 0 && !_dateTimes.Contains(dateTimes[i]))
                {
                    _dateTimes.Add(dateTimes[i]);
                }
            }

            yield return null;
        }
        
        private bool ProcessAndSendAnalytics(DataSnapshot snapshot, List<float> outputList)
        {
            if (snapshot != null && snapshot.Value != null)
            {
                IDictionary anglesDictionary = (IDictionary)snapshot.Value;
                if (anglesDictionary != null)
                {
                    outputList.Add(float.Parse(anglesDictionary["LeftSide"].ToString()));
                    outputList.Add(float.Parse(anglesDictionary["RightSide"].ToString()));
                    outputList.Add(float.Parse(anglesDictionary["LeftForward"].ToString()));
                    outputList.Add(float.Parse(anglesDictionary["RightForward"].ToString()));

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 스탭별 최대값을 모은 Dictionary
        /// </summary>
        /// <param name="exerciseData"></param>
        /// <param name="shoulderAngles"></param>
        /// <returns></returns>
        public IEnumerator ExerciseClear(ExerciseData exerciseData, Dictionary<string, float> shoulderAngles_L,
            Dictionary<string, float> shoulderAngles_R)
        {
            string todayKey = "day " + DateTime.Now.ToString(DateTimeFormatWithMinutes);
            var task = _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                .Child(exerciseData.Title)
                .Child("history").GetValueAsync();


            yield return new WaitUntil(() => task.IsCompleted || task.IsFaulted);

            if (task.IsCompleted)
            {
                string jsonData = "";

                Dictionary<string, float> main = new Dictionary<string, float>();

                foreach (var VA in shoulderAngles_L)
                {
                    main.Add(VA.Key, VA.Value);
                }

                foreach (var VA in shoulderAngles_R)
                {
                    main.Add(VA.Key, VA.Value);
                }


                jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(main);

                _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                    .Child(exerciseData.Title).Child("history")
                    .Child(todayKey)
                    .SetRawJsonValueAsync(jsonData);

                exerciseData.isClear = true;
                
            }
        }
        
        /// <summary>
        /// 일차 업데이트
        /// </summary>
        /// <returns></returns>
        public IEnumerator UpdateDays(DataSnapshot _snap) //id
        {
            //if (!NetworkManager._instance.isAppTutorialEnd) yield break;
            
            DataSnapshot snapshot = _snap;
            if (snapshot.Exists)
            {
                // 유저 값이 존재하는 경우 값 가져오기
                string last = snapshot.Child("lastDate").ToString();
                int days = Convert.ToInt32(snapshot.Child("days").Value);

                if (!last.Contains(DateTime.Today.ToString("yyyy-MM-dd")))
                {
                    // days 값에 1을 더한 후 갱신
                    int updatedDays = days + 1;
                    
                    nm._playerData.day = updatedDays;
                    // 데이터베이스에 갱신된 값 저장
                    var updateTask = _databaseReference.Child(nm._playerData.PatientID)
                        .Child("days").SetValueAsync(updatedDays);
                    var dateTask = _databaseReference.Child(nm._playerData.PatientID)
                        .Child("days_list").Child(nm._playerData.days_list.Count.ToString()).SetValueAsync(DateTime.Today.ToString("yyyy-MM-dd"));
                    yield return new WaitUntil(() => updateTask.IsCompleted || updateTask.IsFaulted);
                    yield return new WaitUntil(() => dateTask.IsCompleted);

                    if (updateTask.IsFaulted)
                    {
                        Debug.LogError(updateTask.Exception);
                    }
                    else if (updateTask.IsCompleted)
                    {
                        Debug.Log($"Days successfully updated to {updatedDays}");
                    }
                }
                else
                {
                    Debug.Log($"Current Days: {days}");
                }
            }
            else
            {
                Debug.Log("Days data does not exist.");
            }
        }
        
        public IEnumerator UpdateDays(int updatedDays = 0) //id
        {
            if (NetworkManager._instance._playerData.day < 20) yield break;
            
            var updateTask = _databaseReference.Child(nm._playerData.PatientID)
                .Child("days").SetValueAsync(updatedDays);
            yield return new WaitUntil(() => updateTask.IsCompleted || updateTask.IsFaulted);

            if (updateTask.IsFaulted)
            {
                Debug.LogError(updateTask.Exception);
            }
            else if (updateTask.IsCompleted)
            {
                Debug.Log($"Days successfully updated to {updatedDays}");
            }
                
        }

        public IEnumerator TodaysRandomClearCheck()
        {
            var task = _databaseReference.Child(nm._playerData.PatientID)
                .Child("ExerciseDatas")
                .GetValueAsync();

            yield return new WaitUntil(() => task.IsFaulted || task.IsCompleted);

            if (task.IsFaulted)
            {
                Debug.LogError("TodaysRandom Error");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                bool exists = snapshot.Exists;

                if (exists)
                {
                    string today = "day " + DateTime.Now.ToString(DateTimeFormat);

                    foreach (DataSnapshot exerciseSnapshot in snapshot.Children)
                    {
                        string exerciseKey = exerciseSnapshot.Key;

                        // Measurement를 제외한 다른 운동들 검사
                        if (exerciseKey != "Measurement")
                        {
                            DataSnapshot historySnapshot = exerciseSnapshot.Child("history");
                            bool historyExists = historySnapshot.Exists;

                            if (historyExists)
                            {
                                foreach (DataSnapshot childSnapshot in historySnapshot.Children)
                                {
                                    string key = childSnapshot.Key;

                                    if (key.StartsWith(today))
                                    {

                                        // NetworkManager의 ExerciseDatas_all 갱신
                                        foreach (var exerciseData in nm.exerciseDatas_all)
                                        {
                                            if (exerciseData.Title == exerciseKey)
                                            {
                                                exerciseData.isClear = true;
                                                Debug.LogError($"{exerciseData.Title} isClear set to true");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogError("Exercise data does not exist.");
                }
            }
        }

        public void TodaysClearFalse()
        {
            foreach (var td in nm.todaysRandom)
            {
                td.isClear = false;
                nm.todaysCleared.Add(false);
            }
        }

        /// <summary>
        /// 회원가입시 운동리스트 생성
        /// </summary>
        /// <param name="exercises"></param>
        public void TodaysActivityCollector(List<string> exercises) //변경 완료
        {
            Debug.LogError("TodaysActivityCollector" + nm._playerData.PatientID);
            ExerciseData_Fire data = new ExerciseData_Fire("", false);
            foreach (var exerciseAsset in exercises)
            {
                data = new ExerciseData_Fire(exerciseAsset, false);
                var jsonData = JsonUtility.ToJson(data);
                _databaseReference.Child(nm._playerData.PatientID).Child("ExerciseDatas")
                    .Child(exerciseAsset).Child("history")
                    .SetValueAsync(jsonData);
            }
            //var data = new ExerciseData_Fire();

        }


        public void CategoryInput(string category) //카테고리 넣어주기
        {
             Dictionary<string, object> jsonData = new Dictionary<string, object>()
             {
                 {"category", category}
             };

            _databaseReference.Child(nm._playerData.PatientID).UpdateChildrenAsync(jsonData);
        }

        public IEnumerator PreMeasurement_Input() //변경 완료
        {
            nm.preMeasurement = true;
            nm.choosedExercise = 1000;
            SceneManager.LoadScene("ExerciseScene");

            yield break;
        }

        /// <summary>
        /// 사전 가동률 검사 실행여부
        /// </summary>
        /// <returns></returns>
        public IEnumerator PreMeasurement_Check(DataSnapshot _snapshot) //변경 완료
        {
            nm.preMeasurement = false;

            DataSnapshot snapshot = _snapshot.Child("ExerciseDatas").Child("Measurement").Child("history");
            bool exists = snapshot.Exists;

            if (exists)
            {
                string today = "day " + DateTime.Now.ToString(DateTimeFormat);
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string key = childSnapshot.Key;
                    if (key.StartsWith(today))
                    {
                        nm.preMeasurement = true;
                        yield break;
                    }
                }
            }
        }

        public void PostMeasurement_Input()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "postMeasurement", DateTime.Today.ToString(DateTimeFormat) }
            };

            _databaseReference.Child(nm._playerData.PatientID).UpdateChildrenAsync(data);
        }

        /// <summary>
        /// 사후 가동률 검사했는지 체크
        /// </summary>
        /// <returns></returns>
        public IEnumerator PostMeasurement_Check(DataSnapshot _snapshot)
        {
            DataSnapshot snapshot = _snapshot.Child("ExerciseDatas").Child("Measurement").Child("history");
            bool exists = snapshot.Exists;

            if (exists)
            {
                int matchCount = 0;
                foreach (DataSnapshot childSnapshot in snapshot.Children)
                {
                    string key = childSnapshot.Key;
                    if (Regex.IsMatch(key, @"^\d{2}월 \d{2}일 \d{2}시"))
                    {
                        matchCount++;
                    }
                }

                if (matchCount >= 2)
                {
                    nm.postMeasurement = true;
                }
                else
                {
                    nm.postMeasurement = false;
                }
            }
            else
            {
                nm.postMeasurement = false;
            }

            yield return null;
        }

        public IEnumerator StopExercise(string nowExercise)
        {
            //모든 어드민계정에 중단 시간 쏘기.
            List<string> admins = new List<string>();
            var task = _databaseReference.GetValueAsync();
            
            yield return new WaitUntil(()=> task.IsCompleted || task.IsFaulted);

            if (task.IsCompleted)
            {
                DataSnapshot snap = task.Result;


                foreach (var users in snap.Children)
                {
                    if (users.Child("isAdmin").Exists)
                    {
                        admins.Add(users.Key);
                    }
                }
            }

            if (admins.Count <= 0) yield break;

            foreach (var admin in admins)
            {
                var updateTask = _databaseReference.Child(admin)
                    .Child("PatientCall").Child(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).Child(nm._playerData.PatientID).SetValueAsync(nowExercise); //정확한 시간을 알기위해 전체 시간 전송
            }
        }
    }
}
