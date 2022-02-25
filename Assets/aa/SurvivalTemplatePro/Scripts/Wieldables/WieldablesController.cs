using SurvivalTemplatePro.InventorySystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.WieldableSystem
{
    /// <summary>
    /// Controller responsible for equipping and holstering wieldables.
    /// </summary>
    [RequireComponent(typeof(IRayGenerator))]
    public class WieldablesController : CharacterBehaviour, IWieldablesController
    {
        public IWieldable ActiveWieldable => m_ActiveWieldable;

        public float LastEquipTime => m_EquipTime;
        public bool IsEquipping => m_WieldableToEquip != null;
        public bool IsHolstering => m_WieldableToDisable != null;
        public bool HasEquippedWieldable => m_ActiveWieldable != null;

        public event UnityAction<IWieldable> onWieldableEquipped;

        [SerializeField]
        [Tooltip("The audio player that every wieldable will use.")]
        private AudioPlayer m_AudioPlayer;

        private List<IWieldable> m_Wieldables = new List<IWieldable>();
        private IWieldable m_ActiveWieldable;

        private IWieldable m_WieldableToDisable;
        private float m_DisableTime;

        private IWieldable m_WieldableToEquip;
        private bool m_Equip;
        private float m_EquipTime;

        private Item m_ItemToAttach;


        public bool GetWieldableOfType<T>(out T wieldable) where T : IWieldable
        {
            if (m_Wieldables != null)
            {
                for (int i = 0; i < m_Wieldables.Count; i++)
                {
                    if (m_Wieldables[i].GetType() == typeof(T))
                    {
                        wieldable = (T)m_Wieldables[i];
                        return true;
                    }
                }
            }

            wieldable = default;

            return false;
        }

        public bool HasWieldable(IWieldable wieldable) => m_Wieldables.Contains(wieldable);

        public bool TryEquipWieldable(IWieldable wieldable, Item itemToAttach = null, float holsterSpeed = 1f)
        {
            if (CanNotEquip(itemToAttach))
                return false;

            holsterSpeed = Mathf.Clamp(holsterSpeed, 0.1f, 10f);

            // Handle disabling the previous wieldable
            HolsterWieldable(holsterSpeed);

            // Handle showing the current item
            if (HasWieldable(wieldable) || wieldable == null)
            {
                m_WieldableToEquip = wieldable;
                m_EquipTime = Time.time;

                if (m_WieldableToDisable != null)
                    m_EquipTime += m_WieldableToDisable.HolsterDuration / holsterSpeed;

                m_Equip = true;
                m_ActiveWieldable = m_WieldableToEquip;
                m_ItemToAttach = itemToAttach;
            }

            return true;
        }

        public IWieldable SpawnWieldable(IWieldable wieldable)
        {
            IWieldable spawnedWieldable = Instantiate(wieldable.gameObject, transform.position, transform.rotation, transform).GetComponent<IWieldable>();
            spawnedWieldable.AudioPlayer = m_AudioPlayer;
            spawnedWieldable.RayGenerator = GetComponent<IRayGenerator>();

            m_Wieldables.Add(spawnedWieldable);
            spawnedWieldable.SetVisibility(false);
            spawnedWieldable.SetWielder(Character);

            return spawnedWieldable;
        }

        public bool DestroyWieldable(IWieldable wieldable) 
        {
            if (HasWieldable(wieldable))
            {
                if (m_ActiveWieldable == wieldable)
                {
                    m_ActiveWieldable.OnHolster(10f);
                    m_ActiveWieldable.SetVisibility(false);
                }

                Destroy(wieldable.gameObject, 1f);

                return true;
            }

            return false;
        }

        private void Update()
        {
            if (!IsInitialized)
                return;

            // Disable prev wieldable
            if (m_WieldableToDisable != null && Time.time > m_DisableTime)
            {
                m_WieldableToDisable.SetVisibility(false);

                m_WieldableToDisable = null;
                m_ActiveWieldable = null;
            }

            // Enable next wieldable
            if (Time.time > m_EquipTime && m_Equip)
            {
                if (m_WieldableToEquip != null)
                {
                    m_WieldableToEquip.SetVisibility(true);
                    m_WieldableToEquip.OnEquip(m_ItemToAttach);

                    m_ActiveWieldable = m_WieldableToEquip;

                    m_WieldableToEquip = null;
                    m_ItemToAttach = null;
                }

                onWieldableEquipped?.Invoke(m_ActiveWieldable);
                m_Equip = false;
            }
        }

        private void HolsterWieldable(float holsterSpeed)
        {
            if (HasEquippedWieldable)
            {
                m_WieldableToDisable = m_ActiveWieldable;
                m_DisableTime = Time.time + (m_ActiveWieldable.HolsterDuration / holsterSpeed);

                m_WieldableToDisable.OnHolster(holsterSpeed);

                m_ActiveWieldable = null;
            }
        }

        private bool CanNotEquip(Item item) => IsEquipping || IsHolstering || (item != null && (HasEquippedWieldable && item == m_ActiveWieldable.AttachedItem));
    }
}