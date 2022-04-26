// https://github.com/ReCogMission/FirstTutorials MIT license
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightController : MonoBehaviour
{
    [SerializeField] private Transform sunTransform;
    [SerializeField] private Light sun;
    [SerializeField] private float angleAtNoon;
    [SerializeField] private Vector3 hourMinuteSecond = new Vector3(6f, 0f, 0f), hmsSunSet = new Vector3(18f, 0f, 0f);
    [SerializeField] public int days = 0;
    [SerializeField] public float speed = 100;
    [SerializeField] private float intensityAtNoon = 1f, intensityAtSunSet = 0.5f;
    [SerializeField] private Color fogColorDay = Color.grey, fogColorNight = Color.black;
    [SerializeField] private Transform starsTransform;
    [SerializeField] private Vector3 hmsStarsLight = new Vector3(19f, 30f, 0f), hmsStarsExtinguish = new Vector3(03f, 30f, 0f);
    [SerializeField] private float starsFadeInTime = 7200f, starsFadeOutTime = 7200f;

    [NonSerialized] public float time;

    private float intensity, rotation, prev_rotation = -1f, sunSet, sunRise, sunDayRatio, fade, timeLight, timeExtinguish;
    private Color tintColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Vector3 dir;
    private Renderer rend;

    void Start()
    {
        rend = starsTransform.GetComponent<ParticleSystem>().GetComponent<Renderer>();
        time = HMS_to_Time(hourMinuteSecond.x, hourMinuteSecond.y, hourMinuteSecond.z);
        sunSet = HMS_to_Time(hmsSunSet.x, hmsSunSet.y, hmsSunSet.z);
        sunRise = 86400f - sunSet;
        sunDayRatio = (sunSet - sunRise) / 43200f;
        dir = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angleAtNoon), Mathf.Sin(Mathf.Deg2Rad * angleAtNoon), 0f);
        //dir = new Vector3(1f, 0f, 0f);
        starsFadeInTime /= speed;
        starsFadeOutTime /= speed;
        fade  = 0;
        timeLight = HMS_to_Time(hmsStarsLight.x, hmsStarsLight.y, hmsStarsLight.z);
        timeExtinguish = HMS_to_Time(hmsStarsExtinguish.x, hmsStarsExtinguish.y, hmsStarsExtinguish.z);
    }

    void Update()
    {
        time += Time.deltaTime * speed;
        if (time > 86400f)
        {
            days += 1;
            time -= 86400f;
        }
        if (prev_rotation == -1f)
        {
            sunTransform.eulerAngles = Vector3.zero;
            prev_rotation = 0f;
        }
        else prev_rotation = rotation;

        rotation = (time - 21600f) / 86400f * 360f;
        sunTransform.Rotate(dir, rotation - prev_rotation);
        starsTransform.Rotate(dir, rotation - prev_rotation);

        if (time < sunRise) intensity = intensityAtSunSet * time / sunRise;
        else if (time < 43200f) intensity = intensityAtSunSet + (intensityAtNoon - intensityAtSunSet) * (time - sunRise) / (43200f - sunRise);
        else if (time < sunSet) intensity = intensityAtNoon - (intensityAtNoon - intensityAtSunSet) * (time - 43200f) / (sunSet - 43200f);
        else intensity = intensityAtSunSet - (1f - intensityAtSunSet) * (time - sunSet) / (86400f - sunSet);

        RenderSettings.fogColor = Color.Lerp(fogColorNight, fogColorDay, intensity * intensity);
        if (sun != null) sun.intensity = intensity;

        if (Time_Falls_Between(time, timeLight, timeExtinguish))
        {
            fade += Time.deltaTime / starsFadeInTime;
            if (fade > 1f) fade = 1f;
        }
        else
        {
            fade -= Time.deltaTime / starsFadeOutTime;
            if (fade < 0f) fade = 0f;
        }
        tintColor.a = fade;
        rend.material.SetColor("_TintColor", tintColor);
    }

    private float HMS_to_Time(float hour, float minute, float second)
    {
        return 3600 * hour + 60 * minute + second;
    }

    private bool Time_Falls_Between(float currentTime, float startTime, float endTime)
    {
        if (startTime<endTime)
        {
            if (currentTime >= startTime && currentTime <= endTime) return true;
            else return false;
        }
        else
        {
            if (currentTime < startTime && currentTime > endTime) return false;
            else return true;
        }
        
    }

}
