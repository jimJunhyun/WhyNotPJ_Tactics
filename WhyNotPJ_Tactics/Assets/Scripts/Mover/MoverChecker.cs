using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class AttackRange
{
	public int xDistance;
	public int yDistance;
	public int atk;
	public int atkModifier;

	[HideInInspector]
	public int totalMod;

	public int totalAtk { get => atk + totalMod; }
	public List<AnomalyIndex> anomaly;
	public List<int> anomalyAmount;
	public bool indiscriminate;
	public bool discriminating;

	[HideInInspector]
	public MoverChecker owner;
	public Vector2 myPos => (Vector2)owner.transform.position + new Vector2(xDistance, yDistance);
}


public class MoverChecker : MonoBehaviour
{


    public List<AttackRange> ranges;
	Dictionary<AttackRange, UnitMover> rangeAttackingPair = new Dictionary<AttackRange, UnitMover>();

	UnitMover myBase;

	private void Awake()
	{
		myBase = GetComponent<UnitMover>();
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].owner = this;
		}
	}
	//부활 이후의 위치 변경이 적용되기 전에 타격이 들어가서 죽어버림.
	//그래서 무적 시간을 적용함.
	//부활 연출 등에도 사용할 수 있을 듯.

	private void Update()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			if (CheckCell(ranges[i], out UnitMover foundUnit))
			{
				if (!rangeAttackingPair.ContainsKey(ranges[i]))
				{
					if (ranges[i].indiscriminate || (ranges[i].discriminating == foundUnit.PSide))
					{
						rangeAttackingPair.Add(ranges[i], foundUnit);
						foundUnit.Damage(ranges[i]);
					}
				}
			}
			else
			{
				if (rangeAttackingPair.ContainsKey(ranges[i]))
				{
					if (ranges[i].indiscriminate || (ranges[i].discriminating == rangeAttackingPair[ranges[i]].PSide))
					{
						rangeAttackingPair[ranges[i]].UnDamage(ranges[i]);
						rangeAttackingPair.Remove(ranges[i]);
					}
				}
			}
			ranges[i].totalMod = ranges[i].atkModifier + myBase.atkModifier;
		}
		//Debug.Log(myBase.name + " : " + myBase.atkModifier.val);
	}

	bool CheckCell(AttackRange rng, out UnitMover mover)
	{
		mover = null;
		Vector3 dest = transform.position + (transform.right * rng.xDistance) + (transform.up * rng.yDistance);
		Collider2D c = Physics2D.OverlapBox(dest, Vector2.one * 0.8f, 0);
		if(c != null)
		{
			mover = c.GetComponent<UnitMover>();
			if (mover != null)
			{

				return true;
			}
		}
		
		return false;
	}

	public void DestActs()
	{
		//Debug.Log("Dest Ref " + name);
		foreach (var item in rangeAttackingPair.Keys)
		{
			rangeAttackingPair[item].UnDamage(item);
		}
		rangeAttackingPair.Clear();
	}

	public void OnDrawGizmos()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			Vector3 dest = transform.position + (transform.right * ranges[i].xDistance) + (transform.up * ranges[i].yDistance);
			Gizmos.DrawWireCube(dest, Vector3.one * 0.8f);
		}
	}

}
