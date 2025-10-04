using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public static class CoroutineExtensions
{
    public static void TryStop(this Coroutine coroutine, MonoBehaviour owner)
    {
        if (coroutine != null)
        {
            owner.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    public static Coroutine TryStart(this Coroutine coroutine, MonoBehaviour owner, IEnumerator routine)
    {
        coroutine.TryStop(owner);
        return owner.StartCoroutine(routine);
    }
}

public class OperationSystem : MonoBehaviour
{
    public static OperationSystem instance = null;
    public OperationData operationData;

    private const string PointingMentionFormat = "Raise the {0} arm {1}";
    [SerializeField] MpMarkfind markfind;
    [SerializeField] MpLinefind linefind;
    [SerializeField] private GuideBlink PainAlert;
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject angleRange;
    float angleRS, angleRE, angleLS, angleLE;

    public bool isEnd;

    [Tooltip("제한시간")]
    public float time = 5f;

    [SerializeField] private int index;
    //private bool isClear;
    private Queue<float> previousDatas = new Queue<float>(20);
    private float sum;
    public float nowData, maxData;

    public TMP_Text pointingMention;

    public VideoSources _videoSources;
    
    private Coroutine anglechanger, indexPlus;
    [SerializeField] private TMP_Text _angleText;

    public GameObject OperationPopup;
    public GameObject resultPopup;
    public GameObject[] falseThings;

    public GameObject completeBoard;
    public GameObject operationGauge;

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
    }
    

    public void reset()
    {
        SoundManager.instance.SEAudioPlay(15);
        //angleChanger();
        _videoSources.ChangeClip_Operation(index);
        _videoSources._video.Play();
        StartCoroutine(angleChanger());
        StartCoroutine(AngleFiller());
        SoundManager.instance.CountAudioPlay(index);
        SoundManager.instance.SEAudioPlay(17);
    }

    void Next()
    {
        // 확장 메서드를 사용해서 코루틴 중지
        anglechanger.TryStop(this);
        
        previousDatas.Clear();
        nowData = 0f;
        maxData = 0f;
        
        if (indexPlus == null)
        {
            indexPlus = StartCoroutine(IndexPlus());
        }
    }

    IEnumerator angleChanger()
    {
        //SoundManager.instance.SEAudioPlay(15);
        Coroutine countdownCoroutine = null;
        //yield return new WaitForSeconds(2f);
        while (true)
        {
            //previousDatas.Clear();
            //TODO: 팔 내리는 UI 추가?
            switch (index)
            {
                case 0:
                    //측면
                    // _videoSources.ChangeClip_Operation(index);
                    // _videoSources._video.Play();
                    pointingMention.text = string.Format(PointingMentionFormat, "Left", "Side");
                    _angleText.text = $"{markfind.LsAngle:000.000}";
                    nowData = markfind.LsAngle;
                    AngleChecker(ref countdownCoroutine, "LeftSide");
                    break;
                case 1:
                    //정면
                    _videoSources.ChangeClip_Operation(index);
                    _videoSources._video.Play();
                    pointingMention.text = string.Format(PointingMentionFormat, "Right", "Side");
                    _angleText.text = $"{markfind.RsAngle:000.000}";
                    nowData = markfind.RsAngle;
                    AngleChecker(ref countdownCoroutine, "RightSide");
                    break;
                case 2:
                    //측면
                    _videoSources.ChangeClip_Operation(index);
                    _videoSources._video.Play();
                    pointingMention.text = string.Format(PointingMentionFormat, "Left", "Forward");
                    _angleText.text = $"{markfind.LsAngle:000.000}";
                    nowData = markfind.LsAngle;
                    AngleChecker(ref countdownCoroutine, "LeftForward");
                    break;
                case 3:
                    //정면
                    _videoSources.ChangeClip_Operation(index);
                    _videoSources._video.Play();
                    pointingMention.text = string.Format(PointingMentionFormat, "Right", "Forward");
                    _angleText.text = $"{markfind.RsAngle:000.000}";
                    nowData = markfind.RsAngle;
                    AngleChecker(ref countdownCoroutine, "RightForward");
                    break;
                case 4:
                    if (isEnd)
                    {
                        resultPopup.SetActive(true);
                        yield return StartCoroutine(CollectAngleDataBefore()); //사후 가동률검사

                        if (!NetworkManager._instance.isMobile)
                        {
                            yield return StartCoroutine(NetworkManager._instance._firebaseLoader.AngleResult(
                                "Angles_Max_Before",
                                "Angles_Ave_Before"));
                        }

                        if (NetworkManager._instance.preMeasurement || NetworkManager._instance.postMeasurement)
                        {
                            //PoseManager.instance.BtnRestart.SetActive(true);
                            this.gameObject.SetActive(false);
                        }

                        int lastIndex = NetworkManager._instance.todaysRandom.Count - 1;
                        
                        if (NetworkManager._instance.choosedExercise == 0) //시작할 때
                        {
                            NetworkManager._instance.preMeasurement = true;
                        }
                        else if (NetworkManager._instance.choosedExercise == lastIndex) //끝날 때
                        {
                            NetworkManager._instance.postMeasurement = true;
                        }

                        this.gameObject.SetActive(false);
                        break;
                    }
                    
                    if (NetworkManager._instance.preMeasurement || NetworkManager._instance.choosedExercise == 1000)
                    {
                        yield return StartCoroutine(CollectAngleDataBefore());
                        PoseManager.instance.BtnRestart.SetActive(true);
                        this.gameObject.SetActive(false);
                        yield break;
                    }
                    
                    if (NetworkManager._instance.postMeasurement || NetworkManager._instance.choosedExercise == 1001)
                    {
                        yield return StartCoroutine(CollectAngleDataBefore());
                        //PoseManager.instance.BtnRestart.SetActive(true);
                        this.gameObject.SetActive(false);
                        yield break;
                    }
                    
                    yield return StartCoroutine(CollectAngleDataBefore());
                    Initialize();
                    _angleText.text = "000.000";
                    ExerciseManager.instance.StartUI.SetActive(true);
                    previousDatas.Clear();
                    anglechanger.TryStop(this);
                    this.gameObject.SetActive(false);
                    break;
            }

            yield return null;
        }
    }
    
    IEnumerator CollectAngleDataBefore()
    {
        yield return StartCoroutine(
            NetworkManager._instance._firebaseLoader.AngleCollector_Before(
                NetworkManager._instance._playerData.operationResult_Max_Before.ToArray(),
                NetworkManager._instance._playerData.operationResult_Ave_Before.ToArray()
            )
        );
    }

    IEnumerator AngleFiller()
    {
        while (true)
        {
            fillImage.fillAmount = maxData / 180;

            angleRange.transform.localEulerAngles = new Vector3(0, 0, -nowData + 90);
            yield return null;
        }
    }

    void AngleChecker(ref Coroutine countdownCoroutine, string key)
    {
        //if(!operationData.operationPos.TryGetValue(key, out Queue<float> value)) return;
        
        if (nowData > maxData)
        {
            maxData = nowData;
            // previousDatas.Clear();
            // sum = 0.0f;
        }

        previousDatas.Enqueue(nowData);
        sum += nowData;
        float average = sum / previousDatas.Count;
        
            linefind.LineColor_OperationOnly(index, 0);
            markfind.PointColor_OperationOnly(index, 0);
            if (countdownCoroutine == null)
            {
                countdownCoroutine = StartCoroutine(TriggerActionAfterDelay(time, key, average));
            }
    }

    IEnumerator TriggerActionAfterDelay(float delayInSeconds, string key, float average)
    {
        float rime = 0;
        while (rime < delayInSeconds)
        {
            rime += Time.deltaTime;
            if (nowData < (maxData - 30) && !NetworkManager._instance.isMobile)
            {
                PainAlert.gameObject.SetActive(true);
            }

            yield return null;
        }
        completeBoard.SetActive(true);
        operationGauge.SetActive(false);
        yield return new WaitForSeconds(2);
        completeBoard.SetActive(false);
        operationGauge.SetActive(true);
        
        SoundManager.instance.SEAudioPlay(8);
        
        if (anglechanger != null)
        {
            // 이전 코루틴 중지
            StopCoroutine(anglechanger);
            anglechanger = null;
        }
        
        PlayerData nowPlayer = NetworkManager._instance._playerData;

        if (NetworkManager._instance.postMeasurement)
        {
            isEnd = NetworkManager._instance.postMeasurement;
        }
        
        nowPlayer.operationResult_Max_Before.Add(maxData); //LS, RS, LF, RF
        nowPlayer.operationResult_Ave_Before.Add(average);
        
        maxData = 0;
        sum = 0;

        // Here!
        Next();

        yield break; 
    }

    IEnumerator IndexPlus()
    {
        index++;
        nowData = 0f;
        maxData = 0f;
        sum = 0f;
        previousDatas.Clear();
        yield return new WaitForSeconds(1f);
        
        SoundManager.instance.CountAudioPlay(index);
        SoundManager.instance.SEAudioPlay(17);
        anglechanger = anglechanger.TryStart(this, angleChanger());
        
        indexPlus = null;
    }

    public void StopOperation()
    {
        if (this.gameObject.activeInHierarchy)
        {
            StopCoroutine(angleChanger());
            StopCoroutine(AngleFiller());
        }
    }
    
    public void TryAgainOperation()
    {
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(angleChanger());
            StartCoroutine(AngleFiller());
        }
    }

    public void EndCheckOperation()
    {
        isEnd = true;
    }
    
    public void Initialize()
    {
        index = 0;
        //isClear = false;
        nowData = 0f;
        maxData = 0f;
        previousDatas.Clear();
    
        if (anglechanger != null)
        {
            StopCoroutine(anglechanger);
            anglechanger = null;
        }
    
        if (indexPlus != null)
        {
            StopCoroutine(indexPlus);
            indexPlus = null;
        }
        reset();
        
    }
}
