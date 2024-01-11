using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 24f)]
    public float TimeHours;
    public GameObject Sun;
    public GameObject Moon;
    public Color SunColor;
    private HDAdditionalLightData sunLightData;
    private HDAdditionalLightData moonLightData;

    public LensFlareDataSRP FlareData;

    public float timeGoBySpeed;

    [Header("Shaders")]
    public Vector3 daySkyMultiplier;
    public Vector3 nightSkyMultiplier;
    public Material waterMaterial;
    public Material riverMaterial;

    [Header("Trailer Vars")]
    [SerializeField] float moonLightIntensity;
    [SerializeField] bool stopTime;

    void Start()
    {
        sunLightData = Sun.GetComponent<HDAdditionalLightData>();
        moonLightData = Moon.GetComponent<HDAdditionalLightData>();

        TimeHours = 17;
    }

    void Update()
    {
        //Timelapse
        if (!stopTime)
            TimeHours += Time.deltaTime * timeGoBySpeed;

        if (TimeHours >= 24 )
            TimeHours = 0;


        DateTime currentTime = DateTime.Now;
        float timeFloat = currentTime.Hour + (float)currentTime.Minute / 60;

        moonLightData.transform.eulerAngles = new Vector3(GetSunPosition(TimeHours) - 170, 225f, 0f);

        sunLightData.transform.eulerAngles = new Vector3(GetSunPosition(TimeHours), 0f, 0f);
        sunLightData.SetColor(SunColor, GetSunColorTemp(TimeHours));
        sunLightData.intensity = GetSunIntensity(TimeHours);

        if ((TimeHours >= 19f && TimeHours <= 24f) || (TimeHours >= 0f && TimeHours <= 6.5f))
        {
            //NOITE
            sunLightData.EnableShadows(false);

            moonLightData.EnableShadows(true);
            moonLightData.intensity = moonLightIntensity;
            moonLightData.SetColor(SunColor, 20000f);

            //gameObject.GetComponent<Exposure>().active = true;

            //Alterar valores do shader da �gua para n�o ficar demasiado ou pouco brilhante a mais
            waterMaterial.SetVector("_SkyMultiplier", nightSkyMultiplier);
            riverMaterial.SetVector("_SkyMultiplier", nightSkyMultiplier);
        }
        else if (TimeHours >= 6.5f && TimeHours <= 19f)
        {
            //DIA
            sunLightData.EnableShadows(true);
            
            moonLightData.EnableShadows(false);
            moonLightData.intensity = 1400f;
            moonLightData.SetColor(SunColor, 5500f);

            //gameObject.GetComponent<Exposure>().active = false;

            //Alterar valores do shader da �gua para n�o ficar demasiado ou pouco brilhante a mais
            waterMaterial.SetVector("_SkyMultiplier", daySkyMultiplier);
            riverMaterial.SetVector("_SkyMultiplier", daySkyMultiplier);
        }
        
        //Enable lens flare it's between 9am and 17pm
        if (TimeHours >= 8 && TimeHours <= 17)
        {
            for (int i = 0; i < FlareData.elements.Length; i++)
            {
                FlareData.elements[i].visible = true;
            }
        }
        else
        {
            for (int i = 0; i < FlareData.elements.Length; i++)
            {
                FlareData.elements[i].visible = false;
            }
        }
    }

    public float GetSunPosition(float systemTime)
    {
        float sunRotation = 0;

        sunRotation = (14 * systemTime) - 98;

        return sunRotation;
    }

    public float GetSunColorTemp(float systemTime)
    {
        float sunColorTemp = 0;

        if (systemTime >= 6 && systemTime <= 12)
        {
            sunColorTemp = (-750f * systemTime) + 14500;
        }
        else if (systemTime > 12 && systemTime <= 20)
        {
            sunColorTemp = (-62.5f * systemTime) + 6250;
        }

        return (sunColorTemp);
    }

    public float GetSunIntensity(float systemTime)
    {
        float sunIntensity = 0;

        if (systemTime >= 6 && systemTime <= 12)
        {
            sunIntensity = (16000f * systemTime) - 89000f;
        }
        else if (systemTime > 12 && systemTime <= 20)
        {
            sunIntensity = (-11875 * systemTime) + 235000;
        }

        return (sunIntensity);
    }
}
