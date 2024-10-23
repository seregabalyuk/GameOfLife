using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class FlagWatcher : MonoBehaviour {
    [SerializeField]
    MyScreen screen;
    public Vector2Int posInScreen;
    [SerializeField]
    RectTransform spaceGame;
    [SerializeField]
    Transform flag;
    [SerializeField]
    SpriteRenderer spriteR;

    // Update is called once per frame
    void Update() {
        Vector2 pos = screen.GetPosPixel(posInScreen);
        var viewPos = spaceGame.InverseTransformPoint(Camera.main.WorldToScreenPoint(pos));
        if (spaceGame.rect.Contains(viewPos)) {
            transform.position = pos;
            transform.rotation = Quaternion.identity;
            flag.rotation = Quaternion.identity;
        } else {
            var to = viewPos;
            var from = new Vector3(0, 0, 0);
            for (int i = 0; i < 10; ++ i) {
                var medium = (to + from) / 2;
                if (spaceGame.rect.Contains(medium)) {
                    from = medium;
                } else {
                    to = medium;
                }
            }
            transform.position = Camera.main.ScreenToWorldPoint(spaceGame.TransformPoint(from));
            var shift = viewPos - from; 
            float angle = Mathf.Atan2(shift.y, shift.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
            flag.rotation = Quaternion.identity;
        }
    }
  
  // public methods
    public Color GetColor() {
        return spriteR.color;
    }
}
