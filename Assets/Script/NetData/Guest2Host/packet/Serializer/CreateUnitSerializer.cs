
public class CreateUnitSerializer : Serializer {

	public bool Serialize(CreateUnitData data)
	{
		bool ret = true;
		ret &= Serialize (data.playerNum);
		ret &= Serialize (data.Id);
		ret &= Serialize (data.unitName);
		ret &= Serialize (data.posX);
		ret &= Serialize (data.posY);
		ret &= Serialize (data.posZ);
		ret &= Serialize (data.level);

		return ret;
	}

	public bool Deserialize(ref CreateUnitData element)
	{
		if (GetDataSize() == 0)
		{
			// 데이터가 설정되지 않았다.
			return false;
		}

		bool ret = true;

		byte playerNum = 0;
		byte Id = 0;
		string unitName = "";
		float posX = 0;
		float posY = 0;
		float posZ = 0;
		byte level = 0;

		ret &= Deserialize (ref playerNum);
		ret &= Deserialize (ref Id);
		ret &= Deserialize(out unitName, (int) GetDataSize() - 22);
		ret &= Deserialize (ref posX);
		ret &= Deserialize (ref posY);
		ret &= Deserialize (ref posZ);
		ret &= Deserialize (ref level);

		element.playerNum = playerNum;
		element.Id = Id;
		element.unitName = unitName;
		element.posX = posX;
		element.posY = posY;
		element.posZ = posZ;
		element.level = level;

		return ret;
	}
}
