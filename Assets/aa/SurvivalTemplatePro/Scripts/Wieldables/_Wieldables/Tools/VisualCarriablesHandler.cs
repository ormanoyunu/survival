using UnityEngine;

namespace SurvivalTemplatePro.WieldableSystem
{
    public class VisualCarriablesHandler : MonoBehaviour, IVisualCarriablesHandler
    {
        public IWieldable AttachedWieldable { get; set; }

        [SerializeField]
        private GameObject[] m_VisualObjects;


        private void Awake()
        {
            for (int i = 0; i < m_VisualObjects.Length; i++)
                m_VisualObjects[i].SetActive(false);
        }

        private void OnValidate()
        {
            m_VisualObjects = new GameObject[transform.childCount];
            int i = 0;

            foreach (Transform trs in transform)
            {
                m_VisualObjects.SetValue(trs.gameObject, i);
                i++;
            }     
        }

        public void UpdateVisuals(int carriedCount)
        {
            for (int i = 0; i < m_VisualObjects.Length; i++)
            {
                bool showObject = carriedCount > i;
                m_VisualObjects[i].SetActive(showObject);
            }
        }
    }
}