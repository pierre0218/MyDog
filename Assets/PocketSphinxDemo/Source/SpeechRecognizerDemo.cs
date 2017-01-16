//#define USE_CHINESE
#define USE_ENGLISH

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class SpeechRecognizerDemo : MonoBehaviour, IPocketSphinxEvents
{
	public static bool UseChinese = false;

    /* Named searches allow to quickly reconfigure the decoder */
#if USE_CHINESE
    private const String KWS_SEARCH = "action";
#else
	private const String KWS_SEARCH = "wakeup";
#endif
    private const String ACTION_SEARCH = "action";


    /* Keyword we are looking for to activate menu */
    private const String KEYPHRASE = "peanut listen";

    #region Public serialized fields
	[SerializeField]
	private DogControl _dogControl;
    [SerializeField]
    private GameObject _pocketSphinxPrefab;
    [SerializeField]
    private Text _infoText;
    [SerializeField]
    private Text _SpeechResult;
    [SerializeField]
    private string[] progressTexts;
    #endregion

    #region Private fields
    private UnityPocketSphinx.PocketSphinx _pocketSphinx;

    private Dictionary<string, string> infoTextDict;
    #endregion

    #region Private methods
    private void SubscribeToPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech += OnBeginningOfSpeech;
        em.OnEndOfSpeech += OnEndOfSpeech;
        em.OnError += OnError;
        em.OnInitializeFailed += OnInitializeFailed;
        em.OnInitializeSuccess += OnInitializeSuccess;
        em.OnPartialResult += OnPartialResult;
        em.OnPocketSphinxError += OnPocketSphinxError;
        em.OnResult += OnResult;
        em.OnTimeout += OnTimeout;
    }

    private void UnsubscribeFromPocketSphinxEvents()
    {
        EM_UnityPocketsphinx em = _pocketSphinx.EventManager;

        em.OnBeginningOfSpeech -= OnBeginningOfSpeech;
        em.OnEndOfSpeech -= OnEndOfSpeech;
        em.OnError -= OnError;
        em.OnInitializeFailed -= OnInitializeFailed;
        em.OnInitializeSuccess -= OnInitializeSuccess;
        em.OnPartialResult -= OnPartialResult;
        em.OnPocketSphinxError -= OnPocketSphinxError;
        em.OnResult -= OnResult;
        em.OnTimeout -= OnTimeout;
    }

    private void switchSearch(string searchKey)
    {
        _pocketSphinx.StopRecognizer();

        if (searchKey.Equals(KWS_SEARCH))
        {
            _pocketSphinx.StartListening(searchKey);
        }
        else
        {
            _pocketSphinx.StartListening(searchKey,3000);
        }

        string text;
        infoTextDict.TryGetValue(searchKey, out text);

        _infoText.text = text;
        _SpeechResult.text = "Say something!";
    }
    #endregion

    #region MonoBehaviour methods
    void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(_pocketSphinxPrefab, "No PocketSphinx prefab assigned.");
        var obj = Instantiate(_pocketSphinxPrefab, this.transform) as GameObject;
        _pocketSphinx = obj.GetComponent<UnityPocketSphinx.PocketSphinx>();

        if (_pocketSphinx == null)
        {
            Debug.LogError("[SpeechRecognizerDemo] No PocketSphinx component found. Did you assign the right prefab???");
        }

        SubscribeToPocketSphinxEvents();

        _infoText.text = "Please wait for Speech Recognition engine to load.";
        _SpeechResult.text = "Loading human dictionary...";
    }

    void Start()
    {
#if USE_CHINESE
		_pocketSphinx.SetAcousticModelPath("zh_broadcastnews_ptm256_8000");
		_pocketSphinx.SetDictionaryPath("zh_broadcastnews_utf8.dict");
#else
		_pocketSphinx.SetAcousticModelPath("en-us-ptm");
		_pocketSphinx.SetDictionaryPath("cmudict-en-us.dict");
#endif
        //Debug.Log("[SpeechRecognizerDemo] " + Application.streamingAssetsPath + "cmudict-en-us.dict");

        _pocketSphinx.SetKeywordThreshold(1e-45f);
        _pocketSphinx.AddBoolean("-allphone_ci", true);


        _pocketSphinx.AddNGramSearchPath(ACTION_SEARCH, "commands.lm");

        _pocketSphinx.SetupRecognizer();

        infoTextDict = new Dictionary<string, string>();
		#if USE_CHINESE
		infoTextDict.Add(KWS_SEARCH, "請下指令");
		#else
        infoTextDict.Add(KWS_SEARCH, progressTexts[0]);
		#endif
		infoTextDict.Add(ACTION_SEARCH, progressTexts[1]);

    }
    
    // Update is called once per frame
    void Update ()
    {
        
    }

    void OnDestroy()
    {
        if (_pocketSphinx != null)
        {
            UnsubscribeFromPocketSphinxEvents();
            _pocketSphinx.DestroyRecognizer();
        }
    }
    #endregion

    #region PocketSphinx event methods
    public void OnPartialResult(string hypothesis)
    {
        _SpeechResult.text = hypothesis;

		string[] tokens = hypothesis.Split(' ');
		string lastword = "";
		string lastword2 = "";
		if (tokens.Length > 0) {
			lastword = tokens [tokens.Length - 1];
		}
		if (tokens.Length > 1) {
			lastword2 = tokens [tokens.Length - 2];
		}

#if USE_CHINESE
		if (lastword2 == "站" && lastword == "好") {
			_dogControl.SetTransition (Transition.ToStand);
			switchSearch (KWS_SEARCH);
		}

		if (lastword2 == "装" && lastword == "死") {
			_dogControl.SetTransition (Transition.ToDead);
			switchSearch (KWS_SEARCH);
		}

		if (lastword2 == "跑" && lastword == "步") {
			_dogControl.SetTransition (Transition.ToRun);
			switchSearch (KWS_SEARCH);
		}

		if (lastword2 == "坐" && lastword == "下") {
			_dogControl.SetTransition (Transition.ToSit);
			switchSearch (KWS_SEARCH);
		}

		if (lastword2 == "趴" && lastword == "下") {
			_dogControl.SetTransition (Transition.ToLay);
			switchSearch (KWS_SEARCH);
		}

		if(lastword == "跳")
		{
			_dogControl.SetTransition (Transition.ToJump);
			switchSearch (KWS_SEARCH);
		}

		if(lastword == "坐")
		{
			_dogControl.SetTransition (Transition.ToSit);
			switchSearch (KWS_SEARCH);
		}

		if(lastword == "跑")
		{
			_dogControl.SetTransition (Transition.ToRun);
			switchSearch (KWS_SEARCH);
		}
			
#else
		if (lastword == "down") {
			if (lastword2 == "sit") {
				_dogControl.SetTransition (Transition.ToSit);
				switchSearch (KWS_SEARCH);
			}
			if (lastword2 == "lay") {
				_dogControl.SetTransition (Transition.ToLay);
				switchSearch (KWS_SEARCH);
			}
		}

		if (lastword2 == "stand" && lastword == "up") {
			_dogControl.SetTransition (Transition.ToStand);
			switchSearch (KWS_SEARCH);
		}

		if (lastword2 == "play" && lastword == "dead") {
			_dogControl.SetTransition (Transition.ToDead);
			switchSearch (KWS_SEARCH);
		}

		if (hypothesis.Equals (KEYPHRASE))
		{
			switchSearch (ACTION_SEARCH);
			_dogControl.SetTransition (Transition.ToStand);
		}
		else if(lastword.Equals ("walk"))
		{
			_dogControl.SetTransition (Transition.ToWalk);
			switchSearch (KWS_SEARCH);
		}
		else if(lastword.Equals ("run"))
		{
			_dogControl.SetTransition (Transition.ToRun);
			switchSearch (KWS_SEARCH);
		}
		else if(lastword.Equals ("stand"))
		{
			_dogControl.SetTransition (Transition.ToStand);
			switchSearch (KWS_SEARCH);
		}
		else if(lastword.Equals ("sit"))
		{
			_dogControl.SetTransition (Transition.ToSit);
			switchSearch (KWS_SEARCH);
		}
		else if(lastword.Equals ("lay"))
		{
			_dogControl.SetTransition (Transition.ToLay);
			switchSearch (KWS_SEARCH);
		}
		else if(lastword.Equals ("jump"))
		{
			_dogControl.SetTransition (Transition.ToJump);
			switchSearch (KWS_SEARCH);
		}
#endif
    }

    public void OnResult(string hypothesis)
    {
        _SpeechResult.text = hypothesis;
    }

    public void OnBeginningOfSpeech()
    {
        
    }

    public void OnEndOfSpeech()
    {
       // switchSearch(KWS_SEARCH);
		_SpeechResult.text = "";
    }

    public void OnError(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred at OnError()");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);

		_SpeechResult.text = error;
    }

    public void OnTimeout()
    {
        Debug.Log("[SpeechRecognizerDemo] Speech Recognition timed out");
        switchSearch(KWS_SEARCH);
    }

    public void OnInitializeSuccess()
    {
#if USE_ENGLISH
        _pocketSphinx.AddKeyphraseSearch(KWS_SEARCH, KEYPHRASE);
#endif
		switchSearch(KWS_SEARCH);
    }

    public void OnInitializeFailed(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred on Initialization PocketSphinx.");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);

		_SpeechResult.text = error;
    }

    public void OnPocketSphinxError(string error)
    {
        Debug.LogError("[SpeechRecognizerDemo] An error ocurred on OnPocketSphinxError().");
        Debug.LogError("[SpeechRecognizerDemo] error = " + error);

		_SpeechResult.text = error;
    }
    #endregion
}
