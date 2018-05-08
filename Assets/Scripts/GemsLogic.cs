using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemsLogic : MonoBehaviour {

    public int rows = 3;
    public int cols = 3;

    public GameObject basePrefab;

    private GemMovement _activeGem;

    // Use this for initialization
    void Start () {
        GenerateLevel();

    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (_activeGem)
                {
                    _activeGem.Deactivate();
                }
                Transform objectHit = hit.transform;
                _activeGem = objectHit.gameObject.GetComponent<GemMovement>();
                if (_activeGem)
                {
                    _activeGem.Activate();    
                }

                // Do something with the object that was hit by the raycast.
            }
        }
	}

    void GenerateLevel()
    {
        for(int i=0; i<rows; ++i)
        {
            for(int j=0; j<cols; ++j)
            {
                Instantiate(basePrefab, Vector3.right*j+Vector3.forward*i, Quaternion.identity);
            }
        }
        Transform camTrans = FindObjectOfType<Camera>().transform;
        camTrans.position = Vector3.right * cols * 0.5F + Vector3.up*rows;
    }
}
