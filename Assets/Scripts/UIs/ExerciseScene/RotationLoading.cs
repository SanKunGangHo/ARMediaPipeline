using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RotationLoading : MonoBehaviour
{
    public float loadingTime, maxTime;
    public float oneStepAngle;
    public RectTransform icon;

    public TMP_Text Title;
    public Image[] Type;
    
    IEnumerator Start()
    {
        if (SceneManager.GetActiveScene().name == "TestScene" || (!NetworkManager._instance.isAppTutorialEnd && SceneManager.GetActiveScene().name == "#4 exercise"))
        {
            SoundManager.instance.AudioStop("BGM");
            Title.text = TutorialManager.instance.TutorialExerciseData.Title;
            TypeSelector(TutorialManager.instance.TutorialExerciseData.Type_string);
            icon.localEulerAngles = Vector3.zero;
            while (loadingTime <= maxTime)
            {
                Vector3 iconAngle = icon.localEulerAngles;
                iconAngle.z = oneStepAngle;

                icon.localEulerAngles += iconAngle;
                loadingTime += 0.1f;
                yield return new WaitForSeconds(0.1f);
            }
            icon.parent.GetChild(0).gameObject.SetActive(true);
            yield break;
        }
        
        SoundManager.instance.AudioStop("BGM");
        if (NetworkManager.Instance.isNative_Activity)
        {
            Title.text = NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise].Title;
            TypeSelector(NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise].Type_string);
        }
        else
        {
            Title.text = NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise].Title;
            TypeSelector(NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise].Type_string);
        }
        icon.localEulerAngles = Vector3.zero;
        while (loadingTime <= maxTime)
        {
            Vector3 iconAngle = icon.localEulerAngles;
            iconAngle.z = oneStepAngle;

            icon.localEulerAngles += iconAngle;
            loadingTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        icon.parent.parent.gameObject.SetActive(false); //StartUI.SetActive(false);
        ExerciseManager.instance.StartExercise();
        
    }

    public void TypeSelector(string type)
    {
        switch (type)
        {
            case "Stretching":
                Type[0].gameObject.SetActive(true);
                break;
            case "Pain Relief":
                Type[1].gameObject.SetActive(true);
                break;
            case "Stabilization":
                Type[2].gameObject.SetActive(true);
                break;
            case "ROM":
                Type[3].gameObject.SetActive(true);
                break;
            case "Strengthening":
                Type[4].gameObject.SetActive(true);
                break;
        }
    }
}
