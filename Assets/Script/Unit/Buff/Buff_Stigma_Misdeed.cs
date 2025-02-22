using UnityEngine;

public class Buff_Stigma_Misdeed : Buff
{
    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Misdeed;

        _name = "�Ǿ�";

        _description = "�Ǿ�.";

        _count = -1;

        _countDownTiming = ActiveTiming.NONE;

        _buffActiveTiming = ActiveTiming.UNIT_TERMINATE;

        _caster = caster;

        _owner = owner;

        _statBuff = false;

        _dispellable = false;

        _stigmaBuff = true;
    }

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        Buff_Vice vice = new();

        caster.SetBuff(vice, caster);

        return false;
    }
}