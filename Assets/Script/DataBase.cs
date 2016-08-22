using System;
using System.Collections;
using System.Collections.Generic;

//local database -> game info
//use single tone
public class Database
{
	//complex database
	static Database databaseInstance = null;
	List<Skill> skillInformation;
	List<UnitInformation> unitInformationSet;

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
		CreateUnitInformationSet();
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

	//initialize unitinformation
	void CreateUnitInformationSet()
	{
		unitInformationSet = new List<UnitInformation> ();
		unitInformationSet.Add( new UnitInformation ( 0001, "UnityChan", 1, 100, 900, 150, 1f, 1f, true, 0f, 8f ) ); 
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

	//search default unit information use default ID
	public UnitInformation FindUnitInformationName( int defaultID )
	{
		for (int i = 0; i < unitInformationSet.Count; i++)
			if (unitInformationSet[i].DefaultID == defaultID)
				return unitInformationSet[i];

		return null;
	}

	//search default unit information use name
	public UnitInformation FindUnitInformationName( string name )
	{
		for (int i = 0; i < unitInformationSet.Count; i++)
			if (unitInformationSet[i].Name == name)
				return unitInformationSet[i];

		return null;
	}
}
