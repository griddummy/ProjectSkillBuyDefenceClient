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
            ret &= Serialize(data.skillIndex);
            ret &= Serialize((byte)data.type);            

            if (data.type == Skill.Type.ActiveNonTarget) // 논타겟
            {
               
            }
            else if (data.type == Skill.Type.ActiveTarget) // 유닛 타겟 스킬
            {
                ret &= Serialize(data.identityTarget.unitOwner);
                ret &= Serialize(data.identityTarget.unitId);
            }
            else if (data.type == Skill.Type.ActiveTargetArea) // 지역 타겟 스킬
            {                
                ret &= Serialize(data.destination.x);
                ret &= Serialize(data.destination.y);
                ret &= Serialize(data.destination.z);
            }

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
            byte type = 0;
            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.skillIndex);           
            ret &= Deserialize(ref type);
            element.type = (Skill.Type)type;
            if (element.type == Skill.Type.ActiveNonTarget) // 논타겟
            {

            }
            else if (element.type == Skill.Type.ActiveTarget) // 유닛 타겟 스킬
            {
                ret &= Deserialize(ref element.identityTarget.unitOwner);
                ret &= Deserialize(ref element.identityTarget.unitId);
            }
            else if (element.type == Skill.Type.ActiveTargetArea) // 지역 타겟 스킬
            {
                ret &= Deserialize(ref element.destination.x);
                ret &= Deserialize(ref element.destination.y);
                ret &= Deserialize(ref element.destination.z);
            }

            

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
