using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

public class UnitDatabase
{
    FileStream fs;
    public List<UnitData> unitData;
    BinaryFormatter bin;

    public UnitDatabase()
    {
        fs = null;
        bin = new BinaryFormatter();
        unitData = new List<UnitData>();
    }

    public UnitDatabase(string dataPath, FileMode fileMode)
    {
        fs = new FileStream(dataPath, fileMode);
        bin = new BinaryFormatter();

        try
        {
            if (fs.Length != 0)
            {
                unitData = (List<UnitData>)bin.Deserialize(fs);
            }
            else
            {
                unitData = new List<UnitData>();
            }
        }

        catch
        {
            Console.WriteLine("UnitDatabase::Initialize.Deserialize 에러");
        }

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