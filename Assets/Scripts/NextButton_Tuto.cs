using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NextButton_Tuto : MonoBehaviour
{
    private Image thisImage;
    public float fadeTime = 0.5f;

    private void Start()
    {
        thisImage = GetComponent<Image>();
    }

    public void OnNextButtonGlow()
    {
        StartCoroutine(buttonGlowing());
    }

    IEnumerator buttonGlowing()
    {
        while (true)
        {
            yield return Fade(0f, 1f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
            yield return Fade(1f, 0f, fadeTime);
            yield return new WaitForSeconds(fadeTime);
        }
    }
    
    IEnumerator Fade(float startAlpha, float endAlpha, float time)
    {
        float speed = 1f / (time*2);
        for (float t = 0f; t < 1f; t += Time.deltaTime * speed)
        {
            Color color = thisImage.color;
            color.a = Mathf.Lerp(startAlpha, endAlpha, t);
            thisImage.color = color;
            yield return null;
        }
    }
}
