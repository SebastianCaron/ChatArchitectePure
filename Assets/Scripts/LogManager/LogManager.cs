using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : MonoBehaviour
{
    [SerializeField] private string path = @"/log.txt";

    private StreamWriter _sw;

    public void Init()
    {
        //Debug.Log(File.Open(path, FileMode.Create).Name);
        if (!File.Exists(path))
        {
            //_sw = File.CreateText(path);
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
