using UnityEngine;
using System.Collections;

public class InGameUnitSetTargetPacket : IPacket<InGameUnitSetTargetData>
{
    public class InGameUnitSetTargetSerializer : Serializer
    {
        public bool Serialize(InGameUnitSetTargetData data)
        {
            bool ret = true;
            ret &= Serialize(data.identitySource.unitOwner);
            ret &= Serialize(data.identitySource.unitId);
            ret &= Serialize(data.identityTarget.unitOwner);
            ret &= Serialize(data.identityTarget.unitId);

            return ret;
        }

        public bool Deserialize(ref InGameUnitSetTargetData element)
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
    InGameUnitSetTargetData m_data;

    public InGameUnitSetTargetPacket(InGameUnitSetTargetData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitSetTargetPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitSetTargetSerializer serializer = new InGameUnitSetTargetSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitSetTargetData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitSetTargetSerializer serializer = new InGameUnitSetTargetSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitSetTargetData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitSetTarget;
    }
}
