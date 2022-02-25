using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    #region Internal
    [Serializable]
    public struct CameraForce
    {
        [Range(0f, 5f)]
        public float Delay;

        public Vector3 Force;

        [Range(1, 20)]
        public int Distribution;
    }

    [Serializable]
    public struct QueuedCameraForce
    {
        public float PlayTime { get; private set; }
        public Vector3 Force { get; private set; }
        public int Distribution { get; private set; }


        public QueuedCameraForce(CameraForce camForce)
        {
            PlayTime = Time.time + camForce.Delay;
            Force = camForce.Force;
            Distribution = camForce.Distribution;
        }
    }
    #endregion

    public class WieldableCameraAnimator : WieldableEffectsHandler
    {
        #region Internal

        [Serializable]
        protected class MotionSettings : WieldableEffect
        {
            [SerializeField, HideInInspector]
            public WieldableCameraAnimator CameraAnimator;

            public CameraForce[] CameraForces;


            public override void TriggerEffect(float value) => CameraAnimator.QueueCameraForces(CameraForces);
        }

        [Serializable]
        public struct CameraForce
        {
            [Range(0f, 5f)]
            public float Delay;

            public Vector3 Force;

            [Range(1, 20)]
            public int Distribution;
        }

        [Serializable]
        public struct QueuedCameraForce
        {
            public float PlayTime { get; private set; }
            public Vector3 Force { get; private set; }
            public int Distribution { get; private set; }


            public QueuedCameraForce(CameraForce camForce)
            {
                PlayTime = Time.time + camForce.Delay;
                Force = camForce.Force;
                Distribution = camForce.Distribution;
            }
        }

        #endregion

        [SerializeField]
        private CameraForce[] m_EquipCamForces;

        [SerializeField]
        private CameraForce[] m_HolsterCamForces;

        [Space]

        [SerializeField]
        private MotionSettings[] m_CustomMotionSettings;

        private ICameraMotionHandler m_Motion;
        private readonly List<QueuedCameraForce> m_QueuedCamForces = new List<QueuedCameraForce>(10);


        public override WieldableEffect[] GetAllEffects() => m_CustomMotionSettings;

        protected void QueueCameraForce(CameraForce force) => m_QueuedCamForces.Add(new QueuedCameraForce(force));

        protected void QueueCameraForces(CameraForce[] forces)
        {
            for (int i = 0; i < forces.Length; i++)
                m_QueuedCamForces.Add(new QueuedCameraForce(forces[i]));
        }

        protected override void InitModule(ICharacter character)
        {
            character.TryGetModule(out m_Motion);
        }

        protected override void OnWieldableEquipped()
        {
            base.OnWieldableEquipped();
            QueueCameraForces(m_EquipCamForces);
        }

        protected override void OnWieldableHolstered(float holsterSpeed)
        {
            m_QueuedCamForces.Clear();
            QueueCameraForces(m_HolsterCamForces);
        }

        protected virtual void Update()
        {
            for (int i = 0; i < m_QueuedCamForces.Count; i++)
            {
                QueuedCameraForce cameraForce = m_QueuedCamForces[i];

                if (Time.time >= cameraForce.PlayTime)
                {
                    m_Motion.AddRotationForce(cameraForce.Force, cameraForce.Distribution);
                    m_QueuedCamForces.RemoveAt(i);
                }
            }
        }

        protected void OnValidate()
        {
            if (m_CustomMotionSettings != null && m_CustomMotionSettings.Length > 0)
            {
                foreach (var motion in m_CustomMotionSettings)
                    motion.CameraAnimator = this;
            }
        }
    }
}