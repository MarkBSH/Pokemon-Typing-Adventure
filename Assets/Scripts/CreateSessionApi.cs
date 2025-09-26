using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Text;
using System.Collections;

public class CreateSessionApi : MonoBehaviour {
    [Serializable]
    private class CreateSessionRequest {
        public string username;
    }

    [Serializable]
    private class CreateSessionResponse {
        public bool ok;
        public int session_id;
        public string error;
    }

    private const string CreateSessionUrl = "http://localhost:5173/api/create-session";

    public IEnumerator CreateSession(string username) {
        CreateSessionRequest reqObj = new CreateSessionRequest { username = username };
        string json = JsonUtility.ToJson(reqObj);

        UnityWebRequest www = new UnityWebRequest(CreateSessionUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success) {
            string text = www.downloadHandler.text;
            try {
                CreateSessionResponse res = JsonUtility.FromJson<CreateSessionResponse>(text);
                if (res != null && res.ok) {
                    Debug.Log($"Session created. session_id = {res.session_id}");
                } else {
                    Debug.LogError($"Server did not return ok. Response: {text}");
                }
            } catch (Exception e) {
                Debug.LogError($"Failed to parse JSON: {e.Message}\nResponse: {text}");
            }
        } else {
            Debug.LogError($"HTTP error: {www.responseCode} - {www.error}");
            try
            {
		            string text = www.downloadHandler.text;
                CreateSessionResponse res = JsonUtility.FromJson<CreateSessionResponse>(text);
                if (res != null && !string.IsNullOrEmpty(res.error))
                {
                    Debug.LogError($"The server provided the following error message: {res.error}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to parse JSON: {e.Message}");
            }
        }
    }

    private void Start() {
		    // Hardcoded gebruikersnaam voor het voorbeeld
        StartCoroutine(CreateSession("Jan"));
    }
}