using UnityEngine;

namespace SurvivalTemplatePro
{
    public enum DisableMethod
    {
        OnAwake, OnStart
    }

    public class ObjectDisabler : MonoBehaviour
    {
        [SerializeField]
        private DisableMethod m_DisableMethod;


        private void Awake()
        {
            if(m_DisableMethod == DisableMethod.OnAwake)
                gameObject.SetActive(false);
        }

        private void Start()
        {
            if(m_DisableMethod == DisableMethod.OnStart)
                gameObject.SetActive(false);
        }
    }
}