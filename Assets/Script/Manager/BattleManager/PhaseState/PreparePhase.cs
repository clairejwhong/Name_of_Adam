using UnityEngine;

public class PreparePhase : Phase
{
    public bool isFirst = true;

    public override void OnStateEnter()
    {
        GameManager.Sound.Play("Stage_Transition/Prepare/PrepareEnter");
        if (!isFirst)
        { 
            BattleManager.Mana.ChangeMana(30);
        }
        BattleManager.Data.TurnPlus();
        BattleManager.BattleUI.UI_playerSkill.Used = false;
        BattleManager.BattleUI.UI_playerSkill.InableSkill();
        BattleManager.BattleUI.UI_turnNotify.SetPlayerTurn();

        BattleManager.BattleUI.ShowTutorial();

        BattleManager.Instance.TurnStart();
    }

    public override void OnStateUpdate()
    {
        BattleManager.Data.BattleUnitOrder();
    }

    public override void OnClickEvent(Vector2 coord)
    {
        BattleManager.Instance.PreparePhaseClick(coord);
    }

    public override void OnStateExit()
    {
        if (isFirst) 
        {
            BattleManager.Data.isDiscount = false;
            foreach (DeckUnit unit in BattleManager.Data.PlayerDeck)
                unit.FirstTurnDiscountUndo();

            foreach (DeckUnit unit in BattleManager.Data.PlayerHands)
                unit.FirstTurnDiscountUndo();

            BattleManager.BattleUI.RefreshHand();
            isFirst = false;
        }

        BattleManager.BattleUI.UI_turnNotify.SetUnitTurn();
    }
}