using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SettingSlider : MonoBehaviour
{
    Vector3 origin, screenSize;
    float width;

    public GameObject settingWindow;

    // Start is called before the first frame update
    void Start ()
    {
        origin = settingWindow.transform.position;
        width = settingWindow.GetComponent<RectTransform>().sizeDelta.x;
        settingWindow.gameObject.SetActive(false);
    }
    
    public void OpenSetting()
    {
        settingWindow.gameObject.SetActive(true);
        StartCoroutine(SlideWindow(true));
    }

    public void CloseSetting()
    {
        StartCoroutine(SlideWindow(false));
    }

    IEnumerator SlideWindow(bool isOpen)
    {
        Debug.Log(isOpen);
        if(isOpen)
        {
            Debug.Log("isOpen");
            while(settingWindow.transform.position.x > Screen.width - width/2)
            {

                settingWindow.transform.position -= Vector3.right*25;
                if(settingWindow.transform.position.x < settingWindow.transform.position.x - width/2) break;
                yield return null;
            } 
        }
        else
        {
            while(settingWindow.transform.position.x < Screen.width + width/2)
            {
                settingWindow.transform.position += Vector3.right*25;
                yield return null;
            } 
            settingWindow.SetActive(false);

        }

        yield return null;

    }

}
