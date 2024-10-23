using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellAutomation : MonoBehaviour {
    [SerializeField]
    string nameRules;
    Dictionary<Vector2Int, CellChunk8x8> chunks = new Dictionary<Vector2Int, CellChunk8x8>();
    CellRule rule;

  // static metods
    static int Pos2Chunk(int x) {
        const uint div = CellChunk8x8.size; // need bias % div == 0
        const uint bias = (1U << 31) / div * div; 
        const uint rbias = bias / div;
        return (int)(((uint)(x) + bias) / div - rbias);
    }

    static Vector2Int Pos2Chunk(Vector2Int pos) {
        return new Vector2Int(Pos2Chunk(pos.x), Pos2Chunk(pos.y));
    }

    static int PosInChunk(int x) {
        const uint div = CellChunk8x8.size; // need bias % div == 0
        const uint bias = (1U << 31) / div * div; 
        return (int)(((uint)(x) + bias) % div);
    }

    static Vector2Int PosInChunk(Vector2Int pos) {
        return new Vector2Int(PosInChunk(pos.x), PosInChunk(pos.y));
    }
  // help metods
    CellChunk8x8 GetOrCreateChunk(Vector2Int pos) {
        var chunkPos = Pos2Chunk(pos);
        if (!chunks.ContainsKey(chunkPos)) {
            chunks.Add(chunkPos, new CellChunk8x8());
        }
        return chunks[chunkPos];
    }

    void StepAddNewChunks() {
        var neig = new Vector2Int[] {
            new Vector2Int(-1, -1),
            new Vector2Int(-1,  0),
            new Vector2Int(-1,  1),
            new Vector2Int( 0, -1),
            new Vector2Int( 0,  1),
            new Vector2Int( 1, -1),
            new Vector2Int( 1,  0),
            new Vector2Int( 1,  1)
        };
        var newChunks = new List<(Vector2Int, CellChunk8x8)>();
        foreach (var pos in chunks.Keys) {
            foreach (var shift in neig) {
                if (!chunks.ContainsKey(pos + shift)) {
                    newChunks.Add((pos + shift, new CellChunk8x8()));
                }
            }
        }
        foreach (var (pos, chunk) in newChunks) {
            if (!chunks.ContainsKey(pos)) {
                chunks.Add(pos, chunk);
            }
        }
    }

    void StepLRSum() {
        foreach (var value in chunks) {
            var pos = value.Key;
            var chunk = value.Value;
            var posL = new Vector2Int(pos.x - 1, pos.y);
            var posR = new Vector2Int(pos.x + 1, pos.y);
            CellChunk8x8 chunkL = null;
            chunks.TryGetValue(posL, out chunkL);
            CellChunk8x8 chunkR = null;
            chunks.TryGetValue(posR, out chunkR);
            chunk.StepLRSum(chunkL, chunkR);
        }
    }

    void StepNeigSum() {
        foreach (var value in chunks) {
            var pos = value.Key;
            var chunk = value.Value;
            var posU = new Vector2Int(pos.x, pos.y - 1);
            var posD = new Vector2Int(pos.x, pos.y + 1);
            CellChunk8x8 chunkU = null;
            chunks.TryGetValue(posU, out chunkU);
            CellChunk8x8 chunkD = null;
            chunks.TryGetValue(posD, out chunkD);
            chunk.StepNeigSum(chunkU, chunkD);
        }
    }

    void StepApplyRule() {
        foreach (var chunk in chunks.Values) {
            chunk.StepApplyRule(rule);
        }
    }

    void StepLoadNext() {
        foreach (var chunk in chunks.Values) {
            chunk.StepLoadNext();
        }
    }

    void StepDeleteChunks() {
        var poses = new List<Vector2Int>();
        foreach (var pair in chunks) {
            if (pair.Value.IsZero()) {
                poses.Add(pair.Key);
            }
        }
        foreach (var pos in poses) {
            chunks.Remove(pos);
        }
    }

  // public metods
    public void NextStep() {
        StepAddNewChunks();
        StepLRSum();
        StepNeigSum();
        StepApplyRule();
        StepLoadNext();
        StepDeleteChunks();
    }

    public void Draw(InfoPOS infoPOS) {
        var pos = infoPOS.pos;
        var screen = infoPOS.screen;
        var width = screen.pixelSize.x;
        var height = screen.pixelSize.y;

        var from = Pos2Chunk(pos);
        var numChunks = Pos2Chunk(screen.pixelSize);
        var to = from +  numChunks;
        var map = new CellChunk8x8.Mask[numChunks.x * numChunks.y];
        int mapIter = 0;
        
        for (int ych = from.y; ych < to.y; ++ ych) {
            for (int xch = from.x; xch < to.x; ++ xch) {
                var chunkPos = new Vector2Int(xch, ych);
                if (chunks.ContainsKey(chunkPos)) {
                    map[mapIter ++] = chunks[chunkPos].GetMask();
                } else {
                    map[mapIter ++] = new CellChunk8x8.Mask();
                }
            }
        }
        //Debug.Log(map.Length);
        
        var data = screen.GetPixelData();
        var color = new Color32(255, 0, 0, 255);

        
        int dataIter = screen.GetPosPixelInData(0, 0);

        //Debug.Log((from, to));
        for (int ych = 0; ych < numChunks.y; ++ ych) {
            for (int y = 0; y < CellChunk8x8.size; ++ y) {
                mapIter = numChunks.x * ych;
                for (int xch = 0; xch < numChunks.x; ++ xch, ++ mapIter) {
                    if (!map[mapIter].Empty()) {
                        for (int x = 0; x < CellChunk8x8.size; ++ x) {
                            if (map[mapIter][x, y]) {
                                data[dataIter] = color;
                            }
                            ++ dataIter;
                        }
                    } else {
                        dataIter += 8;
                    }
                }
            }
        }
    }

    public bool this[Vector2Int pos] {
        set {
            if (value) {
                var chunk = GetOrCreateChunk(pos);
                chunk[PosInChunk(pos)] = true;
            } else {
                var chunkPos = Pos2Chunk(pos);
                if (chunks.ContainsKey(chunkPos)) {
                    chunks[chunkPos][PosInChunk(pos)] = false;
                }
            }
        }
        get {
            var chunkPos = Pos2Chunk(pos);
            if (chunks.ContainsKey(chunkPos)) {
                return chunks[chunkPos][PosInChunk(pos)];
            }
            return false;
        }
    }
  
    public void Clear() {
        chunks.Clear();
    }
  // unity metods
    void Start() {
        rule = new CellRule(nameRules);
    }
}
