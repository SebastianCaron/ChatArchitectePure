using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RandomAgentNoSelecter : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float delay;
    [SerializeField] private GameControlller controlller;

    private float _timeElapsed = 0.0f;
    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > delay)
        {
            if(player.CanMove()) Action();
            _timeElapsed = 0;
        }
    }

    private void Action()
    {
        BuyCard();
        PlayCards();
    }

    private void BuyCard()
    {
        if(player.GetHand().IsHandFull()) return;
        Shop shop = player.GetShop();
        List<ICardDisplayer> cards = shop.GetCards();
        List<ICardDisplayer> cardsUsed = shop.GetCardsUsed();

        List<ICardDisplayer> production = new List<ICardDisplayer>();
        List<ICardDisplayer> damages = new List<ICardDisplayer>();
        List<ICardDisplayer> protection = new List<ICardDisplayer>();
        List<ICardDisplayer> building = new List<ICardDisplayer>();
        
        foreach (ICardDisplayer cardDisplayer in cards)
        {
            Card card = cardDisplayer.GetCard();
            if(card == null) continue;
            if (card.GetDefinition().type == CardTypeEnum.BUILDING)
            {
                if (card.GetDefinition().behaviour == CardBehaviourEnum.PRODUCTION_GOLD)
                {
                    production.Add(cardDisplayer);
                }
                else
                {
                    building.Add(cardDisplayer);
                }
            }

            if (card.GetDefinition().type == CardTypeEnum.ATTACK)
            {
                damages.Add(cardDisplayer);
            }

            if (card.GetDefinition().type == CardTypeEnum.SUPPORT)
            {
                protection.Add(cardDisplayer);
            }
        }
        
        foreach (ICardDisplayer cardDisplayer in cardsUsed)
        {
            Card card = cardDisplayer.GetCard();
            if(card == null || card.GetAllegeance() == player) continue;
            if (card.GetDefinition().type == CardTypeEnum.BUILDING)
            {
                if (card.GetDefinition().behaviour == CardBehaviourEnum.PRODUCTION_GOLD)
                {
                    production.Add(cardDisplayer);
                }
                else if (card.GetDefinition().behaviour == CardBehaviourEnum.DAMAGE_NEIGHBOUR)
                {
                    damages.Add(cardDisplayer);
                }
                else
                {
                    building.Add(cardDisplayer);
                }
            }

            if (card.GetDefinition().type == CardTypeEnum.ATTACK)
            {
                damages.Add(cardDisplayer);
            }

            if (card.GetDefinition().type == CardTypeEnum.SUPPORT)
            {
                protection.Add(cardDisplayer);
            }
        }
        
        
        
        // TODO : SORT LIST
        
        while (production.Count > 0 && !player.GetHand().IsHandFull())
        {
            shop.Buy(production[0], player);
            production.Remove(production[0]);
        }
        while (building.Count > 0 && !player.GetHand().IsHandFull())
        {
            shop.Buy(building[0], player);
            building.Remove(building[0]);
        }
        while (damages.Count > 0 && player.GetHand().NbCards() <= 1)
        {
            shop.Buy(damages[0], player);
            damages.Remove(damages[0]);
        }
        while (protection.Count > 0 && !player.GetHand().IsHandFull())
        {
            shop.Buy(protection[0], player);
            protection.Remove(protection[0]);
        }
        
    }

    private void PlayCards()
    {
        if (player.GetHand().NbCards() == 0) return;
        List<LandCardManager> lands = GetAllLands();

        List<LandCardManager> ennemies = new List<LandCardManager>();
        List<LandCardManager> ally = new List<LandCardManager>();
        List<LandCardManager> nothing = new List<LandCardManager>();
        foreach (LandCardManager landCardManager in lands)
        {
            if(landCardManager == null) continue;

            Card building = landCardManager.GetBuilding();
            if (building != null)
            {
                if (building.GetAllegeance() == player && building.GetProduction() >= 0)
                {
                    ally.Add(landCardManager);
                }
                else
                {
                    ennemies.Add(landCardManager);
                }
            }
            else
            {
                nothing.Add(landCardManager);
            }
            
        }

        ICardDisplayer[] cardDisplayers = player.GetHand().GetCardDisplayers();
        foreach (ICardDisplayer cardDisplayer in cardDisplayers)
        {
            if(cardDisplayer == null) continue;
            Card card = cardDisplayer.GetCard();
            if(card == null) continue;
            
            switch (card.GetDefinition().type)
            {
                case CardTypeEnum.BUILDING:
                    if (nothing.Count > 0)
                    {
                        // BAD IMP
                        // TODO : DO BETTER
                        nothing[0].AddCard(card);
                        nothing.Remove(nothing[0]);
                        cardDisplayer.SetCard(null);
                        player.GetHand().RefreshHandData();
                    }
                    break;
                case CardTypeEnum.ATTACK:
                    if (ennemies.Count > 0)
                    {
                        ennemies[0].AddCard(card);
                        ennemies.Remove(ennemies[0]);
                        cardDisplayer.SetCard(null);
                        player.GetHand().RefreshHandData();
                    }
                    break;
                case CardTypeEnum.SUPPORT:
                    if (ally.Count > 0)
                    {
                        ally[0].AddCard(card);
                        ally.Remove(ally[0]);
                        cardDisplayer.SetCard(null);
                        player.GetHand().RefreshHandData();
                    }
                    break;
            }
        }
    }

    private List<LandCardManager> GetAllLands()
    {
        List<LandCardManager> landCardManagers = new List<LandCardManager>();
        foreach(Player _player in controlller.GetPlayers())
        {
            if(_player == null) continue;
            Land land = _player.GetLand();
            foreach (LandCardManager landCardManager in land.GetLandCardManagers())
            {
                if(landCardManager == null) continue;
                landCardManagers.Add(landCardManager);
            }
        }

        return landCardManagers;
    }
}
