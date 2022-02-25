using UnityEditor;
using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    [CustomEditor(typeof(Firearm))]
    public class FirearmEditor : WieldableEditor
    {
        private Firearm m_Firearm;

        private SerializedProperty m_Aimer;
        private SerializedProperty m_Ammo;
        private SerializedProperty m_Recoil;
        private SerializedProperty m_Reloader;
        private SerializedProperty m_Shooter;
        private SerializedProperty m_Trigger;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Application.isPlaying)
                return;

            if (!HasBaseModules())
            {
                STPEditorGUI.Separator();

                if (GUILayout.Button("Setup Firearm"))
                    SetupBaseModules();
            }

            if (m_Firearm != null)
                SetModulesActivation();
        }
         
        private bool HasBaseModules()
        {
            bool hasAllBaseModules = m_Aimer.objectReferenceValue;
            hasAllBaseModules &= m_Ammo.objectReferenceValue;
            hasAllBaseModules &= m_Recoil.objectReferenceValue;
            hasAllBaseModules &= m_Reloader.objectReferenceValue;
            hasAllBaseModules &= m_Shooter.objectReferenceValue;
            hasAllBaseModules &= m_Trigger.objectReferenceValue;

            return hasAllBaseModules;
        }

        private void SetupBaseModules() 
        {
            SetupModule<FirearmBasicAimer, FirearmAimerBehaviour>(m_Aimer);
            SetupModule<FirearmInventoryAmmo, FirearmAmmoBehaviour>(m_Ammo);
            SetupModule<FirearmBasicRecoil, FirearmRecoilBehaviour>(m_Recoil);
            SetupModule<FirearmBasicReloader, FirearmReloaderBehaviour>(m_Reloader);
            SetupModule<FirearmHitscanShooter, FirearmShooterBehaviour>(m_Shooter);
            SetupModule<FirearmAdvancedTrigger, FirearmTriggerBehaviour>(m_Trigger);

            serializedObject.ApplyModifiedProperties();
        }

        private void SetupModule<TNewType, TCurrentModule>(SerializedProperty property) where TNewType : FirearmAttachmentBehaviour where TCurrentModule : FirearmAttachmentBehaviour
        {
            if (property.objectReferenceValue != null)
                return;

            TCurrentModule currentModule = m_Firearm.gameObject.GetComponent<TCurrentModule>();

            if (currentModule == null)
                property.objectReferenceValue = m_Firearm.gameObject.AddComponent<TNewType>();
            else
                property.objectReferenceValue = currentModule;
        }

        private void SetModulesActivation() 
        {
            foreach (var module in m_Firearm.GetComponentsInChildren<FirearmAttachmentBehaviour>(true))
            {
                module.AttachOnStart = false;

                if (IsBaseModule(module))
                    module.AttachOnStart = true;
            }
        }

        private bool IsBaseModule(FirearmAttachmentBehaviour module)
        {
            bool isBaseModule;

            isBaseModule = module == m_Aimer.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Ammo.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Recoil.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Reloader.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Shooter.objectReferenceValue as FirearmAttachmentBehaviour;
            isBaseModule |= module == m_Trigger.objectReferenceValue as FirearmAttachmentBehaviour;

            return isBaseModule;
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            m_Firearm = m_Wieldable as Firearm;

            m_Aimer = serializedObject.FindProperty("m_BaseAimer");
            m_Ammo = serializedObject.FindProperty("m_BaseAmmo");
            m_Recoil = serializedObject.FindProperty("m_BaseRecoil");
            m_Reloader = serializedObject.FindProperty("m_BaseReloader");
            m_Shooter = serializedObject.FindProperty("m_BaseShooter");
            m_Trigger = serializedObject.FindProperty("m_BaseTrigger");
        }
    }
}