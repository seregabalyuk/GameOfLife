using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    [SerializeField]
    AudioSource background;
    static MusicController main = null;

    void Awake() {
        if (main == null) {
            main = this;
            DontDestroyOnLoad(gameObject);
        } else if (main != this) {
            Destroy(gameObject);
        }
        
        
    }
    void Start() {
        background.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
