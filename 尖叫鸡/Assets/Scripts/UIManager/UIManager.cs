using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //用于加载跳转到指定名字的场景
    public void SelectSeceneLoad(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    //用于退出游戏
    public void QuitGame()
    {
         #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
         #else
		    Application.Quit();
         #endif
    }



}
