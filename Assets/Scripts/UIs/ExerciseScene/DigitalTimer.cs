using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DigitalTimer : MonoBehaviour
{
    public List<Sprite> Timers;
    public Image TimerImage;
    [SerializeField] private int count = 3;

    public GameObject thisUI;
    public GameObject startOnUI;
    
    // Start is called before the first frame update
    void Start()
    {
        TimerImage = GetComponent<Image>();
        TimerImage.sprite = Timers[count];
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while (count >= 0)
        {
            TimerImage.sprite = Timers[count];
            yield return new WaitForSeconds(1f);
            count--;
        }
        
        SoundManager.instance.BGMAudioPlay(0);
        PoseManager.instance.PoseMode(-1);
        startOnUI.SetActive(true);
        thisUI.SetActive(false);
    }
}
