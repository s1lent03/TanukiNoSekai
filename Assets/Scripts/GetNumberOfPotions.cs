using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetNumberOfPotions : MonoBehaviour
{
    void Update()
    {
        if (gameObject.name == "NumberOfPotion1")
            gameObject.GetComponent<TMP_Text>().text = "x" + PlayerPrefs.GetInt("NumberOfPotion1").ToString();
        else if (gameObject.name == "NumberOfPotion2")
            gameObject.GetComponent<TMP_Text>().text = "x" + PlayerPrefs.GetInt("NumberOfPotion2").ToString();
        else if (gameObject.name == "NumberOfPotion3")
            gameObject.GetComponent<TMP_Text>().text = "x" + PlayerPrefs.GetInt("NumberOfPotion3").ToString();
    }
}
