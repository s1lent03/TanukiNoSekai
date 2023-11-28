using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, PartyScreen, BattleOver}

public enum BattleAction { Move, SwitchTanuki, Run}

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
    [Space]
    public GameObject PoisonEffect;
    public GameObject BurningEffect;
    public GameObject ParalysedEffect;
    [Space]
    public Color PoisonEffectColor;
    public Color BurnEffectColor;
    public Color ParalysedEffectColor;
    [Space]
    public GameObject StatEffect;
    public GameObject StatusEffect;
    [Space]

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
        if (playerUnit.Tanuki.Moves[currentMoveIndex].Pp > 0)
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
        else
            yield return dialogBox.TypeDialog($"{playerUnit.Tanuki.Moves[currentMoveIndex].Base.Name} doesn't have power points left.");
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.PerformMove;

        var move = enemyUnit.Tanuki.GetRandomMove();

        do
        {
            if (move.Base.Pp > 0)
            {
                move.Pp--;

                yield return RunMove(enemyUnit, playerUnit, move, true);

                if (state == BattleState.PerformMove)
                {
                    ActionSelection();
                }
            }

            move = enemyUnit.Tanuki.GetRandomMove();
        }
        while (move.Base.Pp > 0);
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move, bool isPlayer)
    {
        bool canRunMove = sourceUnit.Tanuki.OnBeforeMove();
        if (!canRunMove)
        {
            yield return ShowStatusChanges(sourceUnit.Tanuki);
            yield break;
        }
        yield return ShowStatusChanges(sourceUnit.Tanuki);

        move.Pp--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Tanuki.Base.Name} used {move.Base.Name}.");

        if (CheckIfMoveHits(move, sourceUnit, targetUnit))
        {
            sourceUnit.PlayAttackAnimation();
            yield return new WaitForSeconds(1f);


            if (move.Base.Category == MoveCategory.Status)
            {
                yield return RunMoveEffects(move, sourceUnit, targetUnit, isPlayer);
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

            yield return CheckIfFainted(targetUnit, isPlayer);
        }
        else
        {
            yield return dialogBox.TypeDialog($"{sourceUnit.Tanuki.Base.Name}'s attack missed.");
        }

        //Condições de status como poison ou burn vão dar dano após o turno
        sourceUnit.Tanuki.OnAfterTurn();
        yield return ShowStatusChanges(sourceUnit.Tanuki);
        yield return gameObject.GetComponent<BattleManager>().UpdateHP();

        yield return CheckIfFainted(sourceUnit, isPlayer);
    }

    IEnumerator CheckIfFainted(BattleUnit targetUnit, bool isPlayer)
    {
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

    IEnumerator RunMoveEffects(Move move, BattleUnit source, BattleUnit target, bool isPlayer)
    {
        var effects = move.Base.Effects;

        //Boost a stats
        if (effects.Boosts != null)
        {
            if (move.Base.Target == MoveTarget.Self)
            {
                source.Tanuki.ApplyBoosts(effects.Boosts);

                yield return PlayStatChangeAnimation(StatsUpEffect, source);
            }
            else
            {
                target.Tanuki.ApplyBoosts(effects.Boosts);

                yield return PlayStatChangeAnimation(StatsDownEffect, target);
            }
        }

        //Condições de status
        if (effects.Status != ConditionID.none)
        {
            target.Tanuki.SetStatus(effects.Status);

            if (effects.Status == ConditionID.psn)
                yield return PlayStatusEffectAnimation(PoisonEffect, target, isPlayer, ConditionID.psn);

            if (effects.Status == ConditionID.brn)
                yield return PlayStatusEffectAnimation(BurningEffect, target, isPlayer, ConditionID.brn);

            if (effects.Status == ConditionID.par)
                yield return PlayStatusEffectAnimation(ParalysedEffect, target, isPlayer, ConditionID.par);
        }

        yield return ShowStatusChanges(source.Tanuki);
        yield return ShowStatusChanges(target.Tanuki);
    }

    IEnumerator PlayStatChangeAnimation(GameObject effect, BattleUnit target)
    {
        Vector3 pos = new Vector3(target.transform.position.x, 100.2f, target.transform.position.z);
        StatEffect = Instantiate(effect, pos, Quaternion.identity);

        StatEffect.transform.DOScale(new Vector3(1, 1, 1), 1);
        yield return new WaitForSeconds(1f);
    }

    IEnumerator PlayStatusEffectAnimation(GameObject effect, BattleUnit target, bool isPlayer, ConditionID status)
    {
        Vector3 pos = new Vector3(target.transform.position.x, 100.2f, target.transform.position.z);
        StatusEffect = Instantiate(effect, pos, Quaternion.identity, gameObject.transform);

        StatusEffect.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 1);
        yield return new WaitForSeconds(1f);

        if (status == ConditionID.psn)
            gameObject.GetComponent<BattleManager>().ChangeEffectFeedbackImage(isPlayer, PoisonEffectColor);

        if (status == ConditionID.brn)
            gameObject.GetComponent<BattleManager>().ChangeEffectFeedbackImage(isPlayer, BurnEffectColor);

        if (status == ConditionID.par)
            gameObject.GetComponent<BattleManager>().ChangeEffectFeedbackImage(isPlayer, ParalysedEffectColor);


    }

    public void StopStatusEffectAnimation()
    {
        if (gameObject.transform.childCount > 0)
        {
            Transform child = gameObject.transform.GetChild(0);
            Destroy(child.gameObject);
        }
    }

    bool CheckIfMoveHits(Move move, BattleUnit source, BattleUnit target)
    {
        if (move.Base.AlwaysHits)
            return true;

        float moveAccuracy = move.Base.Accuracy;

        int accuracy = source.Tanuki.StatBoosts[Stat.Accuracy];
        int evasion = target.Tanuki.StatBoosts[Stat.Evasion];

        var boostValues = new float[] { 1f, 4f/3f, 5f/3f, 2f, 7f/3f, 8f/3f, 3f};

        if (accuracy > 0)
            moveAccuracy *= boostValues[accuracy];
        else
            moveAccuracy /= boostValues[-accuracy];

        if (evasion > 0)
            moveAccuracy /= boostValues[evasion];
        else
            moveAccuracy *= boostValues[-evasion];

        return Random.Range(1, 101) <= moveAccuracy;
    }

    IEnumerator ShowStatusChanges(Tanuki tanuki)
    {
        while (tanuki.StatusChanges.Count > 0)
        {
            var message = tanuki.StatusChanges.Dequeue();
            yield return dialogBox.TypeDialog(message);
        }

        if (StatEffect != null && StatEffect.gameObject != null)
        {
            StatEffect.transform.DOScale(new Vector3(0.1f, 1, 0.1f), 1);
            yield return new WaitForSeconds(1f);
            Destroy(StatEffect.gameObject);
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
