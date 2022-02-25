using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro.BuildingSystem
{
    /// <summary>
    /// Handles everything regarded object carrying except the visuals.
    /// </summary>
    public class ObjectCarryController : ObjectDropHandler, IObjectCarryController
    {
        public int CarriedObjectsCount
        {
            get => m_CarriedObjectsCount;
            set
            {
                int clampedValue = Mathf.Clamp(value, 0, 24);

                if (value != m_CarriedObjectsCount && clampedValue != m_CarriedObjectsCount)
                {
                    m_CarriedObjectsCount = clampedValue;
                    onCarriedCountChanged?.Invoke(m_CarriedObjectsCount);
                }
            }
        }

        public CarriableDefinition CarriedObject => m_CarriedObject;

        public event UnityAction onObjectCarryStart;
        public event UnityAction onObjectCarryEnd;

        public event UnityAction<int> onCarriedCountChanged;

        [Space, SerializeField]
        private UnityEvent m_OnCarryStart;

        [SerializeField]
        private UnityEvent m_OnCarryEnd;

        private int m_CarriedObjectsCount = 0;
        private CarriableDefinition m_CarriedObject;

        private IStructureDetector m_StructureDetector;


        public override void OnInitialized()
        {
            m_StructureDetector = GetModule<IStructureDetector>();
        }

        public bool TryCarryObject(CarriableDefinition definition)
        {
            bool canCarry = false;

            if (definition != null)
            {
                if (m_CarriedObjectsCount == 0)
                {
                    m_CarriedObject = definition;

                    onObjectCarryStart?.Invoke();
                    m_OnCarryStart?.Invoke();

                    CarriedObjectsCount = 1;

                    canCarry = true;
                }
                else if (m_CarriedObject == definition && m_CarriedObjectsCount < definition.MaxCarryCount)
                {
                    CarriedObjectsCount++;
                    canCarry = true;
                }

                if (canCarry)
                    definition.CarrySound.PlayAtPosition(transform.position);
            }

            return canCarry;
        }

        public void UseCarriedObject()
        {
            if (m_CarriedObjectsCount <= 0)
                return;

            var structureInView = m_StructureDetector.StructureInView;
            var buildingMaterial = GetBuildingMaterialInfo(m_CarriedObject);

            if (structureInView != null && structureInView.TryAddBuildMaterial(buildingMaterial))
                RemoveCarriable(1);

            TryEndObjectCarrying();
        }

        public void DropCarriedObjects(int amount)
        {
            if (m_CarriedObjectsCount <= 0 || m_CarriedObject == null)
                return;

            float dropHeightMod = Character.Mover.ActiveMotions.Has(CharMotionMask.Crouch) ? 0.5f : 1f;

            for (int i = 0; i < amount; i++)
                DropObject(m_CarriedObject.DropSettings, m_CarriedObject.TargetCarriable.gameObject, dropHeightMod);

            RemoveCarriable(amount);
        }

        private void RemoveCarriable(int amount) 
        {
            if (m_CarriedObjectsCount <= 0)
                return;

            CarriedObjectsCount -= amount;
            TryEndObjectCarrying();
        }

        private void TryEndObjectCarrying()
        {
            if (m_CarriedObjectsCount == 0)
            {
                onObjectCarryEnd?.Invoke();
                m_OnCarryEnd?.Invoke();
            }
        }

        private BuildingMaterialInfo GetBuildingMaterialInfo(CarriableDefinition definition)
        {
            return BuildMaterialsDatabase.GetBuildingMaterialById(definition.BuildMaterial);
        }
    }
}