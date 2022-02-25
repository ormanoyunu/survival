using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
	/// <summary>
	/// TODO: Rework
	/// </summary>
    public class Buildable : Placeable
    {
		public StructureManager ParentStructure { get; set; }
		public BuildableActivationState ActivationState => m_ActivationState;

		public BuildableType BuildableType => m_BuildableType;
		public BuildRequirementInfo[] BuildRequirements => m_BuildRequirements;

		public SocketType NeededSpace => m_NeededSpace;
		public SocketType SpacesToOccupy => m_SpacesToOccupy;

		public bool RequiresSockets => m_RequiresSockets;
		public Socket[] Sockets => m_Sockets;

		public MaterialChanger MaterialChanger => m_MaterialChanger;
		
		[BHeader("General (Buildable)")]

		[SerializeField]
		private BuildRequirementInfo[] m_BuildRequirements;

		[Space]

		[SerializeField]
		private BuildableType m_BuildableType;

		[SerializeField, EnableIf("m_BuildableType", (int)BuildableType.SocketBased)]
		private bool m_RequiresSockets;

		[SerializeField, EnableIf("m_BuildableType", (int)BuildableType.SocketBased)]
		private SocketType m_NeededSpace;

		[SerializeField, EnableIf("m_BuildableType", (int)BuildableType.SocketBased)]
		private SocketType m_SpacesToOccupy;

		[Space]

		[SerializeField]
		private MaterialChanger m_MaterialChanger;

		[BHeader("Effects (Buildable)")]

		[SerializeField]
		protected SoundPlayer m_BuildAudio;

		[SerializeField]
		protected GameObject m_BuildFX;

		private Socket[] m_Sockets = new Socket[0];
		private BuildableActivationState m_ActivationState = BuildableActivationState.Preview;

		private bool m_Initialized;


		public void SetActivationState(BuildableActivationState state)
		{
			if (!m_Initialized)
				Awake();
			
			if (state == BuildableActivationState.Placed)
			{
				m_BuildAudio.Play2D();

				if (m_BuildFX != null)
					Instantiate(m_BuildFX, transform.position, Quaternion.identity, null);

				gameObject.layer = LayerMask.NameToLayer("Buildable");
			}
			else
				gameObject.layer = LayerMask.NameToLayer("BuildablePreview");
			
			bool enableSocketsAndColliders = state != BuildableActivationState.Disabled;

			foreach(var col in m_Colliders)
				col.enabled = enableSocketsAndColliders;

			foreach(var socket in m_Sockets)
				socket.gameObject.SetActive(enableSocketsAndColliders);

			if (m_MaterialChanger != null)
			{
				if(state == BuildableActivationState.Placed)
					m_MaterialChanger.SetDefaultMaterial();
				else
					m_MaterialChanger.SetOverrideMaterial(PlaceableDatabase.GetPlaceAllowedMaterial());
			}

			if (state == BuildableActivationState.Preview)
				Place();

			m_ActivationState = state;
		}

		protected override void Awake()
		{
			base.Awake();

			m_Sockets = GetComponentsInChildren<Socket>();

			m_Initialized = true;
		}

		// TODO: Re-enable destruction & stability
		//private void Update()
		//{
		//    if (m_CheckStability && State == BuildableState.Placed)
		//    {
		//        Collider[] cS;
		//        if (!HasSupport(out cS))
		//            On_SocketDeath();
		//    }
		//}

#if UNITY_EDITOR
		protected override void OnValidate()
		{
			if (m_MaterialChanger == null)
				m_MaterialChanger = GetComponentInChildren<MaterialChanger>();
		}
#endif
	}
}