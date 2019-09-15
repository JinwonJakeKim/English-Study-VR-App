using System;
using System.Collections.Generic;
using UnityEngine;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition
{
    public class GCSpeechRecognition : MonoBehaviour
    {
        public event Action<RecognitionResponse, long> RecognitionSuccessEvent;
        public event Action<string, long> RecognitionFailedEvent;
        //Action은 반환값과 인자값이 없는 함수포인터로써, 어떤 결과를 반환하는 것을 목적으로 하는 것이 아니라
        //일련의 작업(Action에 등록한 함수 순서대로)을 수행하는 것이 목적
        //event는 delegate를 담을 그릇, delegate는 method를 담을 그릇

        public event Action StartedRecordEvent;
        public event Action<AudioClip> FinishedRecordEvent;
        public event Action RecordFailedEvent;

        public event Action BeginTalkigEvent;
        public event Action<AudioClip> EndTalkigEvent;

        
        private static GCSpeechRecognition _Instance;
        public static GCSpeechRecognition Instance
        {
            get//get은 Instance의 클래스 _Instance변수 값을 받아올 때 / set은 _instance변수 값을 세팅or치환 할 때
            {
                if (_Instance == null)
                    _Instance = new GameObject("[Singleton]GCSpeechRecognition").AddComponent<GCSpeechRecognition>();

                return _Instance;
            }
        }


        private ServiceLocator _serviceLocator;

        private ISpeechRecognitionManager _speechRecognitionManager;//연두색은 customized mothods
        private IMediaManager _mediaManager;

        private List<string[]> _currentSpeechContexts;

        public ServiceLocator ServiceLocator { get { return _serviceLocator; } }

        [Header("Prefab Config Settings")]
        public int currentConfigIndex = 0;
        public List<Config> configs;

        [Header("Prefab Object Settings")]
        public bool isDontDestroyOnLoad = false;
        public bool isFullDebugLogIfError = false;
        public bool isUseAPIKeyFromPrefab = false;

        [Header("Prefab Fields")]
        public string apiKey;

        private void Awake()//Awake는 스크립트 인스턴스가 로딩될 때 호출, 게임이 시작되기 전에 호출되며 1번만 호출.
            //스크립트 간의 참조는 Awake, 정보를 보내고 받는 경우에는 Start이며 Awake가 Start보다 먼저 호출된다.
        {
            if (_Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            _Instance = this;

            _serviceLocator = new ServiceLocator();
            _serviceLocator.InitServices();

            _mediaManager = _serviceLocator.Get<IMediaManager>();
            _speechRecognitionManager = _serviceLocator.Get<ISpeechRecognitionManager>();

            _mediaManager.StartedRecordEvent += StartedRecordEventHandler;
            _mediaManager.FinishedRecordEvent += FinishedRecordEventHandler;
            _mediaManager.RecordFailedEvent += RecordFailedEventHandler;
            _mediaManager.BeginTalkigEvent += BeginTalkigEventHandler;
            _mediaManager.EndTalkigEvent += EndTalkigEventHandler;

            _speechRecognitionManager.SetConfig(configs[currentConfigIndex]);

            _speechRecognitionManager.RecognitionSuccessEvent += RecognitionSuccessEventHandler;
            _speechRecognitionManager.RecognitionFailedEvent += RecognitionFailedEventHandler;
        }

        private void Update()//프레임마다 호출, 비 물리적인 오브젝트의 움직임이나 타이머, 입력 감지
        {
            if (_Instance == this)
            {
                _serviceLocator.Update();
            }
        }

        private void OnDestroy()
        {
            if (_Instance == this)
            {
                _mediaManager.StartedRecordEvent -= StartedRecordEventHandler;
                _mediaManager.FinishedRecordEvent -= FinishedRecordEventHandler;
                _mediaManager.RecordFailedEvent -= RecordFailedEventHandler;
                _mediaManager.BeginTalkigEvent -= BeginTalkigEventHandler;
                _mediaManager.EndTalkigEvent -= EndTalkigEventHandler;

                _speechRecognitionManager.RecognitionSuccessEvent -= RecognitionSuccessEventHandler;
                _speechRecognitionManager.RecognitionFailedEvent -= RecognitionFailedEventHandler;

                _Instance = null;
                _serviceLocator.Dispose();
            }
        }

        public void StartRecord(bool isEnabledVoiceDetection)
        {
            _mediaManager.IsEnabledVoiceDetection = isEnabledVoiceDetection;
            _mediaManager.StartRecord();
        }

        public void StopRecord()
        {
            _mediaManager.StopRecord();
        }

        public void Recognize(AudioClip clip, List<string[]> contexts, Enumerators.LanguageCode language)
        {
            _speechRecognitionManager.Recognize(clip, contexts, language);
        }

        public void SetLanguage(Enumerators.LanguageCode language)
        {
            _speechRecognitionManager.CurrentConfig.defaultLanguage = language;
        }

        public void SetContext(List<string[]> contexts)
        {
            _currentSpeechContexts = contexts;
        }

        private void RecognitionSuccessEventHandler(RecognitionResponse arg1, long arg2)
        {
            if (RecognitionSuccessEvent != null)
                RecognitionSuccessEvent(arg1, arg2);
        }

        private void RecognitionFailedEventHandler(string arg1, long arg2)
        {
            if (RecognitionFailedEvent != null)
                RecognitionFailedEvent(arg1, arg2);
        }

        private void RecordFailedEventHandler()
        {
            if (RecordFailedEvent != null)
                RecordFailedEvent();
        }

        private void BeginTalkigEventHandler()
        {
            if (BeginTalkigEvent != null)
                BeginTalkigEvent();
        }

        private void EndTalkigEventHandler(AudioClip clip)
        {
            if (EndTalkigEvent != null)
                EndTalkigEvent(clip);

            _speechRecognitionManager.Recognize(clip, _currentSpeechContexts, _speechRecognitionManager.CurrentConfig.defaultLanguage);
        }

        private void StartedRecordEventHandler()
        {
            if (StartedRecordEvent != null)
                StartedRecordEvent();
        }

        private void FinishedRecordEventHandler(AudioClip clip)
        {
            if (FinishedRecordEvent != null)
                FinishedRecordEvent(clip);

            if (!_mediaManager.IsEnabledVoiceDetection)
                _speechRecognitionManager.Recognize(clip, _currentSpeechContexts, _speechRecognitionManager.CurrentConfig.defaultLanguage);
        }
    }
}