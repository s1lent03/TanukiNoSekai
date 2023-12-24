using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Lista com o tipo de quests que podem ser feitas
public enum TypeOfQuests
{
    Defeat
}

// Lista do tipo de recompensas
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

// Class de quests
[System.Serializable]
public class Quest
{
    // Vari�veis
    public TypeOfQuests questType; // Tipo de quest
    public int total; // N�mero de vezes a repetir a quest
    public int progress; // Progresso feito
    public float percentage; // Percentagem do progresso feito
    public string message; // Mensagem que ir� aparecer a indicar a quest
    public Rewards reward; // Tipo de recompensa
    public int amount; // Quantidade da recompensa

    // Construtor
    public Quest(TypeOfQuests qType, int t, string m, Rewards r, int a)
    {
        questType = qType;
        total = t;
        progress = 0;
        percentage = 0f;
        message = m;
        reward = r;
        amount = a;
    }
}

public class QuestManager : MonoBehaviour
{
    // Lista com as quests
    public List<Quest> quests = new List<Quest>();
    public TanukiParty tanukiParty;

    // Fun��o para atualizar as quests de um tipo espec�fico
    public void UpdateQuests(TypeOfQuests qType)
    {
        // Buscar todas as quests ativas
        foreach (Quest q in quests)
        {
            // Se existir quests do tipo passado pela fun��o, atualiza-se o progresso
            if (q.questType == qType)
            {
                q.progress += 1;
                q.percentage = (float)q.progress / (float)q.total;
            }
        }
    }

    // Fun��o para dar as recompensas
    private void GiveRewards(Quest q)
    {
        // Vari�veis
        Rewards reward = q.reward; // Recompensa
        int amount = q.amount; // Quantidade

        // Caso a recompensa seja um item ou moedas, simplesmente modificamos o PlayerPrefs
        if (reward != Rewards.Experience)
        {
            PlayerPrefs.SetInt(reward.ToString(), PlayerPrefs.GetInt(reward.ToString()) + amount);
        } else
        // Caso contr�rio, vamos dividir a experi�ncia pelos tanukis
        {
            List<Tanuki> tanukis = tanukiParty.tanukis; // Obt�m a lista de tanukis na party
            int numOfTanukis = tanukis.Count; // Obt�m o n�mero de tanukis na party
            int xpForEachTanuki = amount / numOfTanukis; // Divide o total pelos tanukis

            // Vai aplicar o n�mero de XP dividido por todos os tanukis na party
            foreach (var tanuki in tanukis)
            {
                tanuki.XpPoints += xpForEachTanuki;
            }
        }
    }

    // Fun��o para adicionar uma nova quest
    public void AddQuest(TypeOfQuests qType, int t, string m, Rewards r, int a)
    {
        // Cria nova quest
        Quest newQuest = new Quest(qType, t, m, r, a);

        // Adiciona a quest � lista
        quests.Add(newQuest);
    }

    private void Start()
    {
        AddQuest(TypeOfQuests.Defeat, 5, "Defeat 5 tanukis", Rewards.Experience, 10000);
        AddQuest(TypeOfQuests.Defeat, 2, "Defeat 2 tanukis", Rewards.Experience, 10000);
    }

    private void Update()
    {
        // Fun��o para ordenar a lista dependendo do progresso da mesma
        quests = quests.OrderByDescending(q => q.percentage).ToList();

        // Buscar todas as quests ativas
        foreach (Quest q in quests)
        {
            // Se houver quests completas, retira-mos a quest da lista e da-mos a recompensa ao jogador
            if (q.progress >= q.total)
            {
                // Remove a quest da list
                quests.Remove(q);

                // D� a recompensa ao jogador
                GiveRewards(q);
            }
        }
    }
}