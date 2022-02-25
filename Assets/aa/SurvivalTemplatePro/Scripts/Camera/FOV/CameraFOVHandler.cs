using System;
using UnityEngine;

namespace SurvivalTemplatePro.CameraSystem
{
    /// <summary>
    /// Handles the World & Overlay FOV of a camera.
    /// </summary>
    public class CameraFOVHandler : CharacterBehaviour, ICameraFOVHandler
    {
        #region Internal
        [Serializable]
        private class FOVCameraState
        {
            [Range(0.1f, 5f)]
            public float FOVMultiplier = 1f;

            [Range(0f, 30f)]
            public float FOVSetSpeed = 30f;

            public FOVCameraState(float fovMultiplier, float fovSetSpeed)
            {
                FOVMultiplier = fovMultiplier;
                FOVSetSpeed = fovSetSpeed;
            }
        }
        #endregion

        public Camera UnityWorldCamera => m_WorldCamera;
        public Camera UnityOverlayCamera => m_OverlayCamera;
        public float BaseWorldFOV => m_BaseWorldFOV;
        public float BaseOverlayFOV => m_BaseOverlayFOV;

        [BHeader("World Camera")]

        [SerializeField]
        private Camera m_WorldCamera;

        [SerializeField, Range(30f, 120f)]
        private float m_BaseWorldFOV = 90f;

        [BHeader("'Field of View' multipliers")]

        [SerializeField]
        private FOVCameraState m_IdleState;

        [SerializeField]
        private FOVCameraState m_CrouchState;

        [SerializeField]
        private FOVCameraState m_RunState;

        [BHeader("Overlay Camera")]

        [SerializeField]
        private Camera m_OverlayCamera;

        [SerializeField, Range(30f, 120f)]
        private float m_BaseOverlayFOV = 50f;

        [SerializeField]
        private FOVCameraState m_IdleOverlayState;

        private ICharacterMover m_CMover;

        private FOVCameraState m_CurrentWorldState;
        private FOVCameraState m_CustomWorldState;
        private FOVCameraState m_CustomOverlayState;


        public override void OnInitialized()
        {
            if (TryGetModule(out m_CMover))
                m_CMover.onMotionChanged += OnMotionChanged;

            m_WorldCamera.fieldOfView = m_BaseWorldFOV * 0.95f;
            m_OverlayCamera.fieldOfView = m_BaseOverlayFOV * 0.95f;

            m_CurrentWorldState = m_IdleState;
            m_CustomWorldState = null;
        }

        public void SetCustomWorldFOV(float fovMultiplier, float setSpeed = 10f)
        {
            m_CustomWorldState = new FOVCameraState(fovMultiplier, setSpeed);

            if (setSpeed == 0f)
                m_WorldCamera.fieldOfView = m_BaseWorldFOV * m_CustomWorldState.FOVMultiplier;
        }

        public void ClearCustomWorldFOV(bool instantly)
        {
            m_CustomWorldState = null;

            if (instantly)
                m_WorldCamera.fieldOfView = m_BaseWorldFOV * m_CurrentWorldState.FOVMultiplier;
        }

        public void SetCustomOverlayFOV(float fov)
        {
            float fovMultiplier = fov / (m_BaseOverlayFOV * m_IdleOverlayState.FOVMultiplier);
            m_CustomOverlayState = new FOVCameraState(fovMultiplier, 100f);
            m_OverlayCamera.fieldOfView = fov;
        }

        public void SetCustomOverlayFOV(float fovMultiplier, float setSpeed = 2f)
        {
            m_CustomOverlayState = new FOVCameraState(fovMultiplier, setSpeed);

            if (setSpeed == 0f)
                m_OverlayCamera.fieldOfView = m_BaseOverlayFOV * fovMultiplier;
        }

        public void ClearCustomOverlayFOV(bool instantly)
        {
            m_CustomOverlayState = null;

            if (instantly)
                m_OverlayCamera.fieldOfView = m_BaseOverlayFOV * m_IdleOverlayState.FOVMultiplier;
        }

        private void Update()
        {
            if (!IsInitialized)
                return;

            // Set World Camera FOV
            SetWorldCameraFOV();

            // Set Overlay Camera FOV
            SetOverlayCameraFOV();
        }

        private void SetWorldCameraFOV()
        {
            float baseFov = m_BaseWorldFOV * m_CurrentWorldState.FOVMultiplier;
            float fovMod = m_CustomWorldState == null ? baseFov : m_CustomWorldState.FOVMultiplier * baseFov;
            float fovSetSpeed = m_CustomWorldState == null ? m_CurrentWorldState.FOVSetSpeed : m_CustomWorldState.FOVSetSpeed;

            m_WorldCamera.fieldOfView = Mathf.Lerp(m_WorldCamera.fieldOfView, fovMod, fovSetSpeed * Time.deltaTime);
        }

        private void SetOverlayCameraFOV()
        {
            float baseFov = m_BaseOverlayFOV * m_IdleOverlayState.FOVMultiplier;
            float fovMod = m_CustomOverlayState == null ? baseFov : m_CustomOverlayState.FOVMultiplier * baseFov;
            float fovSetSpeed = m_CustomOverlayState == null ? m_IdleOverlayState.FOVSetSpeed : m_CustomOverlayState.FOVSetSpeed;

            m_OverlayCamera.fieldOfView = Mathf.Lerp(m_OverlayCamera.fieldOfView, fovMod, fovSetSpeed * Time.deltaTime);
        }

        private void OnMotionChanged(CharMotionMask motionMask, bool active)
        {
            if (motionMask.Has(CharMotionMask.Run) && active)
                m_CurrentWorldState = m_RunState;
            else if (motionMask.Has(CharMotionMask.Crouch) && active)
                m_CurrentWorldState = m_CrouchState;
            else
                m_CurrentWorldState = m_IdleState;
        }
    }
}