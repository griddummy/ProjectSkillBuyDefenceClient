using UnityEngine;
using System.Collections;
using System;

public class UnitLevelPacket : IPacket<UnitLevelData>
{
	UnitLevelData m_data;

	public UnitLevelPacket(UnitLevelData data) // 데이터로 초기화(송신용)
	{
		m_data = data;
	}

	public UnitLevelPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
	{
		UnitLevelSerializer serializer = new UnitLevelSerializer();
		serializer.SetDeserializedData(data);
		m_data = new UnitLevelData();
		serializer.Deserialize(ref m_data); 
	}

	public byte[] GetPacketData() // 바이트형 패킷(송신용)
	{
		UnitLevelSerializer serializer = new UnitLevelSerializer();
		serializer.Serialize(m_data);
		return serializer.GetSerializedData();
	}

	public UnitLevelData GetData() // 데이터 얻기(수신용)
	{
		return m_data;
	}

	public int GetPacketId()
	{
		return (int)GuestPacketId.UnitLevel;
	}
}
