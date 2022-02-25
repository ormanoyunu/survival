using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
	public class BitMaskAttribute : PropertyAttribute
	{
		public Type EnumType;


		public BitMaskAttribute(Type enumType)
		{
			EnumType = enumType;
		}
	}
}