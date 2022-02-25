using System;
using UnityEngine;

namespace SurvivalTemplatePro.BuildingSystem
{
    public class BuildRequirement
    {
        public int BuildingMaterialId { get; }
        public int RequiredAmount { get; set; }
        public int CurrentAmount { get; set; }


        public BuildRequirement(int buildingMaterialId, int requiredAmount, int currentAmount)
        {
            this.BuildingMaterialId = buildingMaterialId;
            this.RequiredAmount = requiredAmount;
            this.CurrentAmount = currentAmount;
        }
    }

    [Serializable]
    public class BuildRequirementInfo
    {
        public int BuildingMaterialId => m_BuildMaterial;
        public int RequiredAmount => m_RequiredAmount;

        [SerializeField]
        private BuildMaterialReference m_BuildMaterial;

        [SerializeField, Range(1, 100)]
        private int m_RequiredAmount = 1;
    }
}