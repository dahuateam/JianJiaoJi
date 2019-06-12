using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoFinish : MonoBehaviour
{
    public Image panel;
    [Tooltip("视频播放完多少秒后切换场景")]
    public int ChangeTime;
    VideoPlayer vp;
    void Start()
    {
        vp = this.GetComponent<VideoPlayer>();  //获取视频播放器组件
        
        VideoPlayer.EventHandler e = new VideoPlayer.EventHandler(Finish);   //建立一个事件触发委托
        
        vp.loopPointReached += e;    //把这个委托添加到一遍循环结束后（到达了循环点）事件触发就触发这个我们的委托
    }
    
    //触发委托调用的函数
    void Finish(VideoPlayer vp)
    {
        panel.enabled=true;
        Invoke("ChangeScene", ChangeTime);
        //UnityEngine.SceneManagement.SceneManager.LoadScene("游戏主界面");  //切换到游戏主界面场景
    }

    private void Update()
    {
        if (Input.anyKeyDown)   //按下任意按键则跳转场景
        {
            ChangeScene();
        }
    }

    //切换到游戏主场景的函数
    private void ChangeScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("游戏主界面");  //切换到游戏主界面场景
    }
}
