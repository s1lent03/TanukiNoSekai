using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

// Lista com o tipo de quests que podem ser feitas
public enum TypeOfQuests
{
    Defeat
}

// Lista de tanukis a ser derrotados
public enum TanukiNames
{
    None,
    Aobochi,
    Bogu,
    Enjin,
    Hikari,
    Jigoku,
    Jikan,
    Jishin,
    Kaiyo,
    Kenmaru,
    Kensei,
    Korobo,
    Kyofu,
    Sanme,
    Tsuchi,
    Uchu,
    Yami,
    Yokoju,
    Yosei,
    Yukatori,
    Yukioni
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
    public TanukiNames questTanukiName; // Tipo de quest
    public int total; // N�mero de vezes a repetir a quest
    public int progress; // Progresso feito
    public float percentage; // Percentagem do progresso feito
    public string message; // Mensagem que ir� aparecer a indicar a quest
    public Rewards reward; // Tipo de recompensa
    public int amount; // Quantidade da recompensa
    public GameObject questUI; // Painel com a quest

    // Construtor
    public Quest(TypeOfQuests qType, TanukiNames qTName, int t, string m, Rewards r, int a, GameObject ui)
    {
        questType = qType;
        questTanukiName = qTName;
        total = t;
        progress = 0;
        percentage = 0f;
        message = m;
        reward = r;
        amount = a;
        questUI = ui;
    }
}

public class QuestManager : MonoBehaviour
{
    // Vari�veis
    public List<Quest> quests = new List<Quest>(); // Lista com as quests
    public TanukiParty tanukiParty; // Tanukis do jogador
    public GameObject questsMenu; // Lista das quests no jogo
    public GameObject questPrefab; // Prefab de um painel para uma quest

    // Fun��o para atualizar as quests de um tipo espec�fico
    public void UpdateQuests(TypeOfQuests qType, string TanukiName)
    {
        // Buscar todas as quests ativas
        foreach (Quest q in quests)
        {
            // Se existir quests do tipo passado pela fun��o, atualiza-se o progresso
            if (q.questType == qType)
            {
                if (q.questTanukiName == TanukiNames.None)
                {
                    q.progress += 1;
                    q.percentage = (float)q.progress / (float)q.total;
                    UpdatePanel(q);
                }
                else if (TanukiName == q.questTanukiName.ToString())
                {
                    q.progress += 1;
                    q.percentage = (float)q.progress / (float)q.total;
                    UpdatePanel(q);
                }
                
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

    // Fun��o para atualizar o painel de uma quest
    private void UpdatePanel(Quest q)
    {
        // Painel da quest
        Transform questPanel = q.questUI.transform;

        // Vari�veis do painel
        TMP_Text questDescription = questPanel.Find("Description").GetComponent<TMP_Text>(); // Descri��o da quest
        Transform questProgressBar = questPanel.Find("ProgressBar"); // Barra de progresso
        RectTransform progressBarSize = questProgressBar.GetComponent<RectTransform>(); // Tamanho da barra de progresso
        RectTransform questCurrentProgressBar = questProgressBar.Find("CurrentProgressBar").GetComponent<RectTransform>(); // Progresso na barra
        TMP_Text questProgressText = questProgressBar.Find("ProgressText").GetComponent<TMP_Text>(); // Progresso em texto

        // Atualizar os valores
        questDescription.text = q.message;
        questCurrentProgressBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, q.percentage * progressBarSize.rect.width);
        questProgressText.text = q.progress + "/" + q.total;
    }

    // Fun��o para adicionar uma nova quest
    public void AddQuest(TypeOfQuests qType, TanukiNames qTanukiName, int t, string m, Rewards r, int a)
    {
        // Cria novo painel para a quest
        GameObject questPanel = Instantiate(questPrefab, questsMenu.transform);

        // Cria nova quest
        Quest newQuest = new Quest(qType, qTanukiName, t, m, r, a, questPanel);

        // Adiciona a quest � lista
        quests.Add(newQuest);

        // Atualiza o painel
        UpdatePanel(newQuest);
    }

    private void Start()
    {
        //AddQuest(TypeOfQuests.Defeat, 5, "Defeat 5 tanukis", Rewards.Experience, 10000);
        //AddQuest(TypeOfQuests.Defeat, 2, "Defeat 2 tanukis", Rewards.Experience, 10000);
    }

    private void Update()
    {
        // Fun��o para ordenar a lista dependendo do progresso da mesma
        quests = quests.OrderByDescending(q => q.percentage).ToList();

        // Buscar todas as quests ativas
        foreach (Quest q in quests)
        {
            // Atualizar ordem da questa na UI
            q.questUI.transform.SetSiblingIndex(quests.IndexOf(q));

            // Se houver quests completas, retira-mos a quest da lista e da-mos a recompensa ao jogador
            if (q.progress >= q.total)
            {
                // Remove a quest da list
                quests.Remove(q);

                // Remove a quest do menu das quests
                Destroy(q.questUI);

                // D� a recompensa ao jogador
                GiveRewards(q);
            }
        }
    }
}