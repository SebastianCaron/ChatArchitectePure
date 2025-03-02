using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Selecter : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private Land land;
    [SerializeField] private Player player;
    [SerializeField] private Color color;
    
    [SerializeField] private KeyCode keyUp = KeyCode.UpArrow;
    [SerializeField] private KeyCode keyDown = KeyCode.DownArrow;
    [SerializeField] private KeyCode keyLeft = KeyCode.LeftArrow;
    [SerializeField] private KeyCode keyRight = KeyCode.RightArrow;
    [SerializeField] private KeyCode keyTrigger = KeyCode.Space;

    [SerializeField] private GameObject[] groupsOfSelectables;
    
    private Dictionary<GameObject, List<List<ISelectable>>> _selectables = new Dictionary<GameObject, List<List<ISelectable>>>();
    private Dictionary<GameObject, List<GameObject>> _behaviours = new Dictionary<GameObject, List<GameObject>>();
    private List<ISelectable> _selection = new List<ISelectable>();

    private void GetSelectables()
    {
        foreach (GameObject group in groupsOfSelectables)
        {
            if (group == null) return;

            List<ISelectable> children = new List<ISelectable>();
            foreach (Transform child in group.transform)
            {
                ISelectable selectable = child.gameObject.GetComponent<HandCardDisplayer>();
                if (selectable != null) children.Add(selectable);
                
                selectable = child.gameObject.GetComponent<LandCardDisplayer>();
                if (selectable != null) children.Add(selectable);
            }

            List<List<ISelectable>> grid = children
                .GroupBy(child => Mathf.Round(child.GetGameObject().transform.position.y * 100f) / 100f)
                .OrderBy(grp => grp.Key)
                .Select(grp => grp.OrderBy(child => child.GetGameObject().transform.position.x).ToList())
                .ToList();

            _selectables[group] = grid;
        }

        foreach (GameObject group in groupsOfSelectables)
        {
            if (group == null) continue;

            List<GameObject> behaviour = new List<GameObject>();
            GameObject nearLeft = null, nearUp = null, nearRight = null, nearDown = null;
            float dLeft = Mathf.Infinity, dUp = Mathf.Infinity, dRight = Mathf.Infinity, dDown = Mathf.Infinity;
            
            Bounds groupBounds = Utils.GetGlobalBounds(group);

            foreach (GameObject other in groupsOfSelectables)
            {
                if (group == other || other == null) continue;

                Bounds otherBounds = Utils.GetGlobalBounds(other);

                float upDistance = otherBounds.min.y - groupBounds.max.y;
                float downDistance = groupBounds.min.y - otherBounds.max.y;
                float leftDistance = groupBounds.min.x - otherBounds.max.x;
                float rightDistance = otherBounds.min.x - groupBounds.max.x;

                // UP
                if (upDistance > 0 && upDistance < dUp)
                {
                    dUp = upDistance;
                    nearUp = other;
                }
                // DOWN
                else if (downDistance > 0 && downDistance < dDown)
                {
                    dDown = downDistance;
                    nearDown = other;
                }

                // LEFT
                else if (leftDistance > 0 && leftDistance < dLeft)
                {
                    dLeft = leftDistance;
                    nearLeft = other;
                }

                // RIGHT
                else if (rightDistance > 0 && rightDistance < dRight)
                {
                    dRight = rightDistance;
                    nearRight = other;
                }
            }

            behaviour.Add(nearLeft);
            behaviour.Add(nearUp);
            behaviour.Add(nearRight);
            behaviour.Add(nearDown);

            _behaviours[group] = behaviour;
        }
    }



    private int _ySelection = 0;
    private int _xSelection = 0;
    private GameObject _current;
    private ISelectable _currentSelectable;
    private void SelectInThisZone(GameObject group, Directions direction)
    {
        if (!_selectables.ContainsKey(group) || _selectables[group] == null || _selectables[group].Count == 0)
        {
            Debug.LogError($"Sélection invalide : pas d'éléments dans _selectables pour {group.name}");
            return;
        }
        
        _selectables[group][_ySelection][_xSelection].SetSelected(false, Color.black);
        switch (direction)
        {
            case Directions.UP:
                _ySelection++;
                break;
            case Directions.DOWN:
                _ySelection--;
                break;
            case Directions.LEFT:
                _xSelection--;
                break;
            case Directions.RIGHT:
                _xSelection++;
                break;
            default:
                break;
        }
        
        // UP
        if (_ySelection >= _selectables[group].Count)
        {
            _ySelection = 0;
            GameObject next = _behaviours[group][1];
            if (next == null) next = group;
            _xSelection =
                Math.Clamp(
                    _xSelection * (_selectables[group][_selectables[group].Count - 1].Count / _selectables[next][0].Count)
                    , 0, _selectables[next][0].Count - 1);
            SelectInThisZone(next, Directions.NODIRECTION);
            return;
        }
        // DOWN
        if (_ySelection < 0)
        {
            GameObject next = _behaviours[group][3];
            if (next == null) next = group;
            _ySelection = _selectables[next].Count - 1;
            _xSelection =
                Math.Clamp(
                    _xSelection * (_selectables[group][0].Count / _selectables[next][_ySelection].Count)
                    , 0, _selectables[next][_ySelection].Count-1);
            SelectInThisZone(next, Directions.NODIRECTION);
            return;
        }
        // LEFT
        if (_xSelection < 0)
        {
            GameObject next = _behaviours[group][0];
            if (next == null) next = group;
            _ySelection =
                Math.Clamp(
                    _ySelection * (_selectables[group].Count / _selectables[next].Count),
                    0, _selectables[next].Count-1);
            _xSelection = _selectables[next][_ySelection].Count - 1;
            SelectInThisZone(next, Directions.NODIRECTION);
            return;
        }
        
        // RIGHT
        if (_xSelection >= _selectables[group][_ySelection].Count)
        {
            GameObject next = _behaviours[group][2];
            if (next == null) next = group;
            _ySelection =
                Math.Clamp(
                    _ySelection * (_selectables[group].Count / _selectables[next].Count),
                    0, _selectables[next].Count-1);
            _xSelection = 0;
            SelectInThisZone(next, Directions.NODIRECTION);
            return;
        }

        _current = group;
        _selectables[group][_ySelection][_xSelection].SetSelected(true, color);
        _currentSelectable = _selectables[group][_ySelection][_xSelection];
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(keyRight))
        {
            SelectInThisZone(_current, Directions.RIGHT);
        }
        else if (Input.GetKeyDown(keyLeft))
        {
            SelectInThisZone(_current, Directions.LEFT);
        }
        else if (Input.GetKeyDown(keyUp))
        {
            SelectInThisZone(_current, Directions.UP);
        }
        else if (Input.GetKeyDown(keyDown))
        {
            SelectInThisZone(_current, Directions.DOWN);
        }

        if (Input.GetKeyDown(keyTrigger))
        {
            //Debug.Log("KEY TRIGGER !");
            _selection.Add(_currentSelectable);
            HandleSelection();
        }
        
        
    }

    public void Init()
    {
        GetSelectables();
        if (groupsOfSelectables.Length == 0 || groupsOfSelectables[0] == null)
        {
            Debug.LogError("groupsOfSelectables est vide ou mal assigné !");
        }
        else
        {
            _current = groupsOfSelectables[0];
            SelectInThisZone(_current, Directions.NODIRECTION);
            //Debug.Log($"Sélection initiale : {_current.name}");
        }
    }

    private void HandleSelection()
    {
        if (!_selection[0].IsSelectable() || (_selection[0].GetPlayer() != null && _selection[0].GetPlayer() != player))
        {
            _selection.Clear();
            return;
        }
        
        switch (_selection.Count)
        {
            case 0:
                return;
            case 1:
            {
                if (_selection[0].IsBuyable())
                {
                    if(_selection[0].IsBuyableBy(player)) player.GetShop().Buy(_selection[0].GetDisplayer(), this.player);
                    _selection.Clear();
                }
            }
                break;
            case 2:
                {
                    // CHECK IF FST == ATCK && SND == BUILDING
                    // CHECK IF FST == SPRT && SND == BUILDING
                    // CHECK IF FST == BUILDING && SND == EMPTY LAND   => CARD LAND MANAGER
                    if (_selection[0].GetDisplayer().GetCard().GetDefinition().type == CardTypeEnum.BUILDING &&
                        _selection[0].IsInHand() &&
                        _selection[1].IsEmptyLand() && _selection[1].GetPlayer() == player)
                    {
                        Card card = _selection[0].GetDisplayer().GetCard();
                        _selection[0].GetDisplayer().SetCard(null);
                        player.GetHand().RefreshHandData();
                        _selection[1].GetManager().AddCard(card);
                        _selection.Clear();
                        return;
                    }
                    else if (_selection[0].GetDisplayer().GetCard().GetDefinition().type == CardTypeEnum.ATTACK &&
                             _selection[0].IsInHand() &&
                             !_selection[1].IsInHand())
                    {
                        // TYPE CHAT TOUILLE
                        if (_selection[0].GetDisplayer().GetCard().GetDefinition().behaviour == CardBehaviourEnum.FREEZE_PLAYER)
                        {
                            _selection[1].GetPlayer().SetFreeze(_selection[0].GetDisplayer().GetCard().GetLife());
                            _selection[0].GetDisplayer().SetCard(null);
                            player.GetHand().RefreshHandData();
                            _selection.Clear();
                            return;
                        }
                        
                        Card card = _selection[0].GetDisplayer().GetCard();
                        _selection[0].GetDisplayer().SetCard(null);
                        player.GetHand().RefreshHandData();
                        _selection[1].GetManager().AddCard(card);
                        _selection.Clear();
                        return;
                    }
                    else if (_selection[0].GetDisplayer().GetCard().GetDefinition().type == CardTypeEnum.SUPPORT &&
                             _selection[0].IsInHand() &&
                             !_selection[1].IsInHand())
                    {
                        Card card = _selection[0].GetDisplayer().GetCard();
                        _selection[0].GetDisplayer().SetCard(null);
                        player.GetHand().RefreshHandData();
                        _selection[1].GetManager().AddCard(card);
                        _selection.Clear();
                        return;
                    }else if (_selection[0].GetDisplayer().GetCard().GetDefinition().behaviour == CardBehaviourEnum.MOVE)
                    {
                        break;
                    }
                    else
                    {
                        _selection.Clear();
                    }
                    
                    // ELSE POP SELECTION
                }
                break;
            default:
                _selection.Clear();
                break;
        }
        
        
    }

    private void Update()
    {
        if(!player.CanMove()) return;
        HandleInput();
    }
}
