using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [CreateAssetMenu(menuName = "Survival Template Pro/Misc/Scene Setup Info")]
    public class SceneSetupInfo : ScriptableObject
    {
        #region Internal
        [Serializable]
        public struct SceneSetupPrefab
        {
            public GameObject Prefab;
            public PrefabSetupType SetupType;
        }

        public enum PrefabSetupType
        {
            DontReplaceInstance,
            ReplaceInstance,
            Ignore
        }
        #endregion

        public SceneSetupPrefab[] SceneSetupPrefabs => m_SceneSetupPrefabs;
        public string[] BaseObjectNames => m_BaseObjects;
        public bool SetupBaseObjects => m_SetupBaseObjects;
        public bool AddSceneToBuildSettings => m_AddSceneToBuild;

        [SerializeField]
        private SceneSetupPrefab[] m_SceneSetupPrefabs;

        [SerializeField]
        private string[] m_BaseObjects;

        [SerializeField]
        private bool m_SetupBaseObjects;

        [SerializeField]
        private bool m_AddSceneToBuild;


        public void ResetToDefault() 
        {
            for (int i = 0; i < m_SceneSetupPrefabs.Length; i++)
                SceneSetupPrefabs[i].SetupType = PrefabSetupType.DontReplaceInstance;

            m_BaseObjects = null;

            m_SetupBaseObjects = true;
            m_AddSceneToBuild = true;
        }
    }
}
