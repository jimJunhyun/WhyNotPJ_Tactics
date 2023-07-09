using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

struct UpdateAct
{
	public Action<AttackRange> act;
	public AttackRange range;

	public UpdateAct(Action<AttackRange> act, AttackRange rng)
	{
		this.act = act;
		range = rng;
	}
}

public class UpdateHandler : MonoBehaviour
{
	public static UpdateHandler instance;

	List<UpdateAct> updActs = new List<UpdateAct>();

	public List<UnitMover> destTargets = new List<UnitMover>();

	private void Awake()
	{
		instance = this;
	}

	// Update is called once per frame
	void Update()
    {
        if(updActs.Count > 0)
		{
			InvokeActs();
		}
		if (destTargets.Count > 0)
		{
			InvokeDests();
		}
	}

	void InvokeActs()
	{
		for (int i = 0; i < updActs.Count; i++)
		{
			updActs[i].act.Invoke(updActs[i].range);
		}
	}

	void InvokeDests()
	{
		for (int i = 0; i < destTargets.Count; i++)
		{
			
			Destroy(destTargets[i].gameObject);
		}
		destTargets.Clear();
	}

    public void AddUpdater(Action<AttackRange> act, AttackRange rng)
	{
		UpdateAct updAct = new UpdateAct(act, rng);

		updActs.Add(updAct);
	}
}
