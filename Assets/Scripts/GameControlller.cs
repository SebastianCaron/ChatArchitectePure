using System;
using UnityEngine;

public class GameControlller : MonoBehaviour
{
    [SerializeField] private CardData[] cardsDefinition;
    
    private Player[] _players;
    private Shop _shop;
    
    private void Awake()
    {
        _shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        _shop.SetDefinitions(cardsDefinition);
        _shop.Init();
        
        GameObject[] playersGameObject = GameObject.FindGameObjectsWithTag("Player");
        _players = new Player[playersGameObject.Length];

        for (int i = 0; i < playersGameObject.Length; i++)
        {
            _players[i] = playersGameObject[i].GetComponent<Player>();
            _players[i].Init();
            _players[i].SetShop(_shop);
        }
        
        for (int i = 0; i < playersGameObject.Length; i++)
        {
            _players[i].InitSelecter();
        }
        
        
        
    }

    private void Update()
    {
        float delta = Time.deltaTime;
        _shop.UpdateShop(delta);
        
        foreach (Player player in _players)
        {
            player.UpdatePlayer(delta);
        }
    }

    public CardData[] GetCards()
    {
        return this.cardsDefinition;
    }
}
