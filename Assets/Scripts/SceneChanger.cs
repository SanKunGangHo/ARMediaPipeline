using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public void SceneChange(string NextScene)
    {
        SceneManager.LoadScene(NextScene);  
    }

    public void PlayerDataReset()
    {
        NetworkManager NM = NetworkManager._instance;
        NM.ResetData();
    }
}
