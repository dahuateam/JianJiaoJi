using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MaterialProp; //工作台是要用到保存材料道具类的对象的，所以引用它的命名空间
/// <summary>
/// 最近修改日期：2019.6.3
/// </summary>
public class CraftBench : MonoBehaviour
{
    public List<PropType> 可以放入的材料的清单;//定义该工作台可放入材料的配方，可以让策划在Inspector面板选择
    private PropType 产出材料;  //定义该工作台最终生成的材料的类型，

    public float 制作时间;    //定义了完成制作材料所需的时间，以秒为单位
    public Image 制作进度条;   //定义了完成制作材料的进度条,

    private Prop[] 放着的材料; //定义了当前放在工作台上的材料，最多两个,后面需要更大数量的配方的时候可以修改
    public SpriteRenderer[] showTexture;   //用于显示当前放在工作台上的材料的图片，

    private Animator miziAnimator;  //米子制造机的动画
    

    private float 已完成时间 = 0;   //定义了制作过程中已完成的时间

    //使用音效播放脚本
    private AudioManager 播放角色音效的脚本;

    public Material outlineMaterial;      //用于显示当玩家靠近该工作台时边缘高亮的材质
    private Material spriteDefault;     //保存台子默认的材质

    private void Awake()
    {
        放着的材料 = new Prop[showTexture.Length];  //初始化
        播放角色音效的脚本 = GameObject.Find("AudioManager").GetComponent<AudioManager>();


        //初始化默认材质
        spriteDefault = this.GetComponent<SpriteRenderer>().material;

        miziAnimator = this.GetComponent<Animator>();
        miziAnimator.enabled = false;
    }

    //当角色与工作台交互时使用该函数:包含的功能有取材料，放材料，是否可以开始制作材料
    public void UseCraftBench(out Prop playerProp, Prop playerPropInfo)
    {

        //进行取材料的函数，如果不能取则执行下一条函数/*2*/,判断能不能放入材料
        /*1*/
        if (Take_Material(playerPropInfo))
        {
            //1.如果可以取材料,获取玩家身上存放携带材料的变量
            //2.判断哪个位置上有材料将 放着的材料[i] 这个Prop对象赋值给玩家身上
            //2019.6.3更新了该循环
            for (int i = 0; i < 放着的材料.Length; i++)
            {
                if (放着的材料[i] != null)
                {
                    playerProp = 放着的材料[i];
                    放着的材料[i] = null;  //3.将这个材料取走后本地的材料就没了，赋值为空
                    播放角色音效的脚本.AudioPlay(AudioEvent.播放角色拾取材料时音效);   //播放音效
                    制作进度条.fillAmount = 0;    //拿走材料后让制作进度条归零
                    Show_Mat_Texture();
                    return;//拿到材料后结束寻找
                }
            }
        }

        //进行放材料的函数,如果放入成功则开始制作材料，即执行下一条函数/*3*/
        /*2*/
        if (!PutInMaterial(playerPropInfo))
        {
            //如果不能放入材料，则什么也不做
            playerProp = playerPropInfo;
        }
        else
        {
            //如果可以放入材料则角色身上的材料变成空
            播放角色音效的脚本.AudioPlay(AudioEvent.播放角色放下材料时音效);   //播放音效
            playerProp = null;

        }

        //判断是否开始制作材料，生成最终的材料
        /*3*/
        Manufacture();
    }

    //进行取材料的函数。2019.6.3更新了该函数
    private bool Take_Material(Prop playerPropInfo)
    {
        
          //如果有则判断玩家身上是否有空位，因为设计时玩家只能同时携带一个材料
          //同时还要判断该制作台有没有在制作，没有在制作就可以取材料
          if (playerPropInfo == null&&!IsInvoking("Manufacting"))
          {
                    return true;
          }
          else
          {
                    //如果玩家身上没有空位了则去判断能不能放入材料 
                    return false;
          }
        
    }
    //进行放材料的函数
    private bool PutInMaterial(Prop playerPropInfo)
    {
        //2.如果工作台没有最终生成的材料，则把玩家身上的材料放到工作台中。

        //2.1首先判断玩家身上是否有材料,同时判断玩家身上的材料的类型是不是本工作台配方所需要的,即配方集合是否包含该材料类型
        if (playerPropInfo != null/*玩家身上有材料*/&& 可以放入的材料的清单.Contains(playerPropInfo.PropType))
        {
            //2.2判断工作台上放着的材料的是否已经含有相同类型的材料,若有相同则不能放入

            //2.3判断工作台上放着的材料的哪个位置为空
            for (int i = 0; i < 放着的材料.Length; i++)
            {
                //判断工作台上放着的材料的是否已经含有相同类型的材料
                if (放着的材料[i] != null && 放着的材料[i].PropType == playerPropInfo.PropType/*玩家身上道具类型*/)
                {
                    //若含相同类型则不能放入
                    return false;
                }


                //2019.6.2更新1：因为木板和铁块无法匹配，因此当材料里有木板时，玩家无法放入铁块
                if (放着的材料[i] != null && 放着的材料[i].PropType == PropType.木板 && playerPropInfo.PropType == PropType.铁块)
                {
                    return false;
                }
                //2019.6.2更新2：因为木板和铁块无法匹配，因此当材料里有铁块时，玩家无法放入木板
                if (放着的材料[i] != null && 放着的材料[i].PropType == PropType.铁块 && playerPropInfo.PropType == PropType.木板)
                {
                    return false;
                }

                if (放着的材料[i] == null)
                {
                    //1.把玩家身上的道具放到这个位置上,
                    /*不会存在位置都不为空的情况，因为当玩家能把材料放进来放满，制作就已经开始了*/

                    放着的材料[i] = playerPropInfo;
                    Show_Mat_Texture();   //显示放入的材料的图片
                    //2.退出该函数后清空玩家身上的材料
                    return true;
                }
            }
            return false;
        }
        else
        {
            //若玩家身上没有材料或类型不符，则不能放入材料
            return false;

        }
    }
    //进行制作材料的函数
    private void Manufacture()
    {
        if (IsInvoking("Manufacting"))//如果该制作台正在制作则什么也不做
        {
            return;
        }

        //判断放着的所有材料是否符合配方的要求
        foreach (Prop p in 放着的材料)
        {
            if (p == null )
            {
                return;    //如果有空位则表示配方没有齐全，就退出制作
            }
        }

        
        //判断是否符合铁横杠的配方：钉子+铁块=铁横杠
        if (contains(PropType.钉子, PropType.铁块))
        {
            //如果符合则将产出材料设为铁横杠
            产出材料 = PropType.铁横杠;
            //材料齐全，可以开始制作了。
            InvokeRepeating("Manufacting", 0, Time.fixedDeltaTime);
            return;
        }
        

        //判断是否符合交叉木板的配方：钉子+木板=交叉木板
        if (contains(PropType.钉子, PropType.木板))
        {
            //如果符合则将产出材料设为交叉木板
            产出材料 = PropType.交叉木板;
            //材料齐全，可以开始制作了。
            InvokeRepeating("Manufacting", 0, Time.fixedDeltaTime);
            return;
        }
        

        //判断是否符合米字型防护的配方：交叉木板+铁横杠=米字型防护
        if (contains(PropType.交叉木板, PropType.铁横杠))
        {
            //如果符合则将产出材料设为铁横杠
            产出材料 = PropType.米字型防护;
            //材料齐全，可以开始制作了。
            InvokeRepeating("Manufacting", 0, Time.deltaTime);
            return;
        }
    }
    //判断放着的材料中是否包含指定材料
    private bool contains(PropType a1, PropType a2)
    {
        bool cf1 = false,cf2=false;  //信号量记录是否包含两种材料,cf1是否包含材料1，cf2是否包含材料2
        
        //判断是否包含第一种材料
        foreach (Prop p in 放着的材料)
        {
            if (p .PropType== a1)
            {
                cf1=true;    //找到第一种材料则退出寻找
                break;
            }
        }

        //判断是否包含第二种材料
        foreach (Prop p in 放着的材料)
        {
            if (p.PropType == a2)
            {
                cf2 = true;    //找到第二种材料则退出寻找
                break;    
            }
        }

        return (cf1&&cf2); //根据与运算算出两种材料都包含则返回真
    }

    //制作材料并显示生产进度的函数,每帧调用一次
    private void Manufacting()
    {
        已完成时间 += Time.deltaTime;
        制作进度条.fillAmount = (float)已完成时间 / 制作时间;

        miziAnimator.enabled = true;

        if (已完成时间 >= 制作时间)  //计算已制作的时间是否等于所需的制作时间
        {
            //清空制作台上所有的材料
            for (int i = 0; i < 放着的材料.Length; i++)
            {
                放着的材料[i] = null;
            }

            放着的材料[0] = new Prop(产出材料);   //生成新的材料
            Show_Mat_Texture();  //显示材料的图片
            已完成时间 = 0;
            StopManufacte();     //停止制作
            return;
        }
    }
    //用于停止制作材料的函数
    private void StopManufacte()
    {
        //如果正在制作材料，则停止制作
        if (IsInvoking("Manufacting"))
        {
            CancelInvoke("Manufacting");
        }

        miziAnimator.enabled = false;

    }
    //用于显示当前放在工作台上的材料的图片的图片
    private void Show_Mat_Texture()
    {
        for (int i = 0; i < showTexture.Length; i++)
        {
            if (放着的材料[i] != null)  //如果该位置材料不为空，则把材料显示在对应位置上
            {
                showTexture[i].sprite = 放着的材料[i].Sprite;
            }
            else
            {
                showTexture[i].sprite = null;
            }
        }
    }

    //如果玩家进入该工作台的可以操作范围则高亮显示该工作台
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().material = outlineMaterial;
            //Resources.Load("SpriteOutline", typeof(Material)) as Material;
        }
    }
    //如果玩家离开工作台则中断制作过程？ 离开则取消高亮显示
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            this.GetComponent<SpriteRenderer>().material = spriteDefault;
            StopManufacte();    //玩家离开制造台则暂停生产
        }
    }
}
