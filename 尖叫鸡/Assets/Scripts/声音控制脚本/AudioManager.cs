using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//定义了播放各种音效时的类型
public enum AudioEvent
{
    播放角色拾取材料时音效,
    播放角色放下材料时音效,
}

public class AudioManager : MonoBehaviour
{
    private static float  soundEffectVolume=1;  //记录音效的音量
    private static float  BGMVolume = 1;  //记录背景音乐的音量

    public AudioSource 音效播放器;         //用于播放背景音乐的组件
    public AudioSource 背景音乐播放器;     

    public Slider soundEffectSlider;        //用于调节音量的滑条
    public Slider BGMSlider;

    public AudioClip 角色拾取材料时音效;
    public AudioClip 角色放下材料时音效;
    //初始化各音量条和各音乐播放器AudioSource的音量
    void Awake()
    {
        //初始化各音量条和各音乐播放器AudioSource的音量
        音效播放器.volume = soundEffectVolume;
        背景音乐播放器.volume = BGMVolume;

        soundEffectSlider.value = soundEffectVolume;
        BGMSlider.value = BGMVolume;
    }

    //实时更改调节音效音量的滑条和音效播放器的音量数值一致
    public void ChangeEffectVolume()
    {
        soundEffectVolume = soundEffectSlider.value;
    }
    //实时更改调节bgm音量的滑条和bgm播放器的音量数值一致
    public void ChangeBGMVolume()
    {
        BGMVolume = BGMSlider.value;
    }

    //定义了播放各种音效的公开函数，可供其他脚本调用
    public void AudioPlay(AudioEvent eventType)
    {
        switch (eventType)
        {
            case AudioEvent.播放角色拾取材料时音效:
                音效播放器.PlayOneShot(角色拾取材料时音效);break;

            case AudioEvent.播放角色放下材料时音效:
                音效播放器.PlayOneShot(角色放下材料时音效);break;
        }
    }

}
