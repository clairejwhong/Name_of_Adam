using System;
using System.Collections;
using UnityEngine;

[Serializable]
public struct Stat
{
    public float HP;
    public float ATK;
    public int SPD;
}

[Serializable]
public enum RangeType
{
    Melee,
    Ranged
}

[Serializable]
public enum Team
{
    Player,
    Enemy,
}

[SerializeField]
public enum Faction
{
    날개의_기사단,
    바벨,
}

[SerializeField]
public enum BehaviorType
{
    원거리,
    근거리,
    서포터,
    탱커,
    전사,
    시즈,
    칼로스,
}

[SerializeField]
public enum Rarity
{
    레어,
    엘리트,
}

[SerializeField]
public enum TargetType
{
    Select,
    Range,
}