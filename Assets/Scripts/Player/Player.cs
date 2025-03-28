using System;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Hand handOfThePlayer;
    [SerializeField] private Land landOfThePlayer;
    [SerializeField] private Selecter selecterOfThePlayer;
    [SerializeField] private TMP_Text goldText;
    
    private Shop _shop;
    private int _goldAmount = 100;
    private float _moveDelay = 0.0f;
    private bool _forfeit = false;

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public void Init()
    {
        handOfThePlayer.SetPlayer(this);
        landOfThePlayer.SetPlayer(this);
        handOfThePlayer.Init();
        landOfThePlayer.Init();
        goldText.SetText(_goldAmount + "€");
    }

    public void InitSelecter()
    {
        selecterOfThePlayer.Init();
    }

    public bool CanMove()
    {
        return this._moveDelay <= 0;
    }

    public void SetShop(Shop shop)
    {
        this._shop = shop;
    }

    public Shop GetShop()
    {
        return this._shop;
    }

    public int GetGold()
    {
        return this._goldAmount;
    }

    public void AddGold(int amount)
    {
        this._goldAmount += amount;
        if (this._goldAmount < 0) _goldAmount = 0;
        RefreshGoldDisplay();
    }

    public void SetGold(int gold)
    {
        this._goldAmount = gold;
        if (this._goldAmount < 0) _goldAmount = 0;
        RefreshGoldDisplay();
    }

    private void RefreshGoldDisplay()
    {
        goldText.SetText(_goldAmount + "€");
    }

    public Hand GetHand()
    {
        return this.handOfThePlayer;
    }

    public Land GetLand()
    {
        return this.landOfThePlayer;
    }
    
    public void UpdatePlayer(float deltaTime)
    {
        if (this._moveDelay > 0) this._moveDelay -= deltaTime;
        
        landOfThePlayer.UpdateLand(deltaTime);
    }

    public void SetFreeze(float amount)
    {
        this._moveDelay = amount;
    }

    public void SetForfeit(bool forfeit)
    {
        this._forfeit = forfeit;
    }

    public bool GetForfeit()
    {
        return this._forfeit;
    }
}
