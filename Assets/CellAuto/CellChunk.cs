using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

class CellChunk8x8 {
    const UInt64 colomn = 
        (1UL + (1UL<<8)) * 
        (1UL + (1UL<<16)) * 
        (1UL + (1UL<<32));
    const UInt64 row = ((1UL<<8) - 1UL);

    public const int size = 8;
    
    public Bit64 prev;
    public Bit64 next;
    Bit64x2 sumLR;
    Bit64x4 sumNeig;

    static CellChunk8x8 zero = new CellChunk8x8();

    public CellChunk8x8() {
        prev = new Bit64(0);
        next = new Bit64(0);
        sumLR = new Bit64x2(0, 0);
        sumNeig = new Bit64x4(0, 0, 0, 0);
        
    }

    Bit64 GetU(CellChunk8x8 other) {
        return (other.prev >> (8 * 7)) | (prev << 8);
    }
    Bit64 GetD(CellChunk8x8 other) {
        return (other.prev << (8 * 7)) | (prev >> 8);
    }
    Bit64 GetL(CellChunk8x8 other) {
        Bit64 mask = new Bit64(colomn);
        return
            ((other.prev >> 7) & mask) | 
            ((prev << 1) & ~mask);
    }
    Bit64 GetR(CellChunk8x8 other) {
        Bit64 mask = new Bit64(colomn << 7);
        return
            ((other.prev << 7) & mask) | 
            ((prev >> 1) & ~mask);
    }

    void Print(Bit64x2 v) {
        string s = "";
        for (int i = 0; i < 64; ++ i) {
            s += (char)('0' + v[i]);
            if (i % 8 == 7) {
                s += '\n';
            }
        }
        Debug.Log(s);
    }

    void Print(Bit64x4 v) {
        string s = "";
        for (int i = 0; i < 64; ++ i) {
            s += (char)('0' + v[i]);
            if (i % 8 == 7) {
                s += '\n';
            }
        }
        Debug.Log(s);
    }

    void Print(Bit64 v) {
        string s = "";
        for (int i = 0; i < 64; ++ i) {
            s += (char)('0' + (v[i] ? 1 : 0));
            if (i % 8 == 7) {
                s += '\n';
            }
        }
        Debug.Log(s);
    }

    public bool IsZero() {
        return prev.IsAllZero();
    }

    public void StepLRSum(CellChunk8x8 left, CellChunk8x8 right) {
        if (left == null) {
            left = zero;
        }
        if (right == null) {
            right = zero;
        }
        sumLR = GetL(left) + GetR(right);
    }

    public void StepNeigSum(CellChunk8x8 up, CellChunk8x8 down) {
        if (up == null) {
            up = zero;
        }
        if (down == null) {
            down = zero;
        }
        var sumUD = GetU(up) + GetD(down);
        var sumDiag = ((up.sumLR >> (8 * 7)) | (sumLR << 8)) + ((down.sumLR << (8 * 7)) | (sumLR >> 8));
        sumNeig = sumUD + sumLR + sumDiag;
    }

    public void StepApplyRule(CellRule rules) {
        next = new Bit64();
        foreach(var rule in rules.survival) {
            next |= Bit64x4.IsEqual(rule, sumNeig) & prev;
        }
        foreach(var rule in rules.birth) {
            next |= Bit64x4.IsEqual(rule, sumNeig) & ~prev;
        }
    }

    public void StepLoadNext() {
        prev = next;
    }

    public bool this[int x, int y] {
        get {
            return prev[x + y * 8];
        }
        set {
            prev[x + y * 8] = value;
        }
    }

    public bool this[Vector2Int pos] {
        get {
            return prev[pos.x + pos.y * 8];
        }
        set {
            prev[pos.x + pos.y * 8] = value;
        }
    }

    public struct Mask {
        Bit64 bits;
        public Mask(Bit64 _bits) {
            bits = _bits;
        }
        public Mask(UInt64 _bits = 0) {
            bits = new Bit64(_bits);
        }

        public bool Empty() {
            return bits.IsAllZero();
        }

        public bool this[Vector2Int pos] {
            get {
                return bits[pos.x + pos.y * 8];
            }
            set {
                bits[pos.x + pos.y * 8] = value;
            }
        }
        public bool this[int x, int y] {
            get {
                return bits[x + y * 8];
            }
            set {
                bits[x + y * 8] = value;
            }
        }
    }

    public Mask GetMask() {
        return new Mask(prev);
    }
}

// not used
struct CellChunk4x4 {
    const UInt64 colomnL =
        ((1UL << 4) - 1UL) *
        (1UL + (1UL<<16)) * 
        (1UL + (1UL<<32));
    const UInt64 colomnR = colomnL << 12;

    static CellChunk4x4 zero = new CellChunk4x4();

    UInt64 state;
    UInt64 sum;

    static UInt64 GetL(UInt64 centre, UInt64 left) {
        return
            ((left >> 12) & colomnL) | 
            ((centre << 4) & ~colomnL);
    }
    /*static UInt64 GetR(UInt64 centre, UInt64 left) {
        return
            ((other << 12) & colomnR) | 
            ((state >> 4) & ~colomnR);
    }*/

    

    public void StepLRSum(ref CellChunk4x4? left, ref CellChunk4x4? right) {
        if (left == null) {
            left = zero;
        }
        if (right == null) {
            right = zero;
        }
        //sum = GetL(left.Value.state) + GetR(right.Value.state) + state;
    }

    public void StepNeigSum(ref CellChunk4x4? up, ref CellChunk4x4? down) {
        if (up == null) {
            up = zero;
        }
        if (down == null) {
            down = zero;
        }
        //sum += ;
    }
}

