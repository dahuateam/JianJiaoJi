using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MaterialProp;

public class RawBench : MonoBehaviour
{

    public PropType 产出材料;  //定义该原料台生成的材料的类型，可以让策划在Inspector面板选择
    
    //使用音效播放脚本
    private AudioManager 播放角色音效的脚本;

    public Material 边缘高亮材质;      //用于显示当玩家靠近该工作台时边缘高亮的材质
    private Material spriteDefault;     //保存台子默认的材质

    private void Start()
    {
        播放角色音效的脚本 = GameObject.Find("AudioManager").GetComponent<AudioManager>();

        //初始化默认材质
        spriteDefault = this.GetComponent<SpriteRenderer>().material;
    }
    //当角色与工作台交互时使用该函数:包含的功能有取材料，放材料，是否可以开始制作材料
    public void UseRawBench(out Prop playerProp, Prop xinxi)
    {

        //判断玩家身上是否有空位，因为设计时玩家只能同时携带一个材料
        if (xinxi == null/*玩家身上有空位*/)
        {
            Prop prop = new Prop(产出材料);
            playerProp = prop;
            播放角色音效的脚本.AudioPlay(AudioEvent.播放角色拾取材料时音效);   //播放音效
        }
        else
        {
            playerProp = xinxi;
            //如果玩家身上没有空位了则什么也不做或者给予一些提示
            //return; //4.然后本次交互就结束了
        }
    }

    //如果玩家进入该工作台的可以操作范围则高亮显示该工作台
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().material = 边缘高亮材质;
            //Resources.Load("SpriteOutline", typeof(Material)) as Material;
        }
    }
    //如果玩家离开工作台则中断制作过程？ 离开则取消高亮显示
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().material = spriteDefault;
        }
    }
}
