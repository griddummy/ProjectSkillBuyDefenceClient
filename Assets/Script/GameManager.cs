using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//class - policy control
public class GameManager : MonoBehaviour
{
	[SerializeField] int playerNumber;
	[SerializeField] bool[] checkAlly;
	[SerializeField] bool isAI;

	//property
	public int PlayerNumber { get { return playerNumber; } }

	public bool[] CheckAlly { get { return checkAlly; } }

	//initialize this script
	void Start()
	{
		checkAlly = new bool[8];
		checkAlly[playerNumber] = true;
	}

	//start game -> game load process
	public void LoadProcess()
	{

	}
}
