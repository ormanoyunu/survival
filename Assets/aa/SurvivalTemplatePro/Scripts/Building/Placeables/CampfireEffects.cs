using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    [RequireComponent(typeof(Campfire))]
    public class CampfireEffects : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Wood;

        [SerializeField]
        private ParticleSystem[] m_ParticleEffects;

        [BHeader("Audio")]

        [SerializeField]
        private AudioClip m_FireLoopSound;

        [SerializeField, Range(0f, 1f)]
        private float m_MinFireVolume = 0.5f;

        [SerializeField]
        private SoundPlayer m_FuelAddAudio;

        [SerializeField]
        private SoundPlayer m_FireExtinguishedAudio;

        [BHeader("Light")]

        [SerializeField]
        private LightEffect m_LightEffect;

        [SerializeField, Range(0f, 1f)]
        private float m_MinLightIntensity = 0.5f;

        [BHeader("Material")]

        [SerializeField]
        private Material m_WoodMaterial;

        [SerializeField]
        private string m_BurnAmountShaderProperty = "_BurnedAmount";

        [SerializeField, Range(0f, 600f)]
        private float m_WoodBurnDuration = 60f;

        private Campfire m_Campfire;
        private AudioSource m_AudioSource;

        private float m_LastFireStartTime;
        private int m_BurnedAmountShaderProperty;


        private void Awake()
        {
            m_Campfire = GetComponent<Campfire>();

            m_BurnedAmountShaderProperty = Shader.PropertyToID(m_BurnAmountShaderProperty);

            CreateAudioSource();
            CreateWoodMaterial();
        }

        private void OnEnable()
        {
            m_Campfire.onFireStarted += OnFireStarted;
            m_Campfire.onFireStopped += OnFireStopped;
            m_Campfire.onFuelAdded += OnFuelAdded;
        }

        private void OnDisable()
        {
            m_Campfire.onFireStarted -= OnFireStarted;
            m_Campfire.onFireStopped -= OnFireStopped;
            m_Campfire.onFuelAdded -= OnFuelAdded;
        }

        private void CreateAudioSource()
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.spatialBlend = 1f;
            m_AudioSource.clip = m_FireLoopSound;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0f;
            m_AudioSource.Play();
        }

        private void CreateWoodMaterial()
        {
            // Create material
            m_WoodMaterial = new Material(m_WoodMaterial);
            m_WoodMaterial.name = m_WoodMaterial.name + "_Instance";

            m_WoodMaterial.EnableKeyword(m_BurnAmountShaderProperty);
            m_WoodMaterial.SetFloat(m_BurnedAmountShaderProperty, 0f);

            // Assign it
            var renderers = m_Wood.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in renderers)
                renderer.material = m_WoodMaterial;
        }

        private void Update()
        {
            if (m_Campfire.FireActive)
            {
                m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, Mathf.Max(m_Campfire.TemperatureStrength, m_MinFireVolume), Time.deltaTime * 1f);

                // Shader effects
                float burnedAmount = (Time.time - m_LastFireStartTime) / m_WoodBurnDuration;
                burnedAmount = Mathf.Clamp01(burnedAmount);

                m_WoodMaterial.SetFloat(m_BurnedAmountShaderProperty, burnedAmount);

                m_LightEffect.IntensityMultiplier = Mathf.Max(m_MinLightIntensity, m_Campfire.TemperatureStrength);
            }
            else
                m_AudioSource.volume = Mathf.Lerp(m_AudioSource.volume, 0f, Time.deltaTime * 1f);
        }

        private void OnFireStarted()
        {
            foreach (var effect in m_ParticleEffects)
                effect.Play(true);

            m_Wood.SetActive(true);

            m_LastFireStartTime = Time.time;

            m_LightEffect.Play(true);
        }

        private void OnFireStopped()
        {
            foreach(var effect in m_ParticleEffects)
                effect.Stop(true);

            m_FireExtinguishedAudio.Play2D();

            m_LightEffect.Stop(true);
        }

        private void OnFuelAdded(float fuelDuration)
        {
            m_FuelAddAudio.Play(m_AudioSource);
        }

        // TODO: Move
        //[BHeader("Visual (Campfire)")]

        //[SerializeField]
        //private float m_MaxRockOffset = 0.175f;

        //[SerializeField]
        //private LayerMask m_RocksMask;

        //[SerializeField]
        //private GameObject[] m_Rocks;

        //private bool CanPlaceAllStones()
        //{
        //    Transform rocksParent = m_Rocks[0].transform.parent;
        //    bool canPlace = true;

        //    for (int i = 0; i < m_Rocks.Length; i++)
        //    {
        //        Vector3 originalWorldPos = rocksParent.TransformPoint(m_OriginalRocksPosition[i]);

        //        if (Physics.Raycast(originalWorldPos + Vector3.up, Vector3.down, out RaycastHit hitInfo, 4f, m_RocksMask))
        //        {
        //            float offset = Vector3.Distance(originalWorldPos, hitInfo.point);

        //            if (offset > m_MaxRockOffset)
        //                canPlace = false;

        //            m_Rocks[i].transform.position = hitInfo.point;
        //            m_Rocks[i].transform.rotation = Quaternion.LookRotation(m_Rocks[i].transform.forward, hitInfo.normal);
        //        }
        //    }

        //    return canPlace;
        //}
    }
}