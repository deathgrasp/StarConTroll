using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
	public class Ability_ShootMissile : Ability {

		// Use this for initialization
		void Start () 
		{
			icon = null;
			hotkey = null;
			command = typeof(Command_ShootMissile);
		}
	}
}
