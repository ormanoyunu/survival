using UnityEngine;

namespace SurvivalTemplatePro
{
    [System.Serializable]
	public class SceneField
	{
		public Object SceneAsset => m_SceneAsset;
		public string SceneName => m_SceneName;

		[SerializeField]
		private Object m_SceneAsset;

		[SerializeField]
		private string m_SceneName = "";


		// makes it work with the existing Unity methods (LoadLevel/LoadScene)
		public static implicit operator string(SceneField sceneField) => sceneField.SceneName;
	}
}