using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour {

    public Transform target = null;
    public Vector3 localOffsetAxis = Vector3.right;
    public float localOffsetAngle = 0F;


    //public Vector3 localOffsetAxis2 = Vector3.right;
    //public float localOffsetAngle2 = 0F;

    private float headToward = 0F;
    private float currentHead = 0F;
    private float nodToward = 0F;
    private float currentNod = 0F;

    void Start () {
        if (!target) target = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
        currentHead = Mathf.Lerp(currentHead, headToward, Time.deltaTime);
        if (Mathf.Abs(currentHead - headToward) <= 0.01F)
        {
            headToward = Random.Range(-70F, 70F);
        }
        currentNod = Mathf.Lerp(currentNod, nodToward, Time.deltaTime);
        if (Mathf.Abs(currentNod - nodToward) <= 0.01F)
        {
            nodToward = Random.Range(-20F, 20F);
        }
        transform.LookAt(target);
        transform.Rotate(localOffsetAxis, localOffsetAngle, Space.Self);
        transform.Rotate(Vector3.forward, currentHead, Space.Self);
        transform.Rotate(Vector3.right, currentNod, Space.Self);
    }
}
