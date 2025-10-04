using UnityEngine;

public class SplittingManager : MonoBehaviour
{
    public ExerciseManager _exerciseManager;
    public TutorialManager _tutorialManager;
    private NetworkManager nm;

    [Space(10)] 
    public GameObject _exerciseStarter;
    public GameObject _tutorialStarter;

    private void Awake()
    {
        nm = NetworkManager._instance;
        if(!nm.isMobile) return;
        if (!nm.isAppTutorialEnd)
        {
            _tutorialStarter.SetActive(true);
            _exerciseManager.gameObject.SetActive(false);
        }
        else
        {
            _exerciseStarter.SetActive(true);
            _tutorialManager.gameObject.SetActive(false);
        }
    }
}
