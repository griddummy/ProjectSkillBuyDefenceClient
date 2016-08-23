using UnityEngine;
using System.Collections;

public enum InGamePacketID
{
    CreateUnit = 20,                 // 유닛 생성
    UnitMove,                   // 유닛 이동 명령 
    UnitImmediatelyMove,        // 유닛 즉시 이동
    UnitAttack,                 // 유닛 공격
    UnitCastSkill,              // 스킬 시전

    UnitStop,                   // 정지
    UnitLevelUp,                // 레벨업
    UnitDamaged,                // 피해입음
    UnitDeath                   // 죽음
}

public struct UnitIdentity // 유닛 정체성
{
    public byte unitOwner;  // 유닛 주인
    public int unitId;      // 인스턴스 유닛의 ID
}

public class InGameCreateUnitData // 유닛생성 데이터
{
    public UnitIdentity identity; // 유닛 소유, ID
    public byte unitType;         // 유닛 종류
    public Vector3 position;      // 생성위치
    public byte level;            // 유닛 레벨
}

public class InGameUnitMoveData // 유닛 이동
{
    public UnitIdentity identity;
    public Vector3 destination;        // 목적지
}

public class InGameUnitImmediatlyMoveData // 유닛 즉시 이동
{
    public UnitIdentity identity;
    public Vector3 destination;    
}

public class InGameUnitAttackData // 목표 공격
{
    public UnitIdentity identitySource;        // 명령을 받느는 유닛
    public UnitIdentity identityTarget;        // 타겟 유닛
}

public class InGameUnitCastSkillData // 스킬 시전
{
    public UnitIdentity identity;
    public int skillId;                    // 스킬 종류
    public int skilllevel;                 // 스킬렙
    public Vector3 castPosition;           // 스킬 사용위치
    public UnitIdentity identityTarget;    // 타겟 유닛
}

public class InGameUnitStopData     // 유닛 스탑
{
    public UnitIdentity identity;    
}

public class InGameUnitLevelUpData  // 레벨업
{
    public UnitIdentity identity;
    public int level;                 
}

public class InGameUnitDamagedData  // 유닛 피해 입음
{
    public UnitIdentity identity;
    public int damage;
}
public class InGameUnitDeathData    // 유닛 사망
{
    public UnitIdentity identity;      
}