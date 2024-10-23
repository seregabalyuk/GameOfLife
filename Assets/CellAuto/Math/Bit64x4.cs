using System;
using Unity.VisualScripting;

public struct Bit64x4 {
    Bit64 array1;
    Bit64 array2;
    Bit64 array3;
    Bit64 array4;
    public Bit64x4(
        Bit64 arr1,
        Bit64 arr2,
        Bit64 arr3,
        Bit64 arr4
    ) {
        array1 = arr1;
        array2 = arr2;
        array3 = arr3;
        array4 = arr4;
    }

    public Bit64x4(
        UInt64 arr1 = 0,
        UInt64 arr2 = 0,
        UInt64 arr3 = 0,
        UInt64 arr4 = 0
    ) {
        array1 = new Bit64(arr1);
        array2 = new Bit64(arr2);
        array3 = new Bit64(arr3);
        array4 = new Bit64(arr4);
    }

    public static Bit64x4 CreateEquals(uint n) {
        return new Bit64x4(
            (n & 1) > 0 ? ~0UL: 0UL,
            (n & 2) > 0 ? ~0UL: 0UL,
            (n & 4) > 0 ? ~0UL: 0UL,
            (n & 8) > 0 ? ~0UL: 0UL
        );
    }

    public static Bit64 IsNoEqual(in Bit64x4 left, in Bit64x4 right) {
        return 
            left.array1 ^ right.array1 |
            left.array2 ^ right.array2 |
            left.array3 ^ right.array3 |
            left.array4 ^ right.array4;
    }

    public static Bit64 IsEqual(in Bit64x4 left, in Bit64x4 right) {
        return ~IsNoEqual(left, right);
    }
  //operator []
    public int this[int i] {
        get { 
            return 
                (array1[i] ? 1 : 0)
                + 2 * (array2[i] ? 1 : 0)
                + 4 * (array3[i] ? 1 : 0)
                + 8 * (array4[i] ? 1 : 0);
        }
        set { 
            array1[i] = (value & 1) != 0;
            array2[i] = (value & 2) != 0;
            array3[i] = (value & 4) != 0;
            array4[i] = (value & 8) != 0;
        }
    }
}