using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;

namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples
{
    public class Example : MonoBehaviour
    {
        private GCSpeechRecognition _speechRecognition;
        private Button _startRecordButton,_stopRecordButton;
        private Text _speechRecognitionResult;
        private InputField _contextPhrases;

        public Text NPC_sentence, Hint, Answer, User_sentence;
        private int Pagenumber;

        private AudioSource _audio2;//url을 통하여 재생되는 음성을 저장할 객체
        public AudioClip correctClip, incorrectClip;
        private AudioSource correct, incorrect;

        //여기서는 transform.Find로 경로 탐색하여 직접참조함
        //start 함수는 프레임이 시작될때 실행되는 default함수라고 생각하면 되며, 유니티 3D는 동영상이므로 프레임이 여러개. 즉 start함수가 계속 주르륵주르륵 실행됨
        private void Start()
        {
            
            _audio2 = gameObject.GetComponent<AudioSource>();//gameObject는 어디서 생성하지는 않는데, 자동으로 생성되는 객체인건가?
            correct = gameObject.GetComponent<AudioSource>();
            incorrect = gameObject.GetComponent<AudioSource>();
            _speechRecognition = GCSpeechRecognition.Instance;
            
            _speechRecognition.RecognitionSuccessEvent += SpeechRecognizedSuccessEventHandler;
            _speechRecognition.RecognitionFailedEvent += SpeechRecognizedFailedEventHandler;

            /*
             GameObject.Find는 해당 씬에 있는 게임오브젝트에 접근할 때, 
             transform.Find는 자식에 있는 게임오브젝트에 접근할 때.
             */

            _startRecordButton = GameObject.Find("Characters/Katie_LP/Dialogues/Play").GetComponent<Button>();
            _stopRecordButton = GameObject.Find("Characters/Katie_LP/Dialogues/Stop").GetComponent<Button>();
            _speechRecognitionResult = GameObject.Find("Characters/Katie_LP/Dialogues/Panel/User_sentence").GetComponent<Text>();
            correct.Play();
            incorrect.Play();
            
            //맨처음 초기 패널(오른쪽)
            NPC_sentence.text = "";
            Hint.text = "안녕하세요.";
            Answer.text = "";
            User_sentence.text = "";
            Pagenumber = 0;

            //_startRecordButton.onClick.AddListener(StartRecordButtonOnClickHandler);
            _stopRecordButton.onClick.AddListener(StopRecordButtonOnClickHandler);
            StartRecordButtonOnClickHandler();
            _startRecordButton.interactable = true;
            _stopRecordButton.interactable = false;
        }

        private void OnDestroy()
        {
            _speechRecognition.RecognitionSuccessEvent -= SpeechRecognizedSuccessEventHandler;
            _speechRecognition.RecognitionFailedEvent -= SpeechRecognizedFailedEventHandler;
        }

        private void StartRecordButtonOnClickHandler()
        {
            _startRecordButton.interactable = false;
            _stopRecordButton.interactable = true;
            _speechRecognitionResult.text = string.Empty;
            _speechRecognition.StartRecord(true);
        }

        private void StopRecordButtonOnClickHandler()
        {
            ApplySpeechContextPhrases();

            _stopRecordButton.interactable = false;
            _speechRecognition.StopRecord();
        }

        private void LanguageDropdownOnValueChanged(int value)
        {
            _speechRecognition.SetLanguage((Enumerators.LanguageCode)value);
        }

        private void ApplySpeechContextPhrases()
        {
            string[] phrases = _contextPhrases.text.Trim().Split(","[0]);

            if (phrases.Length > 0)
                _speechRecognition.SetContext(new List<string[]>() { phrases });
        }

        private void SpeechRecognizedFailedEventHandler(string obj, long requestIndex)
        {
            _speechRecognitionResult.text = "Speech Recognition failed with error: " + obj;
        }
        //SST구현
        //여기서 _speechRecognitionResult.text는 음성인식되어 문장으로 변환된 result 값
        private void SpeechRecognizedSuccessEventHandler(RecognitionResponse obj, long requestIndex)
        {
            if (obj != null && obj.results.Length > 0)
            {
                _speechRecognitionResult.text = obj.results[0].alternatives[0].transcript;//This is a result
            }

            if (Pagenumber == 0)
            {
                Answer.text = "hello";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                //여기서 다음페이지에 들어갈 text가 세팅되고 지금 if-else문 종료
                //그 다음 프레임때 start함수가 실행되면서 다시 이 함수가 실행될때는 pagenumber가 올라가있으므로 
                //다음 if-else문이 실행됨
                NPC_sentence.text = "Hi. Good evening sir, May I take your order?";
                Hint.text = "네. 준비되었습니다. 추천할만한 요리가 있나요?";
            }
            else if(Pagenumber == 1)
            {
                Answer.text = "Yes I'm ready do you have any recommendations";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                NPC_sentence.text = "I recommend grilled chicken with our special source";
                Hint.text = "얼마나 걸리나요?";
            }
            else if (Pagenumber == 2)
            {
                Answer.text = "how long does it take";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                NPC_sentence.text = "It takes about 15 minutes";
                Hint.text = "좋습니다.";
            }
            else if (Pagenumber == 3)
            {
                Answer.text = "I like it";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                NPC_sentence.text = "Would you like somthing to drink?";
                Hint.text = "콜라와 레몬에이드를 마시고 싶습니다. ";
            }
            else if (Pagenumber == 4)
            {   
                Answer.text = "I want to drink coke and lemonade";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                NPC_sentence.text = "Yes. Anything else?";
                Hint.text = "괜찮습니다.";
            }
            else if (Pagenumber == 5)
            {
                Answer.text = "No Thanks.";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
                NPC_sentence.text = "Ok. your order will be delivered soon";
                Hint.text = " ";
            }
            else if (Pagenumber == 6)
            {
                Answer.text = " ";
                User_sentence.text = _speechRecognitionResult.text;

                check_answer(Answer.text, User_sentence.text);
            }

            //답변에 해당하는 Text를 Google Translator로 전송하여 음성으로 반환받기위해 호출
            StartCoroutine(DownloadTheAudio());//DownloadTheAudio 실행
        }

        private void check_answer(string Answer, string User_sentence)
        {
            if (User_sentence.Equals(Answer))
            {
                StartCoroutine(correctSound());
                Pagenumber++;
            }
            else
            {
                StartCoroutine(incorrectSound());
                incorrectSound();
            }
        }

        IEnumerator correctSound()
        {
            yield return new WaitForSeconds(0);//시간지연을 correct.clip.length가 아니라 0으로
            correct.clip = correctClip;
            correct.Play();
        }

        IEnumerator incorrectSound()
        {
            yield return new WaitForSeconds(0);
            incorrect.clip = incorrectClip;
            incorrect.Play();
        }
        
        //TTS 구현
        IEnumerator DownloadTheAudio()
        {
            yield return new WaitForSeconds(2.5f);//시간지연 2.5초
            Regex rgx = new Regex("\\s+");//space때문에 에러발생을 해결, \s는 space를 표현하며 공백문자를 의미한다.
            string result = rgx.Replace(NPC_sentence.text, "+");//NPC_sentence.text에서 space(\\s)가 나오면 "+"로 치환해라
            string url =
                "http://translate.google.com/translate_tts?ie=UTF-8&total=1&idx=0&textlen=32&client=tw-ob&q="
                + result
                + "&tl=en-gb";
            WWW www = new WWW(url);//반환된 음성파일 원본을 www객체에 저장
            yield return www;
            _audio2.clip = www.GetAudioClip(false, true, AudioType.MPEG);//음성원본을 MPEG 타입으로 변환하여 _audio2.clip에 저장
            
            _audio2.Play();
            /*예를들어, 
            NPC_sentence.text = "It takes about 15 minutes" 일 경우, "It+take+about+15+minutes"이 된다.
            
            */
        }
    }
}