using UnityEngine;

namespace SurvivalTemplatePro
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Misc/Material Changer Info")]

	public class MaterialChangerInfo : ScriptableObject
    {
		#region Internal
		[System.Serializable]
		public class ShaderProperty<T>
		{
			public string PropertyName;
			public T PropertyValue;
		}
		#endregion

		public ShaderProperty<Color>[] ColorProperties => m_ColorProperties;
		public ShaderProperty<float>[] FloatProperties => m_FloatProperties;
		public ShaderProperty<int>[] IntProperties => m_IntProperties;

		[SerializeField]
		private ShaderProperty<Color>[] m_ColorProperties;

		[SerializeField]
		private ShaderProperty<float>[] m_FloatProperties;

		[SerializeField]
		private ShaderProperty<int>[] m_IntProperties;
    }
}