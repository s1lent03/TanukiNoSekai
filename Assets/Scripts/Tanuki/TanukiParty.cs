using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TanukiParty : MonoBehaviour
{
    public List<Tanuki> tanukis;

    public List<Tanuki> Tanukis
    {
        get { return tanukis; }
    }

    void Start()
    {
        foreach (var tanuki in tanukis)
        {
            tanuki.Init(true);
        }
    }

    public Tanuki GetHealthyTanuki()
    {
        return tanukis.Where(x => x.Hp > 0).FirstOrDefault();
    }

    public int GetHighestLevelTanuki()
    {
        return tanukis.OrderByDescending(x => x.Level).FirstOrDefault().Level;
    }
}
