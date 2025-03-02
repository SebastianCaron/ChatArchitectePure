using System;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameControlller : MonoBehaviour
{
    [SerializeField] private CardData[] cardsDefinition;
    [SerializeField] private CardData[] champions;
    
    [SerializeField] private float delayForGold = 1.0f;
    [SerializeField] private int goldAmount = 1;
    
    [SerializeField] private GatoEvento[] events;
    [SerializeField] private float delayEvent = 40f;

    [SerializeField] private float gameDuration = 120f;
    [SerializeField] private TMP_Text durationText;
    
    private Player[] _players;
    private Shop _shop;
    private float _elapsedTime = 0.0f;
    private IGatoEvento _gatoEvento = null;
    private float _elapsedTimeEvent = 0.0f;
    
    private void Awake()
    {
        _shop = GameObject.FindGameObjectWithTag("Shop").GetComponent<Shop>();
        _shop.SetDefinitions(cardsDefinition);
        _shop.SetChampions(champions);
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

    private void GameUpdate()
    {
        float delta = Time.deltaTime;

        _elapsedTime += delta;
        if (_elapsedTime >= delayForGold)
        {
            foreach (Player player in _players)
            {
                player.AddGold(goldAmount);
            }

            _elapsedTime = 0;
        }
        
        _shop.UpdateShop(delta);
        
        foreach (Player player in _players)
        {
            player.UpdatePlayer(delta);
        }

        gameDuration -= delta;
        if(durationText) durationText.SetText(Math.Round(gameDuration, 1).ToString());

        _elapsedTimeEvent += delta;
        if (_elapsedTimeEvent > delayEvent)
        {
            TriggerEvent();
            _elapsedTimeEvent = 0;
        }

        if (_gatoEvento != null)
        {
            _gatoEvento.UpdateEvent(delta);
            if (_gatoEvento.IsOver())
            {
                _gatoEvento.Finish();
                _gatoEvento = null;
            }
        }
    }

    private void TriggerEvent()
    {
        GatoEvento evento = events[Random.Range(0, events.Length)];
        switch (evento.eventType)
        {
            case EventEnum.CHAT_CGT:
                _gatoEvento = new ChatCGT();
                break;
            case EventEnum.CHAT_FEPLAISIR:
                break;
            case EventEnum.CHAT_TASTROPHE:
                break;
        }

        if (_gatoEvento != null)
        {
            // TODO : DISPLAY EVENT 
            _gatoEvento.Init(this);
            _gatoEvento.Execute();
        }
    }

    private void Update()
    {
        if (gameDuration > 0)
        {
            GameUpdate();
        }
        else
        {
            // TODO : DISPLAY WINNER & DATAS
        }
    }

    public CardData[] GetCards()
    {
        return this.cardsDefinition;
    }

    public CardData[] GetChampions()
    {
        return this.champions;
    }

    public Player[] GetPlayers()
    {
        return _players;
    }
}
