using TMPro;
using UnityEngine;

public class HandCardDisplayer : MonoBehaviour, ICardDisplayer, ISelectable
{
    [SerializeField] private GameObject priceDisplayer;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer colorRenderer;
    
    private Card _card;
    private bool _isSelected = false;
    private Color _selectionColor;
    
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
        priceDisplayer.SetActive(false);
    }

    public void RefreshDisplay()
    {
        if (_card == null || _card.GetDefinition() == null)
        {
            priceDisplayer.gameObject.SetActive(false);
            priceText.SetText("0");
            nameText.SetText("");
            spriteRenderer.sprite = null;
            colorRenderer.color = new Color(0, 0, 0, 0);
            SetSelected(this._isSelected, _selectionColor);
            return;
        }

        priceDisplayer.SetActive(true);
        priceText.SetText(_card.GetPrice().ToString());
        nameText.SetText(_card.GetDefinition().Name);
        spriteRenderer.sprite = _card.GetDefinition().sprite;
        colorRenderer.color = _card.GetDefinition().type.GetColorByType();
        SetSelected(this._isSelected, _selectionColor);
    }

    public void SetSelected(bool isSelected, Color selectionColor)
    {
        this._isSelected = isSelected;
        this._selectionColor = selectionColor;
        if (this._isSelected)
        {
            colorRenderer.color = this._selectionColor;
            return;
        }

        if (_card == null || _card.GetDefinition() == null)
        {
            colorRenderer.color = new Color(0, 0, 0, 0);
            return;
        }
        
        colorRenderer.color = _card.GetDefinition().type.GetColorByType();
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
