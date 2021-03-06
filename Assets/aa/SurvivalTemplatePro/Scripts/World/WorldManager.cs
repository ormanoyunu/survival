using System.Collections;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurvivalTemplatePro.WorldManagement
{
    public class WorldManager : WorldManagerBase
    {
        public override bool TimeProgressionEnabled 
        {
            get => m_Time.ProgressTime;
            set => m_Time.ProgressTime = value;
        }

        [SerializeField]
        private TimeSettings m_Time;

        [SerializeField]
        private SkySettings m_Sky;

        [SerializeField]
        private LightingSettings m_Lighting;

        [SerializeField]
        private GlobalReflectionsSettings m_Reflections;

        private float m_DayDurationInMinutes;
        private float m_DayDurationInSeconds;
        private float m_TimeIncrementPerSecond;

        private float m_NormalizedTime;
        private TimeOfDay m_TimeOfDay;
        private WaitForSeconds m_ReflProbeUpdateInterval;


#if UNITY_EDITOR
        public void DisplayDebugInfo()
        {
            GUIStyle style = EditorStyles.centeredGreyMiniLabel;
            style.alignment = TextAnchor.MiddleLeft;

            EditorGUILayout.LabelField("Normalized Time: " + (float)System.Math.Round(m_NormalizedTime, 2), style);
            EditorGUILayout.LabelField("Time Of Day: " + m_TimeOfDay, style);
            EditorGUILayout.LabelField("Hour: " + m_Time.Hour, style);
            EditorGUILayout.LabelField("Minute: " + m_Time.Minute, style);
            EditorGUILayout.LabelField("Second: " + m_Time.Second, style);
        }

        private void OnValidate()
        {
            HandleSettings();
        }
#endif

        public override float GetNormalizedTime() => m_NormalizedTime;
        public override TimeOfDay GetTimeOfDay() => m_TimeOfDay;
        public override GameTime GetGameTime() => new GameTime(m_Time.Hour, m_Time.Minute, m_Time.Second);
        public override float GetDayDurationInMinutes() => m_DayDurationInMinutes;
        public override float GetTimeIncrementPerSecond() => m_TimeIncrementPerSecond;

        public override void PassTime(float timeToPass, float duration) => StartCoroutine(C_PassTime(timeToPass, duration));

        private void Awake()
        {
            // Set up global reflections
            if (m_Reflections.ReflectionProbe != null)
            {
                m_Reflections.ReflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.IndividualFaces;

                m_ReflProbeUpdateInterval = new WaitForSeconds(m_Reflections.UpdateInterval);
                StartCoroutine(C_UpdateReflections());
            }

            m_DayDurationInMinutes = m_Time.DayDurationInMinutes + m_Time.NightDurationInMinutes;

            HandleSettings();
        }

        private void Update()
        {
            if (!m_Time.ProgressTime)
                return;

            UpdateTime();
            UpdateSun();
            UpdateMoon();
            UpdateAmbientLight();
        }

        private void UpdateTime()
        {
            m_NormalizedTime += m_TimeIncrementPerSecond * Time.deltaTime;
            m_NormalizedTime = Mathf.Repeat(m_NormalizedTime, 1f);

            m_TimeOfDay = (m_NormalizedTime < 0.25f || m_NormalizedTime >= 0.75f) ? TimeOfDay.Night : TimeOfDay.Day;

            m_Time.Hour = (int)(m_NormalizedTime * 24);
            m_Time.Minute = (int)((m_NormalizedTime * 24 - m_Time.Hour) * 60);
            m_Time.Second = (int)((((m_NormalizedTime * 24 - m_Time.Hour) * 60) - m_Time.Minute) * 60);
        }

        private void UpdateSun()
        {
            float dayThreeshold = (m_TimeOfDay == TimeOfDay.Night) ? 0f : Mathf.InverseLerp(0.25f, 0.75f, m_NormalizedTime);

            // Intensity
            m_Lighting.DirectionalSunLight.intensity = m_Lighting.SunLight.IntensityCurve.Evaluate(m_NormalizedTime) * m_Lighting.SunLight.Intensity;

            // Color
            m_Lighting.DirectionalSunLight.color = m_Lighting.SunLight.Color.Evaluate(dayThreeshold);

            // Rotation
            float sunAngle = Mathf.Lerp(m_Sky.SunRiseAngle, m_Sky.SunSetAngle, dayThreeshold);
            m_Lighting.DirectionalSunLight.transform.localEulerAngles = new Vector3(sunAngle, m_Sky.SunTilt, 0f);
        }

        private void UpdateMoon()
        {
            float nightTreeshold = 0f;
            
            if(m_NormalizedTime <= 0.25f)
                nightTreeshold = m_NormalizedTime * 2 + 0.5f;
            else if(m_NormalizedTime >= 0.75f)
                nightTreeshold = (m_NormalizedTime - 0.75f) * 2;
   
            // Intensity
            m_Lighting.DirectionalMoonLight.intensity = m_Lighting.MoonLight.IntensityCurve.Evaluate(m_NormalizedTime) * m_Lighting.MoonLight.Intensity;

            // Color
            m_Lighting.DirectionalMoonLight.color = m_Lighting.MoonLight.Color.Evaluate(nightTreeshold);

            // Rotation
            float moonAngle = Mathf.Lerp(m_Sky.MoonRiseAngle, m_Sky.MoonSetAngle, nightTreeshold);
            m_Lighting.DirectionalMoonLight.transform.localEulerAngles = new Vector3(moonAngle, m_Sky.MoonTilt, 0f);
        }

        private void UpdateAmbientLight()
        {
            RenderSettings.ambientSkyColor = m_Lighting.AmbientSkyLight.Color.Evaluate(m_NormalizedTime);
            RenderSettings.ambientEquatorColor = m_Lighting.AmbientEquatorLight.Color.Evaluate(m_NormalizedTime);
            RenderSettings.ambientGroundColor = m_Lighting.AmbientGroundLight.Color.Evaluate(m_NormalizedTime);
        }

        private void HandleSettings()
        {
            m_NormalizedTime = (float)m_Time.Hour / 24 + (float)m_Time.Minute / 1440 + (float)m_Time.Second / 86400;
            m_TimeOfDay = (m_NormalizedTime < 0.25f || m_NormalizedTime >= 0.75f) ? TimeOfDay.Night : TimeOfDay.Day;

            m_DayDurationInSeconds = (m_Time.DayDurationInMinutes + m_Time.NightDurationInMinutes) * 60;
            m_TimeIncrementPerSecond = 1f / m_DayDurationInSeconds;

            UpdateSun();
            UpdateMoon();
            UpdateAmbientLight();

            // Update the global reflection probe
            if (m_Reflections.ReflectionProbe != null && m_Reflections.ReflectionProbe.gameObject.activeInHierarchy)
            {
                if (Application.isPlaying)
                    m_ReflProbeUpdateInterval = new WaitForSeconds(m_Reflections.UpdateInterval);
                else
                {
                    m_Reflections.ReflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                    m_Reflections.ReflectionProbe.RenderProbe();

                    m_Reflections.ReflectionProbe.enabled = false;
                    m_Reflections.ReflectionProbe.enabled = true;
                }

                m_Reflections.ReflectionProbe.intensity = m_Reflections.Intensity;
            }
        }

        private IEnumerator C_UpdateReflections()
        {
            while (true)
            {
                if (m_Reflections.ReflectionProbe != null && m_Time.ProgressTime)
                    m_Reflections.ReflectionProbe.RenderProbe();

                yield return m_ReflProbeUpdateInterval;
            }
        }

        private IEnumerator C_PassTime(float timeToPass, float duration)
        {
            yield return null;

            bool timeProgressionActive = m_Time.ProgressTime;
            m_Time.ProgressTime = true;

            m_TimeIncrementPerSecond = timeToPass * (1 / duration);

            yield return new WaitForSeconds(duration);

            m_Time.ProgressTime = timeProgressionActive;
            m_TimeIncrementPerSecond = 1f / m_DayDurationInSeconds;
        }
    }
}