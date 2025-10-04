using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;


[Serializable]
public class GoogleData
{
	public string order, result, msg;
}

public class AppScriptReceiver : MonoBehaviour
{
	const string URL = "https://script.google.com/macros/s/AKfycbxn-FiJ_0VhvkjnP8Qo6KYyq9kyq2kuTkEyUFKqPJRCZEmaumz5FH0pQPClDCQxYRwM/exec"; //TODO : 구글 스프레드시트 주소 추가
	public GoogleData GD;
	public string _order, _result, _msg;
	PlayerData client;

	public PatientIDCheck patientIDCheck;

	// bool SetIDPass()
	// {
	// 	if ( == "" || pass == "") return false;
	// 	else return true;
	// }
	private void Start()
	{ 
		client = NetworkManager._instance._playerData;
	}

	// public void IDCheck(string id)
	// {
	// 	Debug.Log("IDCheck Run!");
	// 	WWWForm form = new WWWForm();
	// 	form.AddField("order", "checkIDExist");
	// 	form.AddField("id", id);
	// 	StartCoroutine(Post(form));
	// }

	public void IDCheckReceive()
	{
		switch (_result)
		{
			case "TRUE":
				patientIDCheck.IDCheck_Collected();
				break;
			case "FALSE":
				Debug.Log($"없는 아이디이거나 실패입니다.");
				break;
		}
	}


	public void Register()
	{
		if (string.IsNullOrEmpty(client.PatientID) || string.IsNullOrEmpty(client.PW) /*추가*/)
		{
			//TODO: 중요정보 없음 팝업
			return;
		}

		WWWForm form = new WWWForm();
		form.AddField("order", "register");
		form.AddField("id", client.PatientID);
		form.AddField("pass", client.PW);
		form.AddField("gender", client.gender);
		form.AddField("dateofbirth", client.dateOfBirth.ToString());
		form.AddField("job", client.job);
		Post(form);
	}


	public void Login()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "login");
		form.AddField("id", client.PatientID);
		form.AddField("pass", client.PW);

		Post(form);
	}
	


	public void SetValue()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "setValue");
		//form.AddField("value", ValueInput.text);

		Post(form);
	}


	public void GetValue()
	{
		WWWForm form = new WWWForm();
		form.AddField("order", "getValue");

		Post(form);
	}

	IEnumerator Post(WWWForm form)
	{
		using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
		{
			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(www.error);
				yield break;
			}

			string jsonStr = www.downloadHandler.text;
			Debug.Log(jsonStr);
			var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);

			_order = jsonObj["order"];
			_result = jsonObj["result"];
			_msg = jsonObj["msg"];
      
			switch (_order)
			{
				case "checkIDExist":
					IDCheckReceive();
					break;
			}

			Response(www.downloadHandler.text);
		}
	}
	
	void Response(string json)
	{
		if (string.IsNullOrEmpty(json)) return;

		GD = JsonUtility.FromJson<GoogleData>(json);

		if (GD.result == "ERROR")
		{
			Debug.Log(GD.order + "을 실행할 수 없습니다. 에러 메시지 : " + GD.msg);
			return;
		}

		Debug.Log(GD.order + "을 실행했습니다. 메시지 : " + GD.msg);
	}


	// public async Task Post(WWWForm form)
	// {
	// 	using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
	// 	{
	// 		var tcs = new TaskCompletionSource<bool>();
	// 		www.SendWebRequest().completed += op => tcs.TrySetResult(true);
	//
	// 		await tcs.Task;
	//
	// 		if (www.result != UnityWebRequest.Result.Success)
	// 		{
	// 			Debug.Log(www.error);
	// 		}
	// 		else
	// 		{
	// 			string jsonStr = www.downloadHandler.text;
	// 			var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonStr);
	//
	// 			_order = jsonObj["order"];
	// 			_result = jsonObj["result"];
	// 			_msg = jsonObj["msg"];
	//
	// 			// switch (_order)
	// 			// {
	// 			// 	case "ERROR":
	// 			// 		//msgChecker();
	// 			// 		break;
	// 			// 	case "OK":
	// 			// 		//EditorPrefs.SetString("_Used", _usedCD);
	// 			// 		break;
	// 			// }
	//
	// 			switch (_order)
	// 			{
	// 				case "checkIDExist":
	// 					IDCheckReceive();
	// 					break;
	// 			}
	// 		}
	//
	// 		Response(www.downloadHandler.text);
	// 	}

/// <summary>
/// 꼭 지울 것 : 아이디와 비밀번호 저장하는 체크
/// </summary>
	public void thisIsNotCurrentMethod(string ID, string PW)
	{
		PlayerPrefs.SetString("ID", ID);
		PlayerPrefs.SetString("PW", PW);
	}
}