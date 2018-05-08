using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemMovement : MonoBehaviour {

	public void Activate () {
        GetComponent<MeshRenderer>().material.color = Color.red;
	}
	
	public void Deactivate () {
        GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
