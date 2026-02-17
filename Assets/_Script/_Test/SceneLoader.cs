// ファイル名: SceneLoader.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// シーン遷移を専門に管理するスクリプト。
/// ローディング画面を挟む場合と、挟まない場合の両方に対応する。
public class SceneLoader : MonoBehaviour
{
    // --- シングルトンパターン：どこからでもアクセスできるようにする ---
    public static SceneLoader Instance { get; private set; }

    // 次に読み込むべきシーンの名前を、ゲーム全体で共有するための変数
    private static string nextSceneName;

    void Start()
    {
        DontDestroyOnLoad(this);
    } 

    void Awake()
    {
        // シーン内にSceneLoaderが1つしか存在しないように保証する
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも破棄されないようにする
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- ローディング画面を挟む場合 ---

    /// 指定された名前のシーンを、「ローディング画面を挟んで」読み込む
    public void LoadSceneWithLoading(string TestScene)
    {
        nextSceneName = TestScene;
        SceneManager.LoadScene("LoadingScene"); // ローディング専用シーンの名前
    }

    /// LoadingScreenControllerが、次に読み込むべきシーン名を取得するための関数
    public static string GetNextSceneName()
    {
        return nextSceneName;
    }

    // --- ローディング画面を挟まない場合 ---

    /// 指定された名前のシーンを、「直接」読み込む
    public void LoadSceneDirect(string sceneName)
    {
        Debug.Log("SceneLoader: " + sceneName + " という名前のシーンを直接読み込みます！");
        SceneManager.LoadScene(sceneName);
    }

    /// 指定された秒数待ってから、「直接」シーンを読み込む
    public void LoadSceneDirectAfterDelay(string sceneName, float delay)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName, delay));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
    
}