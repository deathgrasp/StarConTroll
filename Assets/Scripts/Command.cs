using UnityEngine;

namespace Assets.Scripts
{
	public abstract class Command : ScriptableObject {

		protected Ship ship; // the ship this ability belongs to

		public abstract void Execute();
	}
}
