using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private int nbCards = 4;
    [SerializeField] private float cardOffset = 1.2f;

    private ICardDisplayer[] _hand;
    private ISelectable[] _handSelectable;

    private Card[] _cards;

    private void GenerateHand()
    {
        _hand = new ICardDisplayer[nbCards];
        _handSelectable = new ISelectable[nbCards];

        float cardWidth = Utils.GetGlobalBounds(cardPrefab).size.x;
        float totalWidth = nbCards * cardWidth + (nbCards - 1) * cardOffset;
        float startX = transform.position.x - (totalWidth / 2);

        for (int i = 0; i < nbCards; i++)
        {
            float x = startX + (cardWidth / 2) + i * (cardWidth + cardOffset);
            Vector3 position = new Vector3(x, transform.position.y, 0);
            GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, transform);
            card.name = $"Card_{i}";
            _hand[i] = card.GetComponent<HandCardDisplayer>();
            _handSelectable[i] = card.GetComponent<HandCardDisplayer>();
        }
    }

    private void OnDrawGizmos()
    {
        if (cardPrefab == null || nbCards <= 0) return;

        float cardWidth = Utils.GetGlobalBounds(cardPrefab).size.x;
        float totalWidth = nbCards * cardWidth + (nbCards - 1) * cardOffset;
        float startX = transform.position.x - (totalWidth / 2);

        for (int i = 0; i < nbCards; i++)
        {
            float x = startX + (cardWidth / 2) + i * (cardWidth + cardOffset);
            Vector3 position = new Vector3(x, transform.position.y, 0);
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(position, Utils.GetGlobalBounds(cardPrefab).size);
        }
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public void Init()
    {
        GenerateHand();
        _cards = new Card[nbCards];
        for (int i = 0; i < nbCards; i++)
        {
            _cards[i] = null;
            _hand[i].SetCard(_cards[i]);
        }
    }

    public bool IsHandFull()
    {
        for (int i = 0; i < nbCards; i++)
        {
            if (_cards[i] == null) return false;
        }

        return true;
    }

    public void AddToHand(Card card)
    {
        for (int i = 0; i < nbCards; i++)
        {
            if (_cards[i] == null)
            {
                _cards[i] = card;
                _cards[i].SetBuyable(false);
                _hand[i].SetCard(_cards[i]);
                return;
            }
        }
        Debug.LogWarning("La main est pleine, impossible d'ajouter une nouvelle carte.");
    }

    public void RefreshHandData()
    {
        for (int i = 0; i < nbCards; i++)
        {
            _cards[i] = _hand[i].GetCard();
        }
    }

    public void RefreshHandDisplay()
    {
        for (int i = 0; i < nbCards; i++)
        {
            _hand[i].SetCard(_cards[i]);
        }
    }
}

