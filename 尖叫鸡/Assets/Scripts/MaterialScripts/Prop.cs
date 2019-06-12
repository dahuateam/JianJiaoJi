using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1.当制作台类和人物类需要用到材料和获取材料里面的图片时，使用这个命名空间。
/// 然后实例化它的类，从实例化对象中读取它的名字和图片，类型
/// </summary>
namespace MaterialProp
{   
    //用枚举定义了材料道具的类型
    public enum PropType
    {
          铁块, //铁块
          原木   ,   //原木
          钉子   ,   //钉子
          木板,     //木板
          交叉木板 ,   //交叉木板
          米字型防护     ,  //米字型防护
    }
    
    //该类用于实例化一个材料道具，它的成员保存了一个材料道具所拥有的信息
    public class Prop
    {
        //定义了该材料道具的类型，通过它的只读属性访问
        private PropType propType;
        public PropType PropType
        {
            get { return propType; }
        }
        
        //定义了该材料道具的名字，用中文赋值。通过它的只读属性访问
        private string propName;
        public string PropName
        {
            get { return propName; }
        }

        //定义了该材料道具的图片，当需要显示这个材料道具的ui时，可以使用这个成员
        private Sprite sprite;
        public Sprite Sprite
        {
            get { return sprite; }
        }
        
        //构造函数，传入你想生成的道具的类型，然后生成该道具。
        public Prop(PropType type)
        {
            this.propType = type;  //类型赋值
            
            //根据类型生成道具的中文名字和它的图片
            switch(type)
            {
                case PropType.铁块:
                    ResourceLoad("铁块UI");
                    break;
                case PropType.原木:
                    ResourceLoad("原木UI");
                    break;
                case PropType.钉子:
                    ResourceLoad("钉子UI");
                    break;
                case PropType.木板:
                    ResourceLoad("木板UI");
                    break;
                case PropType.交叉木板:
                    ResourceLoad("交叉木板UI");
                    break;
                case PropType.米字型防护:
                    ResourceLoad("米字型防护UI");
                    break;
                default:
                    Debug.Log("没有定义你需要的材料道具！");
                    break;
            }
        }
        //构造函数，传入你想生成的道具的中文名字，然后生成该道具。
        public Prop(string chineseName)
        {
            //根据中文名字生成道具的中文名字和它的图片
            ResourceLoad(chineseName);
            
            //根据中文名字生成道具的类型
            switch (chineseName)
            {
                
                case "铁块UI":
                    this.propType = PropType.铁块;
                    break;
                case "原木UI":
                    this.propType = PropType.原木;
                    break;
                case "钉子UI":
                    this.propType = PropType.钉子;
                    break;
                case "木板UI":
                    this.propType = PropType.木板;
                    break;
                case "交叉木板UI":
                    this.propType = PropType.交叉木板;
                    break;
                case "米字型防护UI":
                    this.propType = PropType.米字型防护;
                    break;

                default:
                    Debug.Log("没有定义你需要的材料道具!");
                    break;
            }
        }


        //对该材料道具进行命名，然后加载获取它的图片
        private void ResourceLoad(string name)
        {
            this.propName = name;
            Texture2D tex= (Texture2D)Resources.Load(name);//从resource文件夹下读取图片资源texture2d

            //将texture2d图片转换为sprite类型的图片
            this.sprite= Sprite.Create(tex,new Rect(0, 0,tex.width ,tex.height),new Vector2(0.5f,0.5f));
        }     
    }
}

