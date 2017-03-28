using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class AbilityButton : MonoBehaviour 
	{
		private Ability ability;

		public void SetAbility(Ability abilityIn)
		{
			ability = abilityIn;
			gameObject.GetComponent<Image>().sprite = ability.GetIcon();
		}


		public void onClick()
		{
			GameWorld.Instance.SetSelectedAbility(ability);
		}
	}
}
