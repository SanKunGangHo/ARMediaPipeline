using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyOpenManager : MonoBehaviour
{
    [SerializeField] GameObject[] surveys;
    [SerializeField] GameObject selectPopup;
    [SerializeField] Toggle homeToggle;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Surveyclose()
    {
        foreach(var i in surveys)
        {
            if(i.activeInHierarchy) i.SetActive(false);
        }
        homeToggle.isOn = true;
    }
}
