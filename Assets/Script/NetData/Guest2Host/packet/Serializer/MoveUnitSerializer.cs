
public class MoveUnitSerializer : Serializer {

	public bool Serialize(MoveUnitData data)
	{
		bool ret = true;
		ret &= Serialize (data.playerNum);
		ret &= Serialize (data.Id);
		ret &= Serialize (data.posX);
		ret &= Serialize (data.posY);
		ret &= Serialize (data.posZ);

		return ret;
	}

	public bool Deserialize(ref MoveUnitData element)
	{
		if (GetDataSize() == 0)
		{
			// 데이터가 설정되지 않았다.
			return false;
		}

		bool ret = true;

		byte playerNum = 0;
		byte Id = 0;
		float posX = 0;
		float posY = 0;
		float posZ = 0;

		ret &= Deserialize (ref playerNum);
		ret &= Deserialize (ref Id);
		ret &= Deserialize (ref posX);
		ret &= Deserialize (ref posY);
		ret &= Deserialize (ref posZ);

		element.playerNum = playerNum;
		element.Id = Id;
		element.posX = posX;
		element.posY = posY;
		element.posZ = posZ;

		return ret;
	}
}
