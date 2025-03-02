using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Shop : MonoBehaviour
{
    [SerializeField] private int[] gridShopClassic = new[] { 4,4 };
    [SerializeField] private int[] gridUsed = new[] { 4,4 };
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private float rowOffset = 1.5f;
    [SerializeField] private float columnOffset = 1.2f;
    [SerializeField] private float shopRefreshDelay = 10.0f;
    [SerializeField] private Slider slider;

    [SerializeField] private float championPercentage = 0.05f;
    
    private ICardDisplayer[,] _shopClassic;
    private ICardDisplayer[,] _shopUsed;
    private CardData[] _definitions;
    private List<CardData> _champions;
    private float _discount = 0;

    private float _elapsedTime = 0;
    

    private void GenerateShop()
    {
        ClearShop();

        int maxColumns = Mathf.Max(GetMaxColumns(gridShopClassic), GetMaxColumns(gridUsed));
        _shopClassic = new ICardDisplayer[gridShopClassic.Length, maxColumns];
        _shopUsed = new ICardDisplayer[gridUsed.Length, maxColumns];

        float cardWidth = Utils.GetGlobalBounds(cardPrefab).size.x;
        float cardHeight = Utils.GetGlobalBounds(cardPrefab).size.y;
        Transform tTransform = transform;

        for (int row = 0; row < gridShopClassic.Length; row++)
        {
            float rowWidth = gridShopClassic[row] * (cardWidth + columnOffset);
            float startX = tTransform.position.x - rowWidth / 2;

            for (int col = 0; col < gridShopClassic[row]; col++)
            {
                float x = startX + (cardWidth / 2) + col * (cardWidth + columnOffset);
                float y = tTransform.position.y - row * rowOffset;
                Vector3 position = new Vector3(x, y, 0);
                GameObject obj = Instantiate(cardPrefab, position, Quaternion.identity, tTransform);
                obj.name = $"ShopClassic_{row}_{col}";
                _shopClassic[row, col] = obj.GetComponent<HandCardDisplayer>();
            }
        }
        
        float usedStartY = tTransform.position.y - (gridShopClassic.Length * rowOffset) - rowOffset;
        for (int row = 0; row < gridUsed.Length; row++)
        {
            float rowWidth = gridUsed[row] * (cardWidth + columnOffset);
            float startX = tTransform.position.x - rowWidth / 2;

            for (int col = 0; col < gridUsed[row]; col++)
            {
                float x = startX + (cardWidth / 2) + col * (cardWidth + columnOffset);
                float y = usedStartY - row * rowOffset;
                Vector3 position = new Vector3(x, y, 0);
                GameObject obj = Instantiate(cardPrefab, position, Quaternion.identity, tTransform);
                obj.name = $"ShopUsed_{row}_{col}";
                _shopUsed[row, col] = obj.GetComponent<HandCardDisplayer>();
            }
        }
    }
    private void ClearShop()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
    private static int GetMaxColumns(int[] grid)
    {
        int maxColumns = 0;
        foreach (int cols in grid)
        {
            if (cols > maxColumns)
                maxColumns = cols;
        }
        return maxColumns;
    }
    private void OnDrawGizmos()
    {
        if (cardPrefab == null) return;

        float cardWidth = Utils.GetGlobalBounds(cardPrefab).size.x;
        float cardHeight = Utils.GetGlobalBounds(cardPrefab).size.y;
        Transform tTransform = transform;

        for (int row = 0; row < gridShopClassic.Length; row++)
        {
            float rowWidth = gridShopClassic[row] * (cardWidth + columnOffset);
            float startX = tTransform.position.x - rowWidth / 2;

            for (int col = 0; col < gridShopClassic[row]; col++)
            {
                float x = startX + (cardWidth / 2) + col * (cardWidth + columnOffset);
                float y = tTransform.position.y - row * rowOffset;
                Vector3 position = new Vector3(x, y, 0);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(position, new Vector3(cardWidth, cardHeight, 1));
            }
        }

        float usedStartY = tTransform.position.y - gridShopClassic.Length * rowOffset - rowOffset;
        float usedRowWidth = gridUsed[0] * (cardWidth + columnOffset);
        float usedStartX = tTransform.position.x - usedRowWidth / 2;

        for (int col = 0; col < gridUsed[0]; col++)
        {
            float x = usedStartX + (cardWidth / 2) + col * (cardWidth + columnOffset);
            Vector3 position = new Vector3(x, usedStartY, 0);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(position, new Vector3(cardWidth, cardHeight, 1));
        }
    }
    
    public void UpdateShop(float deltaTime)
    {
        _elapsedTime += deltaTime;
        if (_elapsedTime >= shopRefreshDelay)
        {
            RandomizeShop();
            _elapsedTime = 0;
        }
        RefreshSlider();
    }

    public void SetDefinitions(CardData[] defs)
    {
        this._definitions = defs;
    }

    public void SetChampions(CardData[] champions)
    {
        this._champions = new List<CardData>(champions);
    }

    public void Init()
    {
        GenerateShop();
        RandomizeShop();
        InitSlider();
    }

    private void InitSlider()
    {
        if (!slider) return;
        slider.maxValue = shopRefreshDelay;
        slider.minValue = 0;
        slider.value = shopRefreshDelay - _elapsedTime;
    }

    private void RefreshSlider()
    {
        slider.value = shopRefreshDelay - _elapsedTime;
    }

    private void RandomizeShop()
    {
        foreach (ICardDisplayer cardDisplayer in _shopClassic)
        {
            if(cardDisplayer.Equals(null)) continue;
            Card card = GenerateCard();
            cardDisplayer.SetCard(card);
        }
    }

    private Card GenerateCard()
    {
        CardData data;
        if (Random.Range(0, 1) < championPercentage && _champions.Count > 0)
        {
            data = _champions[Random.Range(0, _champions.Count)];
            _champions.Remove(data);
        }
        else
        {
            data = _definitions[Random.Range(0, _definitions.Length)];
        }

        Card card = new Card(data);
        card.SetPrice((int)(card.GetPrice() * (1 - _discount)));
        return card;
    }

    public void AddCardToUsed(Card card)
    {
        if (_shopUsed == null || _shopUsed.GetLength(0) == 0 || _shopUsed.GetLength(1) == 0) return;
        if (card == null) return;
        
        card.ResetExceptAllegeance();
        card.SetPrice((int) ((card.GetPrice() / 2) * (1 - _discount)));
        
        int rows = _shopUsed.GetLength(0);
        int cols = _shopUsed.GetLength(1);
        
        for (int row = rows - 1; row >= 0; row--)
        {
            for (int col = cols - 1; col >= 0; col--)
            {
                if (col == 0)
                {
                    if (row > 0)
                    {
                        _shopUsed[row, col].SetCard(_shopUsed[row - 1, cols - 1].GetCard());
                    }
                }
                else
                {
                    _shopUsed[row, col].SetCard(_shopUsed[row, col - 1].GetCard());
                }
            }
        }
        _shopUsed[0, 0].SetCard(card);
    }

    public void Buy(ICardDisplayer cardDisplayer, Player player)
    {
        if (cardDisplayer.GetCard() == null) return;
        if (player.GetHand().IsHandFull())
        {
            Debug.Log("HAND FULL !");
            return;
        }
        
        if (cardDisplayer.GetCard().GetPrice() <= player.GetGold())
        {
            player.SetGold(player.GetGold() - cardDisplayer.GetCard().GetPrice());
            player.GetHand().AddToHand(cardDisplayer.GetCard());
            cardDisplayer.SetCard(null);
        }
    }

    public void SetDiscount(float discount)
    {
        this._discount = discount;
        RefreshWithDiscount();
    }

    private void RefreshWithDiscount()
    {
        foreach (ICardDisplayer cardDisplayer in _shopClassic)
        {
            if(cardDisplayer == null) continue;
            Card card = cardDisplayer.GetCard();
            if(card == null) continue;
            card.SetPrice((int) (card.GetPrice() * (1 - _discount)));
            cardDisplayer.RefreshDisplay();
        }
        foreach (ICardDisplayer cardDisplayer in _shopUsed)
        {
            if(cardDisplayer == null) continue;
            Card card = cardDisplayer.GetCard();
            if(card == null) continue;
            card.SetPrice((int) (card.GetPrice() * (1 - _discount)));
            cardDisplayer.RefreshDisplay();
        }
    }

    public float GetDiscount()
    {
        return this._discount;
    }
    
}
