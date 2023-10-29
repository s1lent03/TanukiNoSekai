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

    [SerializeField] TanukiType type1;
    [SerializeField] TanukiType type2;

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

    public TanukiType Type1
    {
        get { return type1; }
    }

    public TanukiType Type2
    {
        get { return type2; }
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
