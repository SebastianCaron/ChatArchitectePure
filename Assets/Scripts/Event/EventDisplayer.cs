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

        if (text != null)
        {
            text.text = _gatoEvento.name;
            text.gameObject.SetActive(true);
        }

        if (image != null)
        {
            image.sprite = _gatoEvento.eventSprite;
            image.gameObject.SetActive(true);
        }
    }

    public void HideEvent()
    {
        if (text != null)
        {
            text.text = "";
            text.gameObject.SetActive(false);
        }

        if (image != null)
        {
            image.sprite = null;
            image.gameObject.SetActive(false);
        }
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
