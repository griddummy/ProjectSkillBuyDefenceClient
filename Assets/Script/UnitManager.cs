using System;
using System.Collections.Generic;
using UnityEngine;

class UnitManager
{
    public const int maxPlayerNum = 5;
    public const int maxUnitNum = 100;

    public GameObject[,] unitData;

    public UnitManager()
    {
        unitData = new GameObject[maxPlayerNum, maxUnitNum];
    }

    public int FindEmptySlot(int playerNum)
    {
        for (int i = 0; i < maxUnitNum; i++)
        {
            if (unitData[playerNum - 1, i] == null)
            {
                return i;
            }
        }

        return -1;
    }

    public bool InsertSlot (GameObject unit) // 유닛을 배열에 넣는...
    {
        UnitProcess data = unit.GetComponent<UnitProcess>();
        int index = FindEmptySlot(data.Info.PlayerNumber); // 빈슬롯

        if (index != -1)
        {
            unitData[data.Info.PlayerNumber - 1, index] = unit;
            data.Info.SetUnitId(index); // 아이디 지정
        }
        else
        {
            return false;
        }

        return true;
    }

    //TODO
    // 남이 만든 유닛 생성 메서드
    public bool InsertSlot(GameObject unit, int unitId) // 유닛을 배열에 넣는...
    {
        UnitProcess data = unit.GetComponent<UnitProcess>();

        unitData[data.Info.PlayerNumber - 1, unitId] = unit;

        return true;
    }

    public bool DeleteSlot(int playerNum, int unitId)
    {
        if (unitData[playerNum - 1, unitId] != null)
        {
            unitData[playerNum - 1, unitId] = null;
            return true;
        }
        else
        {
            return false;
        }
    }
  
}