using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    private BattleDataManager _battleData;
    public BattleDataManager Data => _battleData;

    private UIManager _UIMNG;
    private Field _field;
    public Field Field => _field;

    private List<BattleUnit> _BattleUnitOrderList;

    [SerializeField] private bool TestMode = true;
    private void Awake()
    {
        _battleData = new BattleDataManager();
        
        _BattleUnitOrderList = new List<BattleUnit>();
        //_WatingLine = GameManager.UI.WatingLine;
        _UIMNG = GameManager.UI;

        GameObject fieldTmp = GameObject.Find("Field");
        if (fieldTmp != null)
        {
            _field = fieldTmp.GetComponent<Field>().SetClickEvent(OnClickTile);
            Debug.Log(_field);
        }

        StartEnter();
    }

    private void Start()
    {
        if (TestMode)
            InitSelf();
    }

    // Test
    private void InitSelf()
    {
        if(_field == null)
        {
            _field = GameManager.Resource.Instantiate("Field").GetComponent<Field>();
            _field.SetClickEvent(OnClickTile);
        }

        GetComponent<UnitSpawner>().Init();
    }

    public void OnClickTile(Vector2 coord, Tile tile)
    {
        //Prepare 페이즈
        if (CurrentPhase == Phase.Prepare)
        {
            // ----------------변경 예정------------------------
            Unit clickedUnit = _UIMNG.Hands.ClickedUnit;
            if (clickedUnit == null)
                return;

            Data.ChangeMana(-1 * clickedUnit.Data.ManaCost);

            GameObject BattleUnitPrefab = GameManager.Resource.Instantiate("Unit");
            BattleUnit BattleUnit = BattleUnitPrefab.GetComponent<BattleUnit>();

            BattleUnit.Data = clickedUnit.Data;
            BattleUnit.setLocate(coord);

            BattleUnit.Init(Team.Player, coord);

            _UIMNG.Hands.RemoveHand(_UIMNG.Hands.ClickedHand);
            _UIMNG.Hands.ClearHand();
            // ------------------------------------------------
        }
        //Start 페이즈
        else if (CurrentPhase == Phase.Start)
        {
            //범위 외
            if (Field.IsPlayerRange(coord) == false || Field.GetUnit(coord) != null)
                return;

            // ----------------변경 예정------------------------
            Unit clickedUnit = _UIMNG.Hands.ClickedUnit;
            if (clickedUnit == null)
                return;

            _battleData.ChangeMana(-1 * clickedUnit.Data.ManaCost);

            GameObject BattleUnitPrefab = GameManager.Resource.Instantiate("Unit");
            BattleUnit BattleUnit = BattleUnitPrefab.GetComponent<BattleUnit>();

            BattleUnit.Data = clickedUnit.Data;
            BattleUnit.setLocate(coord);

            BattleUnit.Init(Team.Player, coord);

            _UIMNG.Hands.RemoveHand(_UIMNG.Hands.ClickedHand);
            _UIMNG.Hands.ClearHand();
            // ------------------------------------------------
        }
        //Engage 페이즈
        else
        {
            _field.ClearAllColor();
            GetNowUnit().TileSelected(coord);

            // 코루틴 체크
            return;
        }
    }
    
    #region Phase Control
    public enum Phase
    {
        Start,
        Prepare,
        Engage
    }

    private Phase _CurrentPhase;
    public Phase CurrentPhase => _CurrentPhase;

    public void PhaseUpdate()
    {
        if (_CurrentPhase == Phase.Prepare)
        {
            PrepareExit();
            EngageEnter();

            PhaseChanger(Phase.Engage);
        }
        else if (_CurrentPhase == Phase.Engage)
        {
            EngageExit();
            PrepareEnter();

            PhaseChanger(Phase.Prepare);
        }
        else if(_CurrentPhase == Phase.Start)
        {
            StartExit();
            EngageEnter();

            PhaseChanger(Phase.Engage);
        }
    }

    public void PhaseChanger(Phase phase)
    {
        _CurrentPhase = phase;
    }

    public void StartEnter()
    {
        //전투시 맨 처음 Prepare 단계
        Debug.Log("Start Enter");
        PhaseChanger(Phase.Start);
    }

    public void StartExit()
    {
        Debug.Log("Start Exit");
    }

    public void PrepareEnter()
    {
        Debug.Log("Prepare Enter");
        PhaseChanger(Phase.Prepare);
        //UI 튀어나옴
        //UI가 작동할 수 있게 해줌
    }

    public void PrepareExit()
    {
        Debug.Log("Prepare Exit");
        //UI 들어감
        //UI 사용 불가
    }

    public void EngageEnter()
    {
        Debug.Log("Engage Enter");
        PhaseChanger(Phase.Engage);
        //UI 튀어나옴
        //UI가 작동할 수 있게 해줌

        _BattleUnitOrderList.Clear();

        foreach(BattleUnit unit in _battleData.BattleUnitList)
        {
            _BattleUnitOrderList.Add(unit);
        }

        // 턴 시작 전에 다시한번 순서를 정렬한다.
        BattleOrderReplace();
        GameManager.Battle.Field.ClearAllColor();

        UseUnitSkill();
    }

    public void EngageExit()
    {
        Debug.Log("Engage Exit");
        //UI 들어감
        //UI 사용 불가
        _battleData.ChangeMana(2);
        BattleOverCheck();
    }
    #endregion

    public void BattleOverCheck()
    {
        int MyUnit = 0;
        int EnemyUnit = 0;
        foreach(BattleUnit BUnit in Data.BattleUnitList)
        {
            if (BUnit.Team == Team.Player)//아군이면
                MyUnit++;
            else
                EnemyUnit++;
        }

        MyUnit += Data.UnitList.Count;
        //EnemyUnit 대기 중인 리스트만큼 추가하기

        if (MyUnit == 0)
        {
            GameOver();
        }
        else if(EnemyUnit == 0)
        {
            BattleOver();
        }
    }

    public void BattleOver()
    {
        Debug.Log("YOU WIN");
    }

    public void GameOver()
    {
        Debug.Log("YOU LOSE");
    }

    // BattleUnitList를 정렬
    // 1. 스피드 높은 순으로, 2. 같을 경우 왼쪽 위부터 오른쪽으로 차례대로
    public void BattleOrderReplace()
    {
        _BattleUnitOrderList = _BattleUnitOrderList.OrderByDescending(unit => unit.GetStat().SPD)
            .ThenByDescending(unit => unit.Location.y)
            .ThenBy(unit => unit.Location.x)
            .ToList();
    }

    public List<BattleUnit> GetUnitbyOrder()
    {
        return _BattleUnitOrderList;
    }

    public void BattleOrderRemove(BattleUnit _unit)
    {
        _BattleUnitOrderList.Remove(_unit);
    }

    // BattleUnitList의 첫 번째 요소부터 순회
    // 다음 차례의 공격 호출은 CutSceneMNG의 ZoomOut에서 한다.
    public void UseUnitSkill()
    {
        if (_BattleUnitOrderList.Count <= 0)
        {
            PhaseUpdate();
            return;
        }

        if (0 < _BattleUnitOrderList[0].HP.GetCurrentHP())
        {
            SetTileColor(Color.yellow);

            // 입력 대기 코루틴 실행

            //_BattleUnitOrderList[0].ChangeState(BattleUnitState.Move);
            //_BattleUnitOrderList[0].UpdateState();
            if (_BattleUnitOrderList[0].Team == Team.Enemy)
            {
                Unit_AI_Controller ai = _BattleUnitOrderList[0].GetComponent<Unit_AI_Controller>();
                ai.SetCaster(_BattleUnitOrderList[0]);
                ai.AI_Action();    
            }
        }
        else
        {
            UseNextUnit();
        }
    }

    public void UseNextUnit()
    {
        _BattleUnitOrderList.RemoveAt(0);
        UseUnitSkill();
    }
    
    // 이동 경로를 받아와 이동시킨다
    public void MoveLotate(BattleUnit caster, Vector2 coord)
    {
        Vector2 current = caster.Location;
        Vector2 dest = current + coord;

        Field.MoveUnit(current, dest);
    }

    
    public void SetTileColor(Color clr)
    {
        List<Vector2> rangeList = Field.Get_Abs_Pos(GetNowUnit());
        Field.SetTileColor(rangeList, clr);
    }

    public void SetUnit(BattleUnit unit, Vector2 coord)
    {
        Field.EnterTile(unit, coord);
    }

    public BattleUnit GetNowUnit() => _BattleUnitOrderList[0];

    #region AI
    public void BattleUnitAI()
    {
        List<Vector2> FindTileList = new List<Vector2>();
        List<Vector2> RangedVectorList = new List<Vector2>();

        List<Vector2> AttackRangeList = _BattleUnitOrderList[0].GetRange();

        //전달받은 범위에서 유닛을 찾는다.
        foreach (Vector2 arl in AttackRangeList)
        {
            Vector2 vector = _BattleUnitOrderList[0].Location;

            if (Field.IsInRange(vector))
            {
                Vector2 vec = vector;
                if (_field.TileDict[vec].IsOnTile)
                {
                    FindTileList.Add(vec);
                }
            }
        }

        //찾은 유닛이 있는지 확인하고, 있다면 원거리인지, 근거리인지 확인한다.
        if (FindTileList.Count > 0)
        {
            foreach (Vector2 ftl in FindTileList)
            {
                if (_field.TileDict[ftl].Unit.Data.BehaviorType == BehaviorType.원거리)
                {
                    RangedVectorList.Add(ftl);
                }
            }

            if (RangedVectorList.Count > 0)
            {
                //원거리 유닛이 있을 경우
                //Random.Range(0, RangeList.Count);
            }
            else
            {
                //근거리 유닛만 있을 경우
                //Random.Range(0, findUnitList.Count);
            }
        }
        else
        {
            //공격 범위 내에서 찾은 유닛이 없으면 이동하고 공격한다
            SortedSet<Vector3> AttackTileSet = new SortedSet<Vector3>();

            //모든 공격 타일을 AttackTileSet에 저장한다. X, Y는 좌표, Z는 원거리/근거리 유무
            foreach(BattleUnit unit in _battleData.BattleUnitList)
            {
                if (unit.Team == Team.Player)
                {
                    foreach (Vector2 arl in AttackRangeList)
                    {
                        Vector3 vector = unit.Location - arl;
                        if (unit.Data.BehaviorType == BehaviorType.원거리)
                            vector.z = 0f;//원거리면 0
                        else
                            vector.z = 0.1f;//근거리면 0.1


                        AttackTileSet.Add(vector);
                    }
                }
            }

            //유닛을 때릴 수 있는 타일이 이동 범위 내에 있는 지 확인한다.
            //단 위, 아래, 왼, 오른쪽만 이동 가능하다고 가정
            for (int i = -1; i <= 1; i += 2)
            {
                for (float j = 0; j <= 0.1f; j += 0.1f)
                {
                    Vector3 vec1 = new Vector3(_BattleUnitOrderList[0].Location.x + i, _BattleUnitOrderList[0].Location.y, j);
                    if (AttackTileSet.Contains(vec1))
                    {
                        FindTileList.Add(vec1);
                    }

                    Vector3 vec2 = new Vector3(_BattleUnitOrderList[0].Location.x, _BattleUnitOrderList[0].Location.y + i, j);
                    if (AttackTileSet.Contains(vec2))
                    {
                        FindTileList.Add(vec2);
                    }
                }
            }


            if (FindTileList.Count > 0)
            {
                //이동해서 갈 수 있는 공격 타일이 있을 경우
                foreach(Vector3 v in FindTileList)
                {
                    if (v.z == 0)
                    {
                        RangedVectorList.Add(new Vector2(v.x, v.y));
                    }
                }

                if (RangedVectorList.Count > 0)
                {
                    //원거리가 있음
                    //Random.Range(0, RangedVectorList.Count);
                }
                else
                {
                    //근거리만 있음
                    //Random.Range(0, FindTileList.Count);
                }
            }
            else
            {
                Vector3 MyPosition = _BattleUnitOrderList[0].Location;

                float dis = 100f;
                Vector3 minVec = new Vector3();

                foreach (Vector3 v in RangedVectorList)
                {
                    if (dis > Mathf.Abs(v.x - MyPosition.x) + Mathf.Abs(v.y - MyPosition.y)) 
                    {
                        dis = Mathf.Abs(v.x - MyPosition.x) + Mathf.Abs(v.y - MyPosition.y);
                        minVec = v;
                    }
                }
                //가장 가까운 타일 = minVec으로 이동

                dis = 100f;//재활용
                Vector3 moveVec = new Vector3();
                for (int i = -1; i <= 1; i += 2)
                {
                    Vector3 vec1 = new Vector3(MyPosition.x + i, MyPosition.y, 0);
                    if (dis > (vec1 - minVec).sqrMagnitude) 
                    {
                        dis = (vec1 - minVec).sqrMagnitude;
                        moveVec = vec1;
                    }

                    Vector3 vec2 = new Vector3(MyPosition.x, MyPosition.y + i, 0);
                    if (dis > (vec2 - minVec).sqrMagnitude)
                    {
                        dis = (vec2 - minVec).sqrMagnitude;
                        moveVec = vec2;
                    }
                }
                //moveVec으로 이동
            }
        }
    }
    #endregion
}