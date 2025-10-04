using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AngleManager : MonoBehaviour
{
    [Header("Pivot 관련 - 움직임의 축")]
    public RectTransform pivotSet_middle;
    public RectTransform thisObject;

    public Image _bar;
    //thisObject = _moveObject
    
    public void Start()
    {
        thisObject = GetComponent<RectTransform>();
        StartCoroutine(Activate_Operation_Coroutine());
    }

    IEnumerator Activate_Operation_Coroutine()
    {
        while (true)
        {
            float amount = _bar.fillAmount * 180 - 90;
            thisObject.pivot = pivotSet_middle.pivot;
            //thisObject.rotation = Quaternion.Euler(0, 0, amount);
            yield return null;
        }
    }
}
