using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandCardManager : MonoBehaviour
{
    private List<Card> _cardOnLand = new List<Card>();
    private LandCardDisplayer _cardDisplayer;
    private LandCardManager[] _neighbours = new LandCardManager[2];
    private bool _isProtected = false;
    private Player _player;

    private void Awake()
    {
        _cardDisplayer = this.gameObject.GetComponent<LandCardDisplayer>();
    }

    public void UpdateLand(float deltaTime)
    {
        if (_cardOnLand.Count <= 0) return;

        float time = deltaTime;
        //Debug.Log(deltaTime);
        if (_cardOnLand[0].GetLife() < time)
        {
            time -= _cardOnLand[0].GetLife();
            _cardOnLand.RemoveAt(0);
            UpdateDisplay();
            UpdateLand(time);
            return;
        }
        _cardOnLand[0].SetLife(_cardOnLand[0].GetLife() - time);
        UpdateDisplay();
    }

    public void AddCard(Card card)
    {
        if (!CanBePlayed(card))
        {
            _player.GetHand().AddToHand(card);
            return;
        }

        // TYPE CHAT INGENIEUR
        if (card.GetDefinition().behaviour == CardBehaviourEnum.HEAL &&
            card.GetDefinition().type == CardTypeEnum.SUPPORT &&
            ContainsBuilding())
        {
            Card building = GetBuilding();
            building.SetLife(building.GetLife() + card.GetDamage());
        }
        // TYPE CHAT-MIKAZE 
        if (card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE &&
            card.GetDefinition().type == CardTypeEnum.ATTACK &&
            ContainsBuilding())
        {
            Card building = GetBuilding();
            building.SetLife(building.GetLife() - card.GetDamage());
        }
        
        _cardOnLand.Insert(0, card);
        UpdateDisplay();
    }

    public void MakeDamage(float damage)
    {
        if (_cardOnLand.Count <= 0) return;
        
        UpdateLand(damage);
    }

    public bool CanBePlayed(Card card)
    {
        if (!ContainsBuilding())
        {
            if (card.GetDefinition().type == CardTypeEnum.BUILDING)
            {
                return true;
            }
            if (card.GetDefinition().behaviour == CardBehaviourEnum.DUPLICATE ||
                card.GetDefinition().behaviour == CardBehaviourEnum.FREEZE ||
                card.GetDefinition().behaviour == CardBehaviourEnum.FREEZE_PLAYER ||
                card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE_NEIGHBOUR)
            {
                return true;
            }

            return false;
        }

        if (card.GetDefinition().type == CardTypeEnum.BUILDING) return false;
        
        if (card.GetDefinition().behaviour == CardBehaviourEnum.HEAL ||
            card.GetDefinition().behaviour == CardBehaviourEnum.DUPLICATE ||
            card.GetDefinition().behaviour == CardBehaviourEnum.STEAL ||
            card.GetDefinition().behaviour == CardBehaviourEnum.PROTECT ||
            card.GetDefinition().behaviour == CardBehaviourEnum.FREEZE_FUNCTION ||
            card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE ||
            card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE_OVERTIME ||
            card.GetDefinition().behaviour == CardBehaviourEnum.DOUBLE_PRODUCTION)
        {
            return true;
        }
        
        return false;
    }

    public bool ContainsBuilding()
    {
        foreach (Card card in _cardOnLand)
        {
            if (card.GetDefinition().type == CardTypeEnum.BUILDING) return true;
        }

        return false;
    }

    public Card GetBuilding()
    {
        foreach (Card card in _cardOnLand)
        {
            if (card.GetDefinition().type == CardTypeEnum.BUILDING) return card;
        }

        return null;
    }

    public bool IsProtected()
    {
        return this._isProtected;
    }

    public void SetProtected(bool isProtected)
    {
        this._isProtected = isProtected;
    }

    private void UpdateDisplay()
    {
        bool isSet = false;
        for (int i = 0; i < _cardOnLand.Count; i++)
        {
            if (_cardOnLand[i].GetDefinition().type == CardTypeEnum.BUILDING)
            {
                isSet = true;
                _cardDisplayer.SetCard(_cardOnLand[i]);
            }
        }

        if (!isSet)
        {
            _cardDisplayer.SetCard(null);
            return;
        }
        _cardDisplayer.RefreshDisplay();
    }

    public void SetPlayer(Player player)
    {
        this._player = player;
    }

    public Player GetPlayer()
    {
        return this._player;
    }
}
