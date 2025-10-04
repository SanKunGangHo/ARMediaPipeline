using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BriefSurvey : MonoBehaviour
{
    
    #region 1/2

    [Header("1/2")] 
    public ToggleGroup firstQuestion;
    public ToggleGroup secondQuestion;

    public Button NextBTN;

    [SerializeField] private bool[] isAnswers;

    private int[] answers;

    public Questions[] questions;
    Dictionary<string, int> BriefAnswers = new Dictionary<string, int>();

    public GameObject[] pages;

    private void OnEnable()
    {
        isAnswers = new bool[4];
        answers = new int[4];
        isAnswers[3] = true;

        // firstQuestion.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
        // secondQuestion.transform.GetChild(2).GetComponent<Toggle>().isOn = true;
        // thirdQuestion.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
        // forthQuestion.transform.GetChild(3).GetComponent<Toggle>().isOn = true;
        BriefAnswers = new Dictionary<string, int>();
    }

    public void OnToggleChanged_first(int num)
    {
        if(BriefAnswers.ContainsKey(questions[0].QuestNum))BriefAnswers[questions[0].QuestNum] = num;
        else
            BriefAnswers.Add(questions[0].QuestNum, num);
        if (firstQuestion.AnyTogglesOn())
        {
            isAnswers[0] = true;
        }
        else
        {
            isAnswers[0] = false;
        }
        
        NextInteractable();
    }
    
    public void OnToggleChanged_second(int num)
    {
        if(BriefAnswers.ContainsKey(questions[1].QuestNum))BriefAnswers[questions[1].QuestNum] = num;
        else
            BriefAnswers.Add(questions[1].QuestNum, num);
        if (secondQuestion.AnyTogglesOn())
        {
            isAnswers[1] = true;
        }
        else
        {
            isAnswers[1] = false;
        }
        
        NextInteractable();
    }

    private void NextInteractable()
    {
        if (isAnswers[0] && isAnswers[1])
        {
            NextBTN.interactable = true;
        }
        else
        {
            NextBTN.interactable = false;
        }
    }

    #endregion

    [Space(30f)]
    
    #region 2/2

    [Header("2/2")] 
    public ToggleGroup thirdQuestion;
    public ToggleGroup forthQuestion;

    public Button DoneBTN;
    
    public void OnToggleChanged_Third(int num)
    {
        if(BriefAnswers.ContainsKey(questions[2].QuestNum))BriefAnswers[questions[2].QuestNum] = num;
        else
            BriefAnswers.Add(questions[2].QuestNum, num);
        if (thirdQuestion.AnyTogglesOn())
        {
            isAnswers[2] = true;
        }
        else
        {
            isAnswers[2] = false;
        }
        DoneInteractable();
    }
    
    public void OnToggleChanged_Forth(int num)
    {
        if(BriefAnswers.ContainsKey(questions[3].QuestNum))BriefAnswers[questions[3].QuestNum] = num;
        else
            BriefAnswers.Add(questions[3].QuestNum, num);
        if (forthQuestion.AnyTogglesOn())
        {
            isAnswers[3] = true;
        }
        else
        {
            isAnswers[3] = false;
        }
        DoneInteractable();
    }

    private void DoneInteractable()
    {
        DoneBTN.onClick.RemoveListener(BTN_Done);
        if (isAnswers[0] && isAnswers[1] && isAnswers[2] && isAnswers[3])
        {
            DoneBTN.interactable = true;
            DoneBTN.onClick.AddListener(BTN_Done);
        }
        else
        {
            DoneBTN.interactable = false;
        }
    }

    private void BTN_Done()
    {


        NetworkManager._instance._playerData.ExaminationAnswerList = BriefAnswers;
        NetworkManager._instance.BriefData(questions[0].SurveyTitle);
        for (int i = 0; i < isAnswers.Length; i++)
        {
            answers[i] = 0;
            isAnswers[i] = false;
        }
        pages[0].SetActive(true);
        pages[1].SetActive(false);

    }
    
    #endregion

    public void Back()
    {
        
    }
}
