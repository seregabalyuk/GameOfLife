using System;
using Unity.Burst.CompilerServices;

public struct Bit64 {
    UInt64 array;

    public Bit64(Bit64 other) {
        array = other.array;
    }
    public Bit64(UInt64 bits = 0) {
        array = bits;
    }

    public bool IsAllZero() {
        return array == 0;
    }

  // bit operation ^ & | ~ << >>
    public static Bit64 operator^(
        Bit64 left,
        Bit64 right
    ) {
        return new Bit64(
            left.array ^ right.array
        );
    }

    public static Bit64 operator&(
        Bit64 left,
        Bit64 right
    ) {
        return new Bit64(
            left.array & right.array
        );
    }

    public static Bit64 operator|(
        Bit64 left,
        Bit64 right
    ) {
        return new Bit64(
            left.array | right.array
        );
    }  
    public static Bit64 operator~(
        Bit64 bit64
    ) {
        return new Bit64(
            ~bit64.array
        );
    } 
    public static Bit64 operator>>(
        Bit64 bit64,
        int i
    ) {
        return new Bit64(bit64.array >> i);
    }
    public static Bit64 operator<<(
        Bit64 bit64,
        int i
    ) {
        return new Bit64(bit64.array << i);
    }
  // operation +
    public static Bit64x2 operator+(
        Bit64 left,
        Bit64 right
    ) {
        return new Bit64x2(
            left ^ right,
            left & right
        );
    }
  // operator []
    public bool this[int i] {
        get { return (array & (1UL << i)) != 0; }
        set { 
            if (value) {
                array |= 1UL << i;
            } else {
                array &= ~(1UL << i);
            }
        }
    }
    
}
