using System;
using System.Collections;
using System.IO;
#if UNITY_EDITOR
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

[Serializable] public class ScreenShotData
{
    public string name;
    public int width;
    public int height;
}

public class CaptureScreenShot : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    RecorderController m_Controller;
    
    [SerializeField]
    private ScreenShotData[] m_ScreenShotData;

    private void Setting(string name, int width, int height)
    {
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        m_Controller = new RecorderController(controllerSettings);
        
        var mediaOutputFolder = Path.Combine(Application.dataPath, "../../..", "Media");

        var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
        imageRecorder.name = name;
        imageRecorder.Enabled = true;
        imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
        imageRecorder.CaptureAlpha = false;
        
        imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, name+"_"+width+"_"+height+"_") + currentTime;

        imageRecorder.imageInputSettings = new GameViewInputSettings
        {
            OutputWidth = width,
            OutputHeight = height
        };
        
        controllerSettings.AddRecorderSettings(imageRecorder);
        controllerSettings.SetRecordModeToSingleFrame(0);
    }

    void OnGUI()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            StartCoroutine(Capture());
        }
    }

    IEnumerator Capture()
    {
        foreach (ScreenShotData data in m_ScreenShotData)
        {
            Setting(data.name, data.width, data.height);
            m_Controller.PrepareRecording();
            m_Controller.StartRecording();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
#endif
