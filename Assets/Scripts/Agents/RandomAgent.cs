using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class RandomAgent : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Selecter selecter;
    [SerializeField] private float delay;
    [SerializeField] private GameControlller controller;
    [SerializeField] private SelecterGraphGenerator graphGenerator;

    private Dictionary<ISelectable, List<ISelectable>> _graph;
    private Shop _shop;

    private bool _buyMode = true;

    private ISelectable _start;

    private ISelectable _from;
    private ISelectable _target;
    private bool _selected = false;
    private int _actd = 0;
    private List<Directions> _actions = new List<Directions>();
    
    public void Init()
    {
        graphGenerator.SetGroupeOfSelectables(selecter.GetGroupOfSelectables());
        _graph = graphGenerator.InitGrid();
        _shop = controller.GetShop();
    }

    private float _timeElapsed = 0.0f;
    private void Update()
    {
        _timeElapsed += Time.deltaTime;
        if (_timeElapsed > delay)
        {
            _start = selecter.GetPositionSelectable();
            if(player.CanMove()) Action();
            _timeElapsed = 0;
        }
    }
    
    private void Action()
    {

        if (_buyMode)
        {
            // FIND BUILDINGS
            // LIMIT 2 ATTCK CARDS
            if (!BuyCard())
            {
                _buyMode = false;
                _actd = 0;
            }
        }
        else
        {
            // SELECT CARD
            // GO TO FIRST LAND AVAILABLE

            if (_actd >= 20)
            {
                Debug.Log("RESET!");
                _from = null;
                _target = null;
                _selected = false;
                _actd = 0;
                _buyMode = true;
                return;
            }
            
            if (_from == null && _target == null && !SetDownCard())
            {
                _buyMode = true;
                return;
            }
            else
            {
                if (!_selected)
                {
                    GoToFrom();
                }
                else
                {
                    GoToTarget();
                }

                _actd++;
            }
        }
    }

    private bool BuyCard()
    {
        if(player.GetHand().IsHandFull()) return false;
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
        
        if (production.Count > 0 && !player.GetHand().IsHandFull())
        {
            List<Directions> list = Dijkstra(_start, production[0].GetGameObject().GetComponent<HandCardDisplayer>(), _graph);
            if (list[0] == Directions.NODIRECTION)
            {
                selecter.Select();
            }
            else
            {
                selecter.GoDirection(list[0]);
            }

            return true;
        }
        while (building.Count > 0 && !player.GetHand().IsHandFull())
        {
            List<Directions> list = Dijkstra(_start, building[0].GetGameObject().GetComponent<HandCardDisplayer>(), _graph);
            if (list[0] == Directions.NODIRECTION)
            {
                selecter.Select();
            }
            else
            {
                selecter.GoDirection(list[0]);
            }

            return true;
        }
        while (damages.Count > 0 && player.GetHand().NbCards() <= 1)
        {
            List<Directions> list = Dijkstra(_start, damages[0].GetGameObject().GetComponent<HandCardDisplayer>(), _graph);
            if (list[0] == Directions.NODIRECTION)
            {
                selecter.Select();
            }
            else
            {
                selecter.GoDirection(list[0]);
            }

            return true;
        }
        while (protection.Count > 0 && !player.GetHand().IsHandFull())
        {
            List<Directions> list = Dijkstra(_start, protection[0].GetGameObject().GetComponent<HandCardDisplayer>(), _graph);
            if (list[0] == Directions.NODIRECTION)
            {
                selecter.Select();
            }
            else
            {
                selecter.GoDirection(list[0]);
            }

            return true;
        }

        return false;
    }

    private void GoToFrom()
    {
        List<Directions> dir = Dijkstra(_start, _from, _graph);
        if (dir[0] == Directions.NODIRECTION)
        {
            selecter.Select();
            _selected = true;
        }
        else
        {
            selecter.GoDirection(dir[0]);
        }
    }

    private void GoToTarget()
    {
        List<Directions> dir = Dijkstra(_start, _target, _graph);
        if (dir[0] == Directions.NODIRECTION)
        {
            selecter.Select();
            _selected = false;
            _from = null;
            _target = null;
        }
        else
        {
            selecter.GoDirection(dir[0]);
        }
    }
    
    private void ExecAction()
    {
        if (_actions.Count == 0) return;

        if (_actions[0] == Directions.NODIRECTION)
        {
            selecter.Select();
        }
        else
        {
            selecter.GoDirection(_actions[0]);
        }
        _actions.RemoveAt(0);
    }
    private bool SetDownCard()
    {
        ISelectable[] selectablesCards = player.GetHand().GetCardSelectable();
        if (player.GetHand().NbCards() == 0) return false;
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

        ISelectable[] cardDisplayers = player.GetHand().GetCardSelectable();
        foreach (ISelectable cardDisplayerS in cardDisplayers)
        {
            if(cardDisplayerS == null) continue;
            ICardDisplayer cardDisplayer = cardDisplayerS.GetDisplayer();
            if(cardDisplayer == null) continue;
            Card card = cardDisplayer.GetCard();
            if(card == null) continue;
            
            switch (card.GetDefinition().type)
            {
                case CardTypeEnum.BUILDING:
                    if (nothing.Count > 0)
                    {
                        _from = cardDisplayer.GetSelectable();
                        _target = nothing[0].GetCardDisplayer().GetSelectable();
                        return true;
                    }
                    break;
                case CardTypeEnum.ATTACK:
                    if (ennemies.Count > 0)
                    {
                        _from = cardDisplayer.GetSelectable();
                        _target = ennemies[0].GetCardDisplayer().GetSelectable();
                        return true;
                    }
                    break;
                case CardTypeEnum.SUPPORT:
                    if (ally.Count > 0)
                    {
                        _from = cardDisplayer.GetSelectable();
                        _target = ally[0].GetCardDisplayer().GetSelectable();
                        return true;
                    }
                    break;
            }
        }
        
        return false;
    }

    private void AddToActions(List<Directions> actionsList)
    {
        //Debug.Log("START");
        //Debug.Log(actionsList.Count);
        for (int i = 0; i < actionsList.Count; i++)
        {
            //Debug.Log(actionsList[i]);
            _actions.Add(actionsList[i]);
        }
        //Debug.Log("END");
    }
    
    private List<LandCardManager> GetAllLands()
    {
        List<LandCardManager> landCardManagers = new List<LandCardManager>();
        foreach(Player _player in controller.GetPlayers())
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
    
    public List<Directions> Dijkstra(ISelectable start, ISelectable end, Dictionary<ISelectable, List<ISelectable>> graph)
    {
        Dictionary<ISelectable, float> distances = new Dictionary<ISelectable, float>();
        Dictionary<ISelectable, ISelectable> previous = new Dictionary<ISelectable, ISelectable>();
        HashSet<ISelectable> visited = new HashSet<ISelectable>();
        SortedDictionary<float, List<ISelectable>> queue = new SortedDictionary<float, List<ISelectable>>();

        // Initialisation des distances
        foreach (var node in graph.Keys)
        {
            distances[node] = float.MaxValue;
            previous[node] = null;
        }

        distances[start] = 0;
        AddToQueue(queue, 0, start);

        while (queue.Count > 0)
        {
            var firstKey = queue.Keys.GetEnumerator();
            firstKey.MoveNext();
            float currentDistance = firstKey.Current;

            ISelectable current = queue[currentDistance][0];
            queue[currentDistance].RemoveAt(0);
            if (queue[currentDistance].Count == 0)
                queue.Remove(currentDistance);

            if (current == end) break;
            if (visited.Contains(current)) continue;

            visited.Add(current);

            foreach (var neighbor in graph[current])
            {
                if (neighbor == null || visited.Contains(neighbor)) continue;

                float newDist = distances[current] + 1;

                if (newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                    AddToQueue(queue, newDist, neighbor);
                }
            }
        }

        // Reconstruction du chemin
        List<Directions> path = new List<Directions>();
        ISelectable step = end;
        path.Add(Directions.NODIRECTION);

        while (step != null && previous[step] != null)
        {
            ISelectable prev = previous[step];
            if (prev == graph[step][0])
            {
                path.Add(Directions.RIGHT);
            }else if (prev == graph[step][1])
            {
                path.Add(Directions.DOWN);
            }
            else if (prev == graph[step][2])
            {
                path.Add(Directions.LEFT);
            }
            else if (prev == graph[step][3])
            {
                path.Add(Directions.UP);
            }
            step = prev;
        }

        if (path.Count == 1)
        {
            if (start != end)
            {
                if (end == graph[start][0])
                {
                    path.Add(Directions.LEFT);
                }else if (end == graph[start][1])
                {
                    path.Add(Directions.UP);
                }
                else if (end == graph[start][2])
                {
                    path.Add(Directions.RIGHT);
                }
                else if (end == graph[start][3])
                {
                    path.Add(Directions.DOWN);
                }
            }
        }

        //Debug.Log("START FROM " + start.GetGameObject().name + " TO " + end.GetGameObject().name);
        path.Reverse();
        for (int i = 0; i < path.Count; i++)
        {
            //Debug.Log(path[i]);
        }
        //Debug.Log("END");
        return path;
    }
    
    private void AddToQueue(SortedDictionary<float, List<ISelectable>> queue, float key, ISelectable value)
    {
        if (!queue.ContainsKey(key))
        {
            queue[key] = new List<ISelectable>();
        }
        queue[key].Add(value);
    }
    
}
