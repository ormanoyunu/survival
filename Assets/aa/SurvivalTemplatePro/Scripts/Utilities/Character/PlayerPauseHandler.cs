using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    [System.Serializable]
    public class PauseEvent : UnityEvent<PlayerPauseParams> {  }

    public class PlayerPauseHandler : CharacterBehaviour, IPauseHandler
    {
        public bool PauseActive
        {
            get => m_PauseActive;
            set 
            {
                if (value != m_PauseActive)
                {
                    m_PauseActive = value;

                    if (m_PauseActive)
                    {
                        onPause?.Invoke();
                        m_OnPause?.Invoke();
                    }
                    else
                    {
                        onUnpause?.Invoke();
                        m_OnUnpause?.Invoke();
                    }
                }
            }
        }

        public event UnityAction onPause;
        public event UnityAction onUnpause;

        [SerializeField]
        private PauseEvent m_PauseParamsChanged;

        [SerializeField]
        private UnityEvent m_OnPause;

        [SerializeField]
        private UnityEvent m_OnUnpause;

        private Dictionary<Object, PlayerPauseParams> m_PauseLockers = new Dictionary<Object, PlayerPauseParams>();
        private PlayerPauseParams m_PauseParams;
        private bool m_PauseActive = false;


        public void RegisterLocker(Object locker, PlayerPauseParams pauseParams)
        {
            m_PauseLockers.Add(locker, pauseParams);
            CalculatePauseParams();

            m_PauseParamsChanged?.Invoke(pauseParams);

            PauseActive = true;
        }

        public void UnregisterLocker(Object locker)
        {
            m_PauseLockers.Remove(locker);
            CalculatePauseParams();

            m_PauseParamsChanged?.Invoke(m_PauseParams);

            if (m_PauseLockers.Count == 0)
                StartCoroutine(C_UnpauseDelayed());
        }

        public void RemoveAllLockers()
        {
            m_PauseLockers.Clear();

            m_PauseParams = PlayerPauseParams.Default;
            m_PauseParamsChanged?.Invoke(m_PauseParams);

            StartCoroutine(C_UnpauseDelayed());
        }

        private void CalculatePauseParams() 
        {
            m_PauseParams = PlayerPauseParams.Default;

            foreach (var param in m_PauseLockers.Values)
                m_PauseParams += param;
        }

        private IEnumerator C_UnpauseDelayed()
        {
            yield return null;
            PauseActive = false;
        }
    }
}