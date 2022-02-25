using SurvivalTemplatePro.InventorySystem;
using System.Collections;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class StorageCrate : Interactable, IExternalContainer
    {
        public ItemContainer ItemContainer { get; private set; }

        [BHeader("Settings (Storage Crate)")]

        [SerializeField]
        [Tooltip("How many slots should this storage crate have.")]
        private int m_StorageSpots;

        [BHeader("Animation")]

        [SerializeField]
        [Tooltip("Crate cover transform (used for the open/close animation")]
        private Transform m_Cover;

        [SerializeField]
        [Tooltip("Should the cover be animated?")]
        private bool m_AnimateCover;

        [SerializeField]
        [Tooltip("How long should the open/close animations last.")]
        private float m_AnimationDuration = 1f;

        [SerializeField]
        [Tooltip("Animation easing type.")]
        private Easings.Function m_AnimationStyle = Easings.Function.QuadraticEaseInOut;

        [SerializeField]
        [Tooltip("The crate cover closed rotation")]
        private Vector3 m_ClosedRotation;

        [SerializeField]
        [Tooltip("The crate cover open rotation.")]
        private Vector3 m_OpenRotation;

        private Easer m_CoverAnimator;


        public void OpenCrate()
        {
            if (m_AnimateCover)
            {
                StopAllCoroutines();
                StartCoroutine(C_OpenCrate(true));
            }
        }

        public void CloseCrate()
        {
            if (m_AnimateCover)
            {
                StopAllCoroutines();
                StartCoroutine(C_OpenCrate(false));
            }
        }

        private void Awake()
        {
            ItemContainer = new ItemContainer("Storage", 100, m_StorageSpots, ItemContainerFlags.External, null, null, null);
            m_CoverAnimator = new Easer(Easings.Function.QuadraticEaseInOut, m_AnimationDuration);
        }

        private IEnumerator C_OpenCrate(bool open)
        {
            m_CoverAnimator.Reset();
            m_CoverAnimator.Duration = m_AnimationDuration;
            m_CoverAnimator.Function = m_AnimationStyle;

            Quaternion startRotation = m_Cover.localRotation;
            Quaternion targetRotation = Quaternion.Euler(open ? m_OpenRotation : m_ClosedRotation);

            while (m_CoverAnimator.InterpolatedValue < 1f)
            {
                m_CoverAnimator.Update(Time.deltaTime);
                m_Cover.localRotation = Quaternion.Lerp(startRotation, targetRotation, m_CoverAnimator.InterpolatedValue);

                yield return null;
            }
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            if (m_CoverAnimator != null)
            {
                m_CoverAnimator.Function = m_AnimationStyle;
                m_CoverAnimator.Duration = m_AnimationDuration;
            }
        }
        #endif
    }
}