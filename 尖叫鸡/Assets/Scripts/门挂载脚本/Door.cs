using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using MaterialProp;//脚本需要用到Prop等类型
using System;

/// <summary>
/// 表示门事件类型的枚举
/// </summary>
public enum DoorEventType
{
	IDLE,//IDLE类型,闲置等待
	BUSY//BUSY类型,提出需求
}

/// <summary>
/// 表示门的需求达成后发生的行为的枚举
/// </summary>
public enum DemandCompletedAction
{
	NEXT_RIGHT_NOW,//立即执行下一事件
	IDLE_LEFT_TIME,//在需求剩余的时间内闲置
	SKIP_NEXT,//跳过下一个事件执行下下个事件
}

/// <summary>
/// 表示门的需求失败后发生的行为的枚举
/// </summary>
public enum DemandFailedAction
{
	NEXT_RIGHT_NOW,//立即执行下一事件
	DEMAND_AGAIN,//再次提出相同的需求
	SKIP_NEXT,//跳过下一个事件执行下下个事件
}

/// <summary>
/// 门事件的结构
/// </summary>
[Serializable]
public class DoorEvent
{
	public DoorEventType eventType;//门事件类型

	//IDLE类型的配置数据
	public float idleTime;//闲置的时间,单位秒

	//BUSY类型的配置数据
	public PropType propType;//需求材料的种类
	public float busyTime;//需求存在的时间,单位秒
	public DemandCompletedAction completedAction;
	public DemandFailedAction failedAction;
}



public class Door : MonoBehaviour
{
	//可配置属性
	[SerializeField, Range(2, 5)]
	private int maxHealth = 3;//门的最大生命值
	[SerializeField]
	private List<DoorEvent> eventList;//门的事件列表	


	//拖动赋值组件
	[SerializeField]
	private SpriteRenderer[] healthDisplays;
	[SerializeField]
	private SpriteRenderer propDisplay;
	[SerializeField]
	private Image handleDisplay;
	[SerializeField]
	private GameManagerControl m_GameManagerControl;
	[SerializeField]
	private GameObject gameFailedWindow;
	[SerializeField]
	private Material 边缘高亮材质;      //用于显示当玩家靠近该工作台时边缘高亮的材质
	[SerializeField]
	private SpriteRenderer doorDisplay;
	[SerializeField]
	private Sprite spriteGood;
	[SerializeField]
	private Sprite spriteBad;



	//私有属性
	private int currentEventIndex = 0;//当前执行事件在列表中的下标
	private float timer = 0;//计时器
	private bool isLeftTimeIdle = false;//此时的idle状态是否为IDLE_LEFT_TIME导致的
	private bool isListEnd = false;//事件列表是否已经执行完毕
	private int currentHealth;//门的当前生命值
	private int CurrentHealth//门的当前生命值对应的属性
	{
		get { return currentHealth; }
		set
		{
			currentHealth = value;
			//通过属性修改门的生命值时更新health的显示
			int i;
			for (i = 0; i < currentHealth; i++)
			{
				healthDisplays[i].enabled = true;
			}
			for (; i < 5; i++)
			{
				healthDisplays[i].enabled = false;
			}
			//更改门的显示sprite
			if (currentHealth <= 1)
			{
				doorDisplay.sprite = spriteBad;//门血量小于1,显示坏门素材
			}
			else
			{
				doorDisplay.sprite = spriteGood;//否则显示好门素材
			}
			//判断游戏是否结束
			if (currentHealth <= 0)
			{
				gameFailedWindow.SetActive(true);
				m_GameManagerControl.TimeScale();
			}
		}
	}
	private Material spriteDefault;     //保存台子默认的材质

	void Start()
	{
		CurrentHealth = maxHealth;//根据maxHealth初始化生命值
		changeEvent(0);//开始第一个事件

		spriteDefault = this.GetComponent<SpriteRenderer>().material;//初始化默认材质
	}

	void Update()
	{
		if (isListEnd) return;//若事件列表已执行完毕则返回
							  //更新计时器
		if (timer > 0.0f) timer -= Time.deltaTime;
		//若计时结束
		if (timer <= 0.0f)
		{
			if (eventList[currentEventIndex].eventType == DoorEventType.IDLE)//IDLE事件计时结束
			{
				changeEvent(currentEventIndex + 1);//执行下一事件
			}
			else if (eventList[currentEventIndex].eventType == DoorEventType.BUSY)//BUSY事件计时结束
			{
				if (isLeftTimeIdle)//IDLE_LEFT_TIME结束
				{
					changeEvent(currentEventIndex + 1);//执行下一事件
				}
				else//时间耗尽,且需求未完成
				{
					demandFailed();//需求失败
				}
			}
		}
		//根据timer更新进度条的显示
		if (eventList[currentEventIndex].eventType == DoorEventType.BUSY)
		{
			handleDisplay.fillAmount = timer / eventList[currentEventIndex].busyTime;
		}

	}

	/// <summary>
	/// 传入事件在列表中的下标,切换当前执行的事件
	/// </summary>
	/// <param name="index"></param>
	private void changeEvent(int index)
	{
		if (index >= eventList.Count)//如果传入了超出范围的index,代表事件列表执行至尽头
		{
			propDisplay.sprite = null;//关闭需求材料的显示
			handleDisplay.enabled = false;//关闭进度条的显示
			isListEnd = true;
			return;
		}
		currentEventIndex = index;
		switch (eventList[currentEventIndex].eventType)
		{
			case DoorEventType.IDLE://切换至IDLE事件
				propDisplay.sprite = null;//关闭需求材料的显示
				handleDisplay.enabled = false;//关闭进度条的显示
				timer = eventList[currentEventIndex].idleTime;//设置timer
				break;
			case DoorEventType.BUSY:
				propDisplay.sprite = new Prop(eventList[currentEventIndex].propType).Sprite;//显示需求的材料
				handleDisplay.enabled = true;//开启进度条的显示
				timer = eventList[currentEventIndex].busyTime;//设置timer
				isLeftTimeIdle = false;
				break;
		}
	}

	/// <summary>
	/// 需求达成函数
	/// </summary>
	private void demandFailed()
	{
		CurrentHealth = CurrentHealth - 1;//扣除生命值
		switch (eventList[currentEventIndex].failedAction)//根据failedAction做出相应的失败行为
		{
			case DemandFailedAction.DEMAND_AGAIN:
				timer = eventList[currentEventIndex].busyTime;//重置计时器
				break;
			case DemandFailedAction.NEXT_RIGHT_NOW:
				changeEvent(currentEventIndex + 1);//执行下一事件
				break;
			case DemandFailedAction.SKIP_NEXT:
				changeEvent(currentEventIndex + 2);//执行下下个事件
				break;
			default:
				changeEvent(currentEventIndex + 1);//执行下一事件
				break;
		}
	}

	/// <summary>
	/// 需求失败函数
	/// </summary>
	private void demandCompleted()
	{
		switch (eventList[currentEventIndex].completedAction)//根据completedAction做出相应的失败行为
		{
			case DemandCompletedAction.IDLE_LEFT_TIME:
				isLeftTimeIdle = true;
				propDisplay.sprite = null;//关闭需求材料的显示
				handleDisplay.enabled = false;//关闭进度条的显示
				break;
			case DemandCompletedAction.NEXT_RIGHT_NOW:
				changeEvent(currentEventIndex + 1);//执行下一事件
				break;
			case DemandCompletedAction.SKIP_NEXT:
				changeEvent(currentEventIndex + 2);//执行下下个事件
				break;
			default:
				changeEvent(currentEventIndex + 1);//执行下一事件
				break;
		}
	}

	//如果玩家进入该门的可以操作范围则高亮显示该门
	private void OnTriggerStay2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			this.GetComponent<SpriteRenderer>().material = 边缘高亮材质;
			//Resources.Load("SpriteOutline", typeof(Material)) as Material;
		}
	}
	//如果玩家离开门则取消高亮显示
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Player")
		{
			this.GetComponent<SpriteRenderer>().material = spriteDefault;
		}
	}

	/// <summary>
	/// 供角色调用的交互函数
	/// </summary>
	public void UseDoor(out Prop currentProp, Prop playerPropInfo)
	{
		if (!isListEnd && eventList[currentEventIndex].eventType == DoorEventType.BUSY && !isLeftTimeIdle)//判断交互条件
		{
			if (playerPropInfo != null && playerPropInfo.PropType == eventList[currentEventIndex].propType)//判断玩家身上是否携带有正确的材料
			{
				currentProp = null;//清空玩家身上的材料
				demandCompleted();//需求达成
			}
			else//玩家身上没有携带正确的材料
			{
				currentProp = playerPropInfo;//什么也不做
			}
		}
		else//不满足交互条件
		{
			currentProp = playerPropInfo;//什么也不做
		}
	}

}










// 自定义的Inspector布局,用于配置门的事件和属性

[CustomPropertyDrawer(typeof(DoorEvent))]
public class DoorEventDrawer : PropertyDrawer
{
	private float cutOff = 20.0f;//缩进值
	private float rowHeight = 18.0f;//行高

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, rowHeight);
		var Rect1 = new Rect(position.x + cutOff, position.y + rowHeight * 1, position.width - cutOff, rowHeight);
		var Rect2 = new Rect(position.x + cutOff, position.y + rowHeight * 2, position.width - cutOff, rowHeight);
		var Rect3 = new Rect(position.x + cutOff, position.y + rowHeight * 3, position.width - cutOff, rowHeight);
		var Rect4 = new Rect(position.x + cutOff, position.y + rowHeight * 4, position.width - cutOff, rowHeight);
		var Rect5 = new Rect(position.x + cutOff, position.y + rowHeight * 5, position.width - cutOff, rowHeight);

		EditorGUI.LabelField(labelRect, label);
		EditorGUI.PropertyField(Rect1, property.FindPropertyRelative("eventType"), new GUIContent("事件类型"));

		switch (property.FindPropertyRelative("eventType").enumValueIndex)
		{
			case 0://IDLE类型
				EditorGUI.PropertyField(Rect2, property.FindPropertyRelative("idleTime"), new GUIContent("闲置时间(秒)"));
				break;
			case 1://BUSY类型
				EditorGUI.PropertyField(Rect2, property.FindPropertyRelative("propType"), new GUIContent("需求材料"));
				EditorGUI.PropertyField(Rect3, property.FindPropertyRelative("busyTime"), new GUIContent("需求持续时间(秒)"));
				EditorGUI.PropertyField(Rect4, property.FindPropertyRelative("completedAction"), new GUIContent("需求达成"));
				EditorGUI.PropertyField(Rect5, property.FindPropertyRelative("failedAction"), new GUIContent("需求失败"));
				break;
		}
	}

	//修改属性占用的高度
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		switch (property.FindPropertyRelative("eventType").enumValueIndex)
		{
			case 0://IDLE类型
				return rowHeight * 3;
			case 1://BUSY类型
				return rowHeight * 6;
			default:
				return rowHeight * 6;
		}
	}
}

