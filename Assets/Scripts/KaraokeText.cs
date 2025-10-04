using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KaraokeText : MonoBehaviour
{
    public Image karaokeImage;
    [SerializeField] private Image NextImage;
    [SerializeField] private GameObject BlinkingNext;

    private void Awake()
    {
        karaokeImage = GetComponent<Image>();
        //StartCoroutine(karaokeGo());
    }

    public void karaokeStart(float speed = 2)
    {
        StartCoroutine(karaokeGo(speed));
    }

    IEnumerator karaokeGo(float speed = 2)
    {
        while (karaokeImage.fillAmount < 1)
        {
            karaokeImage.fillAmount += Time.deltaTime * speed * 0.1f;
            
            if (karaokeImage.fillAmount >= 1)
            {
                karaokeImage.fillAmount = 1;
                break;
            }
            
            yield return null;
        }

        if (NextImage != null)
        {
            NextImage.GetComponent<KaraokeText>().karaokeStart();
            StopCoroutine(karaokeGo());
            yield break;
        }
        else
        {
            BlinkingNext.GetComponent<NextButton_Tuto>().OnNextButtonGlow();
            yield break;
        }
    }
}
