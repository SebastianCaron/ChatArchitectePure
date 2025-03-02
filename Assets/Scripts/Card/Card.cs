using System;
using UnityEngine;

public class Card
{
    private CardData _definition = null;
    
    private String _name;
    private String _description;
    private CardTypeEnum _type;
    private int _price;
    private float _life;
    private float _damage;
    private int _production;
    private float _delay;

    private Player _allegeance = null;
    private bool _isBuyable = true;

    private float _timeElapsed = 0.0f;

    public Card() { }

    public Card(CardData definition)
    {
        this._definition = definition;
        SetDefinition(definition);
    }
    
    public void SetDefinition(CardData definition)
    {
        this._definition = definition;
        this._name = definition.Name;
        this._description = definition.description;
        this._price = definition.price;
        this._life = definition.life;
        this._damage = definition.damage;
        this._production = definition.production;
        this._delay = definition.delay;
    }

    public CardData GetDefinition()
    {
        return this._definition;
    }

    public void SetPrice(int price)
    {
        this._price = price;
    }

    public int GetPrice()
    {
        return this._price;
    }

    public void SetLife(float life)
    {
        this._life = life;
    }

    public float GetLife()
    {
        return this._life;
    }

    public bool IsDead()
    {
        return this._life <= 0;
    }

    public bool IsBuyableBy(Player player)
    {
        return (this._allegeance != player && this._isBuyable);
    }

    public bool IsBuyable()
    {
        return this._isBuyable;
    }

    public void SetBuyable(bool isBuyable)
    {
        this._isBuyable = isBuyable;
    }

    public float GetDamage()
    {
        return this._damage;
    }

    public int GetProduction()
    {
        return this._production;
    }

    public void SetProduction(int production)
    {
        this._production = production;
    }
    
    public override string ToString()
    {
        return "Name : " + _name + "\nDescription : " + _description;
    }
    
    public void UpdateCardDelay(float deltaTime)
    {
        _timeElapsed += deltaTime;
    }

    public bool IsEffectUsable()
    {
        return _timeElapsed >= _delay;
    }

    public void ResetDelay()
    {
        _timeElapsed = 0;
    }

    public void SetAllegeance(Player player)
    {
        this._allegeance = player;
    }

    public Player GetAllegeance()
    {
        return this._allegeance;
    }
}
