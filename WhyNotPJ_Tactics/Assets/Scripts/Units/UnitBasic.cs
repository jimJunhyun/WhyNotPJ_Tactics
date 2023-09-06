using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum Side
{
	Friendly= 1,
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
	public void AppendAno(AnomalyIndex info, int amt)
	{
		if (anomaly.Contains(info))
		{
			int idx = anomaly.FindIndex(x => x == info);
			anomalyAmount[idx] += amt;
		}
		else
		{
			anomaly.Add(info);
			anomalyAmount.Add(amt);
		}
	}
	public bool RemoveAno(AnomalyIndex info)
	{
		if (anomaly.Contains(info))
		{
			int idx = anomaly.FindIndex(x => x == info);
			anomaly.RemoveAt(idx);
			anomalyAmount.RemoveAt(idx);
			return true;
		}
		return false;

	}
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

	public float moveGap;
	public float moveDist = 1;

	[SerializeField]
	float rayDist = 0.5f;

	public int hp;
	public int hpModifier
	{
		get; set;
	}
	public int atkModifier
	{
		get; set;
	}
	public int defModifier
	{
		get; set;
	}
	public int CurHp
	{
		get;
		set;
	}
	bool pSide = true;
	public bool PSide
	{
		get => pSide;
		set => pSide = value;
	}

	public bool controlable = false;

	public List<AttackRange> attackedBy = new List<AttackRange>();

	public List<InflictedAnomaly> curStatus = new List<InflictedAnomaly>();

	bool moved = false;


	[HideInInspector]
	public bool immunity = false;
	bool movable = true;

	float prevMove;

	public List<AttackRange> ranges;
	Dictionary<AttackRange, UnitBasic> rangeAttackingPair = new Dictionary<AttackRange, UnitBasic>();


	public bool isMoving = false;
	[SerializeField]
	public List<Vector3Int> pathes;

	private void Awake()
	{
		prevMove = 0;
		CurHp = hp;
		for (int i = 0; i < ranges.Count; i++)
		{
			ranges[i].owner = this;
		}
	}

	private void Start()
	{
		UpdateHandler.instance.allUnits.Add(this);

		UpdateHandler.instance.AddFieldUpdateActs(OnUpdateAct);
	}

	void Update()
	{
		Move();

		//if (controlable) //Temp
		//{
		//	if (Time.time - prevMove >= moveGap)
		//	{
		//		movable = true;
		//	}
		//	if (movable)
		//	{
		//		RaycastHit2D hit;
				
		//		if (Input.GetAxisRaw("Horizontal") > 0)
		//		{
		//			if (!Physics2D.Raycast(transform.position + Vector3.right * (transform.localScale.x / 1.8f), Vector3.right, rayDist))
		//			{
		//				moved = true;
		//				transform.eulerAngles = new Vector3(0, 0, 270);

		//				transform.Translate(new Vector3(0, moveDist), Space.Self);
		//				movable = false;
		//				prevMove = Time.time;
		//			}

		//		}
		//		else if (Input.GetAxisRaw("Horizontal") < 0)
		//		{
		//			if (!(hit = Physics2D.Raycast(transform.position - Vector3.right * (transform.localScale.x / 1.8f), Vector3.left, rayDist)))
		//			{

		//				moved = true;
		//				transform.eulerAngles = new Vector3(0, 0, 90);

		//				transform.Translate(new Vector3(0, moveDist), Space.Self);
		//				movable = false;
		//				prevMove = Time.time;
		//			}
		//		}
		//		else if (Input.GetAxisRaw("Vertical") > 0)
		//		{
		//			if (!(hit = Physics2D.Raycast(transform.position + Vector3.up * (transform.localScale.y / 1.8f), Vector3.up, rayDist)))
		//			{
		//				moved = true;
		//				transform.eulerAngles = new Vector3(0, 0, 0);
		//				transform.Translate(new Vector3(0, moveDist), Space.Self);
		//				movable = false;
		//				prevMove = Time.time;
		//			}
		//		}
		//		else if (Input.GetAxisRaw("Vertical") < 0)
		//		{

		//			if (!(hit = Physics2D.Raycast(transform.position - Vector3.up * (transform.localScale.y / 1.8f), Vector3.down, rayDist)))
		//			{

		//				moved = true;
		//				transform.eulerAngles = new Vector3(0, 0, 180);
		//				transform.Translate(new Vector3(0, moveDist), Space.Self);
		//				movable = false;
		//				prevMove = Time.time;
		//			}
		//		}

				
		//	}
		//}
		if (moved)
		{
			moved = false;
			Debug.Log("MOVED");
			UpdateHandler.instance.RequestFUpdate();
		}
	}

	#region Move
	public void SetPath(List<Vector3Int> path)
	{
		pathes = new(path);
	}

	private void Move()
	{
		if (pathes.Count > 0)
		{
			isMoving = true;
			if (Time.time - prevMove >= moveGap)
			{
				movable = true;
			}

			if (movable)
			{
				Vector3 pos = pathes[0];
				Vector3 dir = pos - transform.position;
				float angle = Vector2.SignedAngle(Vector2.up, dir);
				pathes.RemoveAt(0);

				transform.position = pos;
				transform.rotation = Quaternion.Euler(0, 0, angle);

				movable = false;
				prevMove = Time.time;
				moved = true;
			}
		}
		else if (pathes.Count == 0 && isMoving)
		{
			isMoving = false;
		}
	}
	#endregion

	#region Fundamental

	private void OnUpdateAct()
	{
		
		for (int i = 0; i < ranges.Count; i++)
		{
			Debug.Log(transform.name + " Range No." + i);
			ranges[i].totalMod = ranges[i].atkModifier + atkModifier;
			if (CheckCell(ranges[i], out UnitBasic foundUnit))
			{
				
				if (!rangeAttackingPair.ContainsKey(ranges[i]))
				{
					Debug.Log(foundUnit.name + " got hit");
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

	bool CheckCell(AttackRange rng, out UnitBasic mover)
	{
		mover = null;
		Vector3 dest = transform.position + (transform.right * 0.5f * rng.xDistance) + (transform.up * 0.5f * rng.yDistance);
		Collider2D c = Physics2D.OverlapBox(dest, Vector2.one * 0.4f, 0, rng.TargetSide);
		if(c)
		{
			mover = c.GetComponent<UnitBasic>();
			if (mover != null)
			{

				return true;
			}
		}
		
		return false;
	}

	void InflictDistort(UnitBasic inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if (amt <= 0)
			return;
		if (curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if (found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks += amt;
				found.stacks = Mathf.Clamp(found.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
				if (!prevActivate && found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onActivated?.Invoke(this, inflicter, amt);
				}
				else if (prevActivate)
				{
					found.info.onUpdated?.Invoke(this, inflicter, amt);
				}
			}
		}
		else
		{
			InflictedAnomaly ano = new InflictedAnomaly(StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)], amt);
			curStatus.Add(ano);
			if (amt >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
			{
				ano.info.onActivated?.Invoke(this, inflicter, amt);
			}
			ano.stacks = Mathf.Clamp(ano.stacks, 1, StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].maxActivate);
		}
	}

	bool DisflictDistort(UnitBasic inflicter, AnomalyIndex anomaly, int amt = 1)
	{
		if (amt <= 0)
			return false;
		if (curStatus.Exists(item => item.info.Id == (((int)anomaly) + 1)))
		{
			InflictedAnomaly found = curStatus.Find(item => item.info.Id == (((int)anomaly) + 1));
			if (found != null)
			{
				bool prevActivate = found.stacks >= StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate;
				found.stacks -= amt;
				found.info.onUpdated?.Invoke(this, inflicter, -amt);
				if (prevActivate && found.stacks < StatusManager.instance.allAnomalies.allAnomalies[((int)anomaly)].minActivate)
				{
					found.info.onDisactivated?.Invoke(this, inflicter, amt);
				}
				if (found.stacks <= 0)
				{
					curStatus.Remove(found);
					return true;
				}
			}
		}
		return false;
	}

	#endregion

	public virtual void Damage(AttackRange rng, float mult = 1)
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

	}

	public virtual void UnDamage(AttackRange rng)
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
	}

	public virtual void OnDead()
	{
		if (!immunity)
		{
			InflictedAnomaly inf = curStatus.Find(x => x.info.Id == ((int)AnomalyIndex.Revive) + 1);
			if (inf != null && inf.stacks > 0)
			{
				for (int i = 0; i < attackedBy.Count; i++)
				{
					UnDamage(attackedBy[i]);
				}
				attackedBy.Clear();
				DisflictDistort(null, AnomalyIndex.Revive);
				transform.position += Vector3.one - Vector3.forward; // TEST
				StartCoroutine(ImmunitySecond(1f));
			}
			else
			{
				UpdateHandler.instance.destTargets.Add(this);
			}
		}
	}




	#region Locals

	public void DestActs()
	{
		//Debug.Log("Dest Ref " + name);
		foreach (var item in rangeAttackingPair.Keys)
		{
			rangeAttackingPair[item].UnDamage(item);
		}
		rangeAttackingPair.Clear();

		UpdateHandler.instance.RemoveFieldUpdateActs(OnUpdateAct);
	}

	public void OnDrawGizmos()
	{
		for (int i = 0; i < ranges.Count; i++)
		{
			Vector3 dest = transform.position + (transform.right * 0.5f * ranges[i].xDistance) + (transform.up * 0.5f * ranges[i].yDistance);
			Gizmos.DrawWireCube(dest, Vector3.one * 0.4f);
		}
	}

	public void Immobilize()
	{
		movable = false;
	}

	public void Mobilize()
	{
		movable = true;
	}

	IEnumerator ImmunitySecond(float sec)
	{
		immunity = true;
		yield return new WaitForSeconds(sec);
		immunity = false;
	}
	#endregion
}
