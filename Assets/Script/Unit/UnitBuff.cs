using System;
using System.Collections.Generic;
using UnityEngine;

public class UnitBuff : MonoBehaviour
{
    [SerializeField] private List<Buff> _buffList;

    public void SetBuff(Buff buff, BattleUnit caster, BattleUnit owner, int num = 0)
    {
        buff.Init(caster, owner);
        buff.SetValue(num);

        foreach (Buff listedBuff in _buffList)
        {
            if (buff.BuffEnum == listedBuff.BuffEnum)
            {
                listedBuff.Stack();

                return;
            }
        }

        _buffList.Add(buff);
    }

    public bool CheckBuff(BuffEnum buffEnum)
    {
        foreach (Buff listedBuff in _buffList)
        {
            if (buffEnum == listedBuff.BuffEnum)
            {
                return true;
            }
        }

        return false;
    }

    public bool DeleteBuff(BuffEnum buffEnum)
    {
        for (int i = 0; i < _buffList.Count; i++)
        {
            if (buffEnum == _buffList[i].BuffEnum)
            {
                _buffList[i].Destroy();
                _buffList.RemoveAt(i);

                return true;
            }
        }

        return false;
    }

    public Stat GetBuffedStat()
    {
        Stat buffedStat = new();

        foreach (Buff buff in _buffList)
        {
            if (buff.StatBuff)
            {
                buffedStat += buff.GetBuffedStat();
            }
        }

        return buffedStat;
    }

    public List<Buff> CheckActiveTiming(ActiveTiming activeTiming)
    {
        List<Buff> checkedBuffList = new();

        foreach (Buff buff in _buffList)
        {
            if (buff.BuffActiveTiming == activeTiming)
            {
                checkedBuffList.Add(buff);
            }
        }

        return checkedBuffList;
    }

    public void CheckCountDownTiming(ActiveTiming countDownTiming)
    {
        int i;

        for (i = 0; i < _buffList.Count; i++)
        {
            if (_buffList[i].CountDownTiming == countDownTiming)
            {
                _buffList[i].CountChange(-1);
                if (_buffList[i].Count == 0)
                {
                    _buffList[i].Owner.DeleteBuff(_buffList[i].BuffEnum);

                    i--;
                }
            }
        }
    }

    public void DispelBuff()
    {
        for (int i = 0; i < _buffList.Count; i++)
        {
            if (_buffList[i].Dispellable)
            {
                _buffList[i].Owner.DeleteBuff(_buffList[i].BuffEnum);
            }
        }
    }
}