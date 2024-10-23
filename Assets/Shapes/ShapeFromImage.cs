using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShapeFromImage : MonoBehaviour {
    MouseControler controller;
    List<Vector2Int> points;
    // Start is called before the first frame update
    void Start() {}

    static public List<Vector2Int> CreatePoints(Texture2D texture) {
        var image = texture;
        var points = new List<Vector2Int>();
        var center = new Vector2Int(image.width, image.height);
        center /= -2;
        for (int y = 0; y < image.height; ++ y) {
            for (int x = 0; x < image.width; ++ x) {
                var color = image.GetPixel(x, y);
                if (!(color.a < 0.1 || color.g < 0.1 && color.b < 0.1 && color.r < 0.1)) {
                    points.Add(center + new Vector2Int(x, y));
                }
            }
        }
        return points;
    }

    public void SetTexture(Texture2D texture) {
        points = CreatePoints(texture);
    }

    public void SetMouseController(MouseControler mouse) {
        controller = mouse;
    }

    public void Click() {
        controller.SetListOfPoints(points);
    }

    public List<Vector2Int> GetPoints() {
        return points;
    }
}
