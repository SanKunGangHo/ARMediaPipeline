using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgeActivate : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(2f);
        gameObject.SetActive(false);
    }

}
