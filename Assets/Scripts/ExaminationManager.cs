using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class ExaminationManager : MonoBehaviour
{
    [Header("Progerss Bar 관련")]
    public Slider _ProgressSlider;
    public TMP_Text num;
    public Sprite green;
    public QuestionManager QM;
    public GameObject Q_only;
    
    Sprite original;

    private void Awake()
    {
        original = _ProgressSlider.transform.GetChild(0).GetComponent<Image>().sprite;
    }
    private void Start()
    {

        SoundManager.instance.BGMAudioPlay(8);
    }

    public void ValueChanger(int value = 1)
    {
        if(QM.quizArray[0].SurveyTitle == "On-Boarding Survey")
        
        {
            switch (value)
            {
                
                case 0:
                    //_ProgressSlider.value = 1;
                    Debug.LogError($"있을 수 없는 숫자입니다.");
                    break;
                case 1:
                    num.text = $"{value}/10";
                    _ProgressSlider.value = 1;
                    break;
                case 2:
                    num.text = $"{value}/10";
                    _ProgressSlider.value = 3.6f;
                    break;
                case 3:
                    num.text = $"{value}/10";
                    _ProgressSlider.value = 6.2f;
                    break;
                case 4:
                    num.text = $"{value}/10";
                    _ProgressSlider.value = 8.8f;
                    break;
                case 5://4-1
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 8.8f;
                    break;
                case 6://5
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 11.4f;
                    break;
                case 7://6
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 14f;
                    break;
                case 8://7
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 16.6f;
                    break;
                case 9://8
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 19.2f;
                    break;
                case 10://9
                    num.text = $"{value-1}/10";
                    _ProgressSlider.value = 21.8f;
                    break;
                case 11://10
                    num.text = $"10/10";
                    _ProgressSlider.value = 25;
                    break;
                case 12://10-1
                    num.text = $"10/10";
                    _ProgressSlider.value = 25;
                    break;
                case 13://10-2
                    num.text = $"10/10";
                    _ProgressSlider.value = 25;
                    break;
                case 14://10-3
                    num.text = $"10/10";
                    _ProgressSlider.value = 25;
                    break;
                case 15://끝
                    num.text = $"10/10";
                    _ProgressSlider.value = 25;
                    break;
                default:
                    _ProgressSlider.value = 1;
                    break;
            }
            if (value >= 11)
            {
                _ProgressSlider.transform.GetChild(0).GetComponent<Image>().sprite = green; 
            }
            else
            {
                 _ProgressSlider.transform.GetChild(0).GetComponent<Image>().sprite = original; 
            }
        }

        else
        {
            num.text = $"{value}/{QM.quizArray.Length}";
            _ProgressSlider.maxValue = QM.quizArray.Length;
            _ProgressSlider.value = value * 0.75f;
            if (value == QM.quizArray.Length)
            {
                _ProgressSlider.transform.GetChild(0).GetComponent<Image>().sprite = green; 
            }
            else
            {
                _ProgressSlider.transform.GetChild(0).GetComponent<Image>().sprite = original; 
            }
        }

    }

    public void DisplayDiscomportOnly(bool _isOn)
    {
        Q_only.SetActive(_isOn);
    }
}
