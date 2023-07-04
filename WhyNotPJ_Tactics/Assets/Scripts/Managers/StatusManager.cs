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


		//reflection�� ����ϱ⺸�� �׳� �ռ� �ϳ��ϳ� ���ϱ�� ����.
		//����ӵ��� ���̱� ����.

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
}
