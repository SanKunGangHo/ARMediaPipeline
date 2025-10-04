using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class PlayerPrefsDeleter : EditorWindow
{
    
    
    [MenuItem("Window/PlayerPrefs Deleter")]

    static void Init()
    {
        PlayerPrefsDeleter window = (PlayerPrefsDeleter)EditorWindow.GetWindow(typeof(PlayerPrefsDeleter));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Delete All PlayerPrefs"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
#endif
