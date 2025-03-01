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

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public void Init()
    {
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

    public void SetGold(int gold)
    {
        this._goldAmount = gold;
        goldText.SetText(_goldAmount + "€");
    }

    public Hand GetHand()
    {
        return this.handOfThePlayer;
    }
    
    public void UpdatePlayer(float deltaTime)
    {
        if (this._moveDelay > 0) this._moveDelay -= deltaTime;
        
        landOfThePlayer.UpdateLand(deltaTime);
    }
}
