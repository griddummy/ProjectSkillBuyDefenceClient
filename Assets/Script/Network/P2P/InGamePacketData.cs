using UnityEngine;
using System.Collections;

public enum InGamePacketID
{
    CreateUnit = 20,            // 유닛 생성
    UnitSetDestination,         // 유닛 목적지 설정
    UnitImmediatelyMove,        // 유닛 즉시 이동
    UnitSetTarget,              // 유닛 타겟 설정
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
    public byte unitId;      // 인스턴스 유닛의 ID
}

public class InGameCreateUnitData // 유닛생성 데이터
{
    public UnitIdentity identity; // 유닛 소유, ID
    public byte unitType;         // 유닛 종류
    public Vector3 position;      // 생성위치
    public byte level;            // 유닛 레벨
}

public class InGameUnitSetDestinationData // 유닛 목적지로 이동
{
    public UnitIdentity identity;
    public Vector3 destination;        // 목적지
}

public class InGameUnitSetTargetData // 다른 유닛에게로 이동
{
    public UnitIdentity identitySource;
    public UnitIdentity identityTarget;
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
    public byte skillIndex;                 // 배운 스킬의 인덱스
    public Skill.Type type;                 // 타입 - 즉시발동, 타겟위치, 타겟 유닛...
    public Vector3 destination;             // 타격 위치
    public UnitIdentity identityTarget;     // 타격 유닛
}

public class InGameUnitStopData     // 유닛 스탑
{
    public UnitIdentity identity;    
}

public class InGameUnitLevelUpData  // 레벨업
{
    public UnitIdentity identity;
    public byte level;                 
}

public class InGameUnitDamagedData  // 유닛 피해 입음
{
    public UnitIdentity identity;
    public float damage;
}
public class InGameUnitDeathData    // 유닛 사망
{
    public UnitIdentity identity;      
}