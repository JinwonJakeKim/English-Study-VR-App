using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class object_change : MonoBehaviour {
    public GameObject on, off;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void change()
    {
        on.SetActive(true);
        off.SetActive(false);
    }
}
