using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class PlayerLookHandler : MonoBehaviour, ILookHandler
    {
        public Vector2 LookAngle => m_ViewAngle;
        public Vector2 CurrentInput => m_CurrentInput;
        public Transform XTransform { get => m_XTransform; set => m_XTransform = value; }
        public Transform YTransform { get => m_YTransform; set => m_YTransform = value; }

        public event UnityAction onPostViewUpdate;

        [SerializeField]
        [Tooltip("Transform to rotate Up & Down.")]
        private Transform m_XTransform;

        [SerializeField]
        [Tooltip("Transform to rotate Left & Right.")]
        private Transform m_YTransform;

        [Space]

        [SerializeField]
        [Tooltip("The up & down rotation will be inverted, if checked.")]
        private bool m_Invert;

        [SerializeField]
        [Tooltip("Vertical look limits (in angles).")]
        private Vector2 m_LookLimits = new Vector2(-60f, 90f);

        [BHeader("Look Feel")]

        [SerializeField, Range(0.1f, 10f)]
        [Tooltip("Rotation Speed.")]
        private float m_Sensitivity = 1.5f;

        [Space]

        [SerializeField]
        [Tooltip("If enabled, the rotation will not be smoothed")]
        private bool m_Raw;

        [SerializeField, Range(0, 20)]
        [Tooltip("Smooth steps amount (a bigger sample will make means the rotations will be more smoothed).")]
        private int m_SmoothSteps = 10;

        [SerializeField, Range(0f, 1f)]
        [Tooltip("Smoothness affect modifier, a value of 0 means that the rotation will be raw while 1 as smooth as possible")]
        private float m_SmoothWeight = 0.4f;

        [Space]

        [SerializeField]
        [InfoBox("Used in lowering/increasing the current sensitivity based on the FOV")]
        private Camera m_FOVCamera;

        private Vector2 m_ViewAngle;
        private Vector2 m_CurrentInput;

        private float m_CurrentSensitivity;
        private float m_SensitivityMod = 1f;
        private Vector2 m_CurrentMouseLook;
        private Vector2 m_SmoothMove;
        private readonly List<Vector2> m_SmoothBuffer = new List<Vector2>();

        private Vector2 m_CurrentAdditiveLook;
        private Vector2 m_CurrentAdditiveMovementVelocity;
        private float m_AdditiveLookDuration;


        public void AddAdditiveLookOverTime(Vector2 amount, float duration)
        {
            m_CurrentAdditiveLook = amount;
            m_AdditiveLookDuration = duration;
        }

        public void SetSensitivityMod(float mod) => m_SensitivityMod = mod;

        public void UpdateLook(Vector2 input)
        {
            m_CurrentSensitivity = GetTargetSensitivity(m_CurrentSensitivity, Time.deltaTime * 8f);

            m_CurrentInput = input / 10;
            m_CurrentInput.ReverseVector();

            MoveView(m_CurrentInput, Time.deltaTime);
            UpdateAdditiveLook();

            onPostViewUpdate?.Invoke();
        }

        private void Start()
        {
            if (!m_XTransform)
            {
                Debug.LogErrorFormat(this, "Assign the X Transform in the inspector!", name);
                enabled = false;
            }
            else if (!m_YTransform)
            {
                Debug.LogErrorFormat(this, "Assign the Y Transform in the inspector!", name);
                enabled = false;
            }
        }

        private void UpdateAdditiveLook()
        {
            m_CurrentAdditiveLook = Vector2.SmoothDamp(m_CurrentAdditiveLook, Vector2.zero, ref m_CurrentAdditiveMovementVelocity, m_AdditiveLookDuration);

            if (m_CurrentAdditiveLook != Vector2.zero)
                m_ViewAngle += m_CurrentAdditiveLook;
        }

        private void MoveView(Vector2 lookInput, float deltaTime)
        {
            if (!m_Raw)
            {
                CalculateSmoothLookInput(lookInput, deltaTime);

                m_ViewAngle.x += m_CurrentMouseLook.x * m_CurrentSensitivity * (m_Invert ? 1f : -1f);
                m_ViewAngle.y += m_CurrentMouseLook.y * m_CurrentSensitivity;

                m_ViewAngle.x = ClampAngle(m_ViewAngle.x, m_LookLimits.x, m_LookLimits.y);
            }
            else
            {
                m_ViewAngle.x += lookInput.x * m_CurrentSensitivity * (m_Invert ? 1f : -1f);
                m_ViewAngle.y += lookInput.y * m_CurrentSensitivity;

                m_ViewAngle.x = ClampAngle(m_ViewAngle.x, m_LookLimits.x, m_LookLimits.y);
            }

            m_YTransform.localRotation = Quaternion.Euler(0f, m_ViewAngle.y, 0f);
            m_XTransform.localRotation = Quaternion.Euler(m_ViewAngle.x, 0f, 0f);
        }

        /// <summary>
        /// Clamps the given angle between min and max degrees.
        /// </summary>
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle > 360f)
                angle -= 360f;
            else if (angle < -360f)
                angle += 360f;

            return Mathf.Clamp(angle, min, max);
        }

        private void CalculateSmoothLookInput(Vector2 lookInput, float deltaTime)
        {
            if (deltaTime == 0f)
                return;

            m_SmoothMove = new Vector2(lookInput.x, lookInput.y);

            m_SmoothSteps = Mathf.Clamp(m_SmoothSteps, 1, 20);
            m_SmoothWeight = Mathf.Clamp01(m_SmoothWeight);

            while (m_SmoothBuffer.Count > m_SmoothSteps)
                m_SmoothBuffer.RemoveAt(0);

            m_SmoothBuffer.Add(m_SmoothMove);

            float weight = 1f;
            Vector2 average = Vector2.zero;
            float averageTotal = 0f;

            for (int i = m_SmoothBuffer.Count - 1; i > 0; i--)
            {
                average += m_SmoothBuffer[i] * weight;
                averageTotal += weight;
                weight *= m_SmoothWeight / (deltaTime * 60f);
            }

            averageTotal = Mathf.Max(1f, averageTotal);
            m_CurrentMouseLook = average / averageTotal;
        }

        private float GetTargetSensitivity(float currentSens, float delta)
        {
            float targetSensitivity = m_Sensitivity * m_SensitivityMod;
            targetSensitivity *= m_FOVCamera != null ? m_FOVCamera.fieldOfView / 90f : 1f;

            return Mathf.Lerp(currentSens, targetSensitivity, delta);
        }
    }
}