using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.Surfaces
{
	[Serializable, CreateAssetMenu(menuName = "Survival Template Pro/Surfaces/Surface Info")]
	public class SurfaceInfo : ScriptableObject
	{
		#region Internal
		[Serializable]
		public class EffectPair
		{
			public SoundPlayer AudioEffect;
			public GameObject VisualEffect;
		}
		#endregion

		public Texture[] RegisteredTextures;

		[Space]

		[Group]
		public EffectPair SoftFootstepEffect;

		[Group]
		public EffectPair HardFootstepEffect;

		[Group]
		public EffectPair FallImpactEffect;

		[Space]

		[Group]
		public EffectPair BulletHitEffect;

		[Group]
		public EffectPair SlashEffect;

		[Group]
		public EffectPair StabEffect;

		private HashSet<Texture> m_CachedTextures = new HashSet<Texture>();


		public void CacheTextures()
		{
			m_CachedTextures = new HashSet<Texture>();

			foreach (Texture tex in RegisteredTextures)
				m_CachedTextures.Add(tex);
		}

		public bool HasTexture(Texture texture) => m_CachedTextures.Contains(texture);
	}
}