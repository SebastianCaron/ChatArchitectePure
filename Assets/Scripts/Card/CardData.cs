using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "CardData", menuName = "ChatArchitecte/CardData")]
public class CardData : ScriptableObject
{
    public String Name;
    public String description;
    public CardTypeEnum type;
    public Sprite sprite;

    public int price;
    public float life;
    public float damage;
    public int production;
    public bool overtime;
    public bool isChampion;
    public bool alreadyBuy;
    public float delay;

    public CardBehaviourEnum behaviour;
    public AudioClip audio;
}