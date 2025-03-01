using UnityEngine;

public interface ISelectable
{
    public void SetSelected(bool isSelected, Color selectionColor);
    public GameObject GetGameObject();

    public bool IsBuyableBy(Player player);
    public bool IsBuyable();

    public bool IsSelectable();

    public ICardDisplayer GetDisplayer();

    public bool IsEmptyLand();

    public LandCardManager GetManager();

    public bool IsInHand();

    public Player GetPlayer();
}
