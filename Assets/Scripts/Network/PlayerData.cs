using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public string PatientID;
    public string PW;
    public string gender;
    public DateTime dateOfBirth;
    public string job;
    public string category;
    public int day;
    public string lastDate;
    public string firstDate;
    public bool nowActivate;
    public bool firshIncoming;
    public bool Tutorial;

    public Dictionary<int, string> days_list = new Dictionary<int, string>();

    public List<float> operationResult_Max_Before = new List<float>();
    public List<float> operationResult_Ave_Before = new List<float>();

    public DateTime ExamTime = new DateTime();
    public Dictionary<string, int> ExaminationAnswerList = new Dictionary<string, int>();

    public List<DateTime> BriefTime = new List<DateTime>();
    public Dictionary<string, int> BriefAnswerLists = new Dictionary<string, int>();
}
