using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class function : MonoBehaviour
{
    public GameObject on, off;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnMouseOver()
    {
        on.SetActive(false);
        off.SetActive(true);
    }

    void OnMouseExit()
    {
        on.SetActive(true);
        off.SetActive(false);

    }

    public void OnMouseDown()
    {
        SceneManager.LoadScene(1);
    }


}
