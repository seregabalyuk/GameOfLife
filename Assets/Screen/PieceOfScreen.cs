using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

public class PieceOfScreen : MonoBehaviour {
    Texture2D texture;
    SpriteRenderer sr;
    Vector2Int origin;

    public float pixelsPerUnit {
        get {
            return sr.sprite.pixelsPerUnit;
        }
    }
    // Start is called before the first frame update
    void Start() {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTexture(Texture2D inTexture, Rect view, Vector2 size) {
        sr = GetComponent<SpriteRenderer>();
        texture = inTexture;
        float sx = view.size.x;
        float sy = view.size.y;
        float spritePerPixel = (sx + sy) / 2;
        sr.sprite = Sprite.Create(
            texture,
            view,
            new Vector2(0.5f,0.5f), 
            spritePerPixel
        );
        transform.localScale = new Vector2(
            (spritePerPixel * size.x) / sx,
            (spritePerPixel * size.y) / sy
        );
        origin.x = (int)view.position.x;
        origin.y = (int)view.position.y;
    }

    public Vector2 GetPosInImage(Vector3 pos) {
        Vector2 real = transform.InverseTransformPoint(pos) * sr.sprite.pixelsPerUnit;
        real += sr.sprite.pivot;
        return real;
    }

    public Vector3 GetPosFromImage(Vector2 pos) {
        pos -= sr.sprite.pivot;
        pos /= sr.sprite.pixelsPerUnit;
        return transform.TransformPoint(pos);
    }

    public bool IsInImage(Vector2 pos) {
        return sr.sprite.rect.Contains(pos);
    }

    public NativeArray<Color32> GetPixelData() {
        return texture.GetRawTextureData<Color32>();
    }

    public int GetPosPixelInData(int x, int y) {
        return x + origin.x + (y + origin.y) * texture.width;
    }

    public void SetPixel(int x, int y, Color32 color) {
        var data = texture.GetRawTextureData<Color32>();
        data[x + origin.x + (y + origin.y) * texture.width] = color;
        //texture.SetPixel(x + origin.x, y + origin.y, color);
    }

    public void Fill(Color32 color) {
        Rect rect = sr.sprite.textureRect; 
        int from = texture.width * (int)Mathf.Round(rect.yMin) + (int)Mathf.Round(rect.xMin);
        int to   = texture.width * (int)Mathf.Round(rect.yMax) + (int)Mathf.Round(rect.xMax);
        var data = texture.GetRawTextureData<Color32>();
        for (int i = from; i < to; ++ i) {
            data[i] = color;
        }
        
        /*for(int y = (int)Mathf.Round(rect.yMin); y < (int)Mathf.Round(rect.yMax); ++ y) {
            for(int x = (int)Mathf.Round(rect.xMin); x < (int)Mathf.Round(rect.xMax); ++ x) {
                texture.SetPixel(x, y, color);
            }
        }*/
    }

    public Color GetPixel(int x, int y) {
        return texture.GetPixel(x + origin.x, y + origin.y);
    }

    public Vector2 size {
        get {
            return new Vector2(
                transform.localScale.x * sr.sprite.textureRect.width / sr.sprite.pixelsPerUnit,
                transform.localScale.y * sr.sprite.textureRect.height / sr.sprite.pixelsPerUnit
            );
        }
    }

    public Vector2Int pixelSize {
        get {
            return new Vector2Int(
                (int)Mathf.Round(sr.sprite.textureRect.width),
                (int)Mathf.Round(sr.sprite.textureRect.height)
            );
        }
    }
}
