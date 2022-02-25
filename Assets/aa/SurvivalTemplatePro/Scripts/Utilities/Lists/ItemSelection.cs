using System.Collections.Generic;
using UnityEngine;

namespace SurvivalTemplatePro
{
    public static class ItemSelection
	{
		public static T SelectRandom<T>(this T[] array)
		{
			int last = 0;

			return Select(array, ref last, SelectionType.Random);
		}

		public static T Select<T>(this T[] array, ref int last, SelectionType selectionMethod = SelectionType.Random)
		{
			if(array == null || array.Length == 0)
				return default;

			int next = 0;

			if(selectionMethod == SelectionType.Random)
				next = Random.Range(0, array.Length);
			else if(selectionMethod == SelectionType.RandomExcludeLast && array.Length > 1)
			{
				last = Mathf.Clamp(last, 0, array.Length - 1);

				T first = array[0];
				array[0] = array[last];
				array[last] = first;

				next = Random.Range(1, array.Length);
			}
			else if(selectionMethod == SelectionType.Sequence)
				next = (int)Mathf.Repeat(last + 1, array.Length);

			last = next;

			return array[next];
		}

		public static T SelectRandom<T>(this List<T> list)
		{
			int last = 0;

			return Select(list, ref last, SelectionType.Random);
		}

		public static T Select<T>(this List<T> list, ref int last, SelectionType selectionMethod = SelectionType.Random)
		{
			if(list == null || list.Count == 0)
				return default;

			int next = 0;

            switch (selectionMethod)
            {
                case SelectionType.Random: next = Random.Range(0, list.Count); break;
				case SelectionType.RandomExcludeLast:
				{
					if (list.Count > 1)
					{
						last = Mathf.Clamp(last, 0, list.Count - 1);

						T first = list[0];
						list[0] = list[last];
						list[last] = first;

						next = Random.Range(1, list.Count);
					}
				}
				break;
                case SelectionType.Sequence: next = (int)Mathf.Repeat(last + 1, list.Count); break;
			}

			last = next;

			return list[next];
        }
    }

    public enum SelectionType
    {
        /// <summary>The item will be selected randomly.</summary>
        Random,

        /// <summary>The item will be selected randomly, but will exclude the last selected.</summary>
        RandomExcludeLast,

        /// <summary>The items will be selected in sequence.</summary>
        Sequence
    }
}
