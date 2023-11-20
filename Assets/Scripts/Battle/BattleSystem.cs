using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] public BattleUnit enemyUnit;
    [SerializeField] BattleManager playerHud;
    [SerializeField] BattleManager enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public GameObject TanukiDetector;

    [Header("Effects")]
    public GameObject HitEffect;
    public GameObject StatsUpEffect;
    public GameObject StatsDownEffect;

    private GameObject StatusEffect;

    public BattleState state;

    public TanukiParty playerParty;
    Tanuki wildTanuki;

    [Header("Sound Effects")]
    [SerializeField] AudioSource normalHitSoundFX;
    [SerializeField] AudioSource criticalHitSoundFX;

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

        ChooseFirstTurn();
    }

    void ChooseFirstTurn()
    {
        if (playerUnit.Tanuki.Speed >= enemyUnit.Tanuki.Speed)
            ActionSelection();
        else
            StartCoroutine(EnemyMove());
    }

    void BattleOver()
    {
        state = BattleState.BattleOver;

        playerParty.Tanukis.ForEach(t => t.OnBattleOver());
    }

    public void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("Choose an action.."));
    }

    public void OpenPartyScreen() 
    {
        state = BattleState.PartyScreen;
        gameObject.GetComponent<BattleManager>().SetPartyData(playerParty.Tanukis);
    }

    public void MoveSelection()
    {
        state = BattleState.MoveSelection;
    }

    public IEnumerator PlayerMove(int currentMoveIndex)
    {
        if (currentMoveIndex < playerUnit.Tanuki.Moves.Count)
        {
            state = BattleState.PerformMove;

            var move = playerUnit.Tanuki.Moves[currentMoveIndex];

            yield return RunMove(playerUnit, enemyUnit, move, false);

            if (state == BattleState.PerformMove)
                StartCoroutine(EnemyMove());

        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Tanuki.GetRandomMove();
        move.Pp--;

        yield return RunMove(enemyUnit, playerUnit, move, true);

        if (state == BattleState.PerformMove)
            ActionSelection();
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move, bool isPlayer)
    {
        move.Pp--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Tanuki.Base.Name} used {move.Base.Name}.");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);
        

        if (move.Base.Category == MoveCategory.Status)
        {
            yield return RunMoveEffects(move, sourceUnit, targetUnit);
        }
        else
        {
            StartCoroutine(targetUnit.PlayHitAnimation(HitEffect, targetUnit.gameObject.transform));

            var damageDetails = targetUnit.Tanuki.TakeDamage(move, sourceUnit.Tanuki);

            if (damageDetails.Critical > 1f)
                criticalHitSoundFX.Play();
            else
                normalHitSoundFX.Play();

            yield return gameObject.GetComponent<BattleManager>().UpdateHP();
            yield return ShowDamageDetails(damageDetails);
        }

        if (targetUnit.Tanuki.Hp <= 0)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Tanuki.Base.Name} has fainted.");

            if (isPlayer) //Caso o Tanuki a morrer seja do Player
            {
                if (playerUnit.transform.childCount > 0)
                {
                    Transform child = playerUnit.transform.GetChild(0);
                    playerUnit.PlayFaintAnimation(child.gameObject);
                    yield return new WaitForSeconds(1f);
                    Destroy(child.gameObject);
                }

                var nextTanuki = playerParty.GetHealthyTanuki();
                if (nextTanuki != null)
                {
                    gameObject.GetComponent<BattleManager>().PartySelection();
                }
                else
                {
                    TanukiDetector.GetComponent<TanukiDetection>().EndBattle();
                }
            }
            else //Caso o Tanuki a morrer seja Wild
            {
                targetUnit.PlayFaintAnimation(targetUnit.gameObject);
                yield return new WaitForSeconds(1f);

                Destroy(TanukiDetector.GetComponent<TanukiDetection>().WildTanukiDetected);
                TanukiDetector.GetComponent<TanukiDetection>().EndBattle();
            }

            BattleOver();
        }
    }

    IEnumerator RunMoveEffects(Move move, BattleUnit source, BattleUnit target)
    {
        var effects = move.Base.Effects;
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.Tanuki.ApplyBoosts(effects.Boosts);

                yield return PlayStatusChangeAnimation(StatsUpEffect, source);
            }
            else
            {
                target.Tanuki.ApplyBoosts(effects.Boosts);

                yield return PlayStatusChangeAnimation(StatsDownEffect, target);
            }
        }

        yield return ShowStatusChanges(source.Tanuki);
        yield return ShowStatusChanges(target.Tanuki);
    }

    IEnumerator PlayStatusChangeAnimation(GameObject effect, BattleUnit target)
    {
        Vector3 pos = new Vector3(target.transform.position.x, 100.2f, target.transform.position.z);
        StatusEffect = Instantiate(effect, pos, Quaternion.identity);

        StatusEffect.transform.DOScale(new Vector3(1, 1, 1), 1);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator ShowStatusChanges(Tanuki tanuki)
    {
        while (tanuki.StatusChanges.Count > 0)
        {
            var message = tanuki.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }

        if (StatusEffect.gameObject != null)
        {
            StatusEffect.transform.DOScale(new Vector3(0.1f, 1, 0.1f), 1);
            yield return new WaitForSeconds(1f);
            Destroy(StatusEffect.gameObject);
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

    public void HandlePartySelection(int currentTanukiIndex)
    {
        var selectedTanuki = playerParty.Tanukis[currentTanukiIndex];

        if (selectedTanuki.Hp <= 0)
        {
            return;
        }

        if (selectedTanuki == playerUnit.Tanuki)
        {
            return;
        }

        state = BattleState.Busy;
        gameObject.GetComponent<BattleManager>().BackToActionsButton();
        StartCoroutine(SwitchTanuki(selectedTanuki));
    }

    IEnumerator SwitchTanuki(Tanuki newTanuki)
    {
        bool currentTanukiFainted = true;
        if (playerUnit.Tanuki.Hp > 0)
        {
            currentTanukiFainted = false;
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Tanuki.Base.name}!");

            Transform child = playerUnit.transform.GetChild(0);
            playerUnit.PlayFaintAnimation(child.gameObject);

            yield return new WaitForSeconds(1f);
            Destroy(child.gameObject);
        }      

        playerUnit.Setup(true, newTanuki);
        playerHud.SetData(newTanuki, true);

        dialogBox.SetMoveNames(newTanuki.Moves);

        yield return dialogBox.TypeDialog($"Go {newTanuki.Base.Name}!");

        if (currentTanukiFainted)
            ChooseFirstTurn();
        else
            StartCoroutine(EnemyMove());
    }
}
