using System;
using System.Collections.Generic;
using UnityEngine;

class UnitManager
{
    public const int MaxPlayerNum = 5;
    public const int MaxUnitNum = 100;

    private GameObject[,] arrayUnit;                // 빈공간은 있지만 접근이 빠른..
    private LinkedList<GameObject>[] listUnit;      // 빈공간은 없지만 접근이 느린..

    public UnitManager()
    {
        arrayUnit = new GameObject[MaxPlayerNum, MaxUnitNum];

        listUnit = new LinkedList<GameObject>[MaxPlayerNum];
        for(int i = 0; i < MaxPlayerNum; i++)
        {
            listUnit[i] = new LinkedList<GameObject>();
        }
    }

    public GameObject GetUnitObject(int playerNum, int unitId)
    {
        try
        {
            return arrayUnit[playerNum - 1, unitId];
        }
        catch
        {
            return null;
        }
    }

    public int FindEmptySlot(int playerNum)
    {
        for (int i = 0; i < MaxUnitNum; i++)
        {
            if (arrayUnit[playerNum - 1, i] == null)
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
            arrayUnit[data.Info.PlayerNumber - 1, index] = unit;
            listUnit[data.Info.PlayerNumber - 1].AddLast(unit);
            data.Info.SetUnitId(index); // 아이디 지정
        }
        else
        {
            return false;
        }

        return true;
    }
    
    // 남이 만든 유닛 생성 메서드
    public bool InsertSlot(GameObject unit, int unitId) // 유닛을 배열에 넣는...
    {
        UnitProcess data = unit.GetComponent<UnitProcess>();
        Debug.Log("유닛생성::플레이어번호:" + data.Info.PlayerNumber + "  유닛ID : " + unitId);
        if(arrayUnit[data.Info.PlayerNumber - 1, unitId] != null)
        {
            return false;
        }
        arrayUnit[data.Info.PlayerNumber - 1, unitId] = unit;        
        listUnit[data.Info.PlayerNumber - 1].AddLast(unit);
        return true;
    }

    public bool DeleteSlot(int playerNum, int unitId)
    {
        if (arrayUnit[playerNum - 1, unitId] != null)
        {
            listUnit[playerNum - 1].Remove(arrayUnit[playerNum - 1, unitId]);
            arrayUnit[playerNum - 1, unitId] = null;
            
            return true;
        }
        else
        {
            return false;
        }
    }

    public LinkedList<GameObject> GetUnitList(int playerNum)
    {
        return listUnit[playerNum - 1];
    }
    
}