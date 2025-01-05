using SAS.Utilities;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SAS.SceneManagement
{
    struct SceneGroupLoadedEvent : IEvent
    {
        public SceneGroup sceneGroup;
    }

    struct AdditiveSceneLoadedEvent : IEvent
    {
        public Scene scene;
    }

    public class SceneLoader : Singleton<SceneLoader>
    {
        [SerializeField] private bool m_LoadOptionalSceneAtStart = false;
        [SerializeField] private UILoadingScreen m_LoadingScreen;
        [SerializeField] SceneGroup[] sceneGroups;

        float _targetProgress;
        bool _isLoading;
        private readonly SceneGroupManager _manager = new SceneGroupManager();

        protected override async void Start()
        {
            base.Start();
            await LoadSceneGroup(0, !m_LoadOptionalSceneAtStart);
        }

        void Update()
        {
            if (!_isLoading)
                return;

            m_LoadingScreen?.SetLoadProgress(_targetProgress);
        }

        public async Task LoadSceneGroup(string groupName, bool ignoreOptional = false)
        {
            int index = Array.FindIndex(sceneGroups, sceneGroup => sceneGroup.Name == groupName);
            if (index == -1) return;
            await LoadSceneGroup(index, ignoreOptional);
        }

        public async Task LoadSceneGroup(int index, bool ignoreOptional = false)
        {
            _targetProgress = 1f;
            _isLoading = true;

            if (index < 0 || index >= sceneGroups.Length)
            {
                Debug.LogError("Invalid scene group index: " + index);
                return;
            }

            LoadingProgress progress = new LoadingProgress();
            progress.Progressed += target => _targetProgress = Mathf.Max(target, _targetProgress);
            m_LoadingScreen?.SetActive(true);
            await _manager.LoadScenes(sceneGroups[index], progress, false, ignoreOptional);
            await Task.Delay(1000);
            _isLoading = false;
            m_LoadingScreen?.SetActive(false);
        }

        public async Task LoadSceneAdditively(string sceneName, IProgress<float> progress = null)
        {
            await _manager.LoadSceneAdditively(sceneName, progress);
        }

        public async Task UnloadScenes()
        {
            await _manager.UnloadScenes();
        }

        public async Task UnloadScene(string sceneName)
        {
            await _manager.UnloadScene(sceneName);
        }
    }

    public class LoadingProgress : IProgress<float>
    {
        public event Action<float> Progressed;

        const float ratio = 1f;

        public void Report(float value)
        {
            Progressed?.Invoke(value / ratio);
        }
    }
}