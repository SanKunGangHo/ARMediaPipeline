using UnityEngine;

public class disappear : MonoBehaviour
{
    public float disappearTime = 3;
    private float time = 0f;
    void Update()
    {
        time += Time.deltaTime;

        if (time >= disappearTime)
        {
            time = 0;
            gameObject.SetActive(false);
        }
    }
}
