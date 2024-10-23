using System;

public struct Bit64x2 {
    Bit64 array1;
    Bit64 array2;
    public Bit64x2(Bit64 arr1, Bit64 arr2) {
        array1 = arr1;
        array2 = arr2;
    }

    public Bit64x2(
        UInt64 arr1 = 0, 
        UInt64 arr2 = 0
    ) {
        array1 = new Bit64(arr1);
        array2 = new Bit64(arr2);
    }
  // operator >> << |
    public static Bit64x2 operator >>(
        Bit64x2 bit64x2,
        int i
    ) {
        return new Bit64x2(
            bit64x2.array1 >> i,
            bit64x2.array2 >> i
        );
    }

    public static Bit64x2 operator <<(
        Bit64x2 bit64x2,
        int i
    ) {
        return new Bit64x2(
            bit64x2.array1 << i,
            bit64x2.array2 << i
        );
    }

    public static Bit64x2 operator |(
        Bit64x2 left,
        Bit64x2 right
    ) {
        return new Bit64x2(
            left.array1 | right.array1,
            left.array2 | right.array2
        );
    }
  // operator +
    public static Bit64x3 operator+(
        Bit64x2 left,
        Bit64x2 right
    ) {
        Bit64 c = left.array1 & right.array1;
        Bit64 arr2 = left.array2 & right.array2;
        return new Bit64x3(
            left.array1 ^ right.array1,
            c ^ left.array2 ^ right.array2,
            c & (left.array2 | right.array2) | arr2
        );
    }

    public static Bit64x2 Sum(Bit64 a,Bit64 b, Bit64 c) {
        return new Bit64x2(
            a ^ b ^ c,
            a & (b | c) | (b & c)
        );
    }
  //operator []
    public int this[int i] {
        get { 
            return (array1[i] ? 1 : 0) + 2 * (array2[i] ? 1 : 0);
        }
        set { 
            array1[i] = (value & 1) != 0;
            array2[i] = (value & 2) != 0;
        }
    }
}