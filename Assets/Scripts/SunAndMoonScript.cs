using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class SunAndMoonScript : MonoBehaviour
{
    [SerializeField] private float SecondsOfDayAndNight;
    [SerializeField] private Light2D SunLight;
    [SerializeField] private Light2D MoonLight;

    private float timeRemaining;



    private void Start()
    {
        timeRemaining = SecondsOfDayAndNight;
    }




    private void Update()
    {
        timeRemaining -= Time.deltaTime;

        if (Mathf.Abs(timeRemaining) < .1f)
        {
            SwitchDayAndNight();
            timeRemaining = SecondsOfDayAndNight;
        }
    }



    /// <summary>
    /// Disables one light and enalbes another - switching the appearance between night and day
    /// </summary>
    private void SwitchDayAndNight()
    {
        // Switch to NIGHT
        if (GlobalSettings.sunIsOut)
        {
            GlobalSettings.sunIsOut = false;
            SunLight.gameObject.SetActive(false);
            MoonLight.gameObject.SetActive(true);

        }
        // Switch to DAY
        else
        {
            GlobalSettings.sunIsOut = true;

            SunLight.gameObject.SetActive(true);
            MoonLight.gameObject.SetActive(false);
        }
    }
}
