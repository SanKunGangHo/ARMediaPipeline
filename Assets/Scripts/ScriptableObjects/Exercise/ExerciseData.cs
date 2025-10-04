using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "Exercise Data", menuName = "Scriptable Object/Exercise Scriptable Object")]
public class ExerciseData : ScriptableObject
{
    public string Title;
    public Sprite[] Type;
    public string Type_string;
    public Sprite Character, warningCharacter;
    public Sprite Char_mini, warningChar_mini;
    [Tooltip("밑에 초록창")]
    public string[] steps;

    public bool isLeftDisable;
    [Tooltip("왼쪽 어깨 각도")]
    public List<float> LSAngles;
    [Tooltip("왼쪽 팔꿈치 각도")]
    public List<float> LEAngles;

    public bool isRightDisable;
    [Tooltip("오른쪽 어깨 각도")]
    public List<float> RSAngles;
    [Tooltip("오른쪽 팔꿈치 각도")]
    public List<float> REAngles;

    [Header("영상들")] public VideoClip[] VideoClips;

    [Tooltip("통증 시 멈춤 필요")]
    public bool isPausable;
    [Tooltip("세트 수 - 1")]
    public float set; 
    [Tooltip("유지시간")]
    public float time;

    [Tooltip("좌우반전")] 
    public bool isReversible;

    [Tooltip("클리어")] public bool isClear = false;

    [Space(30f)] 
    
    [Header("속성 반복이 필요한가? (현재 이하 사용 안함)")] 
    public bool isRepeatable;
    
    [Header("반복부분-전")] 
    public int repeatBefore;
    
    [Header("반복부분-후")] 
    public int repeatAfter;
}
