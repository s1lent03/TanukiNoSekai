using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DomainAnimation : MonoBehaviour
{
    [Header("Colors")]
    [ColorUsage(showAlpha:true, hdr:true)]
    public Color domainColorDay;
    [ColorUsage(showAlpha: true, hdr: true)]
    public Color domainColorNight;

    private Material domainMaterial;

    [Header("Animation")]
    public float addCutoff;
    public bool open = true;
    public int timeAccelaration = 1;

    void Start()
    {
        domainMaterial = GetComponent<MeshRenderer>().sharedMaterial;
        addCutoff = -50;
    }

    // Update is called once per frame
    void Update()
    {
        if (open)
        {
            openDomain();
        }
        else
        {
            closeDomain();
        }
        
    }

    public void openDomain()
    {
        if (domainMaterial.GetFloat("_CutoffHeight") <= 70)
        {
            addCutoff += Time.deltaTime * timeAccelaration;
            domainMaterial.SetFloat("_CutoffHeight", addCutoff);
        }
    }

    public void closeDomain()
    {
        if (domainMaterial.GetFloat("_CutoffHeight") >= -50)
        {
            addCutoff -= Time.deltaTime * timeAccelaration;
            domainMaterial.SetFloat("_CutoffHeight", addCutoff);
        }
    }
}
