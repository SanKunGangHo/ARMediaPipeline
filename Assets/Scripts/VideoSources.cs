using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSources : MonoBehaviour
{
    public VideoClip[] _videoClips;
    public VideoPlayer _video;

    public RawImage RawImage;

    private void Start()
    {
        _video = transform.GetComponent<VideoPlayer>();
        if (SceneManager.GetActiveScene().name == "TestScene" || (!NetworkManager._instance.isAppTutorialEnd && NetworkManager._instance.isMobile))
        {
            _video.clip = TutorialManager.instance.TutorialExerciseData.VideoClips[0];
            _video.time = 1f;
            _video.Play();
            _video.Pause();
        }
    }
    

    public void frameSetter()
    {
        _video.Stop();
        _video.frame = 1;
    }

    public void CheckIndex(int index)
    {
       changeClip(index);
       _video.Play();
    }

    private void changeClip(int index)
    {
        _video.Stop();
        if (SceneManager.GetActiveScene().name == "TestScene" || (!NetworkManager._instance.isAppTutorialEnd && NetworkManager._instance.isMobile))
        {
            _video.clip = TutorialManager.instance.TutorialExerciseData.VideoClips[0];
            _video.Play();
            return;
        }

        if (NetworkManager._instance.isNative_Activity)
        {
            Debug.LogError("VideoSources 1");
            _video.clip = NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise]
                .VideoClips[index];

            if (NetworkManager._instance.todaysRandom[NetworkManager._instance.choosedExercise].isReversible)
            {
                if (PoseManager.instance.isReverse)
                {
                    Debug.LogError("VideoSources 2");
                    var scale = RawImage.transform.localScale;
                    scale.x = -1;
                    RawImage.transform.localScale = scale;
                }
                else
                {
                    Debug.LogError("VideoSources 3");
                    var scale = RawImage.transform.localScale;
                    scale.x = 1;
                    RawImage.transform.localScale = scale;
                }
            }
        }
        else
        {
            _video.clip = NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise]
                .VideoClips[index];

            if (NetworkManager._instance.exerciseDatas_all[NetworkManager._instance.choosedExercise].isReversible)
            {
                if (PoseManager.instance.isReverse)
                {
                    Debug.LogError("VideoSources 4");
                    var scale = RawImage.transform.localScale;
                    scale.x = -1;
                    RawImage.transform.localScale = scale;
                }
                else
                {
                    Debug.LogError("VideoSources 5");
                    var scale = RawImage.transform.localScale;
                    scale.x = 1;
                    RawImage.transform.localScale = scale;
                }
            }
        }
        _video.Play();
    }
    
    //오퍼레이션 전용
    public void ChangeClip_Operation(int index)
    {
        _video.clip = _videoClips[index];

        if (index is 1 or 3)
        {
            var scale = RawImage.transform.localScale;
            scale.x = -1;
            RawImage.transform.localScale = scale;
        }
        else
        {
            var scale = RawImage.transform.localScale;
            scale.x = 1;
            RawImage.transform.localScale = scale;
        }

        _video.Play();
    }
    
    
}
