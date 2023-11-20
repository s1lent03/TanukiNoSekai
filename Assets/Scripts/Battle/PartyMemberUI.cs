using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [Header("TanukiPartyObjects")]
    [SerializeField] Image partySlotIconSprite;
    [SerializeField] TMP_Text partySlotNameTxt;
    [SerializeField] TMP_Text partySlotLevelTxt;
    [SerializeField] TMP_Text partySlotHpTxt;
    [SerializeField] HpBar partySlotHpBar;
    [SerializeField] string typesTxt;
    [SerializeField] string movesTxt;  

    [Header("Others")]
    Tanuki _playerTanuki;

    public void SetData(Tanuki tanuki)
    {
        _playerTanuki = tanuki;

        partySlotIconSprite.sprite = tanuki.Base.TanukiSprite;
        partySlotNameTxt.text = tanuki.Base.Name;
        partySlotLevelTxt.text = "Lvl. " + tanuki.Level;
        partySlotHpTxt.text = (int)((float)tanuki.Hp / tanuki.MaxHp * 100) + "%";
        partySlotHpBar.SetHp((float)tanuki.Hp / tanuki.MaxHp);
    }

    public string GetMoves(Tanuki tanuki)
    {
        movesTxt = "";
        for (int i = 0; i < 4; i++)
        {
            if (i < tanuki.Moves.Count)
                movesTxt += tanuki.Moves[i].Base.Name;
            else
                movesTxt += "--";

            if (i < 3)
                movesTxt += ", ";
            else
                movesTxt += ".";
        }

        return movesTxt;
    }

    public string GetTypes(Tanuki tanuki)
    {
        typesTxt = "";
        
        if (tanuki.Base.Type2 == TanukiType.None)
            typesTxt = tanuki.Base.Type1 + ".";
        else
            typesTxt = tanuki.Base.Type1 + ", " + tanuki.Base.Type2 + ".";

        return typesTxt;
    }
}
