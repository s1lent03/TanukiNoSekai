using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightPole : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject Light1;
    [SerializeField] GameObject Light2;
    [SerializeField] GameObject TimeController;

    void Start()
    {
        TimeController = GameObject.Find("Global Volume");
    }

    void Update()
    {
        float currentHours = TimeController.GetComponent<DayNightCycle>().TimeHours;

        if ((currentHours > 0 && currentHours < 6.7f) || (currentHours > 19.7 && currentHours < 24))
        {
            Light1.SetActive(true);
            Light2.SetActive(true);
        }
        else
        {
            Light1.SetActive(false);
            Light2.SetActive(false);
        }
    }
}
