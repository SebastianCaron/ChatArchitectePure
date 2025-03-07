using System;
using System.Collections.Generic;
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

    [SerializeField] private EventDisplayer eventDisplayer;

    [SerializeField] private GameObject winnerMenu;
    [SerializeField] private TMP_Text winnerText;
    
    [SerializeField] private LogManager logManager;
    [SerializeField] private Shop shop;
    
    
    private Player[] _players;
    
    private float _elapsedTime = 0.0f;
    private IGatoEvento _gatoEvento = null;
    private float _elapsedTimeEvent = 0.0f;
    private bool _over = false;
    
    private void Awake()
    {
        shop.SetDefinitions(cardsDefinition);
        shop.SetChampions(champions);
        shop.Init();
        
        GameObject[] playersGameObject = GameObject.FindGameObjectsWithTag("Player");
        _players = new Player[playersGameObject.Length];

        for (int i = 0; i < playersGameObject.Length; i++)
        {
            _players[i] = playersGameObject[i].GetComponent<Player>();
            _players[i].Init();
            _players[i].SetShop(shop);
        }
        
        for (int i = 0; i < playersGameObject.Length; i++)
        {
            _players[i].InitSelecter();
        }
        
        winnerMenu.SetActive(false);
        
        logManager.Init();
    }

    private void GameUpdate()
    {
        float delta = Time.deltaTime;

        _elapsedTime += delta;
        if (_elapsedTime >= delayForGold)
        {
            foreach (Player player in _players)
            {
                if(!player.GetForfeit()) player.AddGold(goldAmount);
            }

            _elapsedTime = 0;
        }
        
        shop.UpdateShop(delta);
        
        foreach (Player player in _players)
        {
            if(!player.GetForfeit()) player.UpdatePlayer(delta);
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
                //eventDisplayer.HideEvent();
                //eventDisplayer.SetEvento(null);
                _gatoEvento = null;
            }
        }
        eventDisplayer.UpdateDisplay(delta);

        foreach (Player player in _players)
        {
            if(logManager != null) logManager.WriteLog(gameDuration,player.gameObject.name, player.GetGold());
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
                _gatoEvento = new ChatFeplezir();
                break;
            case EventEnum.CHAT_TASTROPHE:
                _gatoEvento = new ChatTastrophe();
                break;
        }

        if (_gatoEvento != null)
        {
            eventDisplayer.SetEvento(evento);
            eventDisplayer.DisplayEvent();
            _gatoEvento.Init(this, evento);
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
            if (_over) return;
            winnerMenu.SetActive(true);
            winnerText.text = "Le gagnant est : ";
            int maxiGold = Int32.MinValue;
            List<Player> winners = new List<Player>();
            foreach (Player player in _players)
            {
                if (!player.GetForfeit() && player.GetGold() > maxiGold)
                {
                    winners.Clear();
                    winners.Add(player);
                    maxiGold = player.GetGold();
                }
                else if (player.GetGold() == maxiGold)
                {
                    winners.Add(player);
                }
            }

            for (int i = 0; i < winners.Count - 1; i++)
            {
                winnerText.text += winners[i].gameObject.name + ", ";
            }

            winnerText.text += winners[^1].gameObject.name + ".";
            logManager.GenerateImageFromLogs();
            _over = true;
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

    public Shop GetShop()
    {
        return this.shop;
    }

    public void Forfeit(Player player)
    {
        Debug.Log(player.gameObject.name + " Forfeit!");
        player.SetGold(-999999);
        player.SetForfeit(true);

        int nb = 0;
        foreach (Player mpl in _players)
        {
            if (!mpl.GetForfeit()) nb++;
        }

        if (nb <= 1)
        {
            gameDuration = 0;
        }
    }
}
