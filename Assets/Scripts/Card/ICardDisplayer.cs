using UnityEngine;

public interface ICardDisplayer
{
    public void SetCard(Card card);
    public Card GetCard();
    public void HidePrice();
    public void RefreshDisplay();

    public Player GetPlayer();
    public void SetPlayer(Player player);
}
