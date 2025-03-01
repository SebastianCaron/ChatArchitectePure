using System;
using UnityEngine;
using UnityEngine.UI;

public class LandCardDisplayer : MonoBehaviour, ICardDisplayer, ISelectable
{
    
    [SerializeField] private GameObject display;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Slider slider;
    [SerializeField] private SpriteRenderer colorRenderer;

    private Card _card;
    private Color _oldColor;

    public void SetCard(Card card)
    {
        this._card = card;
        RefreshDisplay();
    }

    public Card GetCard()
    {
        return this._card;
    }

    public void HidePrice()
    {
        
    }

    public void RefreshDisplay()
    {
        if (_card == null)
        {
            display.SetActive(false);
        }
        display.SetActive(true);
        spriteRenderer.sprite = _card.GetDefinition().sprite;
        slider.value = _card.GetLife();
    }

    public void SetSelected(bool isSelected, Color selectionColor)
    {
        if (isSelected)
        {
            _oldColor = colorRenderer.color;
            colorRenderer.color = selectionColor;
        }
        else
        {
            colorRenderer.color = _oldColor;
        }
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }
    
    public bool IsBuyableBy(Player player)
    {
        if (this._card == null) return false;
        return this._card.IsBuyableBy(player);
    }
    public bool IsBuyable()
    {
        if (this._card == null) return false;
        return this._card.IsBuyable();
    }

    public ICardDisplayer GetDisplayer()
    {
        return this;
    }
    
    public bool IsSelectable()
    {
        return (this._card != null);
    }
}
