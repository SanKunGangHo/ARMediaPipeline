using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GuideBlink : MonoBehaviour
{
    public float fadeTime = 0.5f;
    private Image _blinkImage;
    public TMP_Text TMPtext;
    private int AlertCheck;

    private void Awake()
    {
        _blinkImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(_Start());
    }

    //시작 코루틴
    IEnumerator _Start()
    {
        while (true)
        {
            yield return Fade(0f, 1f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            yield return Fade(1f, 0f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            if (gameObject.CompareTag("Alert"))
            {
                AlertCheck++;
                if (AlertCheck >= 5)
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float time)
    {
        float speed = 1f / (time*2);
        for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            Color color = _blinkImage.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            _blinkImage.color = color;
            if (gameObject.name == "PainAlert")
            {
                TMPtext.color = color;
            }
            yield return null;
        }
    }
}

