using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlinePlayer : MonoBehaviour
{
    public bool IsControlled = false;
    
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

    public GameObject _VRPlayer;
    private GameObject _holdingItem;

    public GameObject ThirdPersonView;
    
    // Start is called before the first frame update
    void Start()
    {
        this.holdingItemId = -1; // not holding anything
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position != new Vector3(this.x, this.y, this.z))
        {
            this.transform.position = new Vector3(this.x, this.y, this.z);
        }

        if (this.transform.eulerAngles != new Vector3(this.rotX, this.rotY, this.rotZ))
        {
            this.transform.eulerAngles = new Vector3(this.rotX, this.rotY, this.rotZ);
        }

        if (this.IsControlled && !this._VRPlayer)
        {
            this._VRPlayer = GameObject.FindWithTag("XROrigin");
        }
        
        
        if (this.IsControlled)
        {
            this.ThirdPersonView.SetActive(false);
            // this.GetComponent<MeshRenderer>().enabled = false;
            if (this._VRPlayer)
            {
                this.transform.position = _VRPlayer.transform.position;
                this.transform.eulerAngles = _VRPlayer.transform.eulerAngles;
            }
            // if (Input.GetKey(KeyCode.W))
            // {
            //     this.transform.position += Vector3.forward * 25f * Time.deltaTime;
            // }
            // if (Input.GetKey(KeyCode.S))
            // {
            //     this.transform.position -= Vector3.forward * 25f * Time.deltaTime;
            // }
            // if (Input.GetKey(KeyCode.A))
            // {
            //     this.transform.position += Vector3.left * 25f * Time.deltaTime;
            // }
            // if (Input.GetKey(KeyCode.D))
            // {
            //     this.transform.position += Vector3.right * 25f * Time.deltaTime;
            // }
            //
            // if (Input.GetKey(KeyCode.Alpha1))
            // {
            //     this.SetHoldingItem(1);
            // }
            // if (Input.GetKey(KeyCode.Alpha2))
            // {
            //     this.SetHoldingItem(2);
            // }

            
            this.x = this.transform.position.x;
            this.y = this.transform.position.y;
            this.z = this.transform.position.z;
            this.rotX = this.transform.eulerAngles.x;
            this.rotY = this.transform.eulerAngles.y;
            this.rotZ = this.transform.eulerAngles.z;


            if (this.holdingItemId != -1)
            {
                // this.holdingItemX = this.x;
                // this.holdingItemY = this.y;
                // this.holdingItemZ = this.z;
                
                this.holdingItemX = this._holdingItem.transform.position.x;
                this.holdingItemY = this._holdingItem.transform.position.y;
                this.holdingItemZ = this._holdingItem.transform.position.z;
                this.holdingItemRotX = this._holdingItem.transform.eulerAngles.x;
                this.holdingItemRotY = this._holdingItem.transform.eulerAngles.y;
                this.holdingItemRotZ = this._holdingItem.transform.eulerAngles.z;
            }
        }
    }

    public void SetHoldingItem(int itemId)
    {
        this.holdingItemId = itemId;

        OnlineObject[] items = FindObjectsOfType<OnlineObject>();

        this._holdingItem = null;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i].id == itemId) this._holdingItem = items[i].transform.gameObject;
        }
    }

    public void UnsetHoldingItem()
    {
        this._holdingItem = null;
        this.holdingItemId = -1;
    }
}
