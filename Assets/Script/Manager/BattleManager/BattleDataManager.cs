using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleDataManager
{
    public BattleDataManager()
    {
        InitMana();
    }

    #region Turn Count
    private int _TurnCount = 1;
    public int TurnCount => _TurnCount;

    public void TurnPlus()
    {
        _TurnCount++;
    }
    #endregion

    private List<Unit> _UnitList = new List<Unit>();
    public List<Unit> UnitList => _UnitList;

    public void AddUnit(Unit unit) {
        UnitList.Add(unit);
    }

    public void RemoveUnit(Unit unit) {
        UnitList.Remove(unit);
    }

    public Unit GetRandomUnit() {
        if (UnitList.Count == 0)
        {
            return null;
        }
        int randNum = Random.Range(0, UnitList.Count);
        
        Unit unit = UnitList[randNum];
        _UnitList.RemoveAt(randNum);

        return unit;
    }
    
    // 전투를 진행중인 캐릭터가 들어있는 리스트
    List<BattleUnit> _battleUnitList = new List<BattleUnit>();
    public List<BattleUnit> BattleUnitList => _battleUnitList;

    // 현재 리스트를 초기화
    public void UnitListClear() => BattleUnitList.Clear();

    // 리스트에 캐릭터를 추가 / 제거
    public void BattleUnitAdd(BattleUnit unit) => BattleUnitList.Add(unit);

    public void BattleUnitRemove(BattleUnit unit) => BattleUnitList.Remove(unit);


    // Mana Manage
    private const int _maxManaCost = 200;
    private int _currentMana;
    private UI_ManaGuage _manaGuage;

    public void InitMana(int _defaultMana = _maxManaCost)
    {
        _currentMana = _defaultMana;
    }

    private int _mana = 0;

    public void SetManaGuage(UI_ManaGuage guage)
    {
        _manaGuage = guage;
        _manaGuage.DrawGauge(_currentMana);
    }

    public void ChangeMana(int value)
    {
        _currentMana += value;

        if (_maxManaCost <= _currentMana)
            _currentMana = _maxManaCost;
        else if (_currentMana < 0)
            _currentMana = 0;
        
        _manaGuage.DrawGauge(_currentMana);
    }

    public bool CanUseMana(int value)
    {
        if (_currentMana >= value)
            return true;

        Debug.Log("not enough mana");
        return false;
    }
}