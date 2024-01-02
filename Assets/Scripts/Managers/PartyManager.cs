using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PartyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Player;
    [SerializeField] GameObject InfoBox;
    [SerializeField] GameObject TanukiActionsBox;
    [Space]
    [SerializeField] GameObject TanukiButton1;
    [SerializeField] GameObject TanukiButton2;
    [SerializeField] GameObject TanukiButton3;
    [SerializeField] GameObject TanukiButton4;
    [SerializeField] GameObject TanukiButton5;
    [Space]
    [SerializeField] GameObject BackButton;
    [SerializeField] GameObject Heal1Button;
    [SerializeField] GameObject Heal2Button;
    [SerializeField] GameObject Heal3Button;
    [Space]
    [SerializeField] GameObject SwitchButton1;
    [SerializeField] GameObject SwitchButton2;
    [SerializeField] GameObject SwitchButton3;
    [SerializeField] GameObject SwitchButton4;
    [SerializeField] GameObject SwitchButton5;
    [Space]
    [SerializeField] GameObject eventSystemObject;
    [SerializeField] GameObject firstButtonAction;
    [SerializeField] GameObject firstButtonSwitch;

    [Header("Values")]
    [SerializeField] int aquaPotionHpRegenValue;
    [SerializeField] int sunPotionHpRegenValue;
    [SerializeField] int scarletPotionHpRegenValue;

    [Header("Bools")]
    [SerializeField] bool isActionBoxOpen = false;
    [SerializeField] bool isSwitching = false;

    [Header("Others")]
    int currentSelectedTanuki = 0;
    int secondSelectedTanuki = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TanukiActionButton(int currentTanukiIndex)
    {
        currentSelectedTanuki = currentTanukiIndex;
    }

    public void TanukiSwitchButton(int currentTanukiIndex)
    {
        secondSelectedTanuki = currentTanukiIndex;
    }

    //Trocar Tanukis
    public void SwitchTanukis()
    {
        Tanuki tempTanuki = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki];
        Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki] = Player.GetComponent<TanukiParty>().tanukis[secondSelectedTanuki];
        Player.GetComponent<TanukiParty>().tanukis[secondSelectedTanuki] = tempTanuki;

        Player.GetComponent<PlayerHabilities>().UpdateToSeeParty();

        ActivateDeactivateButtons(TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton4, TanukiButton5, true);
        ActivateDeactivateButtons(SwitchButton1, SwitchButton2, SwitchButton3, SwitchButton4, SwitchButton5, false);
        Heal1Button.GetComponent<Button>().interactable = true;
        Heal2Button.GetComponent<Button>().interactable = true;
        Heal3Button.GetComponent<Button>().interactable = true;

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(firstButtonAction);

        isSwitching = false;
        ShowInfoBox();
    }

    //Ativar trocar tanukis
    public void ActivateTanukisSwitchButtons()
    {
        if (!isSwitching)
        {
            switch (currentSelectedTanuki)
            {
                case 0:
                    ActivateSwitchButtons(SwitchButton1, SwitchButton2, SwitchButton3, SwitchButton4, SwitchButton5);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(SwitchButton2);
                    break;
                case 1:
                    ActivateSwitchButtons(SwitchButton2, SwitchButton1, SwitchButton3, SwitchButton4, SwitchButton5);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(SwitchButton1);
                    break;
                case 2:
                    ActivateSwitchButtons(SwitchButton3, SwitchButton1, SwitchButton2, SwitchButton4, SwitchButton5);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(SwitchButton1);
                    break;
                case 3:
                    ActivateSwitchButtons(SwitchButton4, SwitchButton1, SwitchButton2, SwitchButton3, SwitchButton5);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(SwitchButton1);
                    break;
                case 4:
                    ActivateSwitchButtons(SwitchButton5, SwitchButton1, SwitchButton2, SwitchButton3, SwitchButton4);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
                    eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(SwitchButton1);
                    break;
            }

            TanukiButton1.GetComponent<Button>().interactable = false;
            TanukiButton2.GetComponent<Button>().interactable = false;
            TanukiButton3.GetComponent<Button>().interactable = false;
            TanukiButton4.GetComponent<Button>().interactable = false;
            TanukiButton5.GetComponent<Button>().interactable = false;

            BackButton.GetComponent<Button>().interactable = false;
            Heal1Button.GetComponent<Button>().interactable = false;
            Heal2Button.GetComponent<Button>().interactable = false;
            Heal3Button.GetComponent<Button>().interactable = false;

            isSwitching = true;
        }
        else
        {
            ActivateDeactivateButtons(TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton4, TanukiButton5, true);
            ActivateDeactivateButtons(SwitchButton1, SwitchButton2, SwitchButton3, SwitchButton4, SwitchButton5, false);
            Heal1Button.GetComponent<Button>().interactable = true;
            Heal2Button.GetComponent<Button>().interactable = true;
            Heal3Button.GetComponent<Button>().interactable = true;

            isSwitching = false;
        }
    }

    //Trocar caixas
    public void ChangeBoxes()
    {
        if (!isActionBoxOpen)
        {
            ShowActionsBox();
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(Heal1Button);
            switch (currentSelectedTanuki)
            {
                case 0:
                    TanukiButton2.GetComponent<Button>().interactable = false;
                    TanukiButton3.GetComponent<Button>().interactable = false;
                    TanukiButton4.GetComponent<Button>().interactable = false;
                    TanukiButton5.GetComponent<Button>().interactable = false;
                    break;
                case 1:
                    TanukiButton1.GetComponent<Button>().interactable = false;
                    TanukiButton3.GetComponent<Button>().interactable = false;
                    TanukiButton4.GetComponent<Button>().interactable = false;
                    TanukiButton5.GetComponent<Button>().interactable = false;
                    break;
                case 2:
                    TanukiButton1.GetComponent<Button>().interactable = false;
                    TanukiButton2.GetComponent<Button>().interactable = false;
                    TanukiButton4.GetComponent<Button>().interactable = false;
                    TanukiButton5.GetComponent<Button>().interactable = false;
                    break;
                case 3:
                    TanukiButton1.GetComponent<Button>().interactable = false;
                    TanukiButton2.GetComponent<Button>().interactable = false;
                    TanukiButton3.GetComponent<Button>().interactable = false;
                    TanukiButton5.GetComponent<Button>().interactable = false;
                    break;
                case 4:
                    TanukiButton1.GetComponent<Button>().interactable = false;
                    TanukiButton2.GetComponent<Button>().interactable = false;
                    TanukiButton3.GetComponent<Button>().interactable = false;
                    TanukiButton4.GetComponent<Button>().interactable = false;
                    break;
            }
        }
        else
        {
            ShowInfoBox();
            ActivateDeactivateButtons(TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton4, TanukiButton5, true);
        }
    }

    //Mostrar caixa com ações como curar e trocar
    void ShowActionsBox()
    {
        InfoBox.SetActive(false);
        TanukiActionsBox.SetActive(true);
        isActionBoxOpen = true;

        switch (currentSelectedTanuki)
        {
            case 0:
                DeactivateButtons(TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton4, TanukiButton5);
                break;
            case 1:
                DeactivateButtons(TanukiButton2, TanukiButton1, TanukiButton3, TanukiButton4, TanukiButton5);
                break;
            case 2:
                DeactivateButtons(TanukiButton3, TanukiButton1, TanukiButton2, TanukiButton4, TanukiButton5);
                break;
            case 3:
                DeactivateButtons(TanukiButton4, TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton5);
                break;
            case 4:
                DeactivateButtons(TanukiButton5, TanukiButton1, TanukiButton2, TanukiButton3, TanukiButton4);
                break;
        }
    }

    //Ativar ou desativar todos os botões
    void ActivateDeactivateButtons(GameObject Button1, GameObject Button2, GameObject Button3, GameObject Button4, GameObject Button5, bool state)
    {
        Button1.GetComponent<Button>().interactable = state;
        Button2.GetComponent<Button>().interactable = state;
        Button3.GetComponent<Button>().interactable = state;
        Button4.GetComponent<Button>().interactable = state;
        Button5.GetComponent<Button>().interactable = state;
    }

    //Ativar botões para trocar
    void ActivateSwitchButtons(GameObject Deactivate, GameObject Activate1, GameObject Activate2, GameObject Activate3, GameObject Activate4)
    {
        Deactivate.GetComponent<Button>().interactable = false;
        Activate1.GetComponent<Button>().interactable = true;
        Activate2.GetComponent<Button>().interactable = true;
        Activate3.GetComponent<Button>().interactable = true;
        Activate4.GetComponent<Button>().interactable = true;

        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
        eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(firstButtonSwitch);
    }

    //Desativar botões
    void DeactivateButtons(GameObject Activate, GameObject Deactivate1, GameObject Deactivate2, GameObject Deactivate3, GameObject Deactivate4)
    {
        Activate.GetComponent<Button>().interactable = true;
        Deactivate1.GetComponent<Button>().interactable = false;
        Deactivate2.GetComponent<Button>().interactable = false; 
        Deactivate3.GetComponent<Button>().interactable = false;
        Deactivate4.GetComponent<Button>().interactable = false;
        BackButton.GetComponent<Button>().interactable = false;
    }

    //Mostar caixa de infos dos tanukis
    void ShowInfoBox()
    {
        InfoBox.SetActive(true);
        TanukiActionsBox.SetActive(false);
        isActionBoxOpen = false;

        TanukiButton1.GetComponent<Button>().interactable = true;
        TanukiButton2.GetComponent<Button>().interactable = true;
        TanukiButton3.GetComponent<Button>().interactable = true;
        TanukiButton4.GetComponent<Button>().interactable = true;
        TanukiButton5.GetComponent<Button>().interactable = true;
        BackButton.GetComponent<Button>().interactable = true;
    }

    //Curar tanuki selecionado com a respetiva poção
    public void HealTanukiPotion1()
    {
        int CurrentHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].Hp;
        int MaxHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].MaxHp;
        GameObject HpBar = null;
        TMP_Text PercentageText = null;

        if (CurrentHp < MaxHp)
        {
            Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].UpdateHp(-aquaPotionHpRegenValue);

            switch (currentSelectedTanuki)
            {
                case 0:
                    HpBar = TanukiButton1.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton1.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 1:
                    HpBar = TanukiButton2.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton2.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 2:
                    HpBar = TanukiButton3.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton3.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 3:
                    HpBar = TanukiButton4.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton4.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 4:
                    HpBar = TanukiButton5.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton5.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;

            }

            StartCoroutine(HpBar.GetComponent<HpBar>().SetHpSmooth((float)CurrentHp / MaxHp));
            PercentageText.text = (int)((float)CurrentHp / (float)MaxHp * 100) + "%";

            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(firstButtonAction);

            //Remover 1 poção
        }
    }

    //Curar tanuki selecionado com a respetiva poção
    public void HealTanukiPotion2()
    {        
        int CurrentHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].Hp;
        int MaxHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].MaxHp;
        GameObject HpBar = null;
        TMP_Text PercentageText = null;

        if (CurrentHp < MaxHp)
        {
            Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].UpdateHp(-sunPotionHpRegenValue);

            switch (currentSelectedTanuki)
            {
                case 0:
                    HpBar = TanukiButton1.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton1.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 1:
                    HpBar = TanukiButton2.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton2.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 2:
                    HpBar = TanukiButton3.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton3.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 3:
                    HpBar = TanukiButton4.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton4.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 4:
                    HpBar = TanukiButton5.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton5.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;

            }

            StartCoroutine(HpBar.GetComponent<HpBar>().SetHpSmooth((float)CurrentHp / MaxHp));
            PercentageText.text = (int)((float)CurrentHp / (float)MaxHp * 100) + "%";

            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(firstButtonAction);

            //Remover 1 poção
        }
    }

    //Curar tanuki selecionado com a respetiva poção
    public void HealTanukiPotion3()
    {
        int CurrentHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].Hp;
        int MaxHp = Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].MaxHp;
        GameObject HpBar = null;
        TMP_Text PercentageText = null;

        if (CurrentHp < MaxHp)
        {
            Player.GetComponent<TanukiParty>().tanukis[currentSelectedTanuki].UpdateHp(-scarletPotionHpRegenValue);

            switch (currentSelectedTanuki)
            {
                case 0:
                    HpBar = TanukiButton1.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton1.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 1:
                    HpBar = TanukiButton2.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton2.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 2:
                    HpBar = TanukiButton3.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton3.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 3:
                    HpBar = TanukiButton4.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton4.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;
                case 4:
                    HpBar = TanukiButton5.transform.parent.Find("HpBar").gameObject;
                    PercentageText = TanukiButton5.transform.parent.Find("HpBar").transform.Find("HpPercentage").gameObject.GetComponent<TMP_Text>();
                    break;

            }

            StartCoroutine(HpBar.GetComponent<HpBar>().SetHpSmooth((float)CurrentHp / MaxHp));
            PercentageText.text = (int)((float)CurrentHp / (float)MaxHp * 100) + "%";

            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(null);
            eventSystemObject.GetComponent<EventSystem>().SetSelectedGameObject(firstButtonAction);

            //Remover 1 poção
        }
    }

}
