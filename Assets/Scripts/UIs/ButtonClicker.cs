using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClicker : MonoBehaviour
{
    private Button thisButton;
    public int second;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(second);
        thisButton.onClick.Invoke();
    }
    
}
