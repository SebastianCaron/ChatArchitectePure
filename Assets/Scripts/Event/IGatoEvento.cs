using UnityEngine;

public interface IGatoEvento
{
    public void Init(GameControlller controller);
    public void Execute();
    public void UpdateEvent(float deltaTime);
    public void Finish();

    public bool IsOver();
}
