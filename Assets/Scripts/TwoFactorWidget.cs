using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class TwoFactorWidget : MonoBehaviour
{
	public TextMesh KeyText;
    public TextMesh TimeRemainingText;
	public AudioClip Notify;

    private bool _activated;
	private int _key;
	private float _timeElapsed;

	private const float TimerLength = 60.0f;

	public static string WidgetQueryTwofactor = "twofactor";
	public static string WidgetTwofactorKey = "twofactor_key";

	void Awake ()
	{
		GetComponent<KMWidget>().OnQueryRequest += GetQueryResponse;
		GetComponent<KMWidget>().OnWidgetActivate += Activate;
		GenerateKey();
	    KeyText.text = "";
	    TimeRemainingText.text = "";
	}

	void Update()
	{
	    if (!_activated) return;
		_timeElapsed += Time.deltaTime;
        // ReSharper disable once InvertIf
        if (_timeElapsed >= TimerLength)
		{
			_timeElapsed = 0f;
			UpdateKey();
		}
	    TimeRemainingText.text = string.Format("{0,3}", (int)(TimerLength - _timeElapsed)) + ".";
    }

	private void Activate()
	{
		_timeElapsed = 0f;
		DisplayKey();
	    _activated = true;
	}

	void UpdateKey()
	{
		GetComponent<KMAudio>().HandlePlaySoundAtTransform(Notify.name, transform);
		GenerateKey();
		DisplayKey();
	}

	private void GenerateKey()
	{
		_key = Random.Range(0, 1000000);
	}

	private void DisplayKey()
	{
		KeyText.text = string.Format("{0,6}", _key) + ".";
	}

	private string GetQueryResponse(string querykey, string queryinfo)
	{
		if (querykey != WidgetQueryTwofactor) return string.Empty;

		var response = new Dictionary<string, int> {{WidgetTwofactorKey, _key}};
		var serializedResponse = JsonConvert.SerializeObject(response);

		return serializedResponse;
	}
}
