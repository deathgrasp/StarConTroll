using UnityEngine;

namespace Assets.Scripts
{
	public class Command_ShootMissile : Command {

		private Vector3 target;

		public static Command_ShootMissile Create(Ship shipIn, Vector3 targetIn)
		{
			Command_ShootMissile command = ScriptableObject.CreateInstance("Command_ShootMissile") as Command_ShootMissile;
            command.Init(shipIn, targetIn);
			return command;
		}

		private void Init(Ship shipIn, Vector3 targetIn)
		{
			ship = shipIn;
			target = targetIn;
		}
            
		public override void Execute()
		{
			Missile missile = Instantiate(Resources.Load<Missile>("Missile"), ship.transform.position, ship.transform.rotation);
			missile.Destination = target;
		}
	}
}
