using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NativeWebSocket;
using TMPro;
using UnityEngine.SceneManagement;

public class OnlineGameManager : MonoBehaviour
{
    WebSocket websocket;
    public GameObject Racket;
    public GameObject Follower;
    public bool IsSender;
    public bool IsReceiver;
    public string url;

    public GameObject PlayerPrefab;
    public GameObject ObjectPrefab;

    public List<OnlineObject> Objects;
    public List<OnlinePlayer> Players;

    public int ControllablePlayerId;
    public OnlinePlayer ControllablePlayer;

    private bool _isStartingDataLoaded = false;

    public TextMeshPro ScoreText;
    public TextMeshPro TimeLeftText;
    
    [System.Serializable]
    public struct Player
    {
        public int id;
        public int points;
        public float x;
        public float y;
        public float z;
        public float rotX;
        public float rotY;
        public float rotZ;
        public int holdingItemId;
        public float holdingItemX;
        public float holdingItemY;
        public float holdingItemZ;
        public float holdingItemRotX;
        public float holdingItemRotY;
        public float holdingItemRotZ;
    }
    
    [System.Serializable]
    public struct Object
    {
        public int id;
        public float x;
        public float y;
        public float z;
        public float rotX;
        public float rotY;
        public float rotZ;
        public bool isActive;
    }

    [System.Serializable]
    public struct IncomingData
    {
        public List<Player> players;
        public List<Object> objects;
        public int timeLeft;
    }
    
    public struct OutgoingPlayerData
    {
        public int id;
        public int points;
        public float x;
        public float y;
        public float z;
        public float rotX;
        public float rotY;
        public float rotZ;
        public int holdingItemId;
        public float holdingItemX;
        public float holdingItemY;
        public float holdingItemZ;
        public float holdingItemRotX;
        public float holdingItemRotY;
        public float holdingItemRotZ;
    }

    public void OnUserGrabItem(int itemId)
    {
        this.ControllablePlayer.SetHoldingItem(itemId);
    }
    
    public void OnUserUngrabItem(int itemId, Vector3 position)
    {
        
        this.ControllablePlayer.SetHoldingItem(-1);
    }

    private void _generateObjects(int n)
    {
        this.Objects.Clear();
        
        for (int i = 0; i < n; i++)
        {
            OnlineObject newObj = Instantiate(this.ObjectPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<OnlineObject>();
            newObj.id = i;
            this.Objects.Add(newObj);
        }
    }
    
    private void _generatePlayers(int n)
    {
        this.Objects.Clear();
        
        for (int i = 0; i < n; i++)
        {
            OnlinePlayer newPlayer = Instantiate(this.PlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<OnlinePlayer>();
            newPlayer.id = i;
            if (i == this.ControllablePlayerId)
            {
                this.ControllablePlayer = newPlayer;
                newPlayer.IsControlled = true;
            }
            this.Players.Add(newPlayer);
        }
    }
    
    private void _handleStartingData(IncomingData incomingData)
    {
        this._generatePlayers(incomingData.players.Count);
        this._generateObjects(incomingData.objects.Count);

        this._isStartingDataLoaded = true;
    }

    private void _handleIncomingData(IncomingData incomingData)
    {
        for (int i = 0; i < incomingData.objects.Count; i++)
        {
            Debug.Log("X -> " + i + " | " + this.Objects.Count + " | " + incomingData.objects.Count);
            this.Objects[i].x = incomingData.objects[i].x;
            this.Objects[i].y = incomingData.objects[i].y;
            this.Objects[i].z = incomingData.objects[i].z;

            this.Objects[i].rotX = incomingData.objects[i].rotX;
            this.Objects[i].rotY = incomingData.objects[i].rotY;
            this.Objects[i].rotZ = incomingData.objects[i].rotZ;
            this.Objects[i].isActive = incomingData.objects[i].isActive;

        }
        
        for (int i = 0; i < incomingData.players.Count; i++)
        {
            if (!this.Players[i].IsControlled)
            {
                this.Players[i].x = incomingData.players[i].x;
                this.Players[i].y = incomingData.players[i].y;
                this.Players[i].z = incomingData.players[i].z;
                this.Players[i].rotX = incomingData.players[i].rotX;
                this.Players[i].rotY = incomingData.players[i].rotY;
                this.Players[i].rotZ = incomingData.players[i].rotZ;
                this.Players[i].holdingItemId = incomingData.players[i].holdingItemId;
            }
        }

        int scorePlayer1 = incomingData.players[0].points;
        int scorePlayer2 = incomingData.players[1].points;
        
        this.ScoreText.SetText(scorePlayer1.ToString() + " - " + scorePlayer2.ToString());
        this.TimeLeftText.SetText(incomingData.timeLeft.ToString() + "s");

        if (incomingData.timeLeft < 0)
        {
            PlayerPrefs.SetInt("Player1Score", incomingData.players[0].points);
            PlayerPrefs.SetInt("Player2Score", incomingData.players[1].points);
            SceneManager.LoadScene("GameResults");
        }
    }

    private OutgoingPlayerData _handleOutgoingGeneralData()
    {
        OutgoingPlayerData outgoingPlayerData = new OutgoingPlayerData();
        outgoingPlayerData.id = this.ControllablePlayer.id;
        outgoingPlayerData.x = this.ControllablePlayer.x;
        outgoingPlayerData.y = this.ControllablePlayer.y;
        outgoingPlayerData.z = this.ControllablePlayer.z;
        outgoingPlayerData.rotX = this.ControllablePlayer.rotX;
        outgoingPlayerData.rotY = this.ControllablePlayer.rotY;
        outgoingPlayerData.rotZ = this.ControllablePlayer.rotZ;
        outgoingPlayerData.holdingItemId = this.ControllablePlayer.holdingItemId;
        outgoingPlayerData.holdingItemX = this.ControllablePlayer.holdingItemX;
        outgoingPlayerData.holdingItemY = this.ControllablePlayer.holdingItemY;
        outgoingPlayerData.holdingItemZ = this.ControllablePlayer.holdingItemZ;
        outgoingPlayerData.holdingItemRotX = this.ControllablePlayer.holdingItemRotX;
        outgoingPlayerData.holdingItemRotY = this.ControllablePlayer.holdingItemRotY;
        outgoingPlayerData.holdingItemRotZ = this.ControllablePlayer.holdingItemRotZ;

        return outgoingPlayerData;
    }


    // Start is called before the first frame update
    async void Start()
    {
        if (this.ControllablePlayerId == -1)
        {
            this.ControllablePlayerId = PlayerPrefs.GetInt("PlayerNumber");
            Debug.Log("loading player int from prefs... -> " + this.ControllablePlayerId);
        }
        this._isStartingDataLoaded = false;
        this.ScoreText = GameObject.FindWithTag("ScoreText").GetComponent<TextMeshPro>();
        this.TimeLeftText = GameObject.FindWithTag("TimeLimitText").GetComponent<TextMeshPro>();
        
        websocket = new WebSocket(this.url);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
            SceneManager.LoadScene("NoConnection");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            // this.text.text = "Connection closed!";
            
            
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(message);

            IncomingData incomingData = JsonUtility.FromJson<IncomingData>(message);
            Debug.Log("incoming data:");
            Debug.Log(incomingData.players.Count);
            
            if (!this._isStartingDataLoaded)
            {
                _handleStartingData(incomingData);
            }
            else
            {
                _handleIncomingData(incomingData);
            }

            // if (this.IsReceiver)
            // {
            //     Debug.Log("OnMessage!");
            //     //Debug.Log(bytes);
            //
            //     // getting the message as a string
            //     var message = System.Text.Encoding.UTF8.GetString(bytes);
            //     Debug.Log("OnMessage! " + message);
            //
            //     if (message[0] == '{')
            //     {
            //         Vector3 rotation = this.Follower.transform.eulerAngles;
            //         Vector3 v = JsonUtility.FromJson<Vector3>(message);
            //         rotation.x = v.x;
            //         rotation.y = v.y;
            //         rotation.z = v.z;
            //
            //         this.Follower.transform.eulerAngles = rotation;
            //     }
            // }
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
            if (!this._isStartingDataLoaded)
            {
                // request only starting data
                await websocket.SendText("{\"type\": \"get\"}");
                
                return;
            }
            
            
            if (this.IsSender)
            {
                OutgoingPlayerData outgoingPlayerData = this._handleOutgoingGeneralData();
                string parsed = JsonUtility.ToJson(outgoingPlayerData);
                
                await websocket.SendText("{\"type\": \"setPlayerData\", \"data\": " + parsed + "}");
            }

            if (this.IsReceiver)
            {
                await websocket.SendText("{\"type\": \"get\"}");
            }
        }
    }

    public async void SendCoinDelivered(int coinId)
    {
        await websocket.SendText("{\"type\": \"setCoinDelivered\", \"coinId\": " + coinId.ToString() + ", \"playerId\": " + this.ControllablePlayerId.ToString() + "}");
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
}
