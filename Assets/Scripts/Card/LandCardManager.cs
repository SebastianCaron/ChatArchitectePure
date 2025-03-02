using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandCardManager : MonoBehaviour
{
    private List<Card> _cardOnLand = new List<Card>();
    private LandCardDisplayer _cardDisplayer;
    private List<LandCardManager> _neighbours = new List<LandCardManager>();
    private bool _isProtected = false;
    private bool _isFrozen = false;
    private bool _containsBuilding = false;
    private Player _player;
    private Player _forGoldPlayer;

    private void Awake()
    {
        _cardDisplayer = this.gameObject.GetComponent<LandCardDisplayer>();
    }

    public void UpdateLand(float deltaTime)
    {
        if (_cardOnLand.Count <= 0) return;

        // UPDATE DELAY
        foreach (Card card in _cardOnLand)
        {
            card.UpdateCardDelay(deltaTime);
        }
        // USE EFFECT
        foreach (Card card in _cardOnLand)
        {
            if (card.IsEffectUsable())
            {
                UseEffect(card);
                card.ResetDelay();
            }
        }
        
        // UPDATE LIFE OF CARDS
        foreach (Card card in _cardOnLand)
        {
            card.SetLife(card.GetLife() - deltaTime);
            if (card.IsDead())
            {
                _cardOnLand.Remove(card);
                if (card.GetDefinition().type == CardTypeEnum.BUILDING)
                {
                    UpdateDisplay();
                }
            }
        }
    }

    public void AddCard(Card card)
    {
        if (_isFrozen)
        {
            _player.GetHand().AddToHand(card);
        }

        // TYPE CHAT INGENIEUR
        if (card.GetDefinition().behaviour == CardBehaviourEnum.HEAL &&
            card.GetDefinition().type == CardTypeEnum.SUPPORT &&
            ContainsBuilding())
        {
            Card building = GetBuilding();
            building.SetLife(building.GetLife() + card.GetDamage());
            return;
        }
        // TYPE CHAT-MIKAZE - CHAT RAGE
        if (!_isProtected &&
            card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE &&
            card.GetDefinition().type == CardTypeEnum.ATTACK &&
            ContainsBuilding())
        {
            Card building = GetBuilding();
            building.SetLife(building.GetLife() - card.GetDamage());
            return;
        }
        // TYPE CHAT-ARCHITECTE
        if (card.GetDefinition().behaviour == CardBehaviourEnum.MULTIPLY_PRODUCTION &&
            card.GetDefinition().type == CardTypeEnum.SUPPORT &&
            ContainsBuilding())
        {
            Card building = GetBuilding();
            building.SetProduction(building.GetProduction() * card.GetProduction());
            return;
        }
        // TYPE CHAT-MURAILLE
        if (card.GetDefinition().behaviour == CardBehaviourEnum.PROTECT &&
            card.GetDefinition().type == CardTypeEnum.SUPPORT &&
            ContainsBuilding())
        {
            SetProtected(true);
            _cardOnLand.Insert(0, card);
            _containsBuilding = true;
            UpdateDisplay();
            return;
        }
        // TYPE CHAT-SOEUR
        if (card.GetDefinition().behaviour == CardBehaviourEnum.PROTECT_NEIGHBOUR &&
            card.GetDefinition().type == CardTypeEnum.SUPPORT &&
            !ContainsBuilding())
        {
            foreach (LandCardManager neighbour in _neighbours)
            {
                neighbour.SetProtected(true);
            }
            _cardOnLand.Insert(0, card);
            _containsBuilding = true;
            UpdateDisplay();
            return;
        }
        // TYPE CHAT-RME
        if (card.GetDefinition().behaviour == CardBehaviourEnum.STEAL &&
            card.GetDefinition().type == CardTypeEnum.ATTACK &&
            ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            _forGoldPlayer = card.GetAllegeance();
            return;
        }
        // TYPE CHAT-BYTE
        if (card.GetDefinition().behaviour == CardBehaviourEnum.FREEZE &&
            card.GetDefinition().type == CardTypeEnum.ATTACK &&
            !ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            SetFrozen(true);
            return;
        }
        // TYPE MACHE-CABLE
        if (card.GetDefinition().behaviour == CardBehaviourEnum.FREEZE_FUNCTION &&
            card.GetDefinition().type == CardTypeEnum.ATTACK &&
            ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            SetFrozen(true);
            return;
        }
        
        // TYPE CHAT-MALLOW
        if (card.GetDefinition().behaviour == CardBehaviourEnum.HEAL_NEIGHBOUR &&
            card.GetDefinition().type == CardTypeEnum.BUILDING &&
            !ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            _containsBuilding = true;
            UpdateDisplay();
            return;
        }
        
        // TYPE CHAT-PRODUCTION
        if (card.GetDefinition().behaviour == CardBehaviourEnum.PRODUCTION_GOLD &&
            card.GetDefinition().type == CardTypeEnum.BUILDING &&
            !ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            _containsBuilding = true;
            UpdateDisplay();
            return;
        }
        // TYPE CHAT-LUMEAU
        if (card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE_NEIGHBOUR &&
            card.GetDefinition().type == CardTypeEnum.ATTACK)
        {
            MakeDamage(card.GetDamage());
            foreach (LandCardManager neighbour in _neighbours)
            {
                neighbour.MakeDamage(card.GetDamage());
            }
            return;
        }
        // TYPE LITIERE
        if (card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE_NEIGHBOUR &&
            card.GetDefinition().type == CardTypeEnum.BUILDING &&
            !ContainsBuilding())
        {
            _cardOnLand.Insert(0, card);
            _containsBuilding = true;
            UpdateDisplay();
            return;
        }
        
        _player.GetHand().AddToHand(card);
    }

    public void MakeDamage(float damage)
    {
        if (_cardOnLand.Count <= 0) return;
        if (_isProtected) return;

        Card building = GetBuilding();
        building.SetLife(building.GetLife() - damage);

        if (building.IsDead())
        {
            _cardOnLand.Remove(building);
            OnDeath(building);
        }
        
        UpdateDisplay();
    }

    public bool ContainsBuilding()
    {
        return _containsBuilding;
    }

    public Card GetBuilding()
    {
        if (!_containsBuilding) return null;
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

    public bool IsFrozen()
    {
        return this._isFrozen;
    }

    public void SetFrozen(bool isFrozen)
    {
        this._isFrozen = isFrozen;
    }

    private void UpdateDisplay()
    {
        if (!_containsBuilding) return;
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
        this._forGoldPlayer = player;
    }

    public Player GetPlayer()
    {
        return this._player;
    }

    private void OnDeath(Card card)
    {
        if (card.GetDefinition().type == CardTypeEnum.BUILDING)
        {
            this._containsBuilding = false;
            UpdateDisplay();
        }
        
        switch (card.GetDefinition().behaviour)
        {
            case CardBehaviourEnum.PROTECT:
                SetProtected(false);
                break;
            case CardBehaviourEnum.PROTECT_NEIGHBOUR:
                foreach (LandCardManager neighbour in _neighbours)
                {
                    neighbour.SetProtected(false);
                }
                break;
            case CardBehaviourEnum.STEAL:
                this._forGoldPlayer = _player;
                break;
            case CardBehaviourEnum.FREEZE:
            case CardBehaviourEnum.FREEZE_FUNCTION:
                SetFrozen(false);
                break;
        }
    }

    private void UseEffect(Card card)
    {
        switch (card.GetDefinition().behaviour)
        {
            case CardBehaviourEnum.PRODUCTION_GOLD:
                if(!_isFrozen) _forGoldPlayer.AddGold(card.GetProduction());
                break;
            case CardBehaviourEnum.DAMAGE_OVERTIME:
                MakeDamage(card.GetDamage());
                break;
            case CardBehaviourEnum.DAMAGE_NEIGHBOUR:
                foreach (LandCardManager neighbour in _neighbours)
                {
                    neighbour.MakeDamage(card.GetDamage());
                }
                break;
            case CardBehaviourEnum.HEAL_NEIGHBOUR:
                foreach (LandCardManager neighbour in _neighbours)
                {
                    neighbour.MakeDamage(-card.GetDamage());
                }
                break;
        }
    }

    public void AddNeighbour(LandCardManager cardManager)
    {
        _neighbours.Add(cardManager);
    }
}
