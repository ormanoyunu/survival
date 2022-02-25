using UnityEngine;

namespace SurvivalTemplatePro
{
	public interface ISleepingPlace
	{
		GameObject gameObject { get; }
		Vector3 SleepPosition { get; }
		Quaternion SleepRotation { get; }
	}
}