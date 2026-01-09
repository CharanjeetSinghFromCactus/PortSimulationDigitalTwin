namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public interface IRotationUpdatable 
	{
		void UpdateRotation(Quaternion rotation);
	}
}