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
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Tanuki, true);
        enemyHud.SetData(enemyUnit.Tanuki, false);

        dialogBox.SetMoveNames(playerUnit.Tanuki.Moves);

        yield return dialogBox.TypeDialog($"A wild {enemyUnit.Tanuki.Base.name} appeared!");
        yield return new WaitForSeconds(1f);

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
            yield return new WaitForSeconds(1f);

            bool isFainted = enemyUnit.Tanuki.TakeDamage(move, playerUnit.Tanuki);
            yield return gameObject.GetComponent<BattleManager>().UpdateHP();

            if (isFainted)
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
        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Tanuki.TakeDamage(move, enemyUnit.Tanuki);
        yield return gameObject.GetComponent<BattleManager>().UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Base.Name} has fainted.");
            TanukiDetector.GetComponent<TanukiDetection>().EndBattle();
        }
        else
        {
            PlayerAction();
        }
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
