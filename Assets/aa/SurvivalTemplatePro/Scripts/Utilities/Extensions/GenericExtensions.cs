using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public static class ObjectExtensions
	{
		public static string DoUnityLikeNameFormat(this string str)
		{
			if(string.IsNullOrEmpty(str))
				return string.Empty;

			if(str.Length > 2 && str[0] == 'm' && str[1] == '_')
				str = str.Remove(0, 2);

			if(str.Length > 1 && str[0] == '_')
				str = str.Remove(0);

			StringBuilder newText = new StringBuilder(str.Length * 2);
			newText.Append(str[0]);

			for(int i = 1; i < str.Length; i++)
			{
				bool lastIsUpper = char.IsUpper(str[i - 1]);
				bool lastIsSpace = str[i - 1] ==  ' ';
				bool lastIsDigit = char.IsDigit(str[i - 1]);

				if(char.IsUpper(str[i]) && !lastIsUpper && !lastIsSpace)
					newText.Append(' ');

				if(char.IsDigit(str[i]) && !lastIsDigit && !lastIsUpper && !lastIsSpace)
					newText.Append(' ');

				newText.Append(str[i]);
			}

			return newText.ToString();
		}

		/// <summary>
		/// 
		/// </summary>
		public static Transform FindDeepChild(this Transform parent, string childName) 
		{
			var result = parent.Find(childName);

			if (result) 
				return result;

			for (int i = 0;i < parent.childCount;i ++)
			{
				result = parent.GetChild(i).FindDeepChild(childName);
				if (result)
					return result;
			}

			return null;
		}

		public static Vector3 GetRandomPoint(this Bounds bounds)
		{
			return new Vector3(
				Random.Range(bounds.min.x, bounds.max.x),
				Random.Range(bounds.min.y, bounds.max.y),
				Random.Range(bounds.min.z, bounds.max.z));
		}

		public static Vector3 LocalToWorld(this Vector3 vector, Transform transform)
		{
			return transform.rotation * vector;
		}

		public static Vector3 LocalToWorld(this Transform transform, Vector3 localPosition)
		{
			return transform.right * localPosition.x + transform.up * localPosition.y + transform.forward * localPosition.z;
		}

		public static void SetLayerRecursively(this GameObject gameObject, int layer)
		{
			gameObject.layer = layer;

			foreach (Transform child in gameObject.transform)
				child.gameObject.SetLayerRecursively(layer);
		}

		/// <summary>
		/// Checks if the index is inside the list's bounds.
		/// </summary>
		public static bool IndexIsValid<T>(this List<T> list, int index)
		{
			return index >= 0 && index < list.Count;
		}

        public static List<T> CopyOther<T>(this List<T> list, List<T> toCopy)
        {
            if (toCopy == null || toCopy.Count == 0)
                return null;

            list = new List<T>();

            for (int i = 0; i < toCopy.Count; i++)
                list.Add(toCopy[i]);

            return list;
        }

		public static float Jitter(this float refFloat, float jitter)
        {
			return refFloat + Random.Range(-jitter, jitter);
		}

		public static Vector3 Jitter(this Vector3 refVector, float jitter)
		{
			return new Vector3(
				refVector.x + Random.Range(-jitter, jitter),
				refVector.y + Random.Range(-jitter, jitter),
				refVector.z + Random.Range(-jitter, jitter));
		}

		public static Vector3 Jitter(this Vector3 refVector, float xJit, float yJit, float zJit)
		{
			refVector.x -= Mathf.Abs(refVector.x * Random.Range(0, xJit)) * 2f;
			refVector.y -= Mathf.Abs(refVector.y * Random.Range(0, yJit)) * 2f;
			refVector.z -= Mathf.Abs(refVector.z * Random.Range(0, zJit)) * 2f;

			return refVector;
		}

		public static float GetRandomFloat(this Vector2 vector) 
		{
			return Random.Range(vector.x, vector.y);
		}

		public static int GetRandomFloat(this Vector2Int vector)
		{
			return Random.Range(vector.x, vector.y + 1);
		}

		public static void ReverseVector(this ref Vector2 vector)
		{
			float xValue = vector.x;
			vector.x = vector.y;
			vector.y = xValue;
		}
	}
}