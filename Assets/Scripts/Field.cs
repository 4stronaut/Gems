using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
    public Field up = null;
    public Field down = null;
    public Field left = null;
    public Field right = null;
    public Gem gem = null;
    public bool explored = false;
    public List<Field> path;
    public int checkedRound = 0;
    public bool initialized = false;
    public bool highlighted = false;

    private void Start()
    {
        path = new List<Field>();
    }

    public void Activate()
    {
        highlighted = true;
        GetComponentInChildren<MeshRenderer>().material.color = Color.gray;
    }

    public void Deactivate()
    {
        highlighted = false;
        GetComponentInChildren<MeshRenderer>().material.color = Color.white;
    }

    public void Init()
    {
        initialized = true;
        up = GetNeighbourField(Vector3.forward);
        down = GetNeighbourField(Vector3.back);
        left = GetNeighbourField(Vector3.left);
        right = GetNeighbourField(Vector3.right);
        if (up)
        {
            up.down = this;
            if (!up.initialized) up.Init();
        }
        if (down)
        {
            down.up = this;
            if (!down.initialized) down.Init();
        }
        if (left)
        {
            left.right = this;
            if (!left.initialized) left.Init();
        }
        if (right)
        {
            right.left = this;
            if (!right.initialized) right.Init();
        }
    }

    private Field GetNeighbourField(Vector3 dir)
    {
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position + dir + Vector3.up * 5F, Vector3.down), out hit, 10F))
        {
            if (hit.transform.tag == "Field")
            {
                return hit.transform.GetComponent<Field>();
            }
        }
        return null;
    }
}
