using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class SegmentButton : MonoBehaviour 
	{
		private int index;

		public void SetIndex(int indexIn)
		{
			index = indexIn;
		}


		public void onClick()
		{
			GameWorld.Instance.SetSelectedSegment(index);
		}
	}
}
