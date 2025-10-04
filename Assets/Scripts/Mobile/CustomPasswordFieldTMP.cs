using TMPro;
using UnityEngine;

public class CustomPasswordFieldTMP : MonoBehaviour
{
    public TMP_InputField tmpInputField;
    private char maskChar = '•'; // 원하는 마스킹 문자

    void Start()
    {
        if (tmpInputField != null && tmpInputField.contentType == TMP_InputField.ContentType.Password)
        {
            // TMP_InputField의 asteriskChar 속성을 변경합니다.
            tmpInputField.asteriskChar = maskChar;

            // 입력 필드의 내용을 새로 고침합니다.
            tmpInputField.ForceLabelUpdate();
        }
    }
}