using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapArea : MonoBehaviour
{
    [SerializeField] List<Tanuki> wildTanukis;

    public Tanuki GetRandomWildTanuki()
    {
        var wildTanuki = wildTanukis[Random.Range(0, wildTanukis.Count)];
        wildTanuki.Init();
        return wildTanuki;
    }
}
