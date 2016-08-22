using UnityEngine;
using System.Collections;
using System;

public class AttackUnitPacket : IPacket<AttackUnitData>
{
	AttackUnitData m_data;

	public AttackUnitPacket(AttackUnitData data) // 데이터로 초기화(송신용)
	{
		m_data = data;
	}

	public AttackUnitPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
	{
		AttackUnitSerializer serializer = new AttackUnitSerializer();
		serializer.SetDeserializedData(data);
		m_data = new AttackUnitData();
		serializer.Deserialize(ref m_data); 
	}

	public byte[] GetPacketData() // 바이트형 패킷(송신용)
	{
		AttackUnitSerializer serializer = new AttackUnitSerializer();
		serializer.Serialize(m_data);
		return serializer.GetSerializedData();
	}

	public AttackUnitData GetData() // 데이터 얻기(수신용)
	{
		return m_data;
	}

	public int GetPacketId()
	{
		return (int)GuestPacketId.AttackUnit;
	}
}
