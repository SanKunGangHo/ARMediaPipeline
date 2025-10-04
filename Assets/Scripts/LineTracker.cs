using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class LineTracker : MonoBehaviour
{
    public RectTransform object1;
    public RectTransform object2;
    public RectTransform object3;
    public UILineRenderer line1;
    public UILineRenderer line2;

    void Start()
    {
        // Create the first LineRenderer to connect object1 and object2

        // Set the number of line vertices (2 for a single straight line)
        line1.Points = new Vector2[2];

        

        // Similarly create the second LineRenderer to connect object2 and object3
        line2.Points = new Vector2[2];

        StartCoroutine(Liner());

        // LineRenderer will be visible in the scene but for full visibility setup Material, Width etc. 
        // according to the needs in the Inspector or by script.
    }

    IEnumerator Liner()
    {
        while (true)
        {
            line1.Points[0] = object1.anchoredPosition;
            line1.Points[1] = object2.anchoredPosition;
            line1.SetAllDirty();
            
            line2.Points[0] = object2.anchoredPosition;
            line2.Points[1] = object3.anchoredPosition;
            line2.SetAllDirty();
            yield return null;
        }
        // Set the position of the line vertices to the gameobjects' positions
    }
}