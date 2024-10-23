using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine.Events;
using TMPro;

public class MouseControler : MonoBehaviour {
    [SerializeField]
    MyScreen screen;
    [SerializeField]
    CellAutomation automation;
    [SerializeField]
    float scroolSpeed = 20;
    [SerializeField]
    GameObject mouse;
    [SerializeField]
    RectTransform rectTransform;
    [SerializeField]
    UnityEvent<MouseControler> clickLimit;

    List<Vector2Int> points = new List<Vector2Int>(){new Vector2Int(0, 0)};
    Vector3 prevPosMouse = Vector3.zero;
    SpriteRenderer mouseSR;
    Vector2Int posPixel = new Vector2Int();
    bool canDraw = true;
    
    // Start is called before the first frame update
    void Start() {
        mouseSR = mouse.GetComponent<SpriteRenderer>();
        CreateMouse();
    }

    void CreateMouse() {
        int x1 = 0, x2 = 0;
        int y1 = 0, y2 = 0;
        foreach(var point in points) {
            x1 = Math.Min(x1, point.x);
            x2 = Math.Max(x2, point.x);
            y1 = Math.Min(y1, point.y);
            y2 = Math.Max(y2, point.y);
        }
        int width = x2 - x1 + 1;
        int height = y2 - y1 + 1;

        var texture = new Texture2D(
            width,
            height
        );
        texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;

        

        var data = texture.GetRawTextureData<Color32>();
        var color = new Color32(0, 0, 0, 0);
        for (int i = 0; i < data.Length; ++ i) {
            data[i] = color;
        }

        color = new Color32(0, 0, 255, 128);
        foreach(var point in points) {
            texture.SetPixel(point.x - x1, point.y - y1, color);
        }
        
        mouseSR.sprite = Sprite.Create(
            texture,
            new Rect(0, 0, 
                width,
                height
            ),
            new Vector2(
                (0.5f - x1)/ width,
                (0.5f - y1)/ height
            ),
            screen.pixelsPerUnit
        );
        texture.Apply();

        mouse.transform.rotation = Quaternion.identity;
    }


    Vector3 GetMousePos() {
        return Camera.main.ScreenToWorldPoint(
            Input.mousePosition
        );
    }

    void UpdateMoveMap() {
        var pos = GetMousePos();
        if (Input.GetMouseButton(1)) {
            screen.Move(pos-prevPosMouse);
        }
        prevPosMouse = pos;
    }

    void UpdateScroll() {
        Vector2 scrollDelta = Input.mouseScrollDelta;
        if (scrollDelta.y != 0) {
            Vector2 shift = screen.transform.position - GetMousePos();
            float scale = Mathf.Pow(scroolSpeed, scrollDelta.y);
            screen.transform.localScale *= scale;
            if (screen.transform.localScale.x < 1) {
                screen.transform.localScale = new Vector3(1, 1, 1);
            } else {
                screen.Move(shift * scale - shift);
            }
        }
    }

    void UpdateClick() {
        var pos = GetMousePos();
        if (Input.GetMouseButtonDown(0)) {
            posPixel = screen.GetPixelPos(pos);
            canDraw = true;
            clickLimit.Invoke(this);
            if (canDraw) {
                foreach (var point in points) {
                    automation[posPixel + point] = !automation[posPixel + point];
                }
            }
        }
    }

    void UpdateScreen() {
        screen.Fill(new Color(0, 0, 0, 0));
        foreach(var infoPOS in screen.pieces) {
            automation.Draw(infoPOS);
        }
        screen.Apply();

    }
    
    void ChangePoints() {
        if (Input.GetKeyDown(KeyCode.R)) {
            for (int i = 0; i < points.Count; ++ i) {
                points[i] = new Vector2Int(
                    - points[i].y,
                    points[i].x
                );
            }
            mouse.transform.rotation = Quaternion.Euler(0, 0, 90) * mouse.transform.rotation;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            for (int i = 0; i < points.Count; ++ i) {
                points[i] = new Vector2Int(
                    - points[i].x,
                    points[i].y
                );
            }
            mouse.transform.rotation = Quaternion.Euler(0, 180, 0) * mouse.transform.rotation;
        }
    }



    // Update is called once per frame
    void Update() {
        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);
        if (rectTransform.rect.Contains(localMousePosition)) {
            UpdateMoveMap();
            UpdateClick();
            UpdateScroll();
        }
        UpdateScreen();
        ChangePoints();
        var pos = GetMousePos();
        pos.z = 0;

        mouse.transform.position = screen.Align2Pixels(pos);
        

        mouse.transform.localScale = screen.transform.localScale;
       // Debug.Log(Mathf.Round(1f / Time.deltaTime));
    }

  // public methods
    public void SetListOfPoints(List<Vector2Int> points) {
        this.points = points;
        CreateMouse();
    }

    public List<Vector2Int> GetPoints() {
        return points;
    }

    public Vector2Int GetPosPixel() {
        return posPixel;
    }

    public void CanDraw(bool answer) {
        canDraw &= answer;
    }


}