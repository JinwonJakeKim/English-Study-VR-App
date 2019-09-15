using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class out_function : MonoBehaviour {
    public GameObject origin, change;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void NewtoOrigin()
    {
        change.SetActive(false);
        origin.SetActive(true);
    }
}
