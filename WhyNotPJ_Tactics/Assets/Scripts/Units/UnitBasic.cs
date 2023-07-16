using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Side
{
	Friendly = 1,
	Hostile = 2,
	Neutral = 4,

	
}

[Serializable]
public class AttackRange
{
	public int xDistance;
	public int yDistance;
	public int atk;
	public int atkModifier;

	[HideInInspector]
	public int totalMod;
	[HideInInspector]
	public float totalModMult = 1;

	public int totalAtk { get => (int)((atk + totalMod) * totalModMult); }
	public List<AnomalyIndex> anomaly;
	public List<int> anomalyAmount;
	[SerializeField]
	int targetSide;
	public int TargetSide
	{
		get => targetSide << 8;
	}

	[HideInInspector]
	public UnitBasic owner;
	public Vector2 myPos => (Vector2)owner.transform.position + new Vector2(xDistance, yDistance);
}


public class UnitBasic : MonoBehaviour
{
    public List<AttackRange> ranges;
	Dictionary<AttackRange, UnitDetails> rangeAttackingPair = new Dictionary<AttackRange, UnitDetails>();

	UnitDetails myBase;

	private void Awake()
	{
		myBase = GetComponent<UnitDetails>();
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].owner = this;
		}
	}

	private void Start()
	{
		UpdateHandler.instance.fieldUpdateAct += OnUpdateAct;
	}

	//부활 이후의 위치 변경이 적용되기 전에 타격이 들어가서 죽어버림.
	//그래서 무적 시간을 적용함.
	//부활 연출 등에도 사용할 수 있을 듯.

	private void OnUpdateAct()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].totalMod = ranges[i].atkModifier + myBase.atkModifier;
			if (CheckCell(ranges[i], out UnitDetails foundUnit))
			{
				if (!rangeAttackingPair.ContainsKey(ranges[i]))
				{
					rangeAttackingPair.Add(ranges[i], foundUnit);
					foundUnit.Damage(ranges[i]);
				}
			}
			else
			{
				if (rangeAttackingPair.ContainsKey(ranges[i]))
				{
					rangeAttackingPair[ranges[i]].UnDamage(ranges[i]);
					rangeAttackingPair.Remove(ranges[i]);
					
				}
			}
			
		}
		//Debug.Log(myBase.name + " : " + myBase.atkModifier.val);
	}

	bool CheckCell(AttackRange rng, out UnitDetails mover)
	{
		mover = null;
		Vector3 dest = transform.position + (transform.right * rng.xDistance) + (transform.up * rng.yDistance);
		Collider2D c = Physics2D.OverlapBox(dest, Vector2.one * 0.8f, 0, rng.TargetSide);
		if(c != null)
		{
			mover = c.GetComponent<UnitDetails>();
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

		UpdateHandler.instance.fieldUpdateAct -= OnUpdateAct;
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
