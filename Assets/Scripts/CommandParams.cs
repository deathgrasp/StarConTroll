using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class CommandParams {

		private Ship actingShip; // the ship this ability belongs to
		private Ship targetShip; // target ship of command (if any)
		private Vector3 targetPoint; // target point of command (if any)

		public CommandParams(Ship actingShipIn, Vector3 pointIn, Ship targetShipIn = null)
		{
			actingShip = actingShipIn;
			targetPoint = pointIn;
			targetShip = targetShipIn;
		}

		public void SetActingShip(Ship ship) { actingShip = ship; }
		public Ship GetActingShip() { return actingShip; }

		public void SetTargetShip(Ship ship) { targetShip = ship; }
		public Ship GetTargetShip() { return targetShip; }

		public void SetTargetPoint(Vector3 point) { targetPoint = point; }
		public Vector3 GetTargetPoint() { return targetPoint; }

	}
}
