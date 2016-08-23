using UnityEngine;
using System.Collections;

public enum InGamePacketID
{
    CreateUnit,                  // 유닛 생성
    UnitMove,                    // 유닛 이동
}

public class InGameCreateUnitData // 유닛생성 데이터
{
    public byte unitOwner;      // 유닛주인
    public int unitId;         // 유닛 아이디    
    public byte unitType;        // 유닛 리소스 아이디
    public float posX;          // 유닛 위치
    public float posY;
    public float posZ;
    public byte level;          // 유닛 레벨
}

public  class GamePacketData
{
    Vector3 vec;
}

public class InGameUnitMoveData
{
    public byte unitOwner;      // 유닛주인
    public int unitId;         // 유닛 아이디
    public float posX;          // 유닛 위치
    public float posY;
    public float posZ;
}

public class InGameUnitAttack
{

}

public class InGameUnitSkillCast
{

}

public class InGameUnitLevelUp
{

}

public class InGameUnitHold
{

}

public class InGameUnitStop
{

}