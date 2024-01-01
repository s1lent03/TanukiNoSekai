using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject Player;

    [Header("Others")]
    [SerializeField] string saveFileName;

    void Start()
    {
        //Verifica se existe o ficheiro que guarda a save do jogo, se não cria - o
        string filePath = Application.dataPath + saveFileName;
        if (!File.Exists(filePath))
        {
            string[] content =
            {
                "LOCATION:",
                "X: _685",
                "Y: _102",
                "Z: _355",
                "",
            };
     
            WriteTextToFile(filePath, content);

            PlayerPrefs.SetString("SaveDataPath", filePath);
        }

        //Setup da localização do jogador
        float locX = 0;
        float locY = 0;
        float locZ = 0;

        string[] lines = File.ReadAllLines(filePath);
        for (int lineNumber = 1; lineNumber <= 3; lineNumber++)
        {
            int startOfWord = lines[lineNumber].IndexOf("_");

            switch (lineNumber)
            {
                case 1:
                    locX = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    break;
                case 2:
                    locY = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    break;
                case 3:
                    locZ = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                    break;
            }
        }

        Player.transform.position = new Vector3(locX, locY, locZ);
    }

    void Update()
    {
        
    }

    void WriteTextToFile(string fileName, string[] content)
    {
        //Escreve as linhas no ficheiro
        File.WriteAllLines(fileName, content);
    }

    //Save before close
    void OnApplicationQuit()
    {
        //Save 
        SaveData();
    }

    public void SaveData()
    {        
        string filePath = Application.dataPath + saveFileName;

        string[] content =
        {
            "LOCATION:",
            "X: _" + Player.transform.position.x,
            "Y: _" + Player.transform.position.y,
            "Z: _" + Player.transform.position.z,
            "",
        };

        WriteTextToFile(filePath, content);
    }
}
