using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public class ConsumableManager : MonoBehaviour
    {
        public struct RespawnData 
        {
            public Consumable Consumable;
            public float RespawnTime;
        }

        [SerializeField]
        bool m_RespawnConsumables;

        [SerializeField]
        int m_RespawnTimeMin = 45;

        [SerializeField]
        int m_RespawnTimeMax = 60;

        private List<RespawnData> m_RespawnDatas;


        private void Awake()
        {  
            var consumables = GetComponentsInChildren<Consumable>();

            m_RespawnDatas = new List<RespawnData>(consumables.Length);

            foreach (var consumable in consumables)
                consumable.Consumed += OnItemConsumed;
        }

        private void Update()
        {
            int i = 0;

            while (i < m_RespawnDatas.Count)
            {
                var respawnData = m_RespawnDatas[i];

                if (Time.time > respawnData.RespawnTime)
                {
                    respawnData.Consumable.gameObject.SetActive(true);
                    m_RespawnDatas.Remove(respawnData);
                }
                else
                    i++;
            }
        }

        private void OnItemConsumed(Consumable consumable)
        {
            var newRespawnData = new RespawnData();
            newRespawnData.Consumable = consumable;
            newRespawnData.RespawnTime = Time.time + Random.Range(m_RespawnTimeMin, m_RespawnTimeMax);

            m_RespawnDatas.Add(newRespawnData);
        }
    }
}
