using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerControl : MonoBehaviour
{
    [SerializeField]
    private Vector2[] 倒计时设置;   //设置倒计时时间，x为分，y为秒
    private int minute;        //记录倒计时的分针
    private int second;        //记录倒计时的秒针
    private int bout=1;        //记录当前是第几回合

    public Text timeText;     //用于显示倒计时的时间
    public RectTransform pausePanel;  //暂停面板
    public Image 游戏胜利横幅;
    public Image 回合提示横幅;     //用于显示回合攻击提示
    [Tooltip("预存每一回合用于显示回合攻击提示的图片集")]
    public Sprite[] boutSprite;      //用于显示回合攻击提示的图片
    //初始化
    void Start()
    {
        Bout(bout); //开始第一回合的计时
    }
    
    //用于每回合调用的内容
    void Bout(int bout)
    {
        if (bout > 倒计时设置.Length)//判断所有回合是否都已经执行了
        {
            //判断游戏结束，所有回合结束,显示出胜利横幅
            游戏胜利横幅.gameObject.SetActive(true);
            TimeScale();
            return;
        }

        bout -= 1;

        回合提示横幅.sprite = this.boutSprite[bout];  //获取当前回合攻击提示的图片
        回合提示横幅.enabled=true;  //显示当前回合攻击提示的图片

        StartCoroutine(CloseTexture());  //3秒后关闭当前回合攻击提示的图片

        minute = (int)倒计时设置[bout].x;
        second = (int)倒计时设置[bout].y;

        int allTime = minute * 60 + second;   //记录总秒数

        //开始倒计时
        InvokeRepeating("CountDownDisplay", 0, 1);    //一秒钟调用一次
        Invoke("NextBout", allTime + 1);    //多少秒后切换到下一个回合
    }

    IEnumerator CloseTexture()
    {
        yield return new WaitForSeconds(2);//多少秒后关闭当前回合攻击提示的图片
        回合提示横幅.enabled = false;  
    }
    //每一秒后显示当前的时间时调用
    private void CountDownDisplay()
    {
        timeText.text = "";

        if (minute < 10)
        {
            timeText.text += "0" + minute.ToString();
        }
        else
        {
            timeText.text += minute.ToString();
        }

        timeText.text += ":";   //显示分和秒中间的冒号

        if (second < 10)
        {
            timeText.text += "0" + second.ToString();
        }
        else
        {
            timeText.text += second.ToString();
        }
        
        if (second == 0)
        {
            minute--;
            second = 59;
        }
        else
        {
            second--;
        }
        
    }

    //切换到下一个回合
    private void NextBout()
    {
        CancelInvoke("CountDownDisplay"); //停止显示倒计时
        this.bout++; //回合数加一，进入到下一回合的倒计时
        Bout(this.bout);
    }

    //Time.timeScale等于0时FixedUpdate事件函数是不会执行的
    private void Update()
    {
        //如果没有暂停面板则不能执行暂停功能
        if (pausePanel == null)
        {
            return;
        }
        //用于暂停游戏进程
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausePanel.gameObject.activeSelf == false)
            {
                pausePanel.gameObject.SetActive(true);
            }
            else
            {
                pausePanel.gameObject.SetActive(false);
            }

            TimeScale();
        }
    }
    
    //用于暂停游戏或者恢复游戏不再暂停
    public void TimeScale()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;  //暂停游戏
        }
        else
        {
            Time.timeScale = 1;  //恢复游戏
        }
    }
}
