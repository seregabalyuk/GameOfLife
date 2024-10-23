using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunPause : MonoBehaviour {
    [SerializeField]
    Sprite Pause;
    [SerializeField]
    Sprite Run;


    UnityEngine.UI.Image image;
    // Start is called before the first frame update
    void Start() {
        image = GetComponent<UnityEngine.UI.Image>();
        image.sprite = Pause;
    }

    // Update is called once per frame
    public void ChangeState(bool isPause) {
        if (isPause) {
            image.sprite = Pause;
        } else {
            image.sprite = Run;
        }
    }
}
