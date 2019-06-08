using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoFinish : MonoBehaviour
{
     
    void Start()
    {
        VideoPlayer vp = this.GetComponent<VideoPlayer>();  //获取视频播放器组件
        
        VideoPlayer.EventHandler e = new VideoPlayer.EventHandler(Finish);   //建立一个事件触发委托
        
        vp.loopPointReached += e;    //把这个委托添加到一遍循环结束后（到达了循环点）事件触发就触发这个我们的委托
    }
    
    //触发委托调用的函数
    void Finish(VideoPlayer vp)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("游戏主界面");  //切换到游戏主界面场景
    }
    
}
