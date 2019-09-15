using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class on_function : MonoBehaviour {
    public GameObject origin, change;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OrigintoNew()
    {
        origin.SetActive(false);
        change.SetActive(true);
    }

}
