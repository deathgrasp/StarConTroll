using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Ability_ShootMissile : Ability {

		// Use this for initialization
		void Awake()
		{
			// TODO: find a way to make these fields act like "static". no need to save more then 1 icon referance for each ability.
			icon = Resources.Load<Sprite>("Icon_ShootMissile");
			hotkey = null;
			tooltip = null;

			// TODO: make sure each ability does the following code after initialization to ensure no missing icons/hotkeys/whatever:
			/*if (icon == null) { Debug.Log(this.GetType() +": icon is missing"); }
			if (hotkey == null) { Debug.Log(this.GetType() +": hotkey is missing"); }
			if (tooltip == null) { Debug.Log(this.GetType() +": tooltip is missing"); }*/
		}


		public override void Execute(CommandParams cp)
		{
			Missile missile = Instantiate(Resources.Load<Missile>("Missile"), cp.GetActingShip().transform.position, cp.GetActingShip().transform.rotation);
			missile.Destination = cp.GetTargetPoint();
		}
	}
}
