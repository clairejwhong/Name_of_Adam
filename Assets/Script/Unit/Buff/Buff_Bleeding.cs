using UnityEngine;

public class Buff_Bleeding : Buff
{
    public override void Init()
    {
        _buffEnum = BuffEnum.Bleeding;

        _name = "Bleeding";

        _description = "������ ���� 5�� ������� �Խ��ϴ�.";

        _count = 3;

        _countDownTiming = ActiveTiming.TURN_END;

        _buffActiveTiming = ActiveTiming.TURN_END;

        _passiveBuff = false;

        _dispellable = true;
    }

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        caster.ChangeHP(-5);

        return false;
    }

    public override void Stack()
    {
        _count = 3;
    }

    public override Stat GetBuffedStat() 
    {
        Stat stat = new();
        return stat;
    }
}