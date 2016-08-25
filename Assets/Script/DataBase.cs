using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

//local database -> game info
//use single tone
public class Database
{
	//complex database
	static Database databaseInstance = null;
	List<Skill> skillInformation;

    List<UnitData> unitData;

	//property
	public static Database Instance { get { return databaseInstance; } }
    public List<UnitData> UnitData { get { return unitData; } }

    //single tone constructor
    static Database ()
	{
		databaseInstance = new Database ();
	}

	//constructor use initialize
	private Database ()
	{
		CreateSkillInformation();
	}

	//initialize skill information
	void CreateSkillInformation()
	{
		skillInformation = new List<Skill> ();
		skillInformation.Add( new StandardTraining () );
		skillInformation.Add( new RestoreAura () );
		skillInformation.Add( new ShockStun () );
		skillInformation.Add( new Heal () );
		skillInformation.Add( new KaBoom () );
	
		for (int i = 0; i < skillInformation.Count; i++)
			skillInformation[i].SetSkillIcon();
	}

    //유닛 데이터 초기화
    public void CreateUnitData()
    {
        unitData = new List<UnitData>();
        unitData.Add(new UnitData(1, "UnitryChan", 5, 1, 0, 5));
        unitData[0].levelData.Add(new UnitLevelData(1, 51, 900, 150, 200));
        unitData[0].levelData.Add(new UnitLevelData(2, 56, 1000, 200, 400));
        unitData[0].levelData.Add(new UnitLevelData(3, 61, 1100, 250, 700));
        unitData[0].levelData.Add(new UnitLevelData(4, 66, 1200, 300, 1100));
        unitData[0].levelData.Add(new UnitLevelData(5, 71, 1300, 350, 1600));
        unitData[0].levelData.Add(new UnitLevelData(6, 76, 1400, 400, 2200));
        unitData[0].levelData.Add(new UnitLevelData(7, 81, 1550, 480, 2950));
        unitData[0].levelData.Add(new UnitLevelData(8, 86, 1700, 560, 3850));
        unitData[0].levelData.Add(new UnitLevelData(9, 91, 1850, 640, 4900));
        unitData[0].levelData.Add(new UnitLevelData(10, 96, 2000, 720, 6100));

        unitData.Add(new UnitData(2, "GreenFrog", 4, 1, 0, 10));
        unitData[1].levelData.Add(new UnitLevelData(1, 14, 250, 0, 50));
        unitData[1].levelData.Add(new UnitLevelData(2, 17, 400, 0, 75));
        unitData[1].levelData.Add(new UnitLevelData(3, 20, 550, 0, 100));

        unitData.Add(new UnitData(3, "BlueFrog", 3, 1, 5, 10));
        unitData[2].levelData.Add(new UnitLevelData(1, 9, 200, 0, 70));
        unitData[2].levelData.Add(new UnitLevelData(2, 11, 300, 0, 105));
        unitData[2].levelData.Add(new UnitLevelData(3, 13, 400, 0, 140));

        unitData.Add(new UnitData(4, "RedFrog", 5, 1, 0, 10));
        unitData[3].levelData.Add(new UnitLevelData(1, 16, 350, 0, 100));
        unitData[3].levelData.Add(new UnitLevelData(2, 20, 500, 0, 150));
        unitData[3].levelData.Add(new UnitLevelData(3, 24, 650, 0, 200));

        unitData.Add(new UnitData(5, "GiantFrog", 4, 1, 0, 15));
        unitData[4].levelData.Add(new UnitLevelData(1, 25, 500, 0, 200));
        unitData[4].levelData.Add(new UnitLevelData(2, 30, 700, 0, 300));
        unitData[4].levelData.Add(new UnitLevelData(3, 55, 1200, 0, 800));
    }

    //public method

    //search skill use id
    public Skill FindSkillByID( int id )
	{
		for (int i = 0; i < skillInformation.Count; i++)
			if (skillInformation[i].ID == id)
				return skillInformation[i];

		return null;
	}

	//search skill use name
	public Skill FindSkillByName( string name )
	{
		for (int i = 0; i < skillInformation.Count; i++)
			if (skillInformation[i].Name == name)
				return skillInformation[i];

		return null;
	}

    //유닛 기본정보 받아오는 메소드
    public UnitData GetUnitData(int Id)
    {
        int IdIndex = FindWithId(Id);

        if (IdIndex != -1)
        {
            return unitData[IdIndex];
        }
        else
        {
            return null;
        }
    }

    //유닛 레벨정보 받아오는 메소드
    public UnitLevelData GetUnitLevelData(int Id, int level)
    {
        int IdIndex = FindWithId(Id);
        int levelIndex = FindWithLevel(Id, level);

        if (Id != -1 && level != -1)
        {
            UnitLevelData data = unitData[IdIndex].levelData[levelIndex];
            return data;
        }
        else
        {
            return null;
        }
    }

    public int FindWithId(int Id)
    {
        try
        {
            for (int i = 0; i < unitData.Count; i++)
            {
                if (unitData[i].Id == Id)
                {
                    return i;
                }
            }
        }
        catch
        {
            Console.WriteLine("UnitDatabase::FindWithId 에러");
        }

        return -1;
    }

    public int FindWithLevel(int Id, int level)
    {
        try
        {
            for (int i = 0; i < unitData[Id - 1].levelData.Count; i++)
            {
                if (unitData[Id - 1].levelData[i].level == level)
                {
                    return i;
                }
            }
        }
        catch
        {
            Console.WriteLine("UnitDatabase::FindWithLevel 에러");
        }
        return -1;
    }
}

[Serializable]
public class UnitData
{
    public int Id;
    public string unitName;
    public int mvSpeed;
    public int atkSpeed;
    public int atkRange;
    public int searchRange;

    public List<UnitLevelData> levelData;

    public UnitData()
    {
        Id = 0;
        unitName = "";
        mvSpeed = 0;
        atkSpeed = 0;
        atkRange = 0;
        searchRange = 0;
        levelData = new List<UnitLevelData>();
    }

    public UnitData(int newId, string _unitName, int newMvSpeed, int newAtkSpeed, int newAtkRange, int newSearchRange)
    {
        Id = newId;
        unitName = _unitName;
        mvSpeed = newMvSpeed;
        atkSpeed = newAtkSpeed;
        atkRange = newAtkRange;
        searchRange = newSearchRange;
        levelData = new List<UnitLevelData>();
    }
}

[Serializable]
public class UnitLevelData
{
    public int level;
    public int atk;
    public int hp;
    public int mp;
    public int exp;

    public UnitLevelData()
    {
        level = 0;
        atk = 0;
        hp = 0;
        mp = 0;
        exp = 0;
    }

    public UnitLevelData(int newLevel, int newAtk, int newHp, int newMp, int newExp)
    {
        level = newLevel;
        atk = newAtk;
        hp = newHp;
        mp = newMp;
        exp = newExp;
    }
}