using SurvivalTemplatePro.WorldManagement;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class SleepingBag : Interactable, ISleepingPlace
    {
        public Vector3 SleepPosition { get { return transform.position + transform.TransformVector(m_SleepPositionOffset); } }
        public Quaternion SleepRotation { get { return Quaternion.LookRotation(transform.up, transform.right) * Quaternion.Euler(m_SleepRotationOffset); }}

        [BHeader("Sleeping Bag")]

        [SerializeField]
        [InfoBox("Position offset for the player sleeping handler. (Where the camera will be positioned).")]
        private Vector3 m_SleepPositionOffset;

        [Space]

        [SerializeField]
        [InfoBox("Rotation offset for the player sleeping handler. (In which direction will the camera be pointed).")]
        private Vector3 m_SleepRotationOffset;


        public override void OnInteract(ICharacter character)
        {
            if (character.TryGetModule(out ISleepHandler sleepHandler))
            {
                base.OnInteract(character);

                sleepHandler.Sleep(this);
            }
        }

        private void Update()
        {
            if (HoverActive)
                DescriptionText = WorldManagerBase.Instance.GetGameTime().GetTimeToString(true, true, false);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var prevColor = Gizmos.color;
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(SleepPosition, 0.1f);

            Gizmos.color = prevColor;
        }
#endif
    }
}