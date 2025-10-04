using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollingButtons : MonoBehaviour
{
    public Scrollbar scrollbar;
    
    public float scrollStep = 0.3f;  // 한 번에 스크롤될 양

    public void OnLeftClick()
    {
        SetScrollbarValue(scrollbar.value - scrollStep);
    }

    public void OnRightClick()
    {
        SetScrollbarValue(scrollbar.value + scrollStep);
    }

    private void SetScrollbarValue(float value)
    {
        scrollbar.value = Mathf.Clamp(value, 0, 1);
    }
}
