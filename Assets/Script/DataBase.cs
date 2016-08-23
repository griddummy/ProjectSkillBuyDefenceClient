using System;
using System.IO;
using System.Collections;
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

	//single tone constructor
	static Database ()
	{
		databaseInstance = new Database ();
	}

	//constructor use initialize
	private Database ()
	{
		CreateSkillInformation();
        CreateUnitData();
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
    void CreateUnitData()
    {
        FileStream fs = new FileStream("UnitData.data", FileMode.Open);
        BinaryFormatter bin = new BinaryFormatter();

        try
        {
            unitData = (List<UnitData>)bin.Deserialize(fs);
        }
        catch
        {
            Console.WriteLine("Database::Initialize.Deserialize 에러");
        }
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
        if (unitData.Count >= Id - 1) {
            UnitData data = unitData[Id - 1];
            return data;
        }
        else
        {
            return null;
        }
    }

    //유닛 레벨정보 받아오는 메소드
    public UnitLevelData GetUnitLevelData (int Id, int level)
    {
        if (unitData.Count >= Id - 1)
        {
            if (unitData[Id - 1].levelData.Count >= level - 1)
            {
                UnitLevelData data = unitData[Id - 1].levelData[level-1];
                return data;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
