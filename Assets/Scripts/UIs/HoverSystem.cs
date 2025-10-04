using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class HoverSystem : MonoBehaviour
{
    public Sprite normalSprite, hoverSprite;
    private bool isClicked = false;
    private Image thisImage;

    private void Start()
    {
        thisImage = GetComponent<Image>();
    }

    public void _OnMouseEnter()
    {
        if (isClicked) return;
        thisImage.sprite = hoverSprite;
    }

    public void _OnMouseExit()
    {
        if (isClicked) return;
        thisImage.sprite = normalSprite;
    }

    public void OnCliecked()
    {
        isClicked = true;
    }
}
