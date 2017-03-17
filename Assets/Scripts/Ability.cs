using System;
using UnityEngine;

namespace Assets.Scripts
{
	public abstract class Ability : ScriptableObject {

		protected Sprite icon = null; // ability panel icon
		protected UnityEngine.Object hotkey = null;
		protected UnityEngine.Object tooltip = null;

		
		public abstract void Execute(CommandParams commandParams);


		public Sprite GetIcon()
        {
            return icon;
        }
	}
}
