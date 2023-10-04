using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Philosopher : UnitBasic
{
	protected override void InfDisfDamage()
	{
		List<KeyValuePair<int, UnitBasic>> diff = foundUnits.Except(prevFound).ToList(); //»õ°Í
		List<KeyValuePair<int, UnitBasic>> diff2 = prevFound.Except(foundUnits).ToList(); //Çå°Í
		for (int i = 0; i < diff2.Count; i++)
		{
			if (diff2 != null)
			{
				Debug.Log(diff2[i].Value.name + " MISSED");
				rangeAttackingPair[ranges[diff2[i].Key]].UnDamage(ranges[diff2[i].Key]);
				if(diff2[i].Value.gameObject.layer != 10)
				{
					diff2[i].Value.RecallPrevSide();
				}
				rangeAttackingPair.Remove(ranges[diff2[i].Key]);
			}
			//Debug.Log(diff2[i].Value.name + " missed");

		}
		for (int i = 0; i < diff.Count; i++)
		{
			Debug.Log(diff[i].Value.name + " got hit");
			rangeAttackingPair.Add(ranges[diff[i].Key], diff[i].Value);
			if(diff[i].Value.gameObject.layer == 8)
			{
				diff[i].Value.ChangeSide(9);
			}
			else if(diff[i].Value.gameObject.layer == 9)
			{
				diff[i].Value.ChangeSide(8);
			}
			diff[i].Value.Damage(ranges[diff[i].Key]);
		}
		
	}
}
