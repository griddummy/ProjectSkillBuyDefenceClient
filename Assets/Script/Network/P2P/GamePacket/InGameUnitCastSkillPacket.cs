using UnityEngine;
using System.Collections;

public class InGameUnitCastSkillPacket : IPacket<InGameUnitCastSkillData>
{
    public class InGameUnitCastSkillSerializer : Serializer
    {
        public bool Serialize(InGameUnitCastSkillData data)
        {
            bool ret = true;

            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.skillId);
            ret &= Serialize(data.skilllevel);
            ret &= Serialize(data.castPosition.x);
            ret &= Serialize(data.castPosition.y);
            ret &= Serialize(data.castPosition.z);
            ret &= Serialize(data.identityTarget.unitOwner);
            ret &= Serialize(data.identityTarget.unitId);

            return ret;
        }

        public bool Deserialize(ref InGameUnitCastSkillData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;
            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.skillId);
            ret &= Deserialize(ref element.skilllevel);
            ret &= Deserialize(ref element.castPosition.x);
            ret &= Deserialize(ref element.castPosition.y);
            ret &= Deserialize(ref element.castPosition.z);
            ret &= Deserialize(ref element.identityTarget.unitOwner);
            ret &= Deserialize(ref element.identityTarget.unitId);

            return ret;
        }
    }
    InGameUnitCastSkillData m_data;

    public InGameUnitCastSkillPacket(InGameUnitCastSkillData data)
    {
        m_data = data;
    }

    public InGameUnitCastSkillPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitCastSkillSerializer serializer = new InGameUnitCastSkillSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitCastSkillData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitCastSkillSerializer serializer = new InGameUnitCastSkillSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitCastSkillData GetData()
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitCastSkill;
    }
}
