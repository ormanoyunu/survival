using System;
using UnityEngine;

namespace SurvivalTemplatePro.WorldManagement
{
    [Serializable]
    public class TimeSettings
    {
        [BHeader("Time Progression")]

        public bool ProgressTime;

        [Range(0, 24)]
        public int Hour = 12;

        [Range(0, 60)]
        public int Minute;

        [Range(0, 60)]
        public int Second;

        [BHeader("Cycle Duration")]

        [Range(0, 120)]
        [Tooltip("Day duration in real time minutes.")]
        public float DayDurationInMinutes = 5f;

        [Range(0, 120)]
        [Tooltip("Night duration in real time minutes.")]
        public float NightDurationInMinutes = 5f;
    }

    [Serializable]
    public class CelestialBodySettings
    {
        public Light DirectionalLight;

        [BHeader("Light Color")]

        public Gradient Color;
        public Gradient StormColor;

        [BHeader("Light Intensity")]

        [Range(0f, 5f)]
        public float Intensity = 1f;

        public AnimationCurve IntensityCurve;

        [BHeader("Motion")]

        [Range(-60, 60)]
        public int Tilt = 30;

        public int RiseAngle = -30;

        public int SetAngle = 210;
    }

    [Serializable]
    public class LightingSettings
    {
        public Light DirectionalSunLight;
        public Light DirectionalMoonLight;

        [Space]

        [BoxGroup("Sun Light")]
        public LightCycleSettings SunLight;

        [Space]

        [BoxGroup("Moon Light")]
        public LightCycleSettings MoonLight;

        [Space]

        [BoxGroup("Ambient Sky Light")]
        public SimpleLightCycleSettings AmbientSkyLight;

        [Space]

        [BoxGroup("Ambient Equator Light")]
        public SimpleLightCycleSettings AmbientEquatorLight;

        [Space]

        [BoxGroup("Ambient Ground Light")]
        public SimpleLightCycleSettings AmbientGroundLight;
    }

    [Serializable]
    public class SkySettings
    {
        [BHeader("Sun Motion")]

        [Range(-60, 60)]
        public int SunTilt = 30;

        public int SunRiseAngle = -30;

        public int SunSetAngle = 210;

        [BHeader("Moon Motion")]

        [Range(-60, 60)]
        public int MoonTilt = 30;

        public int MoonRiseAngle = 210;

        public int MoonSetAngle = -30;
    }

    [Serializable]
    public class GlobalReflectionsSettings
    {
        public ReflectionProbe ReflectionProbe;

        [Range(0.1f, 1f)]
        public float UpdateInterval = 0.5f;

        [Range(0f, 3f)]
        public float Intensity = 1f;
    }

    [Serializable]
    public class LightCycleSettings
    {
        public Gradient Color;

        public Gradient StormyColor;

        [Range(0f, 5f)]
        public float Intensity = 1f;

        public AnimationCurve IntensityCurve;
    }

    [Serializable]
    public class SimpleLightCycleSettings
    {
        public Gradient Color;
        public Gradient StormyColor;
    }
}