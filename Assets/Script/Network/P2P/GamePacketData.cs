using UnityEngine;
using System.Collections;

public enum InGamePacketID
{
    CreateUnit = 12
}
public class InGameCreateUnitData
{
    public byte unitOwner;      // 유닛주인
    public int unitId;         // 유닛 아이디    
    public byte unitResourceId; // 유닛 리소스 아이디
    public float posX;          // 유닛 위치
    public float posY;
    public float posZ;
    public byte level;          // 유닛 레벨
}

    
