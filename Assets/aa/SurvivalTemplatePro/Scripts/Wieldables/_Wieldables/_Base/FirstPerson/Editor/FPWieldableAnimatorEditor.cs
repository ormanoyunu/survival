using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CustomEditor(typeof(FPWieldableAnimator), true)]
    public class FPWieldableAnimatorEditor : Editor
    {
        private FPWieldableAnimator m_FPAnimator;

        private const int k_OverlayLayer = 11;
        private FPWieldableAnimatorPreset[] m_Presets;
        private string[] m_PresetNames;

        private int m_SelectedPresetIndex;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
                GUI.enabled = false;

            EditorGUILayout.Space();
            STPEditorGUI.Separator();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            m_SelectedPresetIndex = EditorGUILayout.Popup("Preset: ", m_SelectedPresetIndex, m_PresetNames);

            if (GUILayout.Button("Load Preset"))
                LoadStatesPreset(m_Presets[m_SelectedPresetIndex]);

            GUILayout.EndVertical();

            if (CanShowSetupAnimatorButton())
                DrawSetupGUI();

            if (Application.isPlaying)
                GUI.enabled = true;

            if (m_FPAnimator != null && !Application.isPlaying)
                SetAnimatorController();
        }

        private void OnEnable()
        {
            m_FPAnimator = target as FPWieldableAnimator;

            m_Presets = Resources.LoadAll<FPWieldableAnimatorPreset>("");
            m_PresetNames = new string[m_Presets.Length];

            for (int i = 0; i < m_PresetNames.Length; i++)
                m_PresetNames[i] = m_Presets[i].name;
        }

        private void DrawSetupGUI()
        {
            EditorGUILayout.Space();
            STPEditorGUI.Separator();

            GUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.HelpBox("Animator is not properly set up", MessageType.Warning);

            if (GUILayout.Button("Fix First Person Settings"))
            {
                FixModelAndAnimator();

                EditorUtility.SetDirty(m_FPAnimator.gameObject);
                PrefabUtility.RecordPrefabInstancePropertyModifications(m_FPAnimator.gameObject);
            }

            GUILayout.EndVertical();

            STPEditorGUI.Separator();
        }

        private bool CanShowSetupAnimatorButton() 
        {
            if (m_FPAnimator == null)
                return false;

            bool canShow = m_FPAnimator.GetComponentInChildren<SkinnedMeshRenderer>(true).updateWhenOffscreen == false;
            canShow |= m_FPAnimator.GetComponentInChildren<Animator>().cullingMode != AnimatorCullingMode.AlwaysAnimate;
            canShow |= m_FPAnimator.gameObject.layer != k_OverlayLayer;

            foreach (var renderer in m_FPAnimator.GetComponentsInChildren<MeshRenderer>())
                canShow |= renderer.gameObject.layer != k_OverlayLayer;

            return canShow;
        }

        private void FixModelAndAnimator()
        {
            var animator = m_FPAnimator.GetComponentInChildren<Animator>(true);
            var skinnedRenderers = m_FPAnimator.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            var renderers = m_FPAnimator.GetComponentsInChildren<MeshRenderer>(true);

            if (animator != null)
            {
                m_FPAnimator.gameObject.SetLayerRecursively(k_OverlayLayer);

                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.updateMode = AnimatorUpdateMode.Normal;
                animator.applyRootMotion = false;
            }

            if (skinnedRenderers != null)
            {
                foreach (var skinRenderer in skinnedRenderers)
                {
                    skinRenderer.updateWhenOffscreen = true;
                    skinRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    skinRenderer.skinnedMotionVectors = false;
                    skinRenderer.allowOcclusionWhenDynamic = false;
                }
            }

            if (renderers != null)
            {
                foreach (var renderer in renderers)
                {
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.allowOcclusionWhenDynamic = false;
                }
            }
        }

        private void LoadStatesPreset(FPWieldableAnimatorPreset preset)
        {
            Undo.RecordObject(m_FPAnimator, "animator");
            m_FPAnimator.AnimationClips.Controller = preset.AnimatorController;
            m_FPAnimator.SetCustomStates(preset.StatePresets);
        }

        private void SetAnimatorController() 
        {
            var controller = serializedObject.FindProperty("m_Clips").GetValue<AnimationOverrideClips>().Controller;

            var animator = m_FPAnimator.GetComponentInChildren<Animator>();

            if (animator != null)
                animator.runtimeAnimatorController = controller;
        }
    }
}