using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AnomalyIndex
{
    None = -1,
    Dizzy,
    Empower,
    Vital,
    Revive,
    Protect,
    InfSource,
    Infect,
    SoulLink,
    Ambush,

    Max
}

public class InflictedAnomaly
{
    public Anomaly info;
    public int stacks;

    public InflictedAnomaly(Anomaly anInfo, int stk)
    {
        info = anInfo;
        stacks = stk;
    }
}

[System.Serializable]
public class Anomaly
{
    public int Id;
    public string name;
    public bool isBuff;
    public int minActivate;
    public int maxActivate;

    //ȿ���� ���� ��� , ȿ���� �ο��� ���, �ߵ��� Ʈ������ ��
    public UnityAction<UnitBasic, UnitBasic, int> onActivated;
    public UnityAction<UnitBasic, UnitBasic, int> onUpdated;
    public UnityAction<UnitBasic, UnitBasic, int> onDisactivated;
}


