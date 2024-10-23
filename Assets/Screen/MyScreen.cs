using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InfoPOS {
    public PieceOfScreen screen;
    public Vector2Int pos;
    public bool isChange = true;
    public InfoPOS(PieceOfScreen screen, Vector2Int pos) {
        this.screen = screen;
        this.pos = pos;
    }
    public void Checked() { isChange = false; }
}

public class MyScreen : MonoBehaviour {
    [SerializeField]
    PieceOfScreen reference;
    [SerializeField]
    Vector2Int countOfPlates;
    [SerializeField]
    Vector2Int pixelSizeOfPlate;
    [SerializeField]
    Vector2 sizeOfPlate;

    Texture2D texture;
    [HideInInspector]
    public List<InfoPOS> pieces;
    Rect outRect;

    public float pixelsPerUnit {
        get {
            return pixelSizeOfPlate.x / sizeOfPlate.x;
        }
    }

    // Start is called before the first frame update
    void Start() {
        pieces = new List<InfoPOS>();
        texture = new Texture2D(
            pixelSizeOfPlate.x,
            countOfPlates.x * countOfPlates.y * pixelSizeOfPlate.y
        );
        
        texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;

        outRect = new Rect(0, 0, 
            sizeOfPlate.x * countOfPlates.x,
            sizeOfPlate.y * countOfPlates.y
        );
        outRect.center = Vector2.zero;

        int id = 0;
        for (int x = 0; x < countOfPlates.x; ++ x) {
            for (int y = 0; y < countOfPlates.y; ++ y) {
                var pos = new Vector3(
                    sizeOfPlate.x * x,
                    sizeOfPlate.y * y
                );
                var pieceOfScreen = Instantiate(
                    reference,
                    pos + transform.position, 
                    Quaternion.identity,
                    transform
                );
                var pixelPos = new Vector2Int(
                    x * pixelSizeOfPlate.x,
                    y * pixelSizeOfPlate.y
                );
                pieces.Add(new InfoPOS(
                    pieceOfScreen, 
                    pixelPos
                ));
                
                pieceOfScreen.SetTexture(
                    texture,
                    new Rect(
                        new Vector2(
                            0,
                            id++ * pixelSizeOfPlate.y
                        ),
                        pixelSizeOfPlate
                    ),
                    sizeOfPlate
                );
            }
        }
        CorrectedPlates();
    }

    void CorrectedPlates() {
        foreach(var infoPos in pieces) {
            var trans = infoPos.screen.transform;            
            Vector3 move = Vector3.zero;
            Vector2Int movePixel = Vector2Int.zero;
            if (trans.localPosition.x < outRect.xMin) {
                move.x += outRect.width;
                movePixel.x += pixelSizeOfPlate.x * countOfPlates.x;
                infoPos.isChange = true;
            }
            if (trans.localPosition.x > outRect.xMax) {
                move.x -= outRect.width;
                movePixel.x -= pixelSizeOfPlate.x * countOfPlates.x;
                infoPos.isChange = true;
            }
            if (trans.localPosition.y < outRect.yMin) {
                move.y += outRect.height;
                movePixel.y += pixelSizeOfPlate.y * countOfPlates.y;
                infoPos.isChange = true;
            }
            if (trans.localPosition.y > outRect.yMax) {
                move.y -= outRect.height;
                movePixel.y -= pixelSizeOfPlate.y * countOfPlates.y;
                infoPos.isChange = true;
            }
            trans.localPosition += move;
            infoPos.pos += movePixel;
        }
    }
    
    public void Move(Vector3 shift) {
        shift = transform.InverseTransformVector(shift);
        foreach(var infoPos in pieces) {
            infoPos.screen.transform.localPosition += shift; 
        }
        CorrectedPlates();
    }
    
    public Vector2Int GetPixelPos(Vector3 pos) {
        var pos1 = pieces[0].screen.GetPosInImage(pos);
        Vector2Int ret = new Vector2Int((int)Mathf.Floor(pos1.x), (int)Mathf.Floor(pos1.y));
        return ret + pieces[0].pos;
    }

    public Vector3 GetPosPixel(Vector2Int pixel) {
        pixel -= pieces[0].pos;
        Vector2 pos = pixel;
        pos += new Vector2(0.5f, 0.5f);
        return pieces[0].screen.GetPosFromImage(pos);
    }

    public Vector3 Align2Pixels(Vector3 pos) {
        var pos1 = pieces[0].screen.GetPosInImage(pos);
        Vector2 ret = new Vector2(Mathf.Floor(pos1.x) + 0.5f, Mathf.Floor(pos1.y) + 0.5f);
        return pieces[0].screen.GetPosFromImage(ret);
    }
 
    public void Fill(Color32 color) {
        var data = texture.GetRawTextureData<Color32>();
        for (int i = 0; i < data.Length; ++ i) {
            data[i] = color;
        }
    }

    public void Apply() {
        texture.Apply();
    }

    void Update() {
        
    }
}
