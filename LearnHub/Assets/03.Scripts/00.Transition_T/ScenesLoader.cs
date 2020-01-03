using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading;

public class ScenesLoader : MonoBehaviour {

    public GameObject loadingScreen;
    public Slider slider;
    public Text progressText;

    protected virtual void Start() {
        
    }

    protected virtual void Loadlevel(int sceneIndex) {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    protected virtual IEnumerator LoadAsynchronously(int sceneIndex) {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);

        while (!operation.isDone) {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            Display(progress);
            Debug.Log(operation.progress);
            Thread.Sleep(100);
            yield return null;
        }
    }

    protected virtual void Display(float progress) {
        slider.value = progress;
        progressText.text = "Loading " + progress * 100f + "%";
    }

}
