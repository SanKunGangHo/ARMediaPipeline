using System;
using System.Collections;
using Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField _inputFieldID, _inputFieldPW;
    public TMP_Text warningText;

    public Button EnterButton;

    public SceneChanger sceneChanger;
    public PatientIDCheck pic;

    public Toggle saveLogin;
    FireBaseLoader fl;
    
    [SerializeField] private bool isIDChecked, isPWChecked;
    
    Toggle eye_pass;


    private void Start()
    {
        Debug.Log(NetworkManager._instance._firebaseLoader);
        fl = NetworkManager._instance._firebaseLoader;


        NetworkManager._instance._firebaseLoader.loginManager = this;
        if(NetworkManager._instance.isMobile)
        {
            eye_pass = _inputFieldPW.transform.GetChild(1).GetComponent<Toggle>();
            if(PlayerPrefs.HasKey("savedID"))
            {
                 _inputFieldID.text = PlayerPrefs.GetString("savedID");
                 saveLogin.isOn = true;
            }
        }
        if (!string.IsNullOrEmpty(pic.inputID.text))
        {
            _inputFieldID.text = pic.inputID.text;
        }

    }

    public void IDcheck()
    {
        if (string.IsNullOrEmpty(_inputFieldID.text))
        {
            warningText.text = "The userID is incorrect.";
            isIDChecked = false;
            EnterCheck();
            return;
        }
        
        isIDChecked = true;
        warningText.text = "";
        EnterCheck();
    }

    public void PWCheck()
    {
        if (string.IsNullOrEmpty(_inputFieldPW.text))
        {
            warningText.text = "The password is incorrect.";
            isPWChecked = false;
            EnterCheck();
            return;
        }
        
        isPWChecked = true;
        warningText.text = "";
        EnterCheck();
    }

    //TODO : REMEMBER ID 기능 만들기, 비밀번호 눈 기능 만들기
    
    private void EnterCheck()
    {
        if (isIDChecked && isPWChecked)
        {
            EnterButton.interactable = true;
        }
        else
        {
            EnterButton.interactable = false;
        }
    }

    public void EnterOn()
    {
        if(NetworkManager._instance.isMobile)
            SaveLoginID();
        //SceneManager.LoadScene("AdminScene");
        StartCoroutine(EnterOnCoroutine());
    }

    IEnumerator EnterOnCoroutine()
    {

        yield return StartCoroutine(fl.Login(_inputFieldID.text, _inputFieldPW.text));
        

        
    }

    
        public void ToggleEye_pass()
        {
            _inputFieldPW.contentType = !eye_pass.isOn ? TMP_InputField.ContentType.Password : TMP_InputField.ContentType.Standard;
            _inputFieldPW.ForceLabelUpdate();
        }


    public void SaveLoginID()
    {
        if(saveLogin.isOn)
        {   
            PlayerPrefs.SetString("savedID", _inputFieldID.text);
        }
        else
        {
            if(PlayerPrefs.HasKey("savedID")) PlayerPrefs.DeleteKey("savedID");
        }

    
    }





}
