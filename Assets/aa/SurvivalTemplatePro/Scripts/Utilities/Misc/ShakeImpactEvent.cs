using UnityEngine;
using UnityEngine.Events;

namespace SurvivalTemplatePro
{
    public class ShakeImpactEvent
	{
		public Vector3 Position;
		public float Radius;
		public float Scale;

		public static event UnityAction<ShakeImpactEvent> onShakeEvent;


		public ShakeImpactEvent(Vector3 position, float radius, float scale)
		{
			this.Position = position;
			this.Radius = radius;
			this.Scale = scale;
		}

		public static void RaiseEvent(ShakeImpactEvent shake) => onShakeEvent?.Invoke(shake);
	}
}