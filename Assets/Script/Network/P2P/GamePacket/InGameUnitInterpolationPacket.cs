using UnityEngine;
using System.Collections;

public class InGameUnitInterpolationPacket : IPacket<InGameUnitInterpolationData>
{
    public class InGameUnitInterpolationSerializer : Serializer
    {
        public bool Serialize(InGameUnitInterpolationData data)
        {
            bool ret = true;
            ret &= Serialize(data.identity.unitOwner);
            ret &= Serialize(data.identity.unitId);
            ret &= Serialize(data.position.x);
            ret &= Serialize(data.position.y);
            ret &= Serialize(data.position.z);
            ret &= Serialize(data.forward.x);
            ret &= Serialize(data.forward.y);
            ret &= Serialize(data.forward.z);

            return ret;
        }

        public bool Deserialize(ref InGameUnitInterpolationData element)
        {
            if (GetDataSize() == 0)
            {
                // 데이터가 설정되지 않았다.
                return false;
            }

            bool ret = true;

            ret &= Deserialize(ref element.identity.unitOwner);
            ret &= Deserialize(ref element.identity.unitId);
            ret &= Deserialize(ref element.position.x);
            ret &= Deserialize(ref element.position.y);
            ret &= Deserialize(ref element.position.z);
            ret &= Deserialize(ref element.forward.x);
            ret &= Deserialize(ref element.forward.y);
            ret &= Deserialize(ref element.forward.z);

            return ret;
        }
    }
    InGameUnitInterpolationData m_data;

    public InGameUnitInterpolationPacket(InGameUnitInterpolationData data) // 데이터로 초기화(송신용)
    {
        m_data = data;
    }

    public InGameUnitInterpolationPacket(byte[] data) // 패킷을 데이터로 변환(수신용)
    {
        InGameUnitInterpolationSerializer serializer = new InGameUnitInterpolationSerializer();
        serializer.SetDeserializedData(data);
        m_data = new InGameUnitInterpolationData();
        serializer.Deserialize(ref m_data);
    }

    public byte[] GetPacketData() // 바이트형 패킷(송신용)
    {
        InGameUnitInterpolationSerializer serializer = new InGameUnitInterpolationSerializer();
        serializer.Serialize(m_data);
        return serializer.GetSerializedData();
    }

    public InGameUnitInterpolationData GetData() // 데이터 얻기(수신용)
    {
        return m_data;
    }

    public int GetPacketId()
    {
        return (int)InGamePacketID.UnitInterpolation;
    }
}
