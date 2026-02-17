using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreenController : MonoBehaviour
{
    void Start()
    {
        // 非同期でシーンを読み込むコルーチンを開始する
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        // 1. SceneLoaderから、次に読み込むべきシーンの名前を取得
        string sceneToLoad = SceneLoader.GetNextSceneName();

        // もしシーン名がなければ、処理を中断
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("読み込むシーン名が指定されていません！");
            yield break; // コルーチンを終了
        }

        // 2. 非同期でシーンの読み込みを開始
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        // 3. 読み込みが完了するまで待機
        // isDoneは読み込みと有効化の両方が完了したらtrueになる
        while (!asyncOperation.isDone)
        {
            // ここでローディングアニメーション（例：くるくる回るアイコンなど）を
            // 表示することもできます。
            
            yield return null; // 1フレーム待つ
        }
    }
}