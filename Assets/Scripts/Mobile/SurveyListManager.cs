using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SurveyListManager : MonoBehaviour
{
    public TMP_Text userID; 
    public GameObject itemPrefab;
    public Transform listParent;
    
    // Start is called before the first frame update
    void Start()
    {
        userID.text = NetworkManager._instance._playerData.PatientID;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
