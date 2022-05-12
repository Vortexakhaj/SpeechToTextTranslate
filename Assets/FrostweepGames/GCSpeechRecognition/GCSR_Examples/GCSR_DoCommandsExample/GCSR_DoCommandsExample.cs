using System;

using UnityEngine;

using UnityEngine.UI;



namespace FrostweepGames.Plugins.GoogleCloud.SpeechRecognition.Examples

{

	public class GCSR_DoCommandsExample : MonoBehaviour

	{

		[SerializeField] 
		private GCSpeechRecognition _speechRecognition;

		[SerializeField]
		private Button _startRecord, _stopRecord, _recognize;

		[SerializeField]
		private Dropdown _language;
		
		[SerializeField]
		private Text _result;
		
		private void Start(){

			_startRecord.onClick.AddListener(StartRecordButtonOnClickHandler);
			_stopRecord.onClick.AddListener(StopRecordButtonOnClickHandler);
			_recognize.onClick.AddListener(RecognizeButtonOnClickHandler);

			_language.ClearOptions();

			for (int i = 0; i< Enum.GetNames(typeof(Enumerators.LanguageCode)).Length; i++){
				_language.options.Add(new Dropdown.OptionData(((Enumerators.LanguageCode)i).Parse()));
			}

			_language.value = _language.options.IndexOf(_language.options.Find(x => x.text == Enumerators.LanguageCode.en_GB.Parse()));

			// try 1 c if fails
			_speechRecognition.RecognizeSuccessEvent += RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent += RecognizeFailedEventHandler;
			
			_speechRecognition.StartedRecordEvent += StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent += RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent += EndTalkigEventHandler;
		}

		private void OnDestroy(){
			_speechRecognition.RecognizeSuccessEvent -= RecognizeSuccessEventHandler;
			_speechRecognition.RecognizeFailedEvent -= RecognizeFailedEventHandler;
			
			_speechRecognition.StartedRecordEvent -= StartedRecordEventHandler;
			_speechRecognition.RecordFailedEvent -= RecordFailedEventHandler;

			_speechRecognition.EndTalkigEvent -= EndTalkigEventHandler;
		}

		private void StartRecordButtonOnClickHandler(){
			_startRecord.interactable = false;
			_stopRecord.interactable = true;
			_result.text = string.Empty;
			_speechRecognition.StartRecord(false);
		}

		private void StopRecordButtonOnClickHandler(){
			_stopRecord.interactable = false;
			_startRecord.interactable = true;
			_speechRecognition.StopRecord();
		}

		private void RecognizeButtonOnClickHandler(){
			if(_speechRecognition.LastRecordedClip == null || _speechRecognition.LastRecordedRaw == null){
				return;
			}
			RecognitionConfig config = RecognitionConfig.GetDefault();
			// try cap L
			config.languageCode = ((Enumerators.LanguageCode)_language.value).Parse();
			// config.speechContexts = new SpeechContext[]
			// {
			// 	new SpeechContext()
			// 	{
			// 		phrases = _contextPhrasesInputField.text.Replace(" ", string.Empty).Split(',')
			// 	}
			// };
			config.audioChannelCount = _speechRecognition.LastRecordedClip.channels;

			GeneralRecognitionRequest recognitionRequest = new GeneralRecognitionRequest()
			{
				audio = new RecognitionAudioContent()
				{
					content = _speechRecognition.LastRecordedRaw.ToBase64()
				},
				config = config
			};

			_speechRecognition.Recognize(recognitionRequest);
		}

		private void RecognizeSuccessEventHandler(RecognitionResponse recogitionResponse){
			_result.text = "Recognize success.";

			if (recogitionResponse == null || recogitionResponse.results.Length == 0){
				_result.text = "\nWords not detected";
				return;
			}

			_result.text += "\n" + recogitionResponse.results[0].alternatives[0].transcript;
		}

		private void StartedRecordEventHandler(){
			_result.text = "StartedRecordEventHandler";
		}

		

		private void RecognizeFailedEventHandler(string error){
			_result.text = "Recognition failed, error: " + error;
		}


		private void RecordFailedEventHandler(){
			_result.text = "RecordFailedHandler";
			_stopRecord.interactable = false;
			_startRecord.interactable = true;
		}

		private void EndTalkigEventHandler(AudioClip clip, float[] raw){
			_result.text = "EndTalkigEventHandler";
			//FinishedRecordEventHandler(clip, raw);
		}


	
	}

}