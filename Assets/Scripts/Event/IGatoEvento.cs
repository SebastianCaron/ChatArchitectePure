using UnityEngine;

public interface IGatoEvento
{
    public void Init(GameControlller controller);
    public void Execute();
    public void Finish();
}
