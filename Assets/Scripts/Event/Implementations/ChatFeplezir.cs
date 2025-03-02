using UnityEngine;

public class ChatFeplezir : IGatoEvento
{
    private GameControlller _controlller;
    private GatoEvento _evento;
    private float _duration;
    private float _reductionPercentage;
    public void Init(GameControlller controller, GatoEvento evento)
    {
        this._controlller = controller;
        this._evento = evento;

        this._duration = evento.eventDuration;
    }

    public void Execute()
    {
        Debug.Log("CHAT FEPLEZIR EXECUTE!");
        _controlller.GetShop().SetDiscount(_evento.values[0]);
    }

    public void UpdateEvent(float deltaTime)
    {
        _duration -= deltaTime;
    }

    public void Finish()
    {
        _controlller.GetShop().SetDiscount(0);
    }

    public bool IsOver()
    {
        return _duration <= 0;
    }
}
