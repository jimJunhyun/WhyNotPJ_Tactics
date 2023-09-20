using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assassin : UnitBasic
{
	bool thrice = false;
    public override void Damage(AttackRange rng, float mult = 1)
	{
		if (rng.owner.curDir == curDir)
		{
			mult = 3;
		}
		base.Damage(rng, mult);
	}
}
