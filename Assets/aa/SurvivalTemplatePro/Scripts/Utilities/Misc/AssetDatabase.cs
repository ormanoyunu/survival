using UnityEngine;

namespace SurvivalTemplatePro
{
    public abstract class AssetDatabase<T> : ScriptableObject where T : ScriptableObject
    {
        public static bool AssetExists => Instance != null;

        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    var allFiles = Resources.LoadAll<T>("");
                    if (allFiles != null && allFiles.Length > 0)
                        m_Instance = allFiles[0];
                }

                return m_Instance;
            }
        }

        private static T m_Instance;


        protected virtual void OnEnable()
		{
            if (Instance == null)
                return;

            #if UNITY_EDITOR
            // No need to refresh the IDs when not in the editor
            RefreshIDs();
            #endif

            GenerateDictionaries();
		}

#if UNITY_EDITOR
        protected virtual void OnValidate()
		{
            if (Instance == null)
                return;

            RefreshIDs();
            GenerateDictionaries();
		}
#endif

        protected virtual void GenerateDictionaries() { }
        protected virtual void RefreshIDs() { }	
	}
}