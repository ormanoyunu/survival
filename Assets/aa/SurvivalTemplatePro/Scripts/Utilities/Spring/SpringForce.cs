﻿using System;
using UnityEngine;

namespace SurvivalTemplatePro
{
	[Serializable]
	public struct SpringForce
	{
		public SpringForce Default { get { return new SpringForce() { Force = Vector3.zero, Distribution = 1 }; } }

		public Vector3 Force;

		[Range(1, 20)]
		public int Distribution;


		public SpringForce(Vector3 force, int distribution) 
		{
			Force = force;
			Distribution = distribution;
		}

		public static SpringForce operator *(SpringForce springForce, float mod) 
		{
			springForce.Force *= mod;
			return springForce;
		}
	}
}