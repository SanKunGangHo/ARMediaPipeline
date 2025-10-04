using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Network;
using TMPro;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class QuestionManager : MonoBehaviour
{
    public TMP_Text surveyTitleTxt;
    public GameObject _answerToggle;
    public VerticalLayoutGroup answerGroup;
    public Questions[] quizArray;

    private Dictionary<string, int> _categoryDictionary = new Dictionary<string, int>();

    public Button nextBTN;

    public ExaminationManager EM;

    public TMP_Text QuizIndex, QuizDetail;

    public GameObject EndCut;
    public Button EndCutButton;
    
    public Button previousBTN;

    public int shoulder, rotator, warning;

    public Questions nowQuestion;

    public GameObject painIntensity, painDuration;
    
    public int painIntensityValue, painDurationValue;

    
    [System.Serializable]
    public class QuizData
    {
        public string id;
        public List<int> quiznum;
        public List<int> childIndex;
    }

    // public void AddQuizDataToJson(int quiznum, int childIndex, string filePath)
    // {
    //     // 기존 JSON 데이터 불러오기
    //     var oldQuizDataList = LoadQuizDataFromJson(filePath);
    //     if(oldQuizDataList == null) oldQuizDataList = new List<QuizData>();
    //
    //     // 새 QuizData 객체 생성하여 리스트에 추가
    //     QuizData newQuizData = new QuizData();
    //     //TODO: foreach
    //     // newQuizData.quiznum = quiznum;
    //     // newQuizData.childIndex = childIndex;
    //     oldQuizDataList.Add(newQuizData);
    //
    //     // 리스트를 JSON 문자열로 변환
    //     string jsonQuizData = JsonUtility.ToJson(oldQuizDataList);
    //
    //     // JSON 문자열을 파일에 쓰기
    //     System.IO.File.WriteAllText(filePath, jsonQuizData);
    // }
    
    // public List<QuizData> LoadQuizDataFromJson(string filePath)
    // {
    //     if (!System.IO.File.Exists(filePath))
    //     {
    //         // 파일이 존재하지 않는 경우 null 반환
    //         return null;
    //     }
    //
    //     // 파일 내용을 문자열로 읽어옴
    //     string jsonQuizData = System.IO.File.ReadAllText(filePath);
    //
    //     List<QuizData> quizDataList = JsonUtility.FromJson<List<QuizData>>(jsonQuizData);
    //
    //     return quizDataList;
    // }
    
    private void Start()
    {
        //NetworkManager._instance._firebaseLoader.ExamCheck(NetworkManager._instance._playerData.PatientID);
            
        _categoryDictionary.Add("충돌증후군", 0);
        _categoryDictionary.Add("유착성관절", 0);
        _categoryDictionary.Add("주의", 0);
        _categoryDictionary.Add("반려", 0);
        surveyTitleTxt.text = quizArray[0].SurveyTitle;
    }

    /// <summary>
    /// 문제와 답을 바꾸는 메서드
    /// </summary>
    public void OnChange_Early(int quiznum)
    {
        if (quiznum <= 0)
        {
            previousBTN.interactable = false;
        }
        else
        {
            previousBTN.interactable = true;
        }
        EM.ValueChanger(quiznum+1);
        nowQuestion = quizArray[quiznum];
        EM.DisplayDiscomportOnly(nowQuestion.isComport);
        QuizIndex.text = nowQuestion.QuestNum;
        QuizDetail.text = nowQuestion.QuestTitle_EN;
        if(painDuration != null && painIntensity != null)
        {
            painDuration.SetActive(false);
            painIntensity.SetActive(false);
        }
        if(nowQuestion.SurveyTitle == "Brief Survey" && nowQuestion.QuestNum == "02")
        {
            painDuration.SetActive(true);
            Toggle[] toggles = painDuration.GetComponentsInChildren<Toggle>();
            foreach(var t in toggles)
            {
                //t.group = answerGroup.GetComponent<ToggleGroup>();
                t.onValueChanged.AddListener(delegate
                {
                    OnToggleChanger( t.isOn, quiznum, t.gameObject.transform.GetSiblingIndex());
                });
            }

            foreach(Transform a in answerGroup.transform)
            {
                a.gameObject.SetActive(false);
            }
        

        }
        else if(nowQuestion.SurveyTitle == "Brief Survey" && nowQuestion.QuestNum == "03")
        {
            painIntensity.SetActive(true);
            Toggle[] toggles = painIntensity.GetComponentsInChildren<Toggle>();
            foreach(var t in toggles)
            {
                //t.group = answerGroup.GetComponent<ToggleGroup>();
                t.onValueChanged.AddListener(delegate
                {
                    OnToggleChanger( t.isOn, quiznum, t.gameObject.transform.GetSiblingIndex());
                });
            }
            
            foreach(Transform a in answerGroup.transform)
            {
                a.gameObject.SetActive(false);
            }

        }
        else
        {
            
            foreach(Transform a in answerGroup.transform)
            {
                a.gameObject.SetActive(true);
            }
            for (int i = 0; i < nowQuestion.QuestAnswers_EN.Length; i++)
            {
                GameObject answerToggle;
                if (i >= answerGroup.transform.childCount)
                {
                    answerToggle = Instantiate(_answerToggle, answerGroup.transform);
                }
                else
                {
                    answerToggle = answerGroup.transform.GetChild(i).gameObject;
                    answerToggle.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
                }
                answerToggle.transform.GetChild(1).GetComponent<TMP_Text>().text = nowQuestion.QuestAnswers_EN[i];
                answerToggle.GetComponent<Toggle>().group = answerGroup.GetComponent<ToggleGroup>();
                AnswerData answerData = answerToggle.GetComponent<AnswerData>();
                answerToggle.GetComponent<Toggle>().onValueChanged.AddListener(delegate
                {
                    OnToggleChanger( answerToggle.GetComponent<Toggle>().isOn, quiznum, answerToggle.gameObject.transform.GetSiblingIndex());
                });
/*                 answerData.Filter = nowQuestion.Question_Filter[i];
                answerData.Filter_Point = nowQuestion.Question_Filter_Point[i]; */
            }
        }

       for (int j = answerGroup.transform.childCount - 1; j >= nowQuestion.QuestAnswers_EN.Length; j--)
        {
            Destroy(answerGroup.transform.GetChild(j).gameObject);
        } 
    }
    
    // public void OnChange_Brief(int quiznum, int _value)
    // {
    //     for (int i = 0; i < quizArray[quiznum].QuestAnswers_EN.Length; i++)
    //     {
    //         GameObject answerToggle = Instantiate(_answerToggle, _answerGroup.transform);
    //         answerToggle.transform.GetChild(1).GetComponent<TMP_Text>().text = quizArray[quiznum].QuestAnswers_EN[i];
    //         answerToggle.GetComponent<AnswerData>().enabled = false;//데이터 없어도 되니까.
    //     }
    
    
    public void CheckOnBoarding()
    {
        NetworkManager._instance.isOnBoardingEnd = true;
    }
    
    //public void OnToggleChanging()

    public void OnToggleChanger(bool isOn, int quiznum, int childindex)
    {
        nextBTN.onClick.RemoveAllListeners();
        if (isOn)
        {
            Debug.Log(quiznum + " " + childindex);
            nextBTN.interactable = true;
            nextBTN.onClick.AddListener(delegate { SoundManager.instance.SEAudioPlay(0); });
            nextBTN.onClick.AddListener(delegate { changeNextBtn(quiznum, childindex); });
        }
    }

    public void changeNextBtn(int quiznum, int childIndex)
    {
        shoulder = _categoryDictionary["충돌증후군"];
        rotator = _categoryDictionary["유착성관절"];
        warning = _categoryDictionary["주의"];
        
        foreach (Transform child in answerGroup.transform)
        {
            child.GetComponent<Toggle>().isOn = false;
        }
                

        if(NetworkManager._instance._playerData.ExaminationAnswerList.ContainsKey(nowQuestion.QuestNum))
        {
            NetworkManager._instance._playerData.ExaminationAnswerList[nowQuestion.QuestNum] = childIndex;
            if(nowQuestion.SurveyTitle == "On-Boarding Survey")
            {
                if(NetworkManager._instance._playerData.ExaminationAnswerList.ContainsKey("04-1") && NetworkManager._instance._playerData.ExaminationAnswerList["04"] != 0 )
                {
                    NetworkManager._instance._playerData.ExaminationAnswerList.Remove("04-1");
                }
            }
        
        }
        else NetworkManager._instance._playerData.ExaminationAnswerList.Add(nowQuestion.QuestNum, childIndex);

        var preQuestion = nowQuestion;

        NetworkManager._instance._playerData.ExamTime = DateTime.Now;

        for (int i = 0; i < quizArray.Length; i++)
        { 
            if(nowQuestion.nextQuestion == quizArray[i])
            {
                if(nowQuestion.SurveyTitle == "On-Boarding Survey")
                {

                    if(nowQuestion.QuestNum == "04" && childIndex == 0)
                    {
                        OnChange_Early(i-1);
                        break;
                    }
                    else if (nowQuestion.QuestNum == "10")
                    {
                        if(childIndex != 0)
                        {
                            SoundManager.instance.SEAudioPlay(11);
                            EndCut.SetActive(true);
                            break;   
                        }
                        else
                        {           
                            OnChange_Early(i);
                            break;;       
                        }     
                    }
                    else if (nowQuestion.QuestNum == "10-1")
                    {
                        if(childIndex == 1)
                        {
                            OnChange_Early(i+1);
                            break;;    
                        }
                        else
                        {           
                            OnChange_Early(i);
                            break;;       
                        }     

                    }
                    else if (nowQuestion.QuestNum == "10-2")
                    {
                        if(childIndex == 2 || childIndex == 3)
                        {
                            SoundManager.instance.SEAudioPlay(11);
                            EndCut.SetActive(true);
                            break;
                        }
                        else
                        {           
                            OnChange_Early(i);
                            break;;       
                        }     

                    }
                    else if (nowQuestion.QuestNum == "10-3")
                    {
                        Debug.Log("10-3");
                        SoundManager.instance.SEAudioPlay(11);
                        EndCut.SetActive(true);
                        break;
                    }
                    else
                    {           
                        OnChange_Early(i);
                        break;;       
                    }        

                }

                else
                {        
                    OnChange_Early(i);
                    break;;               
                }

            }
            else
            {
                if(nowQuestion.SurveyTitle == "On-Boarding Survey")
                {
                    if (nowQuestion.QuestNum == "10-3")
                    {
                        SoundManager.instance.SEAudioPlay(11);
                        EndCut.SetActive(true);
                        break;
                    }
                }
                else
                {
                    if (nowQuestion.QuestNum == "04")
                    {
                        EndCut.SetActive(true);
                        break;
                    }
                }
            }
        }
        if(preQuestion.nextQuestion == null) return; 
        if (NetworkManager._instance._playerData.ExaminationAnswerList.ContainsKey(preQuestion.nextQuestion.QuestNum))
        {  
             if(nowQuestion.SurveyTitle == "Brief Survey" && preQuestion.nextQuestion.QuestNum == "03")  return;
             if(nowQuestion.SurveyTitle == "Brief Survey" && preQuestion.nextQuestion.QuestNum == "02")  return;

            if (preQuestion.QuestNum == "04" && childIndex == 0)
            {
                nextBTN.interactable = false;
                
            }
            else{        
                nextBTN.interactable = true;
                int answer = NetworkManager._instance._playerData.ExaminationAnswerList[preQuestion.nextQuestion.QuestNum];
                answerGroup.transform.GetChild(answer).GetComponent<Toggle>().isOn = true;
            }
        }
        else
        {
            nextBTN.interactable = false;
        }

        /* if (quiznum == 3)
        {
            switch (childIndex)
            {
                case 0:
                    OnChange_Early(quiznum+1);
                    break;
                case 1:
                    OnChange_Early(quiznum+2);
                    break;
                default:
                    OnChange_Early(quiznum+2);
                    break;
            }
        } 
        else if(quiznum == 10) //11
        {
            switch (childIndex)
            {
                case 0:
                    OnChange_Early(quiznum+1);
                    break;
                default:
                    SoundManager.instance.SEAudioPlay(11);
                    EndCut.SetActive(true);
                    break;
            }
        } 
        else if (quiznum == 11) //12
        {
            switch (childIndex)
            {
                case 0:
                    OnChange_Early(quiznum+1);
                    break;
                case 1:
                    OnChange_Early(quiznum+2);
                    break;
                default:
                    OnChange_Early(quiznum+1);
                    break;
                    //주의 1점
            }
        }
        else if (quiznum == 12) //13
        {
            switch (childIndex)
            {
                default:
                    SoundManager.instance.SEAudioPlay(11);
                    EndCut.SetActive(true);
                    break;
            }
        }
        else if (quiznum == 13) //14
        {
            switch (childIndex)
            {
                default:
                    SoundManager.instance.SEAudioPlay(11);
                    EndCut.SetActive(true);
                    break;
            }
        }
        else
        {
            OnChange_Early(quiznum + 1);
        }
 */
        if(nowQuestion.SurveyTitle == "On-Boarding Survey")
        {
            string quizCase = quizArray[quiznum].Question_Filter[childIndex];
            
            switch (quizCase)
            {
                case "충돌증후군":
                    _categoryDictionary[quizCase] += quizArray[quiznum].Question_Filter_Point[childIndex];
                    break;
                case "유착성관절":
                    _categoryDictionary[quizCase] += quizArray[quiznum].Question_Filter_Point[childIndex];
                    break;
                case "주의":
                    _categoryDictionary[quizCase] += quizArray[quiznum].Question_Filter_Point[childIndex];
                    break;
                case "반려":
                    _categoryDictionary[quizCase] += quizArray[quiznum].Question_Filter_Point[childIndex];
                    break;
                default:
                    break;
            } 
        }

        //TODO : 누른 토글에 맞는 답변, 필터, 필터 수치를 서버로
        // switch (quiznum)
        // {
        //     case 4:
        //         if (_answerGroup.transform.GetChild(0).GetComponent<AnswerData>().Filter == "4-1")
        //         {
        //             nextBTN.onClick.RemoveAllListeners();
        //             nextBTN.onClick.AddListener(() => EM.ValueChanger(5));
        //         }
        //         break;
        // }
    }
 
    public void CategorySet()
    {
        if(nowQuestion.SurveyTitle == "On-Boarding Survey")
        {
            if (_categoryDictionary["반려"] != 0)
            {
                //NetworkManager._instance._playerData.category = "Pending";
                return;
            }

            if (_categoryDictionary["충돌증후군"] > _categoryDictionary["유착성관절"])
            {
                NetworkManager._instance._playerData.category = "Shoulder Impingement Syndrome";
            }
            else if (_categoryDictionary["유착성관절"] > _categoryDictionary["충돌증후군"])
            {
                NetworkManager._instance._playerData.category = "Adhesive Capsulitis";
            }
            else if (_categoryDictionary["충돌증후군"] == _categoryDictionary["유착성관절"])
            {
                //TODO: Brief 버튼 안보이게 하기
                NetworkManager._instance._playerData.category = "Common";
            }
            else if (_categoryDictionary["주의"] >= 7)
            {
                //TODO: 전문가 상담 제공 팝업
                //TODO: Brief 버튼 안보이게 하기
                NetworkManager._instance._playerData.category = "Common";
            }
            else
            {
                //TODO: Brief 버튼 안보이게 하기
                NetworkManager._instance._playerData.category = "Common";
            }
            NetworkManager._instance.CategorySetter(NetworkManager._instance._playerData.category);
        }

        NetworkManager._instance.ExaminationData(nowQuestion.SurveyTitle);
    }

    public void PreviousBTN()
    {
        string preQuestTitle = "";
        for (int i = 0; i < quizArray.Length; i++)
        {
            if(nowQuestion.preQuestion == quizArray[i])
            {
                if(nowQuestion.QuestNum == "05" && nowQuestion.SurveyTitle == "On-Boarding Survey")
                {
                    if(NetworkManager._instance._playerData.ExaminationAnswerList.ContainsKey("04-1"))
                    {        
                        preQuestTitle = "04-1";
                        OnChange_Early(i+1);
                        break;
                    }
                    else
                    {
                        preQuestTitle = nowQuestion.preQuestion.QuestNum;
                        OnChange_Early(i);
                        break;
                    }
                }
                else
                {   
                    preQuestTitle = nowQuestion.preQuestion.QuestNum;
                    OnChange_Early(i);
                    break;
                }

            }
        }
        if(nowQuestion.SurveyTitle == "Brief Survey" && preQuestTitle == "03")  return;
        if(nowQuestion.SurveyTitle == "Brief Survey" && preQuestTitle == "02")  return;
        int answer = NetworkManager._instance._playerData.ExaminationAnswerList[preQuestTitle];
        answerGroup.transform.GetChild(answer).GetComponent<Toggle>().isOn = true;
        
    }




    public void BriefSurveyTogglesIntensity(int _index)
    {
        painIntensityValue = _index;
    }

    public void BriefSurveyTogglesDuration(int _index)
    {
        painDurationValue = _index;
    }
    
}
