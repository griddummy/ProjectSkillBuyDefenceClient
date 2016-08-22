﻿using UnityEngine;
using System.Collections;
using System;

public class SkillUsePacket : IPacket<SkillUseData>
{
	SkillUseData m_data;

	public SkillUsePacket(SkillUseData data) // 데이터로 초기화(송신용)
	{
		m_data = data;
	}

	public SkillUsePacket(byte[] data) // 패킷을 데이터로 변환(수신용)
	{
		SkillUseSerializer serializer = new SkillUseSerializer();
		serializer.SetDeserializedData(data);
		m_data = new SkillUseData();
		serializer.Deserialize(ref m_data); 
	}

	public byte[] GetPacketData() // 바이트형 패킷(송신용)
	{
		SkillUseSerializer serializer = new SkillUseSerializer();
		serializer.Serialize(m_data);
		return serializer.GetSerializedData();
	}

	public SkillUseData GetData() // 데이터 얻기(수신용)
	{
		return m_data;
	}

	public int GetPacketId()
	{
		return (int)GuestPacketId.SkillUse;
	}
}
