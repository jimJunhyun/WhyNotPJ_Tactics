using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shielder : UnitBasic
{
	public override void Damage(AttackRange rng, float mult = 1)
	{
		bool blocked = false;
		switch (curDir)
		{
			case Direction.Up:
				if(rng.owner.transform.position.y > transform.position.y)
				{
					blocked = true;
				}
				break;
			case Direction.Down:
				if (rng.owner.transform.position.y < transform.position.y)
				{
					blocked = true;
				}
				break;
			case Direction.Left:
				if (rng.owner.transform.position.x < transform.position.x)
				{
					blocked = true;
				}
				break;
			case Direction.Right:
				if (rng.owner.transform.position.x > transform.position.x)
				{
					blocked = true;
				}
				break;
		}
		if (blocked)
		{
			Debug.Log("¸·¾Ò´Ù.");
			rng.IsNullified = true;
		}
		base.Damage(rng, mult);
	}
}
