using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : UnitBasic
{
	float mult = 1;
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
			}
			//Debug.Log(diff2[i].Value.name + " missed");

		}

		for (int i = 0; i < diff.Count; i++)
		{
			Debug.Log(diff[i].Value.name + " got hit");
			rangeAttackingPair.Add(ranges[diff[i].Key], diff[i].Value);
			if (diff[i].Value.curDir == curDir)
			{
				mult = 3;
			}
			else
			{
				mult = 1;
			}
			diff[i].Value.Damage(ranges[diff[i].Key], mult);
		}
	}
}
