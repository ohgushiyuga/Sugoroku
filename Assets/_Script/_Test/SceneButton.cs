// ファイル名: SceneButton.cs
using UnityEngine;

public class SceneButton : MonoBehaviour
{
    /// <summary>
    /// 指定されたシーンを、ローディング画面を挟んで読み込む
    /// </summary>
    public void GoToSceneWithLoading(string sceneName)
    {
        // どこからでもアクセスできるSceneLoaderのインスタンスを呼び出す
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneWithLoading(sceneName);
        }
    }

    /// <summary>
    /// 指定されたシーンを、直接読み込む
    /// </summary>
    public void GoToSceneDirect(string StartScene)
    {
        // どこからでもアクセスできるSceneLoaderのインスタンスを呼び出す
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadSceneDirect(StartScene);
        }
    }
}