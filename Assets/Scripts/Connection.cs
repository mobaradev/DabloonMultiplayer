using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using TMPro;

public class Connection : MonoBehaviour
{
    WebSocket websocket;
    public GameObject Racket;
    public GameObject Follower;
    public bool IsSender;
    public bool IsReceiver;
    public string url;

    public TextMeshProUGUI text;

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket(this.url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            this.text.text = "Connection open";
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            this.text.text = "Error! " + e;
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            // this.text.text = "Connection closed!";
        };

        websocket.OnMessage += (bytes) =>
        {
            if (this.IsReceiver)
            {
                Debug.Log("OnMessage!");
                //Debug.Log(bytes);

                // getting the message as a string
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("OnMessage! " + message);

                if (message[0] == '{')
                {
                    Vector3 rotation = this.Follower.transform.eulerAngles;
                    Vector3 v = JsonUtility.FromJson<Vector3>(message);
                    rotation.x = v.x;
                    rotation.y = v.y;
                    rotation.z = v.z;

                    this.Follower.transform.eulerAngles = rotation;
                }
            }
        };

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.03f);

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    async void SendWebSocketMessage()
    {
        if (websocket.State == WebSocketState.Open)
        {
            if (this.IsSender)
            {
                // Sending bytes
                //await websocket.Send(new byte[] { 10, 20, 30 });

                // Sending plain text
                // float x = this.Racket.transform.eulerAngles.x;
                // float y = this.Racket.transform.eulerAngles.y;
                // float z = this.Racket.transform.eulerAngles.z;
                
                float x = this.Racket.transform.position.x;
                float y = this.Racket.transform.position.y;
                float z = this.Racket.transform.position.z;
                
                await websocket.SendText("{\"type\": \"set\", \"x\": " + x + ", \"y\":  " + y + ", \"z\": " + z + "}");
            }

            if (this.IsReceiver)
            {
                await websocket.SendText("{\"type\": \"get\"}");
            }
        }
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}