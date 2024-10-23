using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void Exit() {
        Application.Quit();
    }

    public void Play() {
        SceneManager.LoadScene("War");
    }

    public void Sandbox() {
        SceneManager.LoadScene("Sandbox");
    }

    public void ToMenu() {
        SceneManager.LoadScene("Menu");
    }
}
