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
    Charge,
    Plague,
    SoulLink,
}

public class InflictedAnomaly
{
    public Anomaly info;
    public RefableInt stacks;

    public InflictedAnomaly(Anomaly anInfo, int stk)
    {
        info = anInfo;
        stacks = new RefableInt(stk);
    }
}

public class RefableInt : IComparable
{
    public int val;
    public RefableInt(int val)
    {
        this.val = val;
    }

	public int CompareTo(object obj)
	{
		return val.CompareTo(obj);
	}

	public static RefableInt operator ++(RefableInt v)
	{
        v.val = v.val + 1;
        return v;
	}
    public static RefableInt operator +(RefableInt left, RefableInt right)
    {
        RefableInt nVal = new RefableInt(left.val + right.val);
        return nVal;
    }
    public static RefableInt operator +(RefableInt left, int right)
    {
        RefableInt nVal = new RefableInt(left.val + right);
        return nVal;
    }
    public static RefableInt operator -(RefableInt left, RefableInt right)
    {
        RefableInt nVal = new RefableInt(left.val - right.val);
        return nVal;
    }
    public static RefableInt operator -(RefableInt left, int right)
    {
        RefableInt nVal = new RefableInt(left.val - right);
        return nVal;
    }

    public static bool operator >(RefableInt left, RefableInt right)
	{
        return left.val > right.val;
	}
    public static bool operator >(RefableInt left, int right)
    {
        return left.val > right;
    }
    public static bool operator <(RefableInt left, RefableInt right)
    {
        return left.val < right.val;
    }
    public static bool operator <(RefableInt left, int right)
    {
        return left.val < right;
    }
    public static bool operator >=(RefableInt left, RefableInt right)
    {
        return left.val >= right.val;
    }
    public static bool operator >=(RefableInt left, int right)
    {
        return left.val >= right;
    }
    public static bool operator <=(RefableInt left, RefableInt right)
    {
        return left.val <= right.val;
    }
    public static bool operator <=(RefableInt left, int right)
    {
        return left.val <= right;
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

    //효과를 받은 사람 , 효과를 부여한 사람
    public UnityAction<UnitMover, MoverChecker> onActivated;
    public UnityAction<UnitMover, MoverChecker> onDisactivated;
}


