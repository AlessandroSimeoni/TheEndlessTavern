using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLoad
{
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader _instance = null;
        public static SceneLoader instance 
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("Scene loader");
                    _instance = go.AddComponent<SceneLoader>();
                    DontDestroyOnLoad(go);
                }

                return _instance;
            }
        }

        private TransitionManager transitionManager = null;
        public bool loading = false;
        private const string TRANSITION_SCENE_NAME = @"Scenes/TransitionScene";
        private string targetSceneName = "";
        private Scene unloadScene;

        public delegate void LoadingEvent();
        public event LoadingEvent OnTargetSceneReady = null;
        public event LoadingEvent OnLoadingCompleted = null;

        public void LoadScene(string scene)
        {
            if (loading)
                return;

            targetSceneName = scene;
            loading = true;
            StartCoroutine(LoadTransitionScene());
        }

        private IEnumerator LoadTransitionScene()
        {
            // load the transition scene
            AsyncOperation transitionLoad = SceneManager.LoadSceneAsync(TRANSITION_SCENE_NAME, LoadSceneMode.Additive);
            transitionLoad.allowSceneActivation = false;
            while (transitionLoad.progress < 0.9f)
                yield return null;
            transitionLoad.allowSceneActivation = true;

            // cache the scene to unload
            unloadScene = SceneManager.GetActiveScene();

            // make the transition scene the active one
            Scene transitionScene = SceneManager.GetSceneByName(TRANSITION_SCENE_NAME);
            while (!transitionScene.isLoaded)
                yield return null;
            SceneManager.SetActiveScene(transitionScene);

            //Start transition animation
            transitionManager = FindAnyObjectByType<TransitionManager>();
            transitionManager.OnFirstAnimationDone += HandleFirstAnimationTransition;
            transitionManager.StartingTransition();
        }

        /// <summary>
        /// called when the first animation transition is over
        /// </summary>
        private void HandleFirstAnimationTransition() => StartCoroutine(SwitchScenes());

        private IEnumerator SwitchScenes()
        {
            // to play the transition sfx avoiding double audio listeners in scene:
            // - disable the main camera audio listener
            // - enable the audio listener of the camera in the transition scene
            Camera.main.GetComponent<AudioListener>().enabled = false;
            transitionManager.audioListener.enabled = true;

            // unload previous scene
            AsyncOperation unload = SceneManager.UnloadSceneAsync(unloadScene);
            while (unload.progress < 1.0f)
                yield return null;

            /*
            //garbage collector
            GC.Collect();
            yield return null;
             */

            // load target scene
            AsyncOperation load = SceneManager.LoadSceneAsync(targetSceneName, LoadSceneMode.Additive);
            load.allowSceneActivation = false;
            while (load.progress < 0.9f)
                yield return null;
            load.allowSceneActivation = true;

            // wait for the target scene to load
            Scene targetScene = SceneManager.GetSceneByName(targetSceneName);
            while (!targetScene.isLoaded)
                yield return null;

            // switch audio listener
            transitionManager.audioListener.enabled = false;
            Camera.main.GetComponent<AudioListener>().enabled = true;

            // set the target scene as the active one
            SceneManager.SetActiveScene(targetScene);
            OnTargetSceneReady?.Invoke();       // scene is ready here --> minigames can be initialized

            // ending transition animation
            transitionManager.OnTransitionOver += HandleTransitionOver;
            transitionManager.EndingTransition();
        }

        /// <summary>
        /// called when the second transition animation is over
        /// </summary>
        private void HandleTransitionOver() => StartCoroutine(EndLoading());

        /// <summary>
        /// Last step of the transition: the target scene is ready the transition scene can be unloaded
        /// </summary>
        /// <returns></returns>
        private IEnumerator EndLoading()
        {
            AsyncOperation unload = SceneManager.UnloadSceneAsync(TRANSITION_SCENE_NAME);
            while (unload.progress < 1.0f)
                yield return null;

            OnLoadingCompleted?.Invoke();
            loading = false;
        }
    }
}
