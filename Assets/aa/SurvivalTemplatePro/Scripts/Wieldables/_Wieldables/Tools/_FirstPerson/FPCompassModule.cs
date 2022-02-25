using SurvivalTemplatePro.InventorySystem;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class FPCompassModule : CharacterBehaviour
    {
        [SerializeField]
        private Transform m_CompassRose;

        [SerializeField]
        private Vector3 m_CompassRoseRotationAxis;

        [SerializeField, Range(0f, 100f)]
        private float m_RotationSpeed = 3f;

        private IWieldable m_Wieldable;
        private bool m_IsBroken = false;
        private float m_Angle;


        public override void OnInitialized()
        {
            m_Wieldable = GetComponentsInParent<IWieldable>(true)[0];

            m_Wieldable.onEquippingStarted += OnEquipStarted;
            m_Wieldable.onHolsteringStarted += OnHolsterStarted;
        }

        private void LateUpdate()
        {
            if (m_IsBroken)
                return;

            m_Angle = UpdateRoseAngle(m_Angle, m_RotationSpeed * Time.deltaTime);
            m_CompassRose.localRotation = Quaternion.AngleAxis(m_Angle, m_CompassRoseRotationAxis);
        }

        private float UpdateRoseAngle(float angle, float delta)
        {
            angle = Mathf.LerpAngle(angle, Vector3.SignedAngle(Character.transform.forward, Vector3.forward, Vector3.up), delta);
            angle = Mathf.Repeat(angle, 360f);

            return angle;
        }

        private void OnEquipStarted()
        {
            m_Wieldable.ItemDurability.onChanged += OnDurabilityChanged;
            m_IsBroken = (m_Wieldable.ItemDurability.Float < 0.01f);
        }

        private void OnHolsterStarted(float holsterSpeed)
        {
            if (m_IsBroken)
                m_Wieldable.ItemDurability.onChanged -= OnDurabilityChanged;
        }

        private void OnDurabilityChanged(ItemProperty property)
        {
            if (property.Float > 0.01f)
                m_IsBroken = false;
            else
                m_IsBroken = true;
        }
    }
}