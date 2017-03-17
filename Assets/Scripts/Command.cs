using UnityEngine;

namespace Assets.Scripts
{
	public class Command {

		private Ability ability; // the ship this ability belongs to
		private CommandParams commandParams;
		

		public Command(Ability abilityIn, CommandParams commandParamsIn)
		{
			ability = abilityIn;
			commandParams = commandParamsIn;
		}


		public void Execute()
		{
			ability.Execute(commandParams);
		}


		public Ability GetAbility()
		{
			return ability;
		}
	}
}