using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [SerializeField] private string fileName = "log.txt";
    [SerializeField] private LogGraphGenerator logGraphGenerator;
    
    private string filePath;
    private StreamWriter _sw;

    private Dictionary<string, Dictionary<float, int>> playerLogs = new Dictionary<string, Dictionary<float, int>>();

    private void Awake()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        Init();
    }

    public void Init()
    {
        try
        {
            _sw = new StreamWriter(filePath, true);
            _sw.AutoFlush = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Erreur lors de l'initialisation du log : {e.Message}");
        }
    }

    public void WriteLog(float duration, string playerName, int goldAmount)
    {
        if (!playerLogs.ContainsKey(playerName))
        {
            playerLogs[playerName] = new Dictionary<float, int>();
        }
        
        playerLogs[playerName][duration] = goldAmount;

        if (_sw != null)
        {
            string logEntry = $"{duration}, {playerName}, {goldAmount}";
            _sw.WriteLine(logEntry);
        }
        
    }

    private void OnDestroy()
    {
        _sw?.Close();
    }

    public Image GenerateImageFromLogs()
    {
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("Aucun fichier log trouvé pour générer une image.");
            return null;
        }
        
        logGraphGenerator.GenerateGraph();
        return null;
    }

    public Dictionary<string, Dictionary<float, int>> GetPlayerLogs()
    {
        return playerLogs;
    }
}


