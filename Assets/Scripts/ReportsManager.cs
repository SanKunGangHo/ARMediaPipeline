using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportsManager : MonoBehaviour
{
    public QuestionManager QM;
    private List<QuestionManager.QuizData> _quizDatas;
    private Dictionary<int, int> playerData_Quiz;
    private List<Dictionary<int, int>> playerData_Brief;
    [SerializeField] private TMP_Text ID;

    public GameObject earlyExaminationObject;
    public GameObject BriefSurveyObject;

    public Transform Content;
    public GameObject ReportsPrefab;
    public Sprite img_Start, img_Done;

    private void Awake()
    {
        NetworkManager._instance.rm = this;
    }

    private void OnEnable()
    {
        foreach (Transform v in Content)
        {
            Destroy(v.gameObject);
        }
        StartCoroutine(NetworkManager._instance._firebaseLoader.ExamOutput());
        
        if (!NetworkManager._instance.isOnBoardingEnd) return;
        
        StartCoroutine(NetworkManager._instance._firebaseLoader.BriefOutput());

        ID.text = NetworkManager._instance._playerData.PatientID;
    }

    public void MakeExamDone(DateTime dateTime)
    {
        Transform examination = Instantiate(ReportsPrefab, Content).transform;
        examination.GetChild(0).GetComponent<TMP_Text>().text = "On-Boarding Survey";
        examination.GetChild(1).GetComponent<TMP_Text>().text =
            $"ID {NetworkManager._instance._playerData.PatientID}  {dateTime:yyyy-MM-dd hh:mm}";
        examination.GetChild(2).GetComponent<Image>().sprite = img_Done;
        examination.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SoundManager.instance.SEAudioPlay(1); });
    }

    public void MakeExamStart()
    {
        Transform examination = Instantiate(ReportsPrefab, Content).transform;
        examination.GetChild(0).GetComponent<TMP_Text>().text = "On-Boarding Survey";
        examination.GetChild(1).GetComponent<TMP_Text>().text =
            $"ID {NetworkManager._instance._playerData.PatientID}  ";
        examination.GetChild(2).GetComponent<Image>().sprite = img_Start;
       // examination.GetChild(2).GetComponent<Button>().onClick.AddListener(BTN_ExaminationStart);
       // examination.GetChild(2).GetComponent<Button>().onClick.AddListener(()=>earlyExaminationObject.GetComponent<ExaminationManager>().QM.OnChange_Early(0));
       // examination.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SoundManager.instance.SEAudioPlay(0); });
        //Start 버튼에 들어갈 내용
    }

    public void MakeBriefDone(IDictionary time)//Brief survey 밑의 시간을 가져오던 매개변수
    {
        foreach (string briefTime in time.Keys)//중증도 평가 수만큼 증가하던 로직 
        {
            Transform brief = Instantiate(ReportsPrefab, Content).transform;
            DateTime dateTime = DateTime.Parse(briefTime);
            brief.GetChild(0).GetComponent<TMP_Text>().text = "Brief Survey";
            brief.GetChild(1).GetComponent<TMP_Text>().text =
                $"ID {NetworkManager._instance._playerData.PatientID}  {dateTime:yyyy-MM-dd hh:mm}";
            brief.GetChild(2).GetComponent<Image>().sprite = img_Done;
            brief.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SoundManager.instance.SEAudioPlay(1); });
        }
    }

    public void MakeBriefStart()
    {
        Transform brief = Instantiate(ReportsPrefab, Content).transform;
        brief.GetChild(0).GetComponent<TMP_Text>().text = "Brief Survey";
        brief.GetChild(1).GetComponent<TMP_Text>().text =
            $"ID {NetworkManager._instance._playerData.PatientID}  ";
        brief.GetChild(2).GetComponent<Image>().sprite = img_Start;
        brief.GetChild(2).GetComponent<Button>().onClick.AddListener(BTN_BriefStart);
        brief.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { SoundManager.instance.SEAudioPlay(0); });
    }

    private void BTN_ExaminationStart()
    {
        //earlyExaminationObject.SetActive(true);
        gameObject.SetActive(false);
    }

    private void BTN_BriefStart()
    {
        BriefSurveyObject.SetActive(true);
        if(NetworkManager._instance.isMobile){            
            QM.OnChange_Early(0);
            QM.EndCut.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
