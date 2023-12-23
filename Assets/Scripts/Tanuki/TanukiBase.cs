using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Tanuki", menuName = "Tanuki/Create new Tanuki")]
public class TanukiBase : ScriptableObject
{
    [Header("Main Proprieties")]
    [SerializeField] string name;
    [TextArea][SerializeField] string description;
    [SerializeField] GameObject tanukiModel;
    [SerializeField] BattleUnit evolveModel;
    [SerializeField] Sprite tanukiIconSprite;

    [SerializeField] TanukiType type1;
    [SerializeField] TanukiType type2;

    [SerializeField] int evolveLevel;

    [Header("Base Stats")]
    [SerializeField] int maxHp;
    [SerializeField] int attack;
    [SerializeField] int defense;
    [SerializeField] int spAttack;
    [SerializeField] int spDefense;
    [SerializeField] int speed;

    [Header("Moves")]
    [SerializeField] List<LearnableMove> learnableMoves;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public GameObject TanukiModel
    {
        get { return tanukiModel; }
    }

    public BattleUnit EvolveModel
    {
        get { return evolveModel; }
    }

    public Sprite TanukiSprite
    {
        get { return tanukiIconSprite; }
    }

    public TanukiType Type1
    {
        get { return type1; }
    }

    public TanukiType Type2
    {
        get { return type2; }
    }

    public int EvolveLevel
    {
        get { return evolveLevel; }
    }

    public int MaxHp
    {
        get { return maxHp; }
    }

    public int Attack
    {
        get { return attack; }
    }

    public int Defense
    {
        get { return defense; }
    }
    public int SpAttack
    {
        get { return spAttack; }
    }

    public int SpDefense
    {
        get { return spDefense; }
    }

    public int Speed
    {
        get { return speed; }
    }

    public List<LearnableMove> LearnableMoves
    {
        get { return learnableMoves; }
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase;}
    }

    public int Level
    {
        get { return level; }
    }
}

public enum TanukiType
{
    None,
    Normal,
    Water,
    Fire,
    Ground,
    Grass,
    Air,
    Shadow,
    Light,
    Dragon,
    Ice,
    Eletric,
    Psychic,
    Fighting
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    Accuracy,
    Evasion
}

public class TypeChart
{
    static float[][] chart =
    {
        //                   NOR WAT FIR GRN GRS AIR SHD LIT DRG ICE ELC PSY FIGHT
        /*NOR*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f},
        /*WAT*/ new float[] { 1f, 1f, 2f, 2f, 0.5f, 1f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f},
        /*FIR*/ new float[] { 1f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f},
        /*GRN*/ new float[] { 1f, 0.5f, 2f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 2f, 2f, 1f, 0.5f},
        /*GRS*/ new float[] { 1f, 2f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f},
        /*AIR*/ new float[] { 1f, 1f, 1f, 2f, 2f, 1f, 2f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f},
        /*SHD*/ new float[] { 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f},
        /*LIT*/ new float[] { 1f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 1f, 2f, 2f, 1f, 1f, 2f},
        /*DRG*/ new float[] { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 2f, 0.5f, 1f, 0.5f, 2f},
        /*ICE*/ new float[] { 1f, 2f, 0.5f, 0.5f, 2f, 2f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 0.5f},
        /*ELC*/ new float[] { 1f, 2f, 1f, 0.5f, 0.5f, 2f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f},
        /*PSY*/ new float[] { 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 2f, 1f, 0.5f, 1f, 1f},
        /*FIT*/ new float[] { 2f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f}
    };

    public static float GetEffectiveness(TanukiType attackType, TanukiType defenseType)
    {
        if (attackType == TanukiType.None || defenseType == TanukiType.None)
            return 1;

        int row = (int)attackType - 1;
        int col = (int)defenseType - 1;

        return chart[row][col];
    }
}
