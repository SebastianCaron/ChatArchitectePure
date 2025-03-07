using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogGraphGenerator : MonoBehaviour
{
    [SerializeField] private LogManager logManager;
    [SerializeField] private Image graphImage;
    [SerializeField] private int textureWidth = 200;
    [SerializeField] private int textureHeight = 200;
    [SerializeField] private Color backgroundColor = Color.black;

    private Texture2D graphTexture;

    private List<Color> playerColors = new List<Color>
    {
        Color.magenta, Color.yellow, Color.blue, Color.green, 
        Color.cyan, Color.red, Color.white, Color.gray
    };

    private Dictionary<string, Color> assignedColors = new Dictionary<string, Color>();
    private float maxTime = Int32.MinValue;
    private int maxMoney = Int32.MinValue;

    private void Start()
    {
        GenerateGraph();
    }

    public void GenerateGraph()
    {
        if (logManager == null || graphImage == null)
        {
            Debug.LogError("LogManager ou GraphImage non assigné !");
            return;
        }
        
        graphTexture = new Texture2D(textureWidth, textureHeight);
        ClearTexture();

        var playerLogs = logManager.GetPlayerLogs();
        int colorIndex = 0;
        
        foreach (var playerEntry in playerLogs)
        {
            foreach (var entry in playerEntry.Value)
            {
                float time = entry.Key;
                int money = entry.Value;

                if (time > maxTime)
                {
                    maxTime = time;
                }

                if (money > maxMoney)
                {
                    maxMoney = money;
                }
            }
        }

        foreach (var playerEntry in playerLogs)
        {
            string playerName = playerEntry.Key;

            if (!assignedColors.ContainsKey(playerName))
            {
                assignedColors[playerName] = playerColors[colorIndex % playerColors.Count];
                colorIndex++;
            }

            DrawPlayerGraph(playerEntry.Value, assignedColors[playerName]);
        }

        graphTexture.Apply();
        graphImage.sprite = Sprite.Create(graphTexture, new Rect(0, 0, textureWidth, textureHeight), Vector2.zero);
    }

    private void ClearTexture()
    {
        Color[] colors = new Color[textureWidth * textureHeight];
        for (int i = 0; i < colors.Length; i++)
            colors[i] = backgroundColor;
        graphTexture.SetPixels(colors);
    }

    private void DrawPlayerGraph(Dictionary<float, int> logEntries, Color playerColor)
    {
        List<Vector2> points = new List<Vector2>();
        
        foreach (var entry in logEntries)
        {
            float time = entry.Key;
            int money = entry.Value;
            //Debug.Log(entry);

            int x = Mathf.Clamp(Mathf.RoundToInt((maxTime-time) / maxTime *  textureWidth), 0, textureWidth - 1);
            int y = Mathf.Clamp(Mathf.RoundToInt((money  *  textureHeight) / maxMoney), 0, textureHeight - 1);

            points.Add(new Vector2(x, y));
        }

        //for (int i = 1; i < points.Count; i++)
        //{
        //    DrawLine((int)points[i - 1].x, (int)points[i - 1].y, (int)points[i].x, (int)points[i].y, playerColor);
        //}

        for (int i = 1; i < points.Count; i++)
        {
            //Debug.Log(points[i]);
            //graphTexture.SetPixel((int)points[i].x, (int)points[i].y, playerColor);
            DrawLine((int)points[i - 1].x, (int)points[i - 1].y, (int)points[i].x, (int)points[i].y, playerColor);
        }
    }

    private void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = dx + dy, e2;

        while (true)
        {
            graphTexture.SetPixel(x0, y0, color);
            if (x0 == x1 && y0 == y1) break;
            e2 = 2 * err;
            if (e2 >= dy) { err += dy; x0 += sx; }
            if (e2 <= dx) { err += dx; y0 += sy; }
        }
    }
}
