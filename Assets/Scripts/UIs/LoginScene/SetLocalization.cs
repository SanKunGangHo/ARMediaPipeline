using UnityEngine;
using System.Globalization;

/// <summary>
/// DatePicker 애셋의 지역에 맞춰 현지화되는 부분을 억지로 영어로 바꿈
/// 나중에 해외 릴리즈 때 지워도 무방할 것 같아요.
/// </summary>
#if UNITY_EDITOR
[UnityEditor.InitializeOnLoad]
#endif
public class SetLocalization
{
    [RuntimeInitializeOnLoadMethod]
    static void InitLocalization()
    {
        // replace "en-US" with the culture of your choice
        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        //베트남 "vi-VN"
        //한국 "ko-KR" (보류)
    }

#if UNITY_EDITOR
    static SetLocalization()
    {
        InitLocalization();
    }
#endif
}
