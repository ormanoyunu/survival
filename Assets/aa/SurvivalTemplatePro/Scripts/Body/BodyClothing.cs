﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro.BodySystem
{
    public class BodyClothing : MonoBehaviour
    {
        #region Internal
        [Serializable]
        public class ClothingItem
        {
            public GameObject GameObject { get; set; }

            public string ObjectName;

            [Space]

            public ItemReference Item;

            public ClothingType Type;

            public Texture2D OpacityMask;

            [Reorderable]
            public StringList BlendshapesToEnable; 
        }
        #endregion

        [SerializeField]
        private Transform m_BodyTransform;

        [Space]

        [SerializeField, Reorderable]
        private ClothingItemList m_Items;

        private SkinnedMeshRenderer m_BodyRenderer;
        private List<ClothingItem> m_ActiveClothes = new List<ClothingItem>();


        public void ShowClothing(int clothingID)
        {
            ClothingItem item = GetClothing(clothingID);
 
            if(item != null)
            {
                ClothingItem activeItemOfSameType = GetActiveClothingOfType(item.Type);

                if(activeItemOfSameType != null)
                {
                    activeItemOfSameType.GameObject.SetActive(false);
                    m_ActiveClothes.Remove(activeItemOfSameType);
                }

                m_ActiveClothes.Add(item);
                item.GameObject.SetActive(true);

                UpdateOpacityMasksInShader();
                UpdateBlendshapes();
            }
        }

        public void HideClothing(int clothingId)
        {
            ClothingItem activeItem = GetActiveClothing(clothingId);

            if(activeItem != null)
            {
                m_ActiveClothes.Remove(activeItem);
                activeItem.GameObject.SetActive(false);

                UpdateOpacityMasksInShader();
                UpdateBlendshapes();
            }
        }

        public void HideClothing(ClothingType clothingType)
        {
            ClothingItem activeItem = GetActiveClothingOfType(clothingType);

            if(activeItem != null)
            {
                m_ActiveClothes.Remove(activeItem);
                activeItem.GameObject.SetActive(false);

                UpdateOpacityMasksInShader();
                UpdateBlendshapes();
            }
        }

        public void HideAllClothing()
        {
            foreach(ClothingItem clothing in m_ActiveClothes)
                clothing.GameObject.SetActive(false);

            m_ActiveClothes.Clear();

            UpdateOpacityMasksInShader();
            UpdateBlendshapes();
        }

        private void Awake()
        {
            m_BodyRenderer = m_BodyTransform.GetComponent<SkinnedMeshRenderer>();

            if (m_BodyRenderer == null)
                Debug.LogError(string.Format("Body object with name '{0}' doesn't have a SkinnedMeshRenderer component.", m_BodyTransform.name));

            foreach(ClothingItem clothing in m_Items)
            {
                Transform clothingTransform = transform.FindDeepChild(clothing.ObjectName);

                if(clothingTransform != null)
                    clothing.GameObject = clothingTransform.gameObject;
            }
        }

        private ClothingItem GetClothing(int clothingId)
        {
            foreach(ClothingItem clothing in m_Items)
            {
                if(clothing.Item == clothingId)
                    return clothing;
            }

            return null;
        }

        private ClothingItem GetActiveClothing(int clothingId)
        {
            foreach(ClothingItem clothing in m_ActiveClothes)
            {
                if(clothing.Item == clothingId)
                    return clothing;
            }

            return null;
        }

        private ClothingItem GetActiveClothingOfType(ClothingType clothingType)
        {
            foreach(ClothingItem clothing in m_ActiveClothes)
            {
                if(clothing.Type == clothingType)
                    return clothing;
            }

            return null;
        }

        private void UpdateOpacityMasksInShader()
        {
            for(int i = 0;i < 3;i++)
            {
                string shaderProperty = "_OpacityMask" + i.ToString();
                ClothingItem item = null;

                if(i < m_ActiveClothes.Count)
                    item = m_ActiveClothes[i];

                m_BodyRenderer.material.SetTexture(shaderProperty, item == null ? null : item.OpacityMask);
            }

        }

        private void UpdateBlendshapes()
        {
            // Set all weights to 0
            foreach(var activeCloth in m_ActiveClothes)
            {
                var skinnedRenderer = activeCloth.GameObject.GetComponent<SkinnedMeshRenderer>();

                if(skinnedRenderer != null)
                {
                    for(int i = 0;i < skinnedRenderer.sharedMesh.blendShapeCount;i++)
                        skinnedRenderer.SetBlendShapeWeight(i, 0f);
                }
            }

            foreach(var shapeTrigger in m_ActiveClothes)
            {
                if(shapeTrigger.BlendshapesToEnable.Count > 0)
                {
                    foreach(var shapeReceiver in m_ActiveClothes)
                    {
                        var skinnedRenderer = shapeReceiver.GameObject.GetComponent<SkinnedMeshRenderer>();

                        if(skinnedRenderer != null)
                        {
                            foreach(var shapeName in shapeTrigger.BlendshapesToEnable)
                                SetBlendShapeWeight(skinnedRenderer, shapeName, 100f);
                        }
                    }
                }
            }
        }

        private void SetBlendShapeWeight(SkinnedMeshRenderer renderer, string shapeName, float weight)
        {
            int shapeIndex = renderer.sharedMesh.GetBlendShapeIndex(shapeName);

            if(shapeIndex >= 0 && shapeIndex < renderer.sharedMesh.blendShapeCount)
                renderer.SetBlendShapeWeight(shapeIndex, weight);
        }
    }
}