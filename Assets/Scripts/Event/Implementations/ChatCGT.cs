using UnityEngine;

public class ChatCGT : IGatoEvento
{
    private GameControlller _controlller;
    public void Init(GameControlller controller)
    {
        this._controlller = controller;
    }

    public void Execute()
    {
        Debug.Log("CHAT CGT EXECUTE!");
        Player[] players = _controlller.GetPlayers();
        foreach (Player player in players)
        {
            Land land = player.GetLand();
            LandCardManager[,] landCardManagers = land.GetLandCardManagers();
            foreach (LandCardManager landCardManager in landCardManagers)
            {
                if(landCardManager == null) continue;
                Card card = landCardManager.GetBuilding();
                if(card != null) card.SetProduction(-card.GetProduction());
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
