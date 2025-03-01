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
            if (card.GetDefinition().behaviours.Contains(CardBehaviourEnum.DUPLICATE) ||
                card.GetDefinition().behaviours.Contains(CardBehaviourEnum.FREEZE) ||
                card.GetDefinition().behaviours.Contains(CardBehaviourEnum.FREEZE_PLAYER) ||
                card.GetDefinition().behaviours.Contains(CardBehaviourEnum.DAMAGE_NEIGHBOUR))
            {
                return true;
            }
        }

        if (card.GetDefinition().type == CardTypeEnum.BUILDING) return false;
        
        if (card.GetDefinition().behaviours.Contains(CardBehaviourEnum.HEAL) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.DUPLICATE) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.STEAL) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.PROTECT) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.FREEZE_FUNCTION) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.DAMAGE_OVERTIME) ||
            card.GetDefinition().behaviours.Contains(CardBehaviourEnum.DOUBLE_PRODUCTION))
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
