using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSystem : MonoBehaviour
{
    public float duration_time = 3f;
    public float loading_time = 0f;
    public Image loading_bar;
    private WaitForSeconds _waitForSeconds = new WaitForSeconds(1);

    [Header("환자 아이디 입력창")] 
    public GameObject patientIDGameObject;
    
    void Start()
    {
        string nowScene = SceneManager.GetActiveScene().name;
        if (nowScene == "LoginScene" || nowScene == "#1 Splash")
        {
            SoundManager.instance.SEAudioPlay(12);
        }
        else
        {
            SoundManager.instance.SEAudioPlay(5);
        }

        if (nowScene == "TestScene")
        {
            SoundManager.instance.SEAudioPlay(5);
        }
        
        StartCoroutine(Loading());
    }
    
    

    IEnumerator Loading()
    {
        while (loading_time <= duration_time)
        {
            float percent = loading_time / duration_time;

            loading_bar.fillAmount = percent;

            loading_time += Time.deltaTime;
            yield return null;
        }
        
        loading_bar.fillAmount = 1f;
        SceneChecker();
        yield break;
    }

    public void SceneChecker()
    {
        string nowScene = SceneManager.GetActiveScene().name; //현재 씬 이름
        switch (nowScene)
        {
            case "LoginScene":
                patientIDGameObject.SetActive(true); //환자 아이디 입력창
                SoundManager.instance.SEAudioPlay(9);
                transform.parent.parent.gameObject.SetActive(false); //로딩씬 종료
                break;
            case "TestScene":
                SceneManager.LoadScene("Home");
                NetworkManager._instance.isAppTutorialEnd = true;
                break;
            case "ExerciseScene":
                //StartCoroutine(NetworkManager._instance._firebaseLoader.ExerciseClear());
                SceneManager.LoadScene("Home");
                break;
            case "#1 Splash":
                SceneManager.LoadScene("#2 LogIn");
                //SceneManager.LoadScene("AngleCheckTest");
                break;
            case "#4 exercise":
                SceneManager.LoadScene("#3 homeMain");
                break;
        }
    }
}

//size 20 /Copyright © 2024 Dain Leaders | All Rights