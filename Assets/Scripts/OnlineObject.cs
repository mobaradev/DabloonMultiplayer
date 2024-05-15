using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineObject : MonoBehaviour
{
    public int id;
    public float x;
    public float y;
    public float z;
    public float rotX;
    public float rotY;
    public float rotZ;
    public bool isActive = true;

    public bool isStartDataSet = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // this.transform.position = new Vector3(this.x, this.y, this.z);
        // this.transform.eulerAngles = new Vector3(this.rotX, this.rotY, this.rotZ);
        
        if (this.transform.position != new Vector3(this.x, this.y, this.z))
        {
            this.transform.position = new Vector3(this.x, this.y, this.z);
        }

        if (this.transform.eulerAngles != new Vector3(this.rotX, this.rotY, this.rotZ))
        {
            this.transform.eulerAngles = new Vector3(this.rotX, this.rotY, this.rotZ);
        }
        
        // this.transform.rotation = new Quaternion()

        if (!this.isActive)
        {
            this.gameObject.SetActive(false);
            this.GetComponent<MeshCollider>().enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    public void OnUserGrabbed()
    {
        FindObjectOfType<OnlineGameManager>().OnUserGrabItem(this.id);
    }
    
    public void OnUserUngrabbed()
    {
        FindObjectOfType<OnlineGameManager>().OnUserUngrabItem(this.id, this.transform.position);
    }
}
