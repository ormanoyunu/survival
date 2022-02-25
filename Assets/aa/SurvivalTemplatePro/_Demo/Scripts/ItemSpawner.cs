using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SurvivalTemplatePro.InventorySystem;

namespace SurvivalTemplatePro.Demo
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(AudioSource))]
    public class ItemSpawner : MonoBehaviour
    {
        #region Internal
#pragma warning disable 0649
        [Serializable]
        private class ItemToSpawn
        {
            public ItemGenerationMethod Method;

            public ItemReference SpecificItem;
            public ItemCategoryReference Category;
        }
#pragma warning restore 0649
        #endregion

        [BHeader("General")]

        [SerializeField]
        private ItemToSpawn[] m_ItemsToSpawn = null;

        [SerializeField]
        private Vector2Int m_ItemSpawnCount;

        [SerializeField, Range(0f, 10f)]
        private float m_SpawnDelay = 0.5f;

        [SerializeField, Range(0f, 10f)]
        private float m_ConsecutiveSpawnDelay = 0.1f;

        [SerializeField, Range(0f, 100f)]
        private float m_ItemDestroyDelay = 15f;

        [BHeader("Effects")]

        [SerializeField]
        private ParticleSystem m_ParticleEffects = null;

        [SerializeField]
        private Vector3 m_RandomRotation = Vector3.zero;

        [SerializeField, Range(0f, 100f)]
        private float m_PositionForce = 0f;

        [SerializeField, Range(0f, 100f)]
        private float m_AngularForce = 0f;

        [BHeader("Audio")]

        [SerializeField]
        private SoundPlayer m_StartSpawnAudio = null;

        [SerializeField]
        private SoundPlayer m_EndSpawnAudio = null;

        private BoxCollider m_Collider;
        private AudioSource m_AudioSource;

        private WaitForSeconds m_TimeBetweenSpawns;
        private WaitForSeconds m_ItemDestroyWait;


        public void SpawnItems()
        {
            float spawnCount = Mathf.Clamp(m_ItemSpawnCount.GetRandomFloat(), 0, m_ItemsToSpawn.Length);

            List<GameObject> itemsToSpawn = new List<GameObject>();

            for (int i = 0; i < spawnCount; i++)
            {
                var itemToSpawn = m_ItemsToSpawn.SelectRandom();
                ItemInfo itemInfo = null;

                if (itemToSpawn.Method == ItemGenerationMethod.Specific)
                {
                    itemInfo = ItemDatabase.GetItemById(itemToSpawn.SpecificItem);
                }
                else if (itemToSpawn.Method == ItemGenerationMethod.RandomFromCategory)
                {
                    itemInfo = ItemDatabase.GetRandomItemFromCategory(itemToSpawn.Category);
                }
                else if (itemToSpawn.Method == ItemGenerationMethod.Random)
                {
                    var category = ItemDatabase.GetRandomCategory();

                    if (category != null)
                        itemInfo = ItemDatabase.GetRandomItemFromCategory(category.Name);
                }

                if (itemInfo != null && itemInfo.Pickup != null)
                    itemsToSpawn.Add(itemInfo.Pickup.gameObject);
                else
                    spawnCount--;
            }

            StartCoroutine(C_SpawnItems(itemsToSpawn));
        }

        private void Start()
        {
            m_Collider = GetComponent<BoxCollider>();
            m_AudioSource = GetComponent<AudioSource>();

            m_TimeBetweenSpawns = new WaitForSeconds(m_ConsecutiveSpawnDelay);
            m_ItemDestroyWait = new WaitForSeconds(m_ItemDestroyDelay);
        }

        private IEnumerator C_SpawnItems(List<GameObject> itemsToSpawn)
        {
            yield return new WaitForSeconds(m_SpawnDelay);

            m_StartSpawnAudio.Play(m_AudioSource);

            for (int i = 0; i < itemsToSpawn.Count; i++)
            {
                Quaternion spawnRotation = Quaternion.Euler(
                    Random.Range(-Mathf.Abs(m_RandomRotation.x), Mathf.Abs(m_RandomRotation.x)),
                    Random.Range(-Mathf.Abs(m_RandomRotation.y), Mathf.Abs(m_RandomRotation.y)),
                    Random.Range(-Mathf.Abs(m_RandomRotation.z), Mathf.Abs(m_RandomRotation.z))
                );

                GameObject pickup = Instantiate(itemsToSpawn[i], m_Collider.bounds.GetRandomPoint(), spawnRotation);

                if (m_ParticleEffects != null)
                    Instantiate(m_ParticleEffects, pickup.transform.position, spawnRotation);

                if (m_PositionForce > 0.01f && pickup.TryGetComponent(out Rigidbody rigidB))
                {
                    rigidB.velocity = Random.insideUnitSphere.normalized * m_PositionForce;
                    rigidB.angularVelocity = spawnRotation.eulerAngles * m_AngularForce; ;
                }

                if (pickup != null && m_ItemDestroyDelay > 0.01f)
                    StartCoroutine(C_DelayedItemDestroy(pickup));

                yield return m_TimeBetweenSpawns;
            }

            m_EndSpawnAudio.Play(m_AudioSource);
        }

        private IEnumerator C_DelayedItemDestroy(GameObject pickup)
        {
            yield return m_ItemDestroyWait;
            Destroy(pickup);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            m_TimeBetweenSpawns = new WaitForSeconds(m_ConsecutiveSpawnDelay);
            m_ItemDestroyWait = new WaitForSeconds(m_ItemDestroyDelay);
        }
#endif
    }
}