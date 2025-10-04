using TMPro;
using UnityEngine;

public class AngleUIChanger : MonoBehaviour
{
    public TMP_Text LeftText, RightText; //왼쪽 글자, 오른쪽 글자
    public TMP_Text LeftAngle, RightAngle; //왼쪽 숫자, 오른쪽 숫자
    
    public void UpdateAngle(string Left, string Right, float LeftFloat, float RightFloat)
    {
        if (!NetworkManager._instance.isMobile)
        {
            LeftText.text = Left;
            RightText.text = Right;
        }
        LeftAngle.text = LeftFloat.ToString("000.00");
        RightAngle.text = RightFloat.ToString("000.00");
    }
}
