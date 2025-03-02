using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GatoEvento", menuName = "ChatArchitecte/Event")]
public class GatoEvento : ScriptableObject
{
    public String eventName;
    public String eventDescription;
    public Sprite eventSprite;
    public float eventDuration;

    public float[] values;
}
