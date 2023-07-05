using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public static StatusManager instance;


    public Anomalies allAnomalies;


	private void Awake()
	{
		instance = this;

		allAnomalies.allAnomalies[0].onActivated += OnDizzyActivate;
		allAnomalies.allAnomalies[0].onDisactivated += OnDizzyDisactivate;

		allAnomalies.allAnomalies[1].onActivated += OnEmpowerActivate;
		allAnomalies.allAnomalies[1].onDisactivated += OnEmpowerDisactivate;

		allAnomalies.allAnomalies[2].onActivated += OnVitalActivate;
		allAnomalies.allAnomalies[2].onDisactivated += OnVitalDisactivate;
		//reflection을 사용하기보다 그냥 손수 하나하나 더하기로 결정.
		//실행속도를 높이기 위함.

	}

	public void OnDizzyActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.Immobilize();
	}

	public void OnDizzyDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		effector.Mobilize();
	}

	public void OnEmpowerActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		int r = effector.curStatus.Find(x => x.info.Id == (int)AnomalyIndex.Empower + 1).stacks;
		effector.atkModifier = r;
	}

	public void OnEmpowerDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		//int r = effector.curStatus.Find(x => x.info.Id == (int)AnomalyIndex.Empower + 1).stacks;
		effector.RemoveAtkScope(amt);
	}

	public void OnVitalActivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		int r = effector.curStatus.Find(x => x.info.Id == (int)AnomalyIndex.Vital + 1).stacks;
		effector.hpModifier = r;
	}

	public void OnVitalDisactivate(UnitMover effector, MoverChecker inflicter, int amt)
	{
		//int r = effector.curStatus.Find(x => x.info.Id == (int)AnomalyIndex.Empower + 1).stacks;
		effector.RemoveHpScope(amt);
	}
}
