using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialProp;

public class PlayerController : MonoBehaviour
{
	private SpriteRenderer sr1;   //用于根据人物移动方向，改变贴图方向改变
	public float moveSpeed = 5.0f; //角色移动速度，需要调整

	private Prop currentProp;   //用于保存当前角色所携带的材料的内容;
    private GameObject currentMoveEffect;  //保存当前角色身上的移动特效
	private bool canPutDown = true;    //判断玩家能否放下道具
	private Collider2D collision;     //保存玩家所触摸的对象


	public SpriteRenderer propTexture;   //作为角色的子物体，用于显示当前角色所携带的材料的图片
	public GameObject propPrefab;        //作为扔下材料的预制体
	public GameObject moveEffectPrefab;     //人物移动时产生的特效的预制体
	public BoxCollider2D myCollider;    //人物自身的碰撞体

	public KeyCode 向上移动按键;         //可以自定义玩家在各个方向移动的按键
	public KeyCode 向下移动按键;
	public KeyCode 向左移动按键;
	public KeyCode 向右移动按键;
	public KeyCode 互动按键;    //可以自定义玩家与操作台和门交互时的按键

	//使用音效播放脚本
	private AudioManager 播放角色音效的脚本;


	void Start()
	{
		sr1 = this.GetComponent<SpriteRenderer>();   //得到物品的贴图组件
		播放角色音效的脚本 = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}

	void Update()
	{
		PlayerMove();  //角色移动相关函数

		PlayerInterface();      //角色交互相关函数
	}

	//角色移动相关函数：
	private void PlayerMove()
	{
		//有一个按键按下则产生烟尘
		if (Input.GetKeyDown(向上移动按键) || Input.GetKeyDown(向下移动按键) || Input.GetKeyDown(向左移动按键) || Input.GetKeyDown(向右移动按键))
		{
            if (currentMoveEffect)
            {
                
            }  //判断是否已经存在特效
            else
            {
                currentMoveEffect =Instantiate(moveEffectPrefab, this.transform);//实例化一个特效
                Destroy(currentMoveEffect, 1);   //一秒钟后销毁这个特效
            }
			
		}

		//角色移动相关：
		float distance = Time.deltaTime * moveSpeed;
		Vector2 rayStart = new Vector2(this.transform.position.x + myCollider.offset.x * transform.localScale.x, this.transform.position.y + myCollider.offset.y * transform.localScale.y);
		if (Input.GetKey(向上移动按键))
		{
			if (!Physics2D.Raycast(rayStart, Vector2.up, myCollider.size.y / 2 * transform.localScale.x + distance))
			{
				transform.Translate(Vector3.up * distance, Space.World);
			}
		}
		else if (Input.GetKey(向下移动按键))
		{
			if (!Physics2D.Raycast(rayStart, Vector2.down, myCollider.size.y / 2 * transform.localScale.x + distance))
			{
				transform.Translate(Vector3.down * distance, Space.World);
			}
		}
		else if (Input.GetKey(向左移动按键))
		{
			sr1.flipX = false;
			if (!Physics2D.Raycast(rayStart, Vector2.left, myCollider.size.x / 2 * transform.localScale.x + distance))
			{
				transform.Translate(Vector3.left * distance, Space.World);
			}
		}
		else if (Input.GetKey(向右移动按键))
		{
			sr1.flipX = true;
			if (!Physics2D.Raycast(rayStart, Vector2.right, myCollider.size.x / 2 * transform.localScale.x + distance))
			{
				transform.Translate(Vector3.right * distance, Space.World);
			}
		}


	}
	//角色交互相关函数
	private void PlayerInterface()
	{
		//玩家交互相关
		if (Input.GetKeyDown(互动按键))  //检测按键被按下了，而不是一直在被按着，GetKey（）是按下过程中被触发多次，所以导致制作过程加快。
		{
			if (canPutDown) //判断玩家是否可以扔下材料或者与地上的材料进行交互
			{
				if (this.currentProp != null)  //判断玩家身上有没有材料可以扔下
				{
					//身上有材料则可以扔下材料

					//实例化生成一个材料在地上
					GameObject g = Instantiate(propPrefab, transform.position, transform.rotation);
					g.name = this.currentProp.PropName;

					g.GetComponent<SpriteRenderer>().sprite = this.currentProp.Sprite;

					this.currentProp = null;   //身上的材料变空
					ShowCurrentPropTexture();  //更新显示身上的材料
					播放角色音效的脚本.AudioPlay(AudioEvent.播放角色放下材料时音效);  //播放音效
				}
				else
				{
					if (collision && collision.gameObject.tag == "PropInFloor")  //判断触碰的物体是不是地上的材料
					{

						//捡起这个材料
						播放角色音效的脚本.AudioPlay(AudioEvent.播放角色拾取材料时音效);    //播放音效
						this.currentProp = new Prop(collision.gameObject.name);  //将地上材料的信息放到身上
						Destroy(collision.gameObject);     //销毁捡到的材料
						ShowCurrentPropTexture();   //更新显示身上的材料


					}
				}
			}
			else    //不可以扔下材料或者与地上的材料进行交互
			{
				//判断触碰的是不是原料台
				if (collision && collision.gameObject.tag == "RawBench")
				{
					collision.gameObject.GetComponent<RawBench>().UseRawBench(out currentProp, currentProp); //与原料台交互，获得原料
					ShowCurrentPropTexture();   //获得原料后显示角色身上携带的材料
				}

				//判断触碰的是不是加工台
				if (collision && collision.gameObject.tag == "WorkBench")
				{
					//与加工台交互，将身上用于保存当前角色所携带的材料的变量传给加工台;
					collision.gameObject.GetComponent<WorkBench>().UseWorkBench(out currentProp, currentProp);
					//获得原料后显示角色身上携带的材料
					ShowCurrentPropTexture();
				}

				//判断触碰的是不是制造台
				if (collision && collision.gameObject.tag == "CraftBench")
				{
					//与加工台交互，将身上用于保存当前角色所携带的材料的变量传给加工台;
					collision.gameObject.GetComponent<CraftBench>().UseCraftBench(out currentProp, currentProp);
					//获得原料后显示角色身上携带的材料
					ShowCurrentPropTexture();
				}

				//判断触碰的是不是门
				if (collision && collision.gameObject.tag == "Door")
				{
					//与门交互，将身上用于保存当前角色所携带的材料的变量传给门;
					collision.gameObject.GetComponent<Door>().UseDoor(out currentProp, currentProp);
					//更新显示角色身上携带的材料
					ShowCurrentPropTexture();
				}
			}
		}
	}

	//用于显示当前当前角色所携带材料的图片
	private void ShowCurrentPropTexture()
	{
		if (currentProp == null)  //与各种台交互可能会导致身上的材料没有了，所以需判断是否为空
		{
			propTexture.sprite = null;
		}
		else
		{
			propTexture.sprite = currentProp.Sprite;
		}
	}

	//角色与各种物体的交互
	private void OnTriggerStay2D(Collider2D collision)
	{

		this.collision = collision;

		if (collision.gameObject.tag == "PropInFloor")
		{
			collision.GetComponent<SpriteRenderer>().color = Color.yellow;
		}

		if (collision.gameObject.tag == "RawBench" || collision.gameObject.tag == "WorkBench" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "CraftBench")
		{
			canPutDown = false;
		}
	}

	//角色离开时不可交互
	private void OnTriggerExit2D(Collider2D collision)
	{
		this.collision = null;

		if (collision.gameObject.tag == "PropInFloor")
		{
			collision.GetComponent<SpriteRenderer>().color = Color.white;

		}

		if (collision.gameObject.tag == "RawBench" || collision.gameObject.tag == "WorkBench" || collision.gameObject.tag == "Door" || collision.gameObject.tag == "CraftBench")
		{
			canPutDown = true;
		}
	}
}
