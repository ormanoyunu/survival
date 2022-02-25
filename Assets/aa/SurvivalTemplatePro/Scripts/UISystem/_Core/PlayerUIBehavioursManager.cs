using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.UISystem
{
    public class PlayerUIBehavioursManager : MonoBehaviour, IPlayerUIBehavioursManager
    {
        private readonly List<IPlayerUIBehaviour> m_UIBehaviours = new List<IPlayerUIBehaviour>();
        private ICharacter m_AttachedPlayer;


        public void AttachToPlayer(ICharacter player)
        {
            m_AttachedPlayer = player;

            if (player.IsInitialized)
                OnPlayerInitialized();
            else
                player.onInitialized += OnPlayerInitialized;
        }

        public void DetachFromPlayer(ICharacter player)
        {
            if (m_AttachedPlayer != player)
                return;

            foreach (var behaviour in m_UIBehaviours)
                behaviour.OnDetachment();
        }

        public void AddBehaviour(IPlayerUIBehaviour behaviour)
        {
            if (!m_UIBehaviours.Contains(behaviour))
            {
                behaviour.InitBehaviour(m_AttachedPlayer);
                behaviour.OnAttachment();

                m_UIBehaviours.Add(behaviour);
            }
        }

        public void RemoveBehaviour(IPlayerUIBehaviour behaviour)
        {
            if (m_UIBehaviours.Remove(behaviour))
                behaviour.OnDetachment();
        }

        private void Update()
        {
            if (m_AttachedPlayer != null && m_AttachedPlayer.IsInitialized)
            {
                for (int i = 0; i < m_UIBehaviours.Count; i++)
                    m_UIBehaviours[i].OnInterfaceUpdate();
            }
        }

        private void OnDisable()
        {
            DetachFromPlayer(m_AttachedPlayer);
        }

        private void OnPlayerInitialized()
        {
            if (m_UIBehaviours.Count == 0)
                m_UIBehaviours.AddRange(GetComponentsInChildren<IPlayerUIBehaviour>(true));

            foreach (var behaviour in m_UIBehaviours)
            {
                behaviour.InitBehaviour(m_AttachedPlayer);
                behaviour.OnAttachment();
            }
        }
    }
}
