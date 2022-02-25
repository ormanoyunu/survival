using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class StructureManager : MonoBehaviour
    {
        #region Internal
        [Serializable]
        protected class BuildableState
        {
            public string Name;
            public Vector3 Position;
            public Quaternion Rotation;
            public SocketState[] Sockets;
        }

        [Serializable]
        protected class SocketState
        {
            public SocketType OccupiedSpaces;
        }
        #endregion

        public List<Buildable> Buildables => m_Buildables;

        public event UnityAction<Buildable> OnPartAdded;

        [SerializeField]
        private SoundPlayer m_BuildAudio;

        private List<Buildable> m_Buildables = new List<Buildable>();
        private List<BuildableState> m_BuildableStates = new List<BuildableState>();

        private BuildableActivationState m_ActivationState;


        #region Save&Load
        public void OnLoad()
        {
            foreach (BuildableState state in m_BuildableStates)
            {
                Buildable buildablePrefab = PlaceableDatabase.GetPlaceableByName(state.Name) as Buildable;
                Buildable buildable = null;

                if (buildablePrefab != null)
                {
                    buildable = Instantiate(buildablePrefab, state.Position, state.Rotation, transform);
                    buildable.ParentStructure = this;

                    for (int i = 0; i < buildable.Sockets.Length; i++)
                        buildable.Sockets[i].OccupiedSpaces = state.Sockets[i].OccupiedSpaces;
                }

                AddPart(buildable, false, true);
            }
        }
        #endregion

        public int IndexOfSocket(Socket socket)
        {
            int index = 0;

            for (int b = 0;b < m_Buildables.Count;b++)
            {
                for (int s = 0;s < m_Buildables[b].Sockets.Length;s++)
                {
                    if (m_Buildables[b].Sockets[s] == socket)
                        return index;

                    index++;
                }
            }

            return -1;
        }

        public Socket SocketAtIndex(int socketIndex)
        {
            int index = 0;

            for (int b = 0;b < m_Buildables.Count;b++)
            {
                for (int s = 0;s < m_Buildables[b].Sockets.Length;s++)
                {
                    if (index == socketIndex)
                        return m_Buildables[b].Sockets[s];

                    index++;
                }
            }

            return null;
        }

		public bool HasCollider(Collider col)
		{
			for(int i = 0;i < m_Buildables.Count;i ++)
				if(m_Buildables[i].HasCollider(col))
					return true;

			return false;
		}

        public void SetActivationState(BuildableActivationState state)
        {
            m_ActivationState = state;

            foreach(var buildable in m_Buildables)
                buildable.SetActivationState(state);

            if (state == BuildableActivationState.Placed)
                m_BuildAudio.Play2D();
        }

		public void AddPart(Buildable buildable, bool createNewState = true, bool raiseAddEvent = true)
		{
			if (!m_Buildables.Contains(buildable))
			{
				m_Buildables.Add(buildable);
         
                buildable.SetActivationState(BuildableActivationState.Preview);

                if (createNewState)
                {
                    BuildableState state = new BuildableState()
                    {
                        Name = buildable.PlaceableName,
                        Position = buildable.transform.position,
                        Rotation = buildable.transform.rotation,
                        Sockets = new SocketState[buildable.Sockets.Length],
                    };

                    m_BuildableStates.Add(state);

                    UpdateSocketStates();
                }

                if (raiseAddEvent)
                    OnPartAdded?.Invoke(buildable);
            }
		}

        private void UpdateSocketStates()
        {
            for(int i = 0;i < m_Buildables.Count;i++)
            {
                Socket[] sockets = m_Buildables[i].Sockets;
                SocketState[] states = m_BuildableStates[i].Sockets;

                for(int j = 0;j < sockets.Length;j++)
                {
                    if(states[j] == null)
                        states[j] = new SocketState();

                    states[j].OccupiedSpaces = sockets[j].OccupiedSpaces;
                }
            }
        }
	}
}
