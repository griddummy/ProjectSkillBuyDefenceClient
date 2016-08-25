using UnityEngine;
using System.Collections;

public class InGameUnitAddSkillPacket : IPacket<InGameUnitAddSkillData>
{
    public class InGameUnitAddSkillSerializer : Serializer
    {
        public bool Serialize(InGameUnitAddSkillData data)
        {
            bool ret = true;

            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.skillid);


            return ret;
        }

        public bool Deserialize(ref InGameUnitAddSkillData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.skillid);


            return ret;
        }
    }
    InGameUnitAddSkillData m_data;

    public InGameUnitAddSkillPacket(InGameUnitAddSkillData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitAddSkillPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitAddSkillSerializer serializer = new InGameUnitAddSkillSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitAddSkillData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitAddSkillSerializer serializer = new InGameUnitAddSkillSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitAddSkillData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitAddSKill;
    }
}
