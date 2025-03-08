using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICardDisplayer : MonoBehaviour, ICardDisplayer
{
    [SerializeField] private GameObject priceDisplayer;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private Image spriteRenderer;
    [SerializeField] private Image colorRenderer;
    
    private Card _card;
    
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
            return;
        }

        priceDisplayer.SetActive(true);
        priceText.SetText(_card.GetPrice().ToString());
        nameText.SetText(_card.GetDefinition().Name);
        descriptionText.SetText(_card.GetDefinition().description);
        spriteRenderer.sprite = _card.GetDefinition().sprite;
        colorRenderer.color = _card.GetDefinition().type.GetColorByType();
    }

    public Player GetPlayer()
    {
        return null;
    }

    public void SetPlayer(Player player)
    {
        return;
    }


    public GameObject GetGameObject()
    {
        return this.gameObject;
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
        return false;
    }

    public LandCardManager GetManager()
    {
        return null;
    }
    
    public bool IsInHand()
    {
        return true;
    }
    
}
