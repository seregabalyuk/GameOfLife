using System;

public struct Bit64x3 {
    Bit64 array1;
    Bit64 array2;
    Bit64 array3;

    public Bit64x3(
        Bit64 arr1,
        Bit64 arr2,
        Bit64 arr3
    ) {
        array1 = arr1;
        array2 = arr2;
        array3 = arr3;
    }

    public Bit64x3(
        UInt64 arr1 = 0,
        UInt64 arr2 = 0,
        UInt64 arr3 = 0
    ) {
        array1 = new Bit64(arr1);
        array2 = new Bit64(arr2);
        array3 = new Bit64(arr3);
    }
    
    public static Bit64x3 CreateEquals(uint n) {
        return new Bit64x3(
            (n & 1) > 0 ? ~0UL: 0UL,
            (n & 2) > 0 ? ~0UL: 0UL,
            (n & 4) > 0 ? ~0UL: 0UL
        );
    }

    public static Bit64x4 operator+(
        Bit64x3 left,
        Bit64x3 right
    ) {
        Bit64 c1 = left.array1 & right.array1;
        Bit64 c2 = c1 & (left.array2 | right.array2) | (left.array2 & right.array2);
        Bit64 c3 = c2 & (left.array3 | right.array3) | (left.array3 & right.array3);
        return new Bit64x4(
            left.array1 ^ right.array1,
            c1 ^ left.array2 ^ right.array2,
            c2 ^ left.array3 ^ right.array3,
            c3
        );
    }
}

