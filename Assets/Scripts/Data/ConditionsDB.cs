using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionsDB
{
    public static Dictionary<ConditionID, Condition> Conditions { get; set; } = new Dictionary<ConditionID, Condition>()
    {
        {
            ConditionID.psn, 
            new Condition()
            {
                Name = "Poison",
                StartMessage = "has been poisoned.",
                OnAfterTurn = (Tanuki tanuki) =>
                {
                    tanuki.UpdateHp(tanuki.MaxHp / 8);
                    tanuki.StatusChanges.Enqueue($"{tanuki.Base.Name} hurt itself due to poison.");
                }
            }
        },

        {
            ConditionID.brn,
            new Condition()
            {
                Name = "Burn",
                StartMessage = "has been burned.",
                OnAfterTurn = (Tanuki tanuki) =>
                {
                    tanuki.UpdateHp(tanuki.MaxHp / 16);
                    tanuki.StatusChanges.Enqueue($"{tanuki.Base.Name} hurt itself due to burn.");
                }
            }
        },

        {
            ConditionID.par,
            new Condition()
            {
                Name = "Paralyse",
                StartMessage = "has been paralysed.",
                OnBeforeMove = (Tanuki tanuki) =>
                {
                    if (Random.Range(1, 5) == 1)
                    {
                        tanuki.StatusChanges.Enqueue($"{tanuki.Base.Name}'s paralysed and can't move.");
                        return false;
                    }

                    return true;
                }
            }
        },
    };
}

public enum ConditionID
{
    none,
    psn, 
    brn, 
    par,
}
