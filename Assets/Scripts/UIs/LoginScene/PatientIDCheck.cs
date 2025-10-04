using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// LoginScene 전용 스크립트
/// </summary>
public class PatientIDCheck : MonoBehaviour
{
    [Tooltip("Canvas - PatientIDCheck #2 - GameObject - InputField(TMP)")]
    public TMP_InputField inputID;
    public TMP_Text text;
    [Tooltip("Canvas - Register #3")]
    public GameObject register;
    [Tooltip("Canvas - Login #6")]
    public GameObject login;

    public TMP_Text warningText;

    private void Start()
    {
        NetworkManager._instance._firebaseLoader.patientIDCheck = this;
    }

    private void OnEnable()
    {
        inputID.text = "";
    }

    public void BTN_IDCheck()
    {
        Debug.Log("BTN_IDCheckClicked");
        if (string.IsNullOrEmpty(inputID.text))
        {
            text.text = "Please enter an ID code.";
            return;
        }
        
        Debug.Log("BTN_IDCheck_2");
        NetworkManager._instance._playerData.PatientID = inputID.text;
        //this.GetComponent<Button>().interactable = false;
        StartCoroutine(NetworkManager._instance._firebaseLoader.PlayerCheck(inputID.text));
    }

    public void IDCheck_Collected()
    {
        Debug.Log("idCheck Collected");
        
        //Debug.Log($"Before: gameObject activeSelf: {gameObject.activeSelf}, register activeSelf: {register.activeSelf}");

        //NetworkManager._instance._firebaseLoader.PlayerDataLoad(inputID.text);
        NetworkManager._instance._playerData.PatientID = inputID.text;
        gameObject.SetActive(true);
        register.SetActive(true);
        Debug.Log("BTN_IDCheckClicked_5");
        
        Debug.Log("idCheck Collected_2");

        //Debug.Log($"After: gameObject activeSelf: {gameObject.activeSelf}, register activeSelf: {register.activeSelf}");
    }
}
