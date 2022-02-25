using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace SurvivalTemplatePro.WorldManagement
{
    public class PostProcessingManager : Singleton<PostProcessingManager>
    {
        public static PostProcessVolume OverlayVolume => Instance.m_OverlayVolume;
        public static PostProcessVolume WorldVolume => Instance.m_WorldVolume;

        [SerializeField]
        private PostProcessVolume m_OverlayVolume;

        [SerializeField]
        private PostProcessVolume m_WorldVolume;
    }
}