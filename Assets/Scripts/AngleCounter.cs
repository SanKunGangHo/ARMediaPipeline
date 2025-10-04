using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AngleCounter : MonoBehaviour
{
    [SerializeField] MpLinefind lindFind;
    [SerializeField] MpMarkfind markfind;
    public Text rightAngle, leftAngle;
    public Text rightShoulerAngle, leftShoulderAngle;
    public TMP_Text TMP_RightShoulerAngle, TMP_LeftShoulerAngle;
    public TMP_Text TMP_operationAngleL,TMP_operationAngleR; 

    private void Start()
    {
        StartCoroutine(Changer());
    }

    IEnumerator Changer()
    {
        while (true)
        {
            rightAngle.text = $"{markfind.Rangle:000}";
            leftAngle.text = $"{markfind.Langle:000}";

            leftShoulderAngle.text = $"{markfind.LsAngle:000}";
            rightShoulerAngle.text = $"{markfind.RsAngle:000}";

            if (SceneManager.GetActiveScene().name == "TestScene" || NetworkManager._instance.isMobile)
            {
                TMP_LeftShoulerAngle.text = $"{markfind.LsAngle:000.00}";
                TMP_RightShoulerAngle.text = $"{markfind.RsAngle:000.00}";
                TMP_operationAngleL.text = $"{markfind.LsAngle:000.00}";
                TMP_operationAngleR.text = $"{markfind.RsAngle:000.00}";
            }
            yield return null;
        }
    }
}
