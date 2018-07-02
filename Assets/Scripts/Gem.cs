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
    private AudioSource _sound;


    public void SetGemType(ushort t)
    {
        gemType = t;
        //TODO type specific init

        Material m = GetComponentInChildren<SkinnedMeshRenderer>().materials[1];

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
        _sound.PlayOneShot(_sound.clip);
        switch (gemType)
        {
            case 0:field.Activate(Color.red); break;
            case 1:field.Activate(Color.green); break;
            case 2:field.Activate(Color.blue); break;
            case 3:field.Activate(Color.yellow); break;
            case 4:field.Activate(Color.cyan); break;
            case 5: field.Activate(Color.magenta); break;
            default: break;
        }
    }
	
	public void Deactivate () {
        selected = false;
        field.Deactivate();
        //transform.localScale = Vector3.one;
    }

    public void Move()
    {
        moving = true;
        _movingTime = 0f;
        foreach (Field f in path) f.Activate(Color.gray);
    }

    private void Start()
    {
        transform.GetChild(0).transform.Rotate(Vector3.up, Random.Range(90F, 270F));
        _sound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (moving)
        {
            _movingTime += Time.deltaTime *4F;
            int curIdx = Mathf.Min(Mathf.FloorToInt(_movingTime), path.Count - 1);
            transform.position = path[curIdx].transform.position;
            if (curIdx == path.Count - 1)
            {
                foreach (Field f in path) f.Deactivate();
                field.gem = null;
                field = path[path.Count - 1];
                field.gem = this;
                moving = false;
            }
        }
    }

}
