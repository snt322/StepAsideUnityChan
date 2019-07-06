using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDestroyer : MonoBehaviour {

    Camera mCamera;

	// Use this for initialization
	void Start () {
        GameObject tmpObj = GameObject.Find("Main Camera") as GameObject;

        mCamera = tmpObj.GetComponent<Camera>() as Camera;

    }
	
	// Update is called once per frame
	void Update () {

        float z = mCamera.transform.position.z;

        float thisZ = this.transform.position.z;
        if(z > thisZ)
        {
            Destroy(this.gameObject);
        }
	}
}
