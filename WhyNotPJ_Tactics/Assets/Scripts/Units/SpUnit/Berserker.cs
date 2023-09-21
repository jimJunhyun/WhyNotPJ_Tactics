using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berserker : UnitBasic
{
	int rageCount = 0;

	protected override void CheckAllCell()
	{
		prevFound = new List<KeyValuePair<int, UnitBasic>>(foundUnits);
		foundUnits.Clear();

		UnitBasic mover = null;
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].totalMod = ranges[i].atkModifier + atkModifier + rageCount;
			Vector3 dest = transform.position + (transform.right * 0.5f * ranges[i].xDistance) + (transform.up * 0.5f * ranges[i].yDistance);
			Collider2D c = Physics2D.OverlapBox(dest, Vector2.one * 0.2f, 0, ranges[i].TargetSide);
			if (c && c.transform != transform)
			{
				mover = c.GetComponent<UnitBasic>();
				if (mover != null)
				{
					foundUnits.Add(new KeyValuePair<int, UnitBasic>(i, mover));
				}
			}
		}
	}

	public override void Damage(AttackRange rng, float mult = 1)
	{
		rng.totalModMult = mult;
		attackedBy.Add(rng);

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			InflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}
		int sum = 0;
		for (int i = 0; i < attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(attackedBy[i].totalAtk) - defModifier, 0);
		}
		CurHp = hp + hpModifier - sum;
		rageCount = sum;
		Debug.Log($"HP : {hp} + {hpModifier} - {sum} = {CurHp}");
		for (int i = 0; i < foundUnits.Count; i++)
		{
			foundUnits[i].Value.UnDamage(ranges[foundUnits[i].Key]);
			rangeAttackingPair.Remove(ranges[foundUnits[i].Key]);
		}
		foundUnits.Clear();
		UpdateHandler.instance.RequestFUpdate();
		
	}

	public override void UnDamage(AttackRange rng)
	{
		rng.totalModMult = 1;
		attackedBy.Remove(attackedBy.Find(by => by == rng));

		for (int i = 0; i < rng.anomaly.Count; i++)
		{
			DisflictDistort(rng.owner, rng.anomaly[i], rng.anomalyAmount[i]);
		}

		int sum = 0;
		for (int i = 0; i < attackedBy.Count; i++)
		{
			sum += Mathf.Max((int)(attackedBy[i].totalAtk) - defModifier, 0);
		}
		CurHp = hp + hpModifier - sum;
		rageCount = sum;
		Debug.Log($"{name} HP : {hp} + {hpModifier} - {sum} = {CurHp}");
		for (int i = 0; i < foundUnits.Count; i++)
		{
			foundUnits[i].Value.UnDamage(ranges[foundUnits[i].Key]);
			rangeAttackingPair.Remove(ranges[foundUnits[i].Key]);
		}
		foundUnits.Clear();
		UpdateHandler.instance.RequestFUpdate();
	}
}
