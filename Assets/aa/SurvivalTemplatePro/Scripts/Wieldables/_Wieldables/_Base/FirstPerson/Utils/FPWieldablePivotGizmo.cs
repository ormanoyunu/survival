using UnityEngine;

namespace SurvivalTemplatePro
{
	/// <summary>
	/// Displays an object's pivot as a solid sphere.
	/// </summary>
	public class FPWieldablePivotGizmo : MonoBehaviour
	{
#if UNITY_EDITOR
		[SerializeField] 
		private Color m_Color = Color.red;

		[SerializeField] 
		private float m_Radius = 0.04f;
#endif

		private void Start()
		{
			if (!Application.isEditor)
				Destroy(this);
		}

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
		{
			Gizmos.color = m_Color;
			Gizmos.DrawSphere(transform.position, m_Radius);
		}
		#endif
    }
}
