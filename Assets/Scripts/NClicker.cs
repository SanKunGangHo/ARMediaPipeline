using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class NClicker : MonoBehaviour, IPointerClickHandler
{
    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!_button.interactable)
        {
            OnNonInteractableClick();
        }
    }

    private void OnNonInteractableClick()
    {
        SoundManager.instance.SEAudioPlay(1);
    }
}
