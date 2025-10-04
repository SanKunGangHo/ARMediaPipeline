using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI.Dates;
using UnityEngine;
using UnityEngine.UI;

public class ProfileManager : MonoBehaviour
{
    public TMP_Text id;
    public TMP_InputField pw;
    public TMP_Dropdown job;
    public ToggleGroup gender;
    public DatePicker dateOfBirth;
    public TMP_InputField SetTime;

    Dictionary<string, int> genderToIndex = new Dictionary<string, int>()
    {
        { "Man", 0 },
        { "Woman", 1 },
        { "Rather not say", 2 }
    };

    public Toggle pwToggle;
    
    public WaitForSeconds WaitForOneSeconds = new WaitForSeconds(1f);
    private void Start()
    {
        PlayerData pd = NetworkManager._instance._playerData;
        id.text = pd.PatientID;
        pw.text = pd.PW;
        job.value = job.options.FindIndex(x => x.text  == pd.job);
        if(genderToIndex.TryGetValue(pd.gender, out int index))
        {
            gender.transform.GetChild(index).GetComponent<Toggle>().isOn = true;
        }
        
        dateOfBirth.SelectedDate = pd.dateOfBirth;
        if(NetworkManager._instance.isMobile) return;
        StartCoroutine(timerCheck());
    }



    public void TogglePassword()
    {
        pw.contentType = !pwToggle.isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
        pw.ForceLabelUpdate();
    }

    IEnumerator timerCheck()
    {
        SetTime.text = TimeFormatter(NetworkManager._instance.time_play);
        yield return WaitForOneSeconds;
    }

    public static string TimeFormatter(float timeInSeconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
        return $"{time.Minutes:D2}:{time.Seconds:D2}";
    }
}
