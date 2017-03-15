using System;
using UnityEngine;

namespace Assets.Scripts
{
	public abstract class Ability : ScriptableObject {

		protected UnityEngine.Object icon; // ability panel icon
		protected UnityEngine.Object hotkey;
		protected Type command;

		public void Activate()
		{
			// TODO: this should create a new command object through he static "create" function and add it to the ship's queue
		}
	}
}
