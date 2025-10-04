using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultAngle : MonoBehaviour
{
    public Image maxGraph;
    public Image AveGraph;
    public RectTransform aveLine;
    public TMP_Text maxText, aveText;
    public TMP_Text maxTitle, aveTitle;
    public Color afterB, afterL;
    public Color beforeB, beforeL;

    public void ResultGraph(float maxData, float sum, int count = 2)
    {
      //최대값 그래프
        maxGraph.fillAmount = maxData / 180;
        AveGraph.fillAmount = sum / 180;
        
        //선
        aveLine.localEulerAngles = new Vector3(0, 0, -sum + 90);
        
        //밑에 글씨
        maxText.text = maxData.ToString("000.00");
        aveText.text = sum.ToString("000.00");

        switch (count)
        {
            case 0 : //비포
                maxGraph.color = beforeB;
                AveGraph.color = beforeL;
                aveTitle.color = beforeL;
                break;
            case 1: //애프터
                maxGraph.color = afterB;
                AveGraph.color = afterL;
                aveTitle.color = afterL;
                break;
        }
    }
    
    public void ResultGraph_Mobile(float maxData, float sum, bool isAfter = false)
    {
        Debug.Log("result Doing");
        //최대값 그래프
        maxGraph.fillAmount = maxData / 180;
        AveGraph.fillAmount = sum / 180;
        
        //선
        aveLine.localEulerAngles = new Vector3(0, 0, -sum + 90);
        
        //밑에 글씨
        maxText.text = maxData.ToString("000.00");
        aveText.text = sum.ToString("000.00");

        switch (isAfter)
        {
            case false : //비포
                maxGraph.color = beforeB;
                maxTitle.color = beforeB;
                AveGraph.color = beforeL;
                aveTitle.color = beforeL;
                break;
            case true: //애프터
                maxGraph.color = afterB;
                maxTitle.color = afterB;
                AveGraph.color = afterL;
                aveTitle.color = afterL;
                break;
        }
    }
}
