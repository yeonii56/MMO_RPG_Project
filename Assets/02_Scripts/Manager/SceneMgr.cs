using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class SceneMgr : Singleton<SceneMgr>
{
    private AsyncOperationHandle _sceneHandle;

    // 타이틀씬 호출
    public void LoadTitleScene()
    {
        Action callback = () => SceneManager.LoadSceneAsync("TitleScene");
        AddressableMgr.Instance.StartDefaultAssetLoad(callback);        
    }

    // 메인씬 호출
    public async void LoadMainScene()
    {
        await SceneManager.LoadSceneAsync("MainScene");
    }

    // 던전 씬 등 호출
    public async void LoadScene(string sceneName)
    {
        _sceneHandle = Addressables.LoadSceneAsync(sceneName);
        await _sceneHandle.Task;
    }

    // 던전 씬 등 언로드 (메모리 해제)
    public void UnLoadScene(string name)
    {
        Addressables.UnloadSceneAsync(_sceneHandle);
    }
}
