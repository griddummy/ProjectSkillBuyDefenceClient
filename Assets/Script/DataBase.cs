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

	
		for (int i = 0; i < skillInformation.Count; i++)
			skillInformation[i].SetSkillIcon();
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
}
