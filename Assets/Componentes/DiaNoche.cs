using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaNoche : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (GameManager.instance.deDia)
        {
            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.y = 0;
            this.gameObject.transform.rotation = Quaternion.Euler(rotationVector);
        }

        else
        {
            var rotationVector = transform.rotation.eulerAngles;
            rotationVector.y = 180;
            this.gameObject.transform.rotation = Quaternion.Euler(rotationVector);
        }
	}
}
