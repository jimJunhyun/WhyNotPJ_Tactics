using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class HorseMaster : UnitBasic
{
	protected override void InfDisfDamage()
	{
		List<KeyValuePair<int, UnitBasic>> diff = foundUnits.Except(prevFound).ToList(); //»õ°Í
		List<KeyValuePair<int, UnitBasic>> diff2 = prevFound.Except(foundUnits).ToList(); //Çå°Í
		for (int i = 0; i < diff2.Count; i++)
		{
			if (diff2 != null)
			{
				rangeAttackingPair[ranges[diff2[i].Key]].UnDamage(ranges[diff2[i].Key]);
				rangeAttackingPair.Remove(ranges[diff2[i].Key]);
				diff2[i].Value.ResetSpeed();
			}
			//Debug.Log(diff2[i].Value.name + " missed");

		}
		for (int i = 0; i < diff.Count; i++)
		{
			rangeAttackingPair.Add(ranges[diff[i].Key], diff[i].Value);
			diff[i].Value.Damage(ranges[diff[i].Key]);
			diff[i].Value.ChangeSpeed(moveGap);
		}
	}
}
