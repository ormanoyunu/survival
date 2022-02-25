using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class MaterialChanger : MonoBehaviour
    {
		#region Internal
		protected struct RendererSetup
		{
			public MeshRenderer Renderer;
			public int Id;
		}

		protected struct MaterialSetup
		{
			public Material[] DefaultMaterials;
			public Material[] MaterialsWithEffects;
		}
		#endregion

		[SerializeField]
		private MaterialChangerInfo m_Info;

		private static Dictionary<int, MaterialSetup> m_Materials = new Dictionary<int, MaterialSetup>();
		private RendererSetup[] m_Renderers;


		public void SetDefaultMaterial() => SetMaterials(false);
        public void SetMaterialWithEffects() => SetMaterials(true);

		public void SetOverrideMaterial(Material material)
		{
            for (int i = 0; i < m_Renderers.Length; i++)
            {
				Material[] overrideMats = new Material[m_Renderers[i].Renderer.sharedMaterials.Length];
				for (int j = 0; j < overrideMats.Length; j++)
					overrideMats[j] = material;

				m_Renderers[i].Renderer.materials = overrideMats;
			}
		}

		private void Awake()
		{
			SetupMaterials(m_Info);
			SetMaterials(false);
		}

        private void SetMaterials(bool withEffects)
		{
			if (m_Renderers == null)
				return;

            for (int i = 0; i < m_Renderers.Length; i++)
            {
				if (m_Materials.TryGetValue(m_Renderers[i].Id, out MaterialSetup materialSetup))
					m_Renderers[i].Renderer.sharedMaterials = withEffects ? materialSetup.MaterialsWithEffects : materialSetup.DefaultMaterials;
			}
		}

		private void SetupMaterials(MaterialChangerInfo info)
		{
			MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(true);

			if (renderers.Length == 0)
				return;

			m_Renderers = new RendererSetup[renderers.Length];

			int rendererIndex = 0;

			foreach (MeshRenderer renderer in renderers)
			{
				int rendererId = CalculateRendererId(renderer);

				if (!m_Materials.ContainsKey(rendererId))
				{
					Material[] defaultMaterials = new Material[renderer.sharedMaterials.Length];
					Material[] materialsWithEffects = new Material[renderer.sharedMaterials.Length];

					int matIndex = 0;

					foreach (Material sharedMat in renderer.sharedMaterials)
					{
						defaultMaterials[matIndex] = sharedMat;

						if (info != null)
						{
							Material materialWithEffects = new Material(sharedMat);
							materialWithEffects.name = materialWithEffects.name + "_WithEffects";

							foreach (var property in info.ColorProperties)
								materialWithEffects.SetColor(property.PropertyName, property.PropertyValue);

							foreach (var property in info.FloatProperties)
								materialWithEffects.SetFloat(property.PropertyName, property.PropertyValue);

							foreach (var property in info.IntProperties)
								materialWithEffects.SetInt(property.PropertyName, property.PropertyValue);

							materialsWithEffects[matIndex] = materialWithEffects;
						}
						else
							materialsWithEffects[matIndex] = sharedMat;

						matIndex++;
					}

					m_Materials.Add(rendererId, new MaterialSetup() { DefaultMaterials = defaultMaterials, MaterialsWithEffects = materialsWithEffects });
				}

				m_Renderers[rendererIndex] = new RendererSetup() { Renderer = renderer, Id = rendererId };
				rendererIndex++;
			}
		}

		private int CalculateRendererId(Renderer renderer)
		{
			int id = 0;

			foreach (var material in renderer.sharedMaterials)
			{
				if (material == null)
					continue;

				id += material.GetHashCode() / 2;
			}

			return id;
		}
	}
}