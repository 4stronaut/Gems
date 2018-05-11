using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {
    public ushort gemType = 0;
    public bool selected = false;
    public bool moving = false;
    public Field field = null;
    public List<Field> path = null;

    private float _movingTime;


    public void SetGemType(ushort t)
    {
        gemType = t;
        //TODO type specific init

        Material m = GetComponentInChildren<MeshRenderer>().material;

        switch(gemType)
        {
            case 0: m.color = Color.red;  break;
            case 1: m.color = Color.green; break;
            case 2: m.color = Color.blue; break;
            case 3: m.color = Color.yellow; break;
            case 4: m.color = Color.cyan; break;
            case 5: m.color = Color.magenta; break;
            default: m.color = Color.black; break;
        }
    }

	public void Activate () {
        selected = true;
	}
	
	public void Deactivate () {
        selected = false;
        transform.localScale = Vector3.one;
    }

    public void Move()
    {
        moving = true;
        _movingTime = 0f;
        foreach (Field f in path) f.Activate();
    }

    private void Update()
    {
        if (selected)
        {
            transform.localScale = new Vector3(1F, Mathf.Sin(Time.time)*0.3F+0.7F, 1F);
        }
        if (moving)
        {
            _movingTime += Time.deltaTime *4F;
            int curIdx = Mathf.Min(Mathf.FloorToInt(_movingTime), path.Count - 1);
            transform.position = path[curIdx].transform.position;
            if (curIdx == path.Count - 1)
            {
                moving = false;
                foreach (Field f in path) f.Deactivate();
                field.gem = null;
                field = path[path.Count - 1];
                field.gem = this;
            }
        }
    }

}
