using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class TcpGuest : MonoBehaviour
{
	public Queue<byte[]> receiveMsgs;
	public GameObject[,] playerUnit;
	public int playerNum = 2;
	public int myNum = 0;
	public const int maxUnitNum = 100;

	object receiveLock;

	byte[] header;
	byte[] packet = new byte[1024];
	byte[] data;
	byte[] packetLength = new byte[2];
	byte packetId;

	public delegate void RecvNotifier(byte[] data);
	private Dictionary<int, RecvNotifier> m_notifier = new Dictionary<int, RecvNotifier>();

	public TcpGuest (Queue<byte[]> receiveQueue, object newReceiveLock)
	{
		receiveMsgs = receiveQueue;
		receiveLock = newReceiveLock;
		playerUnit = new GameObject[playerNum, maxUnitNum];
	}

	public int FindEmptyUnitId(int playerNum)
	{
		for (int i = 0; i < maxUnitNum; i++)
		{
			if (playerUnit [playerNum, i] != null)
				return i;
		}

		return -1;
	}

	public void CreateUnit (byte[] data)
	{
		CreateUnitPacket createUnitPacket = new CreateUnitPacket (data);
		CreateUnitData unitData = createUnitPacket.GetData ();

		GameObject unit = Instantiate(Resources.Load(unitData.unitName, typeof(GameObject))) as GameObject;
		Vector3 unitPos = new Vector3 (unitData.posX, unitData.posY, unitData.posZ);
		//unit.GetComponent<UnitProcess> ().SetUnit ();

		UnitInformation unitInfo = unit.GetComponent<UnitInformation> ();

		//unitInfo.UnitName = unitData.unitName;
		//unit.SetUnit(default);

//		playerUnit [unitData.playerNum] = unit;
	}

	public void MoveUnit (byte[] data)
	{
		MoveUnitPacket moveUnitPacket = new MoveUnitPacket (data);
		MoveUnitData unitData = moveUnitPacket.GetData ();

//		GameObject unit = playerUnit [unitData.playerNum] [unitData.Id];
//		Vector3 destination = new Vector3 (unitData.posX, unitData.posY, unitData.posZ);
//		unit.GetComponent<UnitProcess> ().Destination = destination;
	}

	public void AttackUnit (byte[] data)
	{
		AttackUnitPacket attackUnitPacket = new AttackUnitPacket (data);
		AttackUnitData unitData = attackUnitPacket.GetData ();

//		GameObject unit = playerUnit [unitData.playerNum] [unitData.Id];
//		GameObject targetUnit = playerUnit [unitData.targetPlayerNum] [unitData.targetId];
	}

	public void SkillUse (byte[] data)
	{
		SkillUsePacket skillUsePacket = new SkillUsePacket (data);
		SkillUseData unitData = skillUsePacket.GetData ();

//		GameObject unit = playerUnit [unitData.playerNum] [unitData.Id];
//		GameObject targetUnit = playerUnit [unitData.targetPlayerNum] [unitData.targetId];
	}



	public static byte[] CombineByte (byte[] array1, byte[] array2)
	{
		byte[] array3 = new byte[array1.Length + array2.Length];
		Array.Copy (array1, 0, array3, 0, array1.Length);
		Array.Copy (array2, 0, array3, array1.Length, array2.Length);
		return array3;
	}

	public static byte[] CombineByte (byte[] array1, byte[] array2, byte[] array3)
	{
		byte[] array4 = CombineByte (CombineByte (array1, array2), array3);;
		return array4;
	}
}