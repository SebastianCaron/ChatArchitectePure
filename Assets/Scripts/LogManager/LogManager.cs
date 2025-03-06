using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [SerializeField] private string path = "./logs/log.txt";

    private StreamWriter _sw;

    public void Init()
    {
        if (!File.Exists(path))
        {
            _sw = File.CreateText(path);
        }
    }

    public void WriteLog(string line)
    {
        if (_sw == null) return;
        _sw.WriteLine(line);
    }

    public Image GenerateImageFromLogs()
    {

        return null;
    }
}
