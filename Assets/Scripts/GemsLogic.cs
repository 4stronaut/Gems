using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GemsLogic : MonoBehaviour {

    public ushort rows = 9;
    public ushort cols = 9;
    public ushort numColors = 6;
    public ushort gemsPerRound = 3;
    public ushort neededInRow = 5;
    private int _round = 0;
    private bool _waitForGem = false;
    private int _score = 0;
    private int _bonusMulti = 1;

    public GameObject basePrefab;
    public GameObject gemPrefab;

    public GameObject startMenu;
    public GameObject lostMenu;
    public Text scoreDisplay;
    public AudioSource music;
    public Image mute;

    private Gem _activeGem;
    private List<Field> _fields;
    private List<Gem> _gems;

    private Camera _mainCam;

    void Start()
    {
        _mainCam = FindObjectOfType<Camera>();
        _fields = new List<Field>();
        _gems = new List<Gem>();
    }

    public void Mute()
    {
        music.mute = !music.mute;
        mute.color = music.mute ? Color.white : new Color(0f, 0f, 0f, 0f); 
        foreach (Gem g in _gems) g.GetComponent<AudioSource>().mute = music.mute;
    }

    public void Restart()
    {
        Debug.Log("RESTART");        
        Destroy(startMenu.gameObject);
        lostMenu.SetActive(false);
        foreach (Gem g in _gems) Destroy(g.gameObject);
        foreach (Field f in _fields) Destroy(f.gameObject);
        _gems.Clear();
        _fields.Clear();
        _round = 0;
        _score = 0;
        _bonusMulti = 1;
        scoreDisplay.text = _score+" x"+_bonusMulti;
        StartCoroutine(GenerateLevel());        
    }

    void NewGems(int amount)
    {
        //Debug.Log(amount);
        _round++;
        List<Field> free = new List<Field>();
        for (int i = 0; i < _fields.Count; ++i)
        {
            if (_fields[i].gem == null) free.Add(_fields[i]);
        }
        //Debug.Log(free.Count + " free fields...");

        for (int i = 0; i < amount; ++i)
        {
            if (free.Count > 0)
            {
                int idx = Random.Range(0, free.Count);
                Field f = free[idx];
                Gem g = Instantiate(gemPrefab, f.transform.position, Quaternion.identity).GetComponent<Gem>();
                ushort type = (ushort)Random.Range(0, numColors);
                g.SetGemType(type);
                g.field = f;
                f.gem = g;
                _gems.Add(g);
                free.RemoveAt(idx);
            }
        }
        //possibly a row was finished by the new gems
        CheckField();
        ClearLines();
    }

    IEnumerator CheckLost()
    {
        yield return new WaitForSeconds(0.2F);
        //Debug.Log(_gems.Count + " birds on " + _fields.Count + " fields");
        if (_gems.Count >= _fields.Count) LoseGame();
        //else Debug.Log("Continue...");
        yield return null;
    }

    void LoseGame()
    {
        Debug.LogWarning("Game Lost");
        lostMenu.SetActive(true);
    }


    public List<Field> GetPath(Field start, Field target)
    {
        // reset explored state
        foreach (Field f in _fields)
        {
            f.explored = false;
        }
        List<Field> _queue = new List<Field>();
        _queue.Add(start);
        start.path.Clear();
        int step = 0;
        // solve queue
        while (_queue.Count > 0)
        {
            Field cur = _queue[0];
            _queue.RemoveAt(0);
            if (!cur.explored)
            {
                // explore current field
                step++;
                cur.explored = true;
                // stop when target found
                if (cur == target)
                {
                    //Debug.Log("Path found after " + step + " steps");
                    cur.path.Add(cur);
                    return cur.path;
                }
                if (cur.up && !cur.up.explored && !cur.up.gem)
                {
                    cur.up.path = new List<Field>(cur.path);
                    cur.up.path.Add(cur);
                    _queue.Add(cur.up);
                }
                if (cur.right && !cur.right.explored && !cur.right.gem)
                {
                    cur.right.path = new List<Field>(cur.path);
                    cur.right.path.Add(cur);
                    _queue.Add(cur.right);
                }
                if (cur.down && !cur.down.explored && !cur.down.gem)
                {
                    cur.down.path = new List<Field>(cur.path);
                    cur.down.path.Add(cur);
                    _queue.Add(cur.down);
                }
                if (cur.left && !cur.left.explored && !cur.left.gem)
                {
                    cur.left.path = new List<Field>(cur.path);
                    cur.left.path.Add(cur);
                    _queue.Add(cur.left);
                }
            }
        }
        Debug.Log("Path not found after " + step + " steps");
        return new List<Field>();
    }

    // counts lines and highlights fields
    private int CheckField()
    {
        int totalLines = 0;
        List<Gem> gs = new List<Gem>();
        Field f = null;
        ushort gcount = 0;

        //check left/right 
        foreach (Gem g in _gems) g.field.explored = false;
        foreach (Gem g in _gems)
        {           
            f = g.field;
            if (f.explored) continue;
            gcount = 0;
            gs.Clear();
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.right; }
            f = g.field.left;
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.left; }
            if (gcount >= neededInRow)
            {
                Debug.Log(gs.Count + " in a row");
                foreach (Gem l in gs) l.field.Activate(Color.yellow);
                totalLines++;
            }
            foreach (Gem l in gs) l.field.explored = true;
        }
        //check up/down
        foreach (Gem g in _gems) g.field.explored = false;
        foreach (Gem g in _gems)
        {
            f = g.field;
            if (f.explored) continue;
            gcount = 0;
            gs.Clear();
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.up; }
            f = g.field.down;
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.down; }
            if (gcount >= neededInRow)
            {
                Debug.Log(gs.Count + " in a column");
                foreach (Gem l in gs) l.field.Activate(Color.yellow);
                totalLines++;
            }
            foreach (Gem l in gs) l.field.explored = true;
        }
        //check diagonally/up
        foreach (Gem g in _gems) g.field.explored = false;
        foreach (Gem g in _gems)
        {
            f = g.field;
            if (f.explored) continue;
            gcount = 0;
            gs.Clear();
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.up; if (f) f = f.right; }
            f = g.field.down; if (f) f = f.left;
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.down; if (f) f = f.left; }
            if (gcount >= neededInRow)
            {
                Debug.Log(gs.Count + " diagonal");
                foreach (Gem l in gs) l.field.Activate(Color.red);
                totalLines++;
            }
            foreach (Gem l in gs) l.field.explored = true;
        }
        //check diagonally/down
        foreach (Gem g in _gems) g.field.explored = false;
        foreach (Gem g in _gems)
        {
            f = g.field;
            if (f.explored) continue;
            gcount = 0;
            gs.Clear();
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.up; if (f) f = f.left; }
            f = g.field.down; if (f) f = f.right;
            while (f && f.gem && f.gem.gemType == g.gemType) { gcount++; gs.Add(f.gem); f = f.down; if (f) f = f.right; }
            if (gcount >= neededInRow)
            {
                Debug.Log(gs.Count + " diagonal");
                foreach (Gem l in gs) l.field.Activate(Color.yellow);
                totalLines++;
            }
            foreach (Gem l in gs) l.field.explored = true;
        }
        return totalLines;
    }

    // removes gems on highlighted fields
    void ClearLines()
    {
        foreach(Field f in _fields)
        {
            if (f.highlighted)
            {
                //remove gem
                _score += _bonusMulti;
                _gems.Remove(f.gem);
                Destroy(f.gem.gameObject);
                f.gem = null;
                f.Deactivate();
            }
        }
        // update score display
        scoreDisplay.text = ""+_score;
        if(_bonusMulti>1)scoreDisplay.text += " x"+_bonusMulti;
    }


    void Update()
    {
        if (_waitForGem)
        {
            if (_activeGem.moving) return;
            _waitForGem = false;
            // deselect active
            _activeGem.Deactivate();
            _activeGem = null;
            int linescore = CheckField();
            if (linescore > 0)
            {
                Debug.Log("Lines cleared: " + linescore);
                _bonusMulti = Mathf.Max(linescore, _bonusMulti);
                ClearLines();
            }
            else
            {
                _bonusMulti = 1;
                NewGems(gemsPerRound);
                StartCoroutine(CheckLost());
            }
        }
        // Click or touch
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = FindObjectOfType<Camera>().ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                
                Transform objectHit = hit.transform;
                //Debug.Log("Hit @ "+objectHit.position+ " "+objectHit.name);   
                if (objectHit.gameObject.GetComponent<Gem>())
                {
                    if (_activeGem) _activeGem.Deactivate();
                    _activeGem = objectHit.gameObject.GetComponent<Gem>();
                    _activeGem.Activate();
                }
                else if(objectHit.gameObject.GetComponent<Field>()&&_activeGem)
                {
                    _activeGem.path = GetPath(_activeGem.field, objectHit.gameObject.GetComponent<Field>());
                    if (_activeGem.path.Count > 1)
                    {
                        _activeGem.Move();
                        _waitForGem = true;
                    }

                }
            }
        }
        //DEBUG KEYS
        //if (Input.GetKeyDown(KeyCode.R)) this.LoseGame();
	}

    IEnumerator GenerateLevel()
    {
        Field aField = null;
        // create the fields
        for(int i=0; i<rows; ++i)
        {
            for(int j=0; j<cols; ++j)
            {
                aField = Instantiate(basePrefab, Vector3.right * j + Vector3.forward * i, Quaternion.identity).GetComponent<Field>();
                aField.name = "Field" + j + "/" + i;
                _fields.Add(aField);
                yield return new WaitForSeconds(0.03f);
            }
        }
        yield return new WaitForSeconds(0.3f);
        // initialize recursive
        aField.Init();
        //_mainCam.transform.position = Vector3.right * cols * 0.5F + Vector3.up*rows;
        yield return new WaitForSeconds(0.3f);
        NewGems(gemsPerRound);
        yield return null;
    }
}
