using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMissionNPC : MonoBehaviour
{
    [SerializeField] QuestManager questManager;

    void Start()
    {
        if (questManager.quests[0].message == "Defeat all 5 Yukatoris")
        {
            Destroy(gameObject);
        }
    }
}
