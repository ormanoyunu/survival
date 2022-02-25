namespace SurvivalTemplatePro.BuildingSystem
{
    [System.Flags]
	public enum SocketType
	{
		Foundation = 1,
		Pillar = 2,
		Wall = 4,
		Floor = 8,
		Roof = 16,
		OuterStairs = 32,
		InnerStairs = 64,
		Door = 128,
		Window = 256
	}

	public enum BuildableActivationState { Disabled, Preview, Placed }
	public enum BuildableType { Free, SocketBased }

	public static class SocketTypeFlagExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		public static SocketType SetFlag(this SocketType thisFlag, SocketType flag)
		{
			thisFlag |= flag;
			return thisFlag;
		}

		/// <summary>
		/// 
		/// </summary>
		public static SocketType UnsetFlag(this SocketType thisFlag, SocketType flag)
		{
			thisFlag &= (~flag);
			return thisFlag;
		}

		/// <summary>
		/// 
		/// </summary>
		public static bool Has(this SocketType thisFlags, SocketType flag)
		{
			return (thisFlags & flag) == flag;
		}
	}
}
