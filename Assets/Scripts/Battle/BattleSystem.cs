using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] public BattleUnit enemyUnit;
    [SerializeField] BattleManager playerHud;
    [SerializeField] BattleManager enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public GameObject TanukiDetector;

    public BattleState state;

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(true);
        enemyUnit.Setup(false);
        playerHud.SetData(playerUnit.Tanuki, true);
        enemyHud.SetData(enemyUnit.Tanuki, false);

        dialogBox.SetMoveNames(playerUnit.Tanuki.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Tanuki.Base.name} appeared!");

        PlayerAction();
    }

    public void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Choose an action.."));
    }

    public void PlayerMove()
    {
        state = BattleState.PlayerMove;
    }

    public IEnumerator PerformPlayerMove(int currentMoveIndex)
    {
        if (currentMoveIndex < playerUnit.Tanuki.Moves.Count)
        {
            state = BattleState.Busy;

            var move = playerUnit.Tanuki.Moves[currentMoveIndex];
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Base.Name} used {move.Base.Name}.");

            var damageDetails = enemyUnit.Tanuki.TakeDamage(move, playerUnit.Tanuki);
            yield return gameObject.GetComponent<BattleManager>().UpdateHP();
            yield return ShowDamageDetails(damageDetails);

            if (damageDetails.Fainted)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Tanuki.Base.Name} has fainted.");
                Destroy(TanukiDetector.GetComponent<TanukiDetection>().WildTanukiDetected);
                TanukiDetector.GetComponent<TanukiDetection>().EndBattle();

                
            }
            else
            {
                StartCoroutine(EnemyMove());
            }
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Tanuki.GetRandomMove();

        yield return dialogBox.TypeDialog($"{enemyUnit.Tanuki.Base.Name} used {move.Base.Name}.");

        var damageDetails = playerUnit.Tanuki.TakeDamage(move, enemyUnit.Tanuki);
        yield return gameObject.GetComponent<BattleManager>().UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Base.Name} has fainted.");
            TanukiDetector.GetComponent<TanukiDetection>().EndBattle();
        }
        else
        {
            PlayerAction();
        }
    }

    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");

        if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective..");

    }

    public void HandleMoveSelection(int currentMoveIndex)
    {
        if (currentMoveIndex < playerUnit.Tanuki.Moves.Count)
        {
            dialogBox.UpdateMoveSelection(playerUnit.Tanuki.Moves[currentMoveIndex], true);
        }
        else
        {
            dialogBox.UpdateMoveSelection(playerUnit.Tanuki.Moves[0], false);
        }
    }
}
