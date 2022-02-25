using Malee;
using SurvivalTemplatePro.BodySystem;
using SurvivalTemplatePro.InventorySystem;
using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
    [Serializable]
    public class StringList : ReorderableArray<string> { }

    [Serializable]
    public class AudioClipList : ReorderableArray<AudioClip> { }

    [Serializable]
    public class CraftRequirementList : ReorderableArray<CraftRequirement> { }

    [Serializable]
    public class ClothingItemList : ReorderableArray<BodyClothing.ClothingItem> { }

    [Serializable]
    public class ItemPropertyDefinitionList : ReorderableArray<ItemPropertyDefinition> { }

    [Serializable]
    public class ItemPropertyInfoList : ReorderableArray<ItemPropertyInfo> { }

    [Serializable]
    public class ContainerGeneratorList : ReorderableArray<ContainerGenerator> { }
}