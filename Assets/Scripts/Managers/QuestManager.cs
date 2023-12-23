using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypeOfQuests
{
    Defeat
}

public enum Rewards
{
    NumberOfBall1,
    NumberOfBall2,
    NumberOfBall3,
    NumberOfBerry1,
    NumberOfBerry2,
    NumberOfBerry3,
    CurrentMoney,
    Experience
}

[System.Serializable]
public class Quest
{
    public TypeOfQuests questType;
    public int total;
    public int progress;
    public string message;
    public Rewards reward;
    public int amount;

    public Quest(TypeOfQuests qType, int t, string m, Rewards r, int a)
    {
        questType = qType;
        total = t;
        progress = 0;
        message = m;
        reward = r;
        amount = a;
    }
}

public class QuestManager : MonoBehaviour
{
    public List<Quest> quests = new List<Quest>();

    private void Start()
    {
        Quest newQuest = new Quest(TypeOfQuests.Defeat, 1, "lol", Rewards.NumberOfBall1, 1);
        quests.Add(newQuest);
    }
}