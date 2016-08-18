using UnityEngine;
using System.Collections;

public class UIControl : MonoBehaviour
{
	//simple data field

	//complex data field
	[SerializeField] UnitProcess presentUnit;
	[SerializeField] UIUnitStatus unitStatus;
	[SerializeField] UICommandButton commandButton;


	//initialize this script
	void Start()
	{
		LinkElement();
	}

	//link low rank element
	void LinkElement()
	{

	}


	//public method

	//update ui Information
	public void UpdateUIInformation()
	{

	}
}
