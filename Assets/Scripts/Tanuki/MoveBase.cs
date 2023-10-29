using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Tanuki", menuName = "Tanuki/Create new Move")]
public class MoveBase : ScriptableObject
{
    [Header("Main Proprieties")]
    [SerializeField] string name;
    [TextArea][SerializeField] string description;

    [SerializeField] TanukiType type;
    [SerializeField] int power;
    [SerializeField] int accuracy;
    [SerializeField] int pp;

    public string Name
    {
        get { return name; }
    }

    public string Description
    {
        get { return description; }
    }

    public TanukiType Type
    {
        get { return type; }
    }

    public int Power
    {
        get { return power; }
    }

    public int Accuracy
    {
        get { return accuracy; }
    }

    public int Pp
    {
        get { return pp; }
    }
}
