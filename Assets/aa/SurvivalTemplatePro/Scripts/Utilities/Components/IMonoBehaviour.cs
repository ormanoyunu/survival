using UnityEngine;

namespace SurvivalTemplatePro
{
    public interface IMonoBehaviour
    {
        GameObject gameObject { get; }
        Transform transform { get; }

        T GetComponent<T>();
        T GetComponentInParent<T>();
        T GetComponentInChildren<T>();

        T[] GetComponents<T>();
        T[] GetComponentsInParent<T>(bool includeInactive = false);
        T[] GetComponentsInChildren<T>(bool includeInactive = false);
    }
}