using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gazeobject : MonoBehaviour {

    public int positionX, positionY;
    

    // Use this for initialization
    void Start () {
     
}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(positionX,positionY,1);
	}
}
