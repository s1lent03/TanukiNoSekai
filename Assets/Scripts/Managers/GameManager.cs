using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Game Objects")]
    [SerializeField] GameObject Player;
    [SerializeField] BattleUnit tanukiBattleUnit1;
    [SerializeField] BattleUnit tanukiBattleUnit2;
    [SerializeField] BattleUnit tanukiBattleUnit3;
    [SerializeField] BattleUnit tanukiBattleUnit4;
    [SerializeField] BattleUnit tanukiBattleUnit5;

    [Header("Others")]
    [SerializeField] string saveFileName;
    [SerializeField] string questFileName;

    void Awake()
    {
        //Verifica se existe o ficheiro que guarda a save do jogo, se não cria - o
        string filePath = Application.dataPath + saveFileName;
        if (!File.Exists(filePath))
        {
            string[] contentFire =
            {
                "LOCATION:",
                "X: _692",
                "Y: _101.67",
                "Z: _324.25",
                "ROTATION:",
                "X: _0",
                "Y: _-90",
                "Z: _0",
                "TANUKIS:",
                "Tanuki0: _Madsung (TanukiBase)",
                "Tanuki0: _125",
            };

            string[] contentWater =
            {
                "LOCATION:",
                "X: _692",
                "Y: _101.67",
                "Z: _324.25",
                "ROTATION:",
                "X: _0",
                "Y: _-90",
                "Z: _0",
                "TANUKIS:",
                "Tanuki0: _Namiro (TanukiBase)",
                "Tanuki0: _125",
            };

            string[] contentGrass =
            {
                "LOCATION:",
                "X: _692",
                "Y: _101.67",
                "Z: _324.25",
                "ROTATION:",
                "X: _0",
                "Y: _-90",
                "Z: _0",
                "TANUKIS:",
                "Tanuki0: _Sladake (TanukiBase)",
                "Tanuki0: _125",
            };

            string[] content = contentFire;

            int rand = UnityEngine.Random.Range(1, 4);
            switch (rand)
            {
                case 1:
                    content = contentFire;
                    break;
                case 2:
                    content = contentWater;
                    break;
                case 3:
                    content = contentGrass;
                    break;
            }
     
            WriteTextToFile(filePath, content);

            PlayerPrefs.SetString("SaveDataPath", filePath);
        }

        //Setup da localização do jogador
        float locX = 0;
        float locY = 0;
        float locZ = 0;

        //Setup da rotação do jogador
        float rotX = 0;
        float rotY = 0;
        float rotZ = 0;

        string[] lines = File.ReadAllLines(filePath);
        for (int lineNumber = 1; lineNumber < lines.Length; lineNumber++)
        {
            int startOfWord = lines[lineNumber].IndexOf("_");
            int endOfWord = lines[lineNumber].IndexOf(" ");
            string assetsPath = "Assets/Resources/TanukiScriptableObjects/";

            if (lines[lineNumber].Contains("_"))
            {
                switch (lineNumber)
                {
                    //lOCALIZAÇÃO
                    case 1:
                        locX = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    case 2:
                        locY = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    case 3:
                        locZ = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    //ROTAÇÃO
                    case 5:
                        rotX = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    case 6:
                        rotY = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    case 7:
                        rotZ = float.Parse(lines[lineNumber].Substring(startOfWord + 1));
                        break;
                    //TANUKIS
                    //Tanuki1
                    case 9:
                        tanukiBattleUnit1.tanukiUnitData._base = AssetDatabase.LoadAssetAtPath<TanukiBase>(assetsPath + lines[lineNumber].Substring(startOfWord + 1, endOfWord - 1) + ".asset");
                        break;
                    case 10:
                        tanukiBattleUnit1.tanukiUnitData.XpPoints = float.Parse(lines[lineNumber].Substring(startOfWord + 1));

                        //Adiciona o tanuki à party                                
                        Player.GetComponent<TanukiParty>().Tanukis.Add(tanukiBattleUnit1.tanukiUnitData);
                        break;
                    //Tanuki2
                    case 11:
                        tanukiBattleUnit2.tanukiUnitData._base = AssetDatabase.LoadAssetAtPath<TanukiBase>(assetsPath + lines[lineNumber].Substring(startOfWord + 1, endOfWord - 1) + ".asset");
                        break;
                    case 12:
                        tanukiBattleUnit2.tanukiUnitData.XpPoints = float.Parse(lines[lineNumber].Substring(startOfWord + 1));

                        //Adiciona o tanuki à party                                
                        Player.GetComponent<TanukiParty>().Tanukis.Add(tanukiBattleUnit2.tanukiUnitData);
                        break;
                    //Tanuki3
                    case 13:
                        tanukiBattleUnit3.tanukiUnitData._base = AssetDatabase.LoadAssetAtPath<TanukiBase>(assetsPath + lines[lineNumber].Substring(startOfWord + 1, endOfWord - 1) + ".asset");
                        break;
                    case 14:
                        tanukiBattleUnit3.tanukiUnitData.XpPoints = float.Parse(lines[lineNumber].Substring(startOfWord + 1));

                        //Adiciona o tanuki à party                                
                        Player.GetComponent<TanukiParty>().Tanukis.Add(tanukiBattleUnit3.tanukiUnitData);
                        break;
                    //Tanuki4
                    case 15:
                        tanukiBattleUnit4.tanukiUnitData._base = AssetDatabase.LoadAssetAtPath<TanukiBase>(assetsPath + lines[lineNumber].Substring(startOfWord + 1, endOfWord - 1) + ".asset");
                        break;
                    case 16:
                        tanukiBattleUnit4.tanukiUnitData.XpPoints = float.Parse(lines[lineNumber].Substring(startOfWord + 1));

                        //Adiciona o tanuki à party                                
                        Player.GetComponent<TanukiParty>().Tanukis.Add(tanukiBattleUnit4.tanukiUnitData);
                        break;
                    //Tanuki5
                    case 17:
                        tanukiBattleUnit5.tanukiUnitData._base = AssetDatabase.LoadAssetAtPath<TanukiBase>(assetsPath + lines[lineNumber].Substring(startOfWord + 1, endOfWord - 1) + ".asset");
                        break;
                    case 18:
                        tanukiBattleUnit5.tanukiUnitData.XpPoints = float.Parse(lines[lineNumber].Substring(startOfWord + 1));

                        //Adiciona o tanuki à party                                 
                        Player.GetComponent<TanukiParty>().Tanukis.Add(tanukiBattleUnit5.tanukiUnitData);
                        break;
                }
            }
        }

        filePath = Application.dataPath + questFileName;
        if (!File.Exists(filePath))
        {
            string[] content =
            {
                "QUEST:",
            };

            WriteTextToFile(filePath, content);

            PlayerPrefs.SetString("SaveQuestsPath", filePath);
        }

        
        TypeOfQuests questType = TypeOfQuests.Defeat;
        TanukiNames questTanuki = TanukiNames.None;
        int questTotal = 0;
        string questMessage = "";
        Rewards questReward = Rewards.CurrentMoney;
        int questAmount = 0;

        lines = File.ReadAllLines(filePath);

        int start = lines[0].IndexOf("(");
        int end = lines[0].IndexOf(")");
        int numOfQuests = int.Parse(lines[0].Substring(start + 1, end - start - 1));        
        
        for (int i = 0; i < numOfQuests; i++)
        {
            for (int lineNumber = 1 + (5 * i); lineNumber < 6 + (5 * i); lineNumber++)
            {
                int startOfWord = lines[lineNumber].IndexOf("_");

                if (lines[lineNumber].Contains("_"))
                {
                    switch (lineNumber)
                    {
                        case 1:
                            questTanuki = (TanukiNames)Enum.Parse(typeof(TanukiNames), lines[lineNumber].Substring(startOfWord + 1));
                            break;
                        case 2:
                            questTotal = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                            break;
                        case 3:
                            questMessage = lines[lineNumber].Substring(startOfWord + 1);
                            break;
                        case 4:
                            questReward = (Rewards)Enum.Parse(typeof(Rewards), lines[lineNumber].Substring(startOfWord + 1));
                            break;
                        case 5:
                            questAmount = int.Parse(lines[lineNumber].Substring(startOfWord + 1));
                            break;
                    }
                }
            }

            gameObject.GetComponent<QuestManager>().AddQuest(questType, questTanuki, questTotal, questMessage, questReward, questAmount);
        }

        Player.transform.position = new Vector3(locX, locY, locZ);
        Player.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
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

        List<Tanuki> Tanukis = Player.GetComponent<TanukiParty>().Tanukis;

        List<string> saveDataContentList = new List<string>
        {
            "LOCATION:",
            "X: _" + Player.transform.position.x,
            "Y: _" + Player.transform.position.y,
            "Z: _" + Player.transform.position.z,
            "ROTATION:",
            "X: _" + Player.transform.rotation.x,
            "Y: _" + Player.transform.rotation.y,
            "Z: _" + Player.transform.rotation.z,
            "TANUKIS:",
        };

        for (int i = 0; i < Tanukis.Count; i++)
        {
            saveDataContentList.Add("Tanuki" + i + ": _" + Tanukis[i].Base.ToString());
            saveDataContentList.Add("Tanuki" + i + ": _" + Tanukis[i].XpPoints.ToString());
        }
        WriteTextToFile(filePath, saveDataContentList.ToArray());


        filePath = Application.dataPath + questFileName;

        int numOfQuests = gameObject.GetComponent<QuestManager>().quests.Count;
        

        List<string> saveQuestsContentList = new List<string>
        {
            "QUESTS(" + numOfQuests + "):",
        };

        for (int i = 0; i < numOfQuests; i++)
        {
            saveQuestsContentList.Add("Quest" + i + ": _" + gameObject.GetComponent<QuestManager>().quests[i].questTanukiName);
            saveQuestsContentList.Add("Quest" + i + ": _" + gameObject.GetComponent<QuestManager>().quests[i].total);
            saveQuestsContentList.Add("Quest" + i + ": _" + gameObject.GetComponent<QuestManager>().quests[i].message);
            saveQuestsContentList.Add("Quest" + i + ": _" + gameObject.GetComponent<QuestManager>().quests[i].reward);
            saveQuestsContentList.Add("Quest" + i + ": _" + gameObject.GetComponent<QuestManager>().quests[i].amount);
        }
        WriteTextToFile(filePath, saveQuestsContentList.ToArray());
    }
}
