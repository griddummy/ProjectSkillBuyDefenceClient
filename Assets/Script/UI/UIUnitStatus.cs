using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//unit status projection
//UIcontrol -> presentUnit
public class UIUnitStatus : MonoBehaviour
{
	[SerializeField] Text unitName;
	[SerializeField] Text unitLevel;
	[SerializeField] Image healthPointBar;
	[SerializeField] Text healthPoint;
	[SerializeField] Image manaPointBar;
	[SerializeField] Text manaPoint;
	[SerializeField] Image attackSpeedIcon;
	[SerializeField] Text attackSpeed;
	[SerializeField] Image moveSpeedIcon;
	[SerializeField] Text moveSpeed;
	[SerializeField] Image attackType;
	[SerializeField] Text damage;

	//link component
	void LinkComponent()
	{

	}

	//public method
	public void RestComponent()
	{
		unitName.enabled = false;
		unitLevel.enabled = false;
		healthPointBar.enabled = false;
		healthPoint.enabled = false;
		manaPointBar.enabled = false;
		manaPoint.enabled = false;
		attackSpeedIcon.enabled = false;
		attackSpeed.enabled = false;
		moveSpeedIcon.enabled = false;
		moveSpeed.enabled = false;
		attackType.enabled = false;
		damage.enabled = false;
	}

	public void ActiveComponent()
	{
		unitName.enabled = true;
		unitLevel.enabled = true;
		healthPointBar.enabled = true;
		healthPoint.enabled = true;
		manaPointBar.enabled = true;
		manaPoint.enabled = true;
		attackSpeedIcon.enabled = true;
		attackSpeed.enabled = true;
		moveSpeedIcon.enabled = true;
		moveSpeed.enabled = true;
		attackType.enabled = true;
		damage.enabled = true;
	}

	//unit status update ->
	public void UpdateStatus( UnitProcess data )
	{
		unitName.text = data.Info.Name;
		unitLevel.text = "LV." + data.Info.Level.ToString();
		healthPointBar.fillAmount = data.Info.PresentHealthPoint / data.Info.HealthPoint;
		healthPoint.text = ( (int) data.Info.PresentHealthPoint ).ToString() + " / " + ( (int) data.Info.HealthPoint ).ToString();
		manaPointBar.fillAmount = data.Info.PresentManaPoint / data.Info.ManaPoint;
		manaPoint.text = ( (int) data.Info.PresentManaPoint ).ToString() + " / " + ( (int) data.Info.ManaPoint ).ToString();
		attackSpeed.text = data.Info.AttackSpeed.ToString();
		moveSpeed.text = data.Info.MoveSpeed.ToString();
		damage.text = data.Info.Damage.ToString();

		if (data.Info.IsMelee)
			attackType.sprite = Resources.Load<Sprite>( "Status/AttackTypeMelee" );
		else
			attackType.sprite = Resources.Load<Sprite>( "Status/AttackTypeRange" );
	}

}
