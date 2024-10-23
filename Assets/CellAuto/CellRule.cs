using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class CellRule {
    public Bit64x4[] survival;
    public Bit64x4[] birth;

    public CellRule(string str) {
        List<uint> sur = new List<uint>();
        List<uint> bir = new List<uint>();
        List<uint> link = null;
        for (int i = 0; i < str.Length; ++ i) {
            switch (str[i]) {
            case 'B':
            case 'b':
                link = bir;
                break;
            case 'S':
            case 's':
                link = sur;
                break;
            case '/':
                link = null;
                break;
            default:
                if ('0' <= str[i] && str[i] <= '9') {
                    if (link != null) {
                        link.Add((uint)(str[i] - '0'));
                    }
                }
                break;
            }
        }
        survival = new Bit64x4[sur.Count];
        birth = new Bit64x4[bir.Count];
        
        for (int i = 0; i < sur.Count; ++ i) {
            survival[i] = Bit64x4.CreateEquals(sur[i]);
        }
        for (int i = 0; i < bir.Count; ++ i) {
            birth[i] = Bit64x4.CreateEquals(bir[i]);
        }
    }
}


