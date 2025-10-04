using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionData", menuName = "Scriptable Object/Questrions Scriptable Object")]
public class Questions : ScriptableObject
{
    public string SurveyTitle;
    public string QuestNum;
    public string QuestTitle_EN;
    public string QuestTitle_VT;
    public string QuestTitle_KR;
    public string[] QuestAnswers_EN;
    public string[] QuestAnswers_VT;
    public string[] QuestAnswers_KR;

    [Header("Filter")] public string[] Question_Filter;
    public int[] Question_Filter_Point;

    public bool isComport;
    public Questions preQuestion;
    public Questions nextQuestion;
}
