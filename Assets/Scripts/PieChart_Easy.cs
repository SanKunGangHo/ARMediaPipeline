using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PieChart_Easy : MonoBehaviour
{
    public Image[] imagesPieChart;
    public float[] values;
    public Color32[] pieChartColors;
    
    public Image[] chartColorImage;
    
    public TMP_Text[] _count;

    public void SetValues(float[] valuesToSet)
    {
        values = valuesToSet;
        float totalValues = 0;
        for (int i = 0; i < imagesPieChart.Length; i++)
        {
            Debug.Log(valuesToSet[i]);
            totalValues += FindPercentage(valuesToSet, i);
            imagesPieChart[i].fillAmount = totalValues;
            imagesPieChart[i].color = pieChartColors[i];
            _count[i].text = valuesToSet[i].ToString();
            chartColorImage[i].color = pieChartColors[i];
        }
    }

    private float FindPercentage(float[] valueToSet, int index)
    {
        float totalAmount = 0;
        for (int i = 0; i < valueToSet.Length; i++)
        {
            totalAmount += valueToSet[i];
        }

        return valueToSet[index] / totalAmount;
    }
}