using UnityEngine;

public class Buff_Immortal : Buff
{    public override void Init(BattleUnit caster, BattleUnit owner)
    {
        _buffEnum = BuffEnum.Immortal;

        _name = "불사";

        _sprite = GameManager.Resource.Load<Sprite>($"Arts/Buff/Buff_Immortal_Sprite");

        _description = "죽음에 이르는 피해를 입을 시 1의 체력으로 살아남습니다.";

        _count = 1;

        _countDownTiming = ActiveTiming.UNIT_DEAD;

        _buffActiveTiming = ActiveTiming.UNIT_DEAD;

        _caster = caster;

        _owner = owner;

        _statBuff = false;

        _dispellable = true;

        _stigmaBuff = false;
    }

    public override bool Active(BattleUnit caster, BattleUnit receiver)
    {
        caster.ChangeHP(-1 * caster.HP.GetCurrentHP() + 1);
        return true;
    }

    public override void Stack()
    {
        _count += 1;
    }
}