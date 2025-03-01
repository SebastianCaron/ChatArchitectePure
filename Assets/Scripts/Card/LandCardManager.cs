using System;
using System.Collections.Generic;
using UnityEngine;

public class LandCardManager : MonoBehaviour
{
    private List<Card> _cardOnLand = new List<Card>();
    private LandCardDisplayer _cardDisplayer;
    private bool _isProtected = false;

    private void Awake()
    {
        _cardDisplayer = this.gameObject.GetComponent<LandCardDisplayer>();
    }

    public void UpdateLand(float deltaTime)
    {
        if (_cardOnLand.Count <= 0) return;

        float time = deltaTime;
        if (_cardOnLand[0].GetLife() < time)
        {
            time -= _cardOnLand[0].GetLife();
            _cardOnLand.RemoveAt(0);
            UpdateDisplay();
            UpdateLand(time);
            return;
        }
    }

    public void AddCard(Card card)
    {
        _cardOnLand.Insert(0, card);
        UpdateDisplay();
    }

    public void MakeDamage(float damage)
    {
        if (_cardOnLand.Count <= 0) return;
        
        UpdateLand(damage);
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
        for (int i = 0; i < _cardOnLand.Count; i++)
        {
            if (_cardOnLand[i].GetDefinition().type == CardTypeEnum.BUILDING)
            {
                _cardDisplayer.SetCard(_cardOnLand[i]);
            }
        }
    }
}
