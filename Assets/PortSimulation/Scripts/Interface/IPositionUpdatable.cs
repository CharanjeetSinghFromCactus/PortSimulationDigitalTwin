namespace PortSimulation
{
	using UnityEngine;
	using System;
	using System.Collections;

	public interface IPositionUpdatable
	{
		void UpdatePosition(Vector3 position);
	}
}