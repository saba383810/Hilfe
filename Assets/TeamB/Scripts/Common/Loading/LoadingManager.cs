using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static async UniTask LoadingScene(string sceneName)
    {
        var currentScene = SceneManager.GetSceneAt(1);
        // LoadingSceneを表示
        var loading = SceneManager.LoadSceneAsync("Loading",LoadSceneMode.Additive);
        await UniTask.WaitUntil(() => loading.isDone);
        var loadingSceneController = FindAnyObjectByType<LoadingSceneController>();
        loadingSceneController.Show();
        
        // 現在のSceneをUnload
        var currentSceneUnloadAsync = SceneManager.UnloadSceneAsync(currentScene);
        await UniTask.WaitUntil(() => currentSceneUnloadAsync.isDone);
        
        await UniTask.Delay(TimeSpan.FromSeconds(3));
        
        // 次のSceneをロード
        var asyncLoad = SceneManager.LoadSceneAsync(sceneName,LoadSceneMode.Additive);
        await UniTask.WaitUntil(() => asyncLoad.isDone);
        loadingSceneController.Hide();
        await UniTask.Delay(TimeSpan.FromSeconds(1.2f));
        SceneManager.UnloadSceneAsync("Loading");
    }
}
