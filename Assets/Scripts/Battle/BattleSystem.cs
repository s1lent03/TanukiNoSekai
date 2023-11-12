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
    public GameObject HitEffect;

    public BattleState state;

    TanukiParty playerParty;
    Tanuki wildTanuki;

    public void StartBattle(TanukiParty playerParty, Tanuki wildTanuki, int wildLevel)
    {
        this.playerParty = playerParty;
        this.wildTanuki = wildTanuki;
        this.wildTanuki.level = wildLevel;
        this.wildTanuki.Init();
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(true, playerParty.GetHealthyTanuki());
        enemyUnit.Setup(false, wildTanuki);
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
            move.Pp--;
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Base.Name} used {move.Base.Name}.");

            playerUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);

            StartCoroutine(enemyUnit.PlayHitAnimation(HitEffect, enemyUnit.gameObject.transform));

            var damageDetails = enemyUnit.Tanuki.TakeDamage(move, playerUnit.Tanuki);
            yield return gameObject.GetComponent<BattleManager>().UpdateHP();
            yield return ShowDamageDetails(damageDetails);

            if (damageDetails.Fainted)
            {
                yield return dialogBox.TypeDialog($"{enemyUnit.Tanuki.Base.Name} has fainted.");

                enemyUnit.PlayFainAnimation(enemyUnit.gameObject);
                yield return new WaitForSeconds(1f);

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
        move.Pp--;
        yield return dialogBox.TypeDialog($"{enemyUnit.Tanuki.Base.Name} used {move.Base.Name}.");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        StartCoroutine(enemyUnit.PlayHitAnimation(HitEffect, playerUnit.gameObject.transform));

        var damageDetails = playerUnit.Tanuki.TakeDamage(move, enemyUnit.Tanuki);
        yield return gameObject.GetComponent<BattleManager>().UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Base.Name} has fainted.");

            if (playerUnit.transform.childCount > 0)
            {
                Transform child = playerUnit.transform.GetChild(0);
                playerUnit.PlayFainAnimation(child.gameObject);
                yield return new WaitForSeconds(1f);
                Destroy(child.gameObject);
            }

            var nextTanuki = playerParty.GetHealthyTanuki();
            if (nextTanuki != null)
            {
                playerUnit.Setup(true, nextTanuki);
                playerHud.SetData(nextTanuki, true);

                dialogBox.SetMoveNames(nextTanuki.Moves);

                yield return dialogBox.TypeDialog($"Go {nextTanuki.Base.Name}!");

                PlayerAction();
            }
            else
            {
                TanukiDetector.GetComponent<TanukiDetection>().EndBattle();
            }
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
