using UnityEngine;

public class ChatTastrophe : IGatoEvento
{
    private GameControlller _controlller;
    public void Init(GameControlller controller, GatoEvento evento)
    {
        this._controlller = controller;
    }

    public void Execute()
    {
        Debug.Log("CHATASTROPHE EXECUTE !");
        Player[] players = _controlller.GetPlayers();
        foreach (Player player in players)
        {
            Land land = player.GetLand();
            LandCardManager[,] landCardManagers = land.GetLandCardManagers();
            foreach (LandCardManager landCardManager in landCardManagers)
            {
                if(landCardManager == null) continue;
                landCardManager.ResetLand();
            }
        }
    }

    public void UpdateEvent(float deltaTime)
    {
        
    }

    public void Finish()
    {
        
    }

    public bool IsOver()
    {
        return true;
    }
}
