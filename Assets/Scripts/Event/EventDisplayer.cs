using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventDisplayer : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private Image image;
    [SerializeField] private float displayDuration;
    

    private GatoEvento _gatoEvento;
    private float _elapsedTime = 0.0f;
    

    public void SetEvento(GatoEvento evento)
    {
        this._gatoEvento = evento;
        _elapsedTime = 0.0f;
    }

    public void DisplayEvent()
    {
        if (_gatoEvento == null) return;
        if (_elapsedTime >= displayDuration) return;

        if(text != null) text.text = _gatoEvento.name;
        if (image != null) image.sprite = _gatoEvento.eventSprite;
    }

    public void HideEvent()
    {
        if (_gatoEvento == null) return;
        if (_elapsedTime >= displayDuration) return;

        if(text != null) text.text = "";
        if (image != null) image.sprite = null;
    }

    public void UpdateDisplay(float delta)
    {
        _elapsedTime += delta;
        if (_elapsedTime >= displayDuration)
        {
            HideEvent();
        }
        else
        {
            DisplayEvent();
        }
    }
}
