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
    private LandCardManager _manager;

    private void Awake()
    {
        this._manager = this.gameObject.GetComponent<LandCardManager>();
        this._oldColor = colorRenderer.color;
    }

    public void SetCard(Card card)
    {
        this._card = card;
        if(this._card != null) this.slider.maxValue = this._card.GetDefinition().life;
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
            return;
        }
        display.SetActive(true);
        spriteRenderer.sprite = _card.GetDefinition().sprite;
        slider.value = _card.GetLife();
    }

    public void SetSelected(bool isSelected, Color selectionColor)
    {
        if (isSelected)
        {
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

    public bool IsEmptyLand()
    {
        return this._card == null;
    }

    public LandCardManager GetManager()
    {
        return this._manager;
    }

    public bool IsInHand()
    {
        return false;
    }
    
    public void SetPlayer(Player player)
    {
        this._manager.SetPlayer(player);
    }

    public Player GetPlayer()
    {
        return this._manager.GetPlayer();
    }
}
