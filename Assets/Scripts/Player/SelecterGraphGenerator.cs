using System.Collections.Generic;
using UnityEngine;

public class SelecterGraphGenerator : MonoBehaviour
{
    [SerializeField] private GameObject[] groupsOfSelectables;

    private List<ISelectable> _selectables = new List<ISelectable>();

    private Dictionary<ISelectable, List<ISelectable>> _graph = new Dictionary<ISelectable, List<ISelectable>>();

    public Dictionary<ISelectable, List<ISelectable>> InitGrid()
    {
        foreach (GameObject group in groupsOfSelectables)
        {
            if (group == null) continue;
            foreach (Transform child in group.transform)
            {
                ISelectable selectable = child.gameObject.GetComponent<HandCardDisplayer>();
                if (selectable != null)
                {
                    _selectables.Add(selectable);
                }
                
                selectable = child.gameObject.GetComponent<LandCardDisplayer>();
                if (selectable != null)
                {
                    _selectables.Add(selectable);
                }
            }
        }
        //Debug.Log(_selectables.Count);
        foreach (ISelectable selectable in _selectables)
        {
            //Debug.Log(selectable.GetGameObject().name);
            Transform _transform = selectable.GetGameObject().transform;
            ISelectable left = null, up = null, right = null, down = null;
            float minLeft = float.MaxValue, minUp = float.MaxValue, minRight = float.MaxValue, minDown = float.MaxValue;
            Dictionary<ISelectable, float> candidateNeighbors = new Dictionary<ISelectable, float>();
            
            foreach (ISelectable other in _selectables)
            {
                if (other == selectable) continue;
                Transform otherTransform = other.GetGameObject().transform;
                float deltaX = otherTransform.position.x - _transform.position.x;
                float deltaY = otherTransform.position.y - _transform.position.y;
                float distance = Mathf.Abs(deltaX) + Mathf.Abs(deltaY);
                candidateNeighbors[other] = distance;
            }

            foreach (var candidate in candidateNeighbors)
            {
                Transform otherTransform = candidate.Key.GetGameObject().transform;
                float deltaX = otherTransform.position.x - _transform.position.x;
                float deltaY = otherTransform.position.y - _transform.position.y;
                float distance = candidate.Value;
                
                if (deltaX > 0 && distance < minRight)
                {
                    minRight = distance;
                    right = candidate.Key;
                }
                else if (deltaX < 0 && distance < minLeft)
                {
                    minLeft = distance;
                    left = candidate.Key;
                }
                
                if (deltaY < 0 && distance < minDown)
                {
                    minDown = distance;
                    down = candidate.Key;
                }
                else if (deltaY > 0 && distance < minUp)
                {
                    minUp = distance;
                    up = candidate.Key;
                }
                
            }

            _graph[selectable] = new List<ISelectable> { left, up, right, down };
        }

        return _graph;
    }

    public void SetGroupeOfSelectables(GameObject[] group)
    {
        this.groupsOfSelectables = group;
    }
}
