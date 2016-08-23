using UnityEngine;
using System.Collections;

public class InGameUnitAttackPacket : IPacket<InGameUnitAttackData>
{

    public class InGameUnitTargetAttackSerializer : Serializer
    {
        public bool Serialize(InGameUnitAttackData data)
        {
            bool ret = true;

            ret &= Serialize(data.identitySource.unitOwner);
            ret &= Serialize(data.identitySource.unitId);
            ret &= Serialize(data.identityTarget.unitOwner);
            ret &= Serialize(data.identityTarget.unitId);

            return ret;
        }

        public bool Deserialize(ref InGameUnitAttackData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identitySource.unitOwner);
            ret &= Deserialize(ref element.identitySource.unitId);
            ret &= Deserialize(ref element.identityTarget.unitOwner);
            ret &= Deserialize(ref element.identityTarget.unitId);

            return ret;
        }
    }
    InGameUnitAttackData m_data;

    public InGameUnitAttackPacket(InGameUnitAttackData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitAttackPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitTargetAttackSerializer serializer = new InGameUnitTargetAttackSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitAttackData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitTargetAttackSerializer serializer = new InGameUnitTargetAttackSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitAttackData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitAttack;
    }
}
