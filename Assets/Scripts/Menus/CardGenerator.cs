using System;
using UnityEngine;

public class CardGenerator : MonoBehaviour
{
    [SerializeField] private CardData[] cards;
    [SerializeField] private GameObject prefabDisplayer;

    [SerializeField] private GameObject content;

    private void Awake()
    {
        if (prefabDisplayer == null) return;
        content.GetComponent<RectTransform>().rect.Set(0,0,cards.Length * (prefabDisplayer.GetComponent<RectTransform>().rect.width + 20), content.GetComponent<RectTransform>().rect.height);
        foreach (CardData cardData in cards)
        {
            GameObject go = Instantiate(prefabDisplayer, content.transform);
            go.GetComponent<UICardDisplayer>().SetCard(new Card(cardData));
        }
        
        
    }
}
