using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationData : MonoBehaviour
{
    public Dictionary<string, Queue<float>> operationPos;
    public Queue<float> leftForward, leftSide, rightForward, rightSide;

    private void Start()
    {
        operationPos = new Dictionary<string, Queue<float>>();
        operationPos.Add("leftForward", leftForward);
        operationPos.Add("leftSide", leftSide);
        operationPos.Add("rightForward", rightForward);
        operationPos.Add("rightSide", rightSide);
    }
}
