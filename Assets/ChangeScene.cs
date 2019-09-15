using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ChangeScene_Jev_Restaurant()
    {
        SceneManager.LoadScene("Jev_1.8");
    }

    public void ChangeScene_Jev_Airport()
    {
        SceneManager.LoadScene("Jev_1.8");
    }

    public void ChangeScene_Jev_Hotel()
    {
        SceneManager.LoadScene("Jev_1.8");
    }

}
