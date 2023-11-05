using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] TMP_Text dialogText;
    [SerializeField] int lettersPerSecond;

    [SerializeField] List<TMP_Text> moveTexts;

    [SerializeField] TMP_Text ppText;
    [SerializeField] TMP_Text typeText;
    [SerializeField] TMP_Text powerText;
    [SerializeField] TMP_Text accText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f/lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.name;
            else
                moveTexts[i].text = "--";
        }
    }

    public void UpdateMoveSelection(Move move, bool moveExists)
    {
        if (moveExists)
        {
            ppText.text = $"PP: {move.Pp}/{move.Base.Pp}";
            typeText.text = $"Type: {move.Base.Type.ToString()}";
            powerText.text = $"Power: {move.Base.Power}";
            accText.text = $"Accuracy: {move.Base.Accuracy}";
        }
        else
        {
            ppText.text = $"PP: 0/0";
            typeText.text = $"Type: null";
            powerText.text = $"Power: 0";
            accText.text = $"Accuracy: 100";
        }  
    }
}
