using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] TanukiBase _base;
    [SerializeField] int level;

    public Tanuki Tanuki { get; set; }
    public void Setup()
    {
        Tanuki = new Tanuki(_base, level);

        GameObject newTanuki = Instantiate(Tanuki.Base.TanukiModel, gameObject.transform);
        newTanuki.tag = "Untagged";
    }
}
