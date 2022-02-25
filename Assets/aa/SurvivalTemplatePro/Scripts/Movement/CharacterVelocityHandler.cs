using SurvivalTemplatePro.WieldableSystem;
using UnityEngine;

namespace SurvivalTemplatePro.MovementSystem
{
    public class CharacterVelocityHandler : CharacterBehaviour
    {
        [SerializeField, Range(0f, 1f)]
        [Tooltip("How much will the max velocity be affected by low vitals (e.g. hunger, thirst etc.)")]
        private float m_LowVitalsVelocityMod = 1f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("How much will the max velocity be affected by the equipped wieldable weight.")]
        private float m_WieldableWeightVelocityMod = 1f;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("How much will the max velocity be affected by the amount of carried 'carriables'")]
        private float m_CarriablesCountVelocityMod = 1f;

        private ICharacterMover m_Mover;
        private IObjectCarryController m_ObjectCarry;
        private IWieldablesController m_WieldableController;

        private IWeightHandler m_WeightHandler;
        private IEnergyManager m_EnergyManager;
        private IHungerManager m_HungerManager;
        private IThirstManager m_ThirstManager;

        private const float m_VitalsCheckDelay = 3f;
        private float m_NextTimeCanCheckVitals;


        public override void OnInitialized()
        {
            GetModule(out m_Mover);
            GetModule(out m_ObjectCarry);
            GetModule(out m_WieldableController);
            GetModule(out m_EnergyManager);
            GetModule(out m_HungerManager);
            GetModule(out m_ThirstManager);

            m_ObjectCarry.onCarriedCountChanged += (int amount) => SetVelocityMod();
            m_WieldableController.onWieldableEquipped += OnWieldableChanged;
        }

        private void Update()
        {
            if (!IsInitialized || m_NextTimeCanCheckVitals > Time.time)
                return;

            SetVelocityMod();

            m_NextTimeCanCheckVitals = Time.time + m_VitalsCheckDelay;
        }

        private void OnWieldableChanged(IWieldable wieldable)
        {
            if (m_WeightHandler != null)
                m_WeightHandler.onWeightChanged -= SetVelocityMod;

            if (wieldable != null)
            {
                m_WeightHandler = wieldable as IWeightHandler;

                if (m_WeightHandler != null)
                    m_WeightHandler.onWeightChanged += SetVelocityMod;
            }
            else
                m_WeightHandler = null;

            SetVelocityMod();
        }

        private void SetVelocityMod() 
        {
            float velocityMod = 1f;

            velocityMod *= CalculateVelocityModForWeight(m_WeightHandler, m_WieldableWeightVelocityMod);
            velocityMod *= CalculateVelocityModForObjectCount(m_ObjectCarry.CarriedObjectsCount, m_CarriablesCountVelocityMod);
            velocityMod *= CalculateVelocityModForVitals(m_LowVitalsVelocityMod);

            m_Mover.VelocityMod = velocityMod;
        }

        private float CalculateVelocityModForWeight(IWeightHandler weighthandler, float mod) 
        {
            if (weighthandler == null)
                return 1f;

            float wieldableWeight = weighthandler.TotalWeight;
            float velocityMod = 1f - Mathf.Clamp01(wieldableWeight / 60f);

            return (1 - mod) + (velocityMod * mod);
        }

        private float CalculateVelocityModForObjectCount(int count, float mod) 
        {
            if (count > 0)
            {
                float velocityMod = Mathf.Clamp01(Mathf.Pow(0.95f, m_ObjectCarry.CarriedObjectsCount));
                return (1 - mod) + (velocityMod * mod);
            }

            return 1f;
        }

        private float CalculateVelocityModForVitals(float mod)
        {
            float velocityMod = 1f;
            velocityMod -= (m_EnergyManager.MaxEnergy - m_EnergyManager.Energy) / (m_EnergyManager.MaxEnergy * 10f);
            velocityMod -= (m_ThirstManager.MaxThirst - m_ThirstManager.Thirst) / (m_ThirstManager.MaxThirst * 10f);
            velocityMod -= (m_HungerManager.MaxHunger - m_HungerManager.Hunger) / (m_HungerManager.MaxHunger * 10f);

            return (1 - mod) + (velocityMod * mod);
        }
    }
}