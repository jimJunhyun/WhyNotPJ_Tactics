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


		//reflection을 사용하기보다 그냥 손수 하나하나 더하기로 결정.
		//실행속도를 높이기 위함.

	}

	public void OnDizzyActivate(UnitMover effector, MoverChecker inflicter)
	{
		effector.Immobilize();
	}

	public void OnDizzyDisactivate(UnitMover effector, MoverChecker inflicter)
	{
		effector.Mobilize();
	}

	public void OnEmpowerActivate(UnitMover effector, MoverChecker inflicter)
	{
		effector.atkModifier = effector.curStatus.Find(x => x.info.Id == instance.allAnomalies.allAnomalies[((int)AnomalyIndex.Empower)].Id).stacks;
	}

	public void OnEmpowerDisactivate(UnitMover effector, MoverChecker inflicter)
	{
		effector.RemoveAtkScope(effector.curStatus.Find(x => x.info.Id == instance.allAnomalies.allAnomalies[((int)AnomalyIndex.Empower)].Id).stacks);
	}
}
