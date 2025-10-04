using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PoseManager : MonoBehaviour
{
    public static PoseManager instance = null;
    
    [SerializeField] MpMarkfind markfind;
    [SerializeField] MpLinefind linefind;
    public float angleRS, angleRE, angleLS, angleLE;
    public StepLayOutMaker stepMaker;
    public StepLayOutMaker_ForMobile stepMaker_ForMobile;

    [Header("추가 작업")]
    [SerializeField] private Image poseTimeUI;
    //[SerializeField] private Button startUI;
    [SerializeField] private GameObject CompleteUI;
    [SerializeField] private TextMeshProUGUI term;
    [SerializeField] private Timer timer;
    public GameObject imageSourceConfigWindow;
    public VideoSources _videoSources;
    public GameObject poseTimeCheckUI;
    public TMP_Text poseTimeText;
    private NetworkManager NM;
    private TutorialManager TM;
    public OperationSystem _operationSystem;
    public bool isReverse;
    public ExerciseData NowExercise;
    public GameObject AlertSet;
    float nowTime = 0;
    public float normalTime = 5f;
    public Canvas Ending;
    public GameObject progress, startBtn, BtnRestart, body;
    
    public List<float> shoulderAngleDatas_Left = new List<float>();
    public List<float> shoulderAngleDatas_Right = new List<float>();
    public Dictionary<string, float> shoulderAngleDatas_L = new Dictionary<string, float>();
    public Dictionary<string, float> shoulderAngleDatas_R = new Dictionary<string, float>();
    
    private WaitForSeconds oneSec = new WaitForSeconds(1f);
    private WaitForSeconds twoSec = new WaitForSeconds(2f);
    public int index;
    public int currentSet = 0;

    public GameObject[] stepLayout;

    public bool isReverseChanged;

    private Coroutine poseSequence2;

    private bool isPause = false;

    public AngleUIChanger angleUi;

    public List<GameObject> exerciseFalseThings;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (SceneManager.GetActiveScene().name == "Exercise" || SceneManager.GetActiveScene().name == "TestScene")
        {
            poseTimeCheckUI = poseTimeUI.transform.parent.GetChild(2).gameObject;
        }

        NM = NetworkManager._instance;
        TM = TutorialManager.instance;
    }
    
    void Start()
    {
        Debug.Log("Pose0");
        string exerciseType = "none";

        if (NM.isAppTutorialEnd)
        {
            if (NM.choosedExercise != 1000 && NM.choosedExercise != 1001 &&
                SceneManager.GetActiveScene().name != "TestScene")
            {
                Debug.Log("Pose1");
                if (NM.isNative_Activity)
                {
                    if (NM.todaysRandom[NM.choosedExercise].Type_string != null)
                    {
                        exerciseType = NM.todaysRandom[NM.choosedExercise].Type_string;
                    }
                    else
                    {
                        exerciseType = NM.todaysRandom[NM.choosedExercise].Type_string;
                    }
                }
                else
                {
                    exerciseType = NM.exerciseDatas_all[NM.choosedExercise].Type_string;
                }
            }

            switch (exerciseType)
            {
                case "Stretching":
                case "Pain Relief":
                case "Stabilization":
                    SoundManager.instance.BGMAudioPlay(3);
                    break;
                case "ROM":
                case "Strengthening":
                    SoundManager.instance.BGMAudioPlay(4);
                    break;
                case "none":
                    Debug.Log("error");
                    break;
            }
        }

        if (TM != null || (NM.isMobile && !NM.isAppTutorialEnd))
        {
            NowExercise = TM.TutorialExerciseData;
        }
        else
        {
            if (NM.isNative_Activity)
            {
                NowExercise = NM.todaysRandom[NM.choosedExercise];
            }
            else
            {
                NowExercise = NM.exerciseDatas_all[NM.choosedExercise];
            }
            
        }
        
        if (NowExercise.isPausable && !NM.isMobile)
        {
            AlertSet.SetActive(true);
        }
    }

    public void reset()
    {
        _videoSources._video.Stop();
        index = -1;
        PoseChanger();
    }
    
    
    /// <summary>
    /// 포즈 변경 함수
    /// </summary>
    public void PoseChanger()
    {
        index++;
        PoseMode(index);
        shoulderAngleDatas_Left = new List<float>();
        shoulderAngleDatas_Right = new List<float>();
    }

    public void PoseMode(int _index)
    {
        term.text = (currentSet+1).ToString();
        switch (_index)
        {
            case 0:
                Debug.Log("Pose2");
                PoseSetting(_index);
                SoundManager.instance.SEAudioPlay(15);
                SoundManager.instance.CountAudioPlay(_index);
                _videoSources.CheckIndex(_index);
                poseSequence2 = StartCoroutine(PoseSequence2());
                break;
            case -1:
                Debug.Log("Pose3");
                _videoSources.CheckIndex(_index+1);
                _videoSources.frameSetter();
                SoundManager.instance.SEAudioPlay(8);
                SoundManager.instance.BGMAudioPlay(2);
                StartCoroutine(PoseSequence());
                break;
            default:
                Debug.Log("Pose4");
                if ((index > 0 && SceneManager.GetActiveScene().name == "TestScene") || (index > 1 && !NM.isAppTutorialEnd))
                {
                    Success();
                    return;
                }
                
                if (index >= NowExercise.steps.Length)
                {
                    if (NowExercise.isReversible)
                    {
                        isReverse = !isReverse; // 리버스 값 변경
                    }

                    if ((!isReverse && NowExercise.isReversible) || !NowExercise.isReversible) //만약 리버스가 트루거나, 리버스할 필요없는 운동일 경우 세트값 ++
                    {
                        currentSet++;
                    }

                    SoundManager.instance.SEAudioPlay(16);
                    // if (!isReverse && currentSet >= NowExercise.set)
                    // {
                    //     Success();
                    //     return;
                    // }//빼야됨
                    
                    if (currentSet >= NowExercise.set && !NowExercise.isReversible)
                    { 
                        Success();
                        return;
                    }
                    
                    if (currentSet >= NowExercise.set && NowExercise.isReversible && !isReverse) //진행된 세트수와 설정된 세트 수가 같을 때 && 현재 운동에 isReversible이 체크 되어있을 때.
                    {
                        // isReverse = true;
                        // currentSet = 0;
                        // SoundManager.instance.SEAudioPlay(7);
                        Success();
                        return;
                    }

                    if (!isReverse && NowExercise.isReversible)
                    {
                        //isReverse = true;
                        SoundManager.instance.SEAudioPlay(17);
                    }
                    
                    index = 0;  // 운동 세트를 처음부터 다시 시작합니다.
                    if (NetworkManager._instance.isMobile)
                    {
                        stepMaker_ForMobile.Restart(index);
                    }
                    else
                    {
                        stepMaker.Restart(index);
                    }
                    PoseMode(index);
                    return;
                }

                PoseSetting(_index);
                SoundManager.instance.CountAudioPlay(_index);
                SoundManager.instance.SEAudioPlay(8);
                _videoSources.CheckIndex(_index);
                //timer.StartStopwatch();
                poseSequence2 = StartCoroutine(PoseSequence2());
                break;
        }
    }

    public void PoseSetting(int _index)
    {
        Debug.Log("Pose5");
        term.text = (currentSet + 1).ToString();
        
        //poseTimeText.gameObject.SetActive(true);
        if (SceneManager.GetActiveScene().name == "TestScene" || (NM.isMobile && !NM.isAppTutorialEnd))
        {
            angleRS = TutorialManager.instance.TutorialExerciseData.RSAngles[_index];
            angleLS = TutorialManager.instance.TutorialExerciseData.LSAngles[_index];
            angleRE = TutorialManager.instance.TutorialExerciseData.REAngles[_index];
            angleLE = TutorialManager.instance.TutorialExerciseData.LEAngles[_index];
            return;
        }
        
        if (NowExercise.isReversible && isReverse)
        {
            //리버스일 때
            if (NM.isNative_Activity)
            {
                angleRS = NM.todaysRandom[NM.choosedExercise].LSAngles[_index];
                angleLS = NM.todaysRandom[NM.choosedExercise].RSAngles[_index];
                angleRE = NM.todaysRandom[NM.choosedExercise].LEAngles[_index];
                angleLE = NM.todaysRandom[NM.choosedExercise].REAngles[_index];
            }
            else
            {
                angleRS = NM.exerciseDatas_all[NM.choosedExercise].LSAngles[_index];
                angleLS = NM.exerciseDatas_all[NM.choosedExercise].RSAngles[_index];
                angleRE = NM.exerciseDatas_all[NM.choosedExercise].LEAngles[_index];
                angleLE = NM.exerciseDatas_all[NM.choosedExercise].REAngles[_index];
            }
        }
        else
        {
            //기본값 세팅
            if (NM.isNative_Activity)
            {   
                angleRS = NM.todaysRandom[NM.choosedExercise].RSAngles[_index];
                angleLS = NM.todaysRandom[NM.choosedExercise].LSAngles[_index];
                angleRE = NM.todaysRandom[NM.choosedExercise].REAngles[_index];
                angleLE = NM.todaysRandom[NM.choosedExercise].LEAngles[_index];
            }
            else
            {
                angleRS = NM.exerciseDatas_all[NM.choosedExercise].RSAngles[_index];
                angleLS = NM.exerciseDatas_all[NM.choosedExercise].LSAngles[_index];
                angleRE = NM.exerciseDatas_all[NM.choosedExercise].REAngles[_index];
                angleLE = NM.exerciseDatas_all[NM.choosedExercise].LEAngles[_index];
            }
        }
        Debug.Log("Pose6");
    }

    IEnumerator PoseSequence()
    {
        Debug.Log("Pose7");
        //Recognize
        //reset();
        term.text = "SET";
        term.fontSize = 80;
        float time = 0;
        
        poseTimeText.text = time.ToString("0");
        while(true)
        {
            linefind.LineColor(0);
            markfind.PointColor(0);
            poseTimeUI.fillAmount = time / NowExercise.time;
            time += 1;
            poseTimeText.text = time.ToString("0");
            if (time > NowExercise.time)
            {
                SoundManager.instance.SEAudioPlay(8);
                poseTimeText.gameObject.SetActive(false);
                poseTimeCheckUI.SetActive(true);
                yield return twoSec;
                poseTimeCheckUI.SetActive(false);
                //stepMaker.ChangeStep(0);
                PoseChanger();
                break;
            }

            yield return oneSec;
        }

        yield return null;
    }

    IEnumerator PoseSequence2()
    {
        Debug.Log("Pose9");
        term.text = (currentSet+1).ToString();

        if (NetworkManager._instance.isMobile)
        {
            stepMaker_ForMobile.ChangeStep(index);
        }
        else
        {
            stepMaker.ChangeStep(index);
        }
        
        while (true)
        {
            if (NowExercise.isRightDisable  && isReverse) // == LeftDisable
            {
                yield return StartCoroutine(LeftDisable());
            }
            else if (NowExercise.isLeftDisable && isReverse) // ==RightDisable
            {
                yield return StartCoroutine(RightDisable());
            }
            if (NowExercise.isLeftDisable && !isReverse)
            {
                yield return StartCoroutine(LeftDisable());
            } 
            else if (NowExercise.isRightDisable && !isReverse)
            {
                yield return StartCoroutine(RightDisable());
            }
            
            if (!NowExercise.isLeftDisable && !NowExercise.isRightDisable)
            {
                yield return StartCoroutine(NotDisable());
            }

            yield return StartCoroutine(TimeChecker());

            if (NowExercise.isRightDisable && NowExercise.isLeftDisable)
            {
                SceneManager.LoadScene("Home");
                break;
            }
            yield return null;
        }
        Debug.Log("Pose10");
    }

    IEnumerator ExerciseMaxDataCollector()
    {
        float MaxAngle_L = 0;
        float MaxAngle_R = 0;
        if (NowExercise.isRightDisable && !NowExercise.isLeftDisable && isReverse) // == LeftDisable
        {
            foreach (var rightAngle in shoulderAngleDatas_Right)
            {
                if (MaxAngle_R < rightAngle)
                {
                    MaxAngle_R = rightAngle;
                }
            }

            if (shoulderAngleDatas_R.ContainsKey($"step{index} max range - right"))
            {
                if (shoulderAngleDatas_R[$"step{index} max range - right"] < MaxAngle_R)
                {
                    shoulderAngleDatas_R[$"step{index} max range - right"] = MaxAngle_R;
                }
            }
            else
            {
                shoulderAngleDatas_R.Add($"step{index} max range - right", MaxAngle_R);
            }
        }
        else if (NowExercise.isLeftDisable && !NowExercise.isRightDisable && isReverse) // ==RightDisable
        {
            foreach (var leftAngle in shoulderAngleDatas_Left)
            {
                if (MaxAngle_L < leftAngle)
                {
                    MaxAngle_L = leftAngle;
                }
            }
            if (shoulderAngleDatas_L.ContainsKey($"step{index} max range - left"))
            {
                if (shoulderAngleDatas_L[$"step{index} max range - left"] < MaxAngle_L)
                {
                    shoulderAngleDatas_L[$"step{index} max range - left"] = MaxAngle_L;
                }
            }
            else
            {
                shoulderAngleDatas_L.Add($"step{index} max range - left", MaxAngle_L);
            }
        } 
        if (NowExercise.isLeftDisable && !NowExercise.isRightDisable && !isReverse)
        {
            foreach (var rightAngle in shoulderAngleDatas_Right)
            {
                if (MaxAngle_R < rightAngle)
                {
                    MaxAngle_R = rightAngle;
                }
            }
            if (shoulderAngleDatas_R.ContainsKey($"step{index} max range - right"))
            {
                if (shoulderAngleDatas_R[$"step{index} max range - right"] < MaxAngle_R)
                {
                    shoulderAngleDatas_R[$"step{index} max range - right"] = MaxAngle_R;
                }
            }
            else
            {
                shoulderAngleDatas_R.Add($"step{index} max range - right", MaxAngle_R);
            }
        } 
        else if (NowExercise.isRightDisable && !NowExercise.isLeftDisable && !isReverse)
        {
            foreach (var leftAngle in shoulderAngleDatas_Left)
            {
                if (MaxAngle_L < leftAngle)
                {
                    MaxAngle_L = leftAngle;
                }
            }
            if (shoulderAngleDatas_L.ContainsKey($"step{index} max range - left"))
            {
                if (shoulderAngleDatas_L[$"step{index} max range - left"] < MaxAngle_L)
                {
                    shoulderAngleDatas_L[$"step{index} max range - left"] = MaxAngle_L;
                }
            }
            else
            {
                shoulderAngleDatas_L.Add($"step{index} max range - left", MaxAngle_L);
            }
        }
            
        if (!NowExercise.isLeftDisable && !NowExercise.isRightDisable)
        {
            foreach (var rightAngle in shoulderAngleDatas_Right)
            {
                if (MaxAngle_R < rightAngle)
                {
                    MaxAngle_R = rightAngle;
                }
            }
            if (shoulderAngleDatas_R.ContainsKey($"step{index} max range - right"))
            {
                if (shoulderAngleDatas_R[$"step{index} max range - right"] < MaxAngle_R)
                {
                    shoulderAngleDatas_R[$"step{index} max range - right"] = MaxAngle_R;
                }
            }
            else
            {
                shoulderAngleDatas_R.Add($"step{index} max range - right", MaxAngle_R);
            }
            
            foreach (var leftAngle in shoulderAngleDatas_Left)
            {
                if (MaxAngle_L < leftAngle)
                {
                    MaxAngle_L = leftAngle;
                }
            }
            if (shoulderAngleDatas_L.ContainsKey($"step{index} max range - left"))
            {
                if (shoulderAngleDatas_L[$"step{index} max range - left"] < MaxAngle_L)
                {
                    shoulderAngleDatas_L[$"step{index} max range - left"] = MaxAngle_L;
                }
            }
            else
            {
                shoulderAngleDatas_L.Add($"step{index} max range - left", MaxAngle_L);
            }
        }
        
        yield return null;

    }

    IEnumerator NotDisable()
    {
        StopCoroutine(LeftDisable());
        StopCoroutine(RightDisable());
        while (true)
        {
            if (angleUi != null)
            {
                angleUi.UpdateAngle("Left Shoulder", "Right Shoulder", markfind.LsAngle, markfind.RsAngle);
            }

            if (
                markfind.RsAngle >= (angleRS - 30) && markfind.RsAngle <= (angleRS + 30) &&
                markfind.LsAngle >= (angleLS - 30) && markfind.LsAngle <= (angleLS + 30) &&
                markfind.Rangle >= (angleRE - 30) && markfind.Rangle <= (angleRE + 30) &&
                markfind.Langle >= (angleLE - 30) && markfind.Langle <= (angleLE + 30)
            )
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor(0);
                markfind.PointColor(0);
                shoulderAngleDatas_Left.Add(markfind.LsAngle);
                shoulderAngleDatas_Right.Add(markfind.RsAngle);
                yield break;
            }
            else
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor(1);
                markfind.PointColor(1);
                shoulderAngleDatas_Left.Add(markfind.LsAngle);
                shoulderAngleDatas_Right.Add(markfind.RsAngle);
                yield break;
            }
        }
    }

    IEnumerator LeftDisable()
    {
        StopCoroutine(NotDisable());
        StopCoroutine(RightDisable());
        while (true)
        {
            if (angleUi != null)
            {
                angleUi.UpdateAngle("Shoulder", "Elbow", markfind.RsAngle, markfind.Rangle);
            }
            if (
                markfind.RsAngle >= (angleRS - 30) && markfind.RsAngle <= (angleRS + 30) &&
                markfind.Rangle >= (angleRE - 30) && markfind.Rangle <= (angleRE + 30) 
            )
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor_Kai(false, true, 0);
                markfind.PointColor_kai(false, true, 0);
                shoulderAngleDatas_Right.Add(markfind.RsAngle);
                yield break;
            }
            else
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor_Kai(false, true, 1);
                markfind.PointColor_kai(false, true, 1);
                shoulderAngleDatas_Right.Add(markfind.RsAngle);
                 yield break;
            }
        }
    }

    IEnumerator RightDisable()
    {
        StopCoroutine(NotDisable());
        StopCoroutine(LeftDisable());
        while (true)
        {
            if (angleUi != null)
            {
                angleUi.UpdateAngle("Shoulder", "Elbow", markfind.LsAngle, markfind.Langle);
            }

            if (
                markfind.LsAngle >= (angleLS - 30) && markfind.LsAngle <= (angleLS + 30) &&
                markfind.Langle >= (angleLE - 30) && markfind.Langle <= (angleLE + 30)
            )
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor_Kai(true, false, 0);
                markfind.PointColor_kai(true, false, 0);
                shoulderAngleDatas_Left.Add(markfind.LsAngle);
                yield break;
            }
            else
            {
                nowTime += Time.deltaTime * 3;
                poseTimeText.text = nowTime.ToString("0");
                poseTimeUIAmount();
                linefind.LineColor_Kai(true, false, 1);
                markfind.PointColor_kai(true, false, 1);
                shoulderAngleDatas_Left.Add(markfind.LsAngle);
                yield break;
            }
        }
    }

    private void poseTimeUIAmount()
    {
        if (NowExercise.repeatBefore == 0 && NowExercise.repeatAfter == 0)
        {
            poseTimeUI.fillAmount = nowTime / normalTime;
        }
        else
        {
            poseTimeUI.fillAmount = nowTime / normalTime;
        }
    }

    IEnumerator TimeChecker()
    {
        if (NowExercise.repeatBefore == 0 && NowExercise.repeatAfter == 0 && NowExercise.isRepeatable)
        {
            if (nowTime >= normalTime)
            {
                yield return StartCoroutine(ExerciseMaxDataCollector());
                yield return StartCoroutine(TimeChecker_Same());
            }
        }
        // else if(index >= NowExercise.repeatBefore && index <= NowExercise.repeatAfter && NowExercise.isRepeatable)
        // {
        //     if (nowTime > NowExercise.time)
        //     {
        //         yield return StartCoroutine(TimeChecker_Same());
        //     }
        // }
        else
        {
            if (nowTime >= normalTime)
            {
                yield return StartCoroutine(ExerciseMaxDataCollector());
                yield return StartCoroutine(TimeChecker_Same());
            }
        }
    }

    IEnumerator TimeChecker_Same()
    {
        SoundManager.instance.SEAudioPlay(8);
        poseTimeText.text = "0";
        poseTimeCheckUI.SetActive(true);
        _videoSources.RawImage.gameObject.SetActive(false);
        StopCoroutine(poseSequence2);
        yield return twoSec;
        _videoSources.RawImage.gameObject.SetActive(true);
        nowTime = 0f;
        poseTimeCheckUI.SetActive(false);
        PoseChanger();
        yield break;
    }
    

    public void Success() //성공
    {
        if (SceneManager.GetActiveScene().name == "TestScene" || (NM.isMobile && !NM.isAppTutorialEnd))
        {
            NetworkManager._instance.isAppTutorialEnd = true;
            BtnRestart.SetActive(true);
            NetworkManager._instance._firebaseLoader.TutorialInput();
        }

        if (SceneManager.GetActiveScene().name == "ExerciseScene" ||
            (SceneManager.GetActiveScene().name == "#4 exercise" && NM.isMobile))
        {
            StartCoroutine(Result());
        }
    }

    IEnumerator Result()
    {
        timer.StopStopwatch();

        if (NM.isMobile)
        {
            foreach (var ft in exerciseFalseThings)
            {
                ft.SetActive(false);
            }
        }

        if (!NetworkManager._instance.isNative_Activity)
        {
            yield return StartCoroutine(NetworkManager._instance._firebaseLoader
                .ExerciseClear(NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise], 
                    shoulderAngleDatas_L, shoulderAngleDatas_R));
            BtnRestart.SetActive(true);
            yield break;
        }

        if (NetworkManager._instance.choosedExercise == NetworkManager._instance.todaysRandom.Count-1)
        {
            yield return StartCoroutine(NetworkManager._instance._firebaseLoader
                .ExerciseClear(NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise], 
                    shoulderAngleDatas_L, shoulderAngleDatas_R));

            
            
            _operationSystem.isEnd = true;
            _operationSystem.gameObject.SetActive(true);
            _operationSystem.OperationPopup.SetActive(true);
            foreach (var thing in _operationSystem.falseThings)
            {
                thing.SetActive(false);
            }
            
            yield break;
        }
        
        yield return StartCoroutine(NetworkManager._instance._firebaseLoader
            .ExerciseClear(NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise], 
                shoulderAngleDatas_L, shoulderAngleDatas_R));
        
        BtnRestart.SetActive(true);
    }

    public void PauseExercise()
    {
        if (poseSequence2 != null)
        {
            StopCoroutine(poseSequence2);
            isPause = true;
        }
    }

    public void ResumeExercise()
    {
        if (NetworkManager._instance.isMobile && TutorialManager.instance != null)
        {
            //if (TutorialManager.instance.gameObject.activeInHierarchy) return;
            if (ExerciseManager.instance.OperationUI.gameObject.activeInHierarchy) return;
        }

        if (isPause)
        {
            isPause = false;
            if (poseSequence2 != null)
            {
                PoseMode(index);
            }
        }
    }
}
