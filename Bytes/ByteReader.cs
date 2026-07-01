//using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives; //binaryprimitives is preferred to bitconverter cause it is more efficient and little endians for me
using System.Numerics;
using static BonesClassLibrary.Bytes.ByteReader;
using System.Numerics;

namespace BonesClassLibrary.Bytes;

public static class ByteExtensions
{
        //AsBytes converts to Little Endian, it matches the format that IL stores bytes as
        //i may add a bool option for bigendian    
    public static byte[] AsBytes(this UInt128 uin) => uin.AsBytesInternal();
    public static byte[] AsBytes(this Int128 num) => num.AsBytesInternal();
    public static byte[] AsBytes(this nuint nuin) => nuin.AsBytesInternal();
    public static byte[] AsBytes(this nint nin) => nin.AsBytesInternal();
    public static byte[] AsBytes(this ulong ul) => ul.AsBytesInternal();
    public static byte[] AsBytes(this uint uin) => uin.AsBytesInternal();
    public static byte[] AsBytes(this double float64) => float64.AsBytesInternal();
    public static byte[] AsBytes(this long int64) => int64.AsBytesInternal();
    public static byte[] AsBytes(this float float32) => float32.AsBytesInternal();
    public static byte[] AsBytes(this int num) => num.AsBytesInternal();
    public static byte[] AsBytes(this Half half) => half.AsBytesInternal();
    public static byte[] AsBytes(this ushort shrt) => shrt.AsBytesInternal();
    public static byte[] AsBytes(this short shrt) => shrt.AsBytesInternal();
    public static byte[] AsBytes(this char chr) => chr.AsBytesInternal();

    static byte[] AsBytesInternal<T>(this T num) where T : INumber<T>
    {
        byte[] bytes = [];

        switch (num)
        {
            case byte bites:
                bytes = new byte[1];
                bytes[0] = bites;
                break;
            case char utf16:
                bytes = new byte[sizeof(char)]; // 2 bytes
                WriteUInt16LittleEndian(bytes, utf16);
                break;
            case short sint16:
                bytes = new byte[sizeof(short)]; // 2 bytes
                WriteInt16LittleEndian(bytes, sint16);
                break;
            case ushort uint16:
                bytes = new byte[sizeof(ushort)]; // 2 bytes
                WriteUInt16LittleEndian(bytes, uint16);
                break;
            case Half float16:
                bytes = new byte[x16bit];
                WriteHalfLittleEndian(bytes, float16);
                break;
            case int sint32:
                bytes = new byte[sizeof(int)]; // 4 bytes
                WriteInt32LittleEndian(bytes, sint32);
                break;
            case uint uint32:
                bytes = new byte[sizeof(uint)]; // 4 bytes
                WriteUInt32LittleEndian(bytes, uint32);
                break;
            case float float32:
                bytes = new byte[sizeof(float)]; // 4 bytes
                WriteSingleLittleEndian(bytes, float32);
                break;
            case long sint64:
                bytes = new byte[sizeof(long)]; // 8 bytes
                WriteInt64LittleEndian(bytes, sint64);
                break;
            case ulong uint64:
                bytes = new byte[sizeof(ulong)]; // 8 bytes
                WriteUInt64LittleEndian(bytes, uint64);
                break;
            case double float64:
                bytes = new byte[sizeof(double)]; // 8 bytes
                WriteDoubleLittleEndian(bytes, float64);
                break;
            case Int128 sint128:
                bytes = new byte[x128bit]; //absolutely massive
                WriteInt128LittleEndian(bytes, sint128);
                break;
            case UInt128 uint128:
                bytes = new byte[x128bit];
                WriteUInt128LittleEndian(bytes, uint128);
                break;
            case nint sint32or64:
                if (IntPtr.Size == x32bit)
                {
                    bytes = new byte[x32bit];
                    WriteInt32LittleEndian(bytes, (int)sint32or64);
                }
                else if (IntPtr.Size == x64bit)
                {
                    bytes = new byte[x64bit];
                    WriteInt64LittleEndian(bytes, sint32or64);
                }
                else
                    throw new PlatformNotSupportedException("Must be 32bit or 64bit process");
                break;
            case nuint uint32or64:
                if (!Environment.Is64BitProcess)
                {
                    bytes = new byte[x32bit];
                    WriteUInt32LittleEndian(bytes, (uint)uint32or64);
                }
                else if (Environment.Is64BitProcess)
                {
                    bytes = new byte[x64bit];
                    WriteUInt64LittleEndian(bytes, uint32or64);
                }
                else
                    throw new PlatformNotSupportedException("Must be 32bit or 64bit process");
                break;
        }
        if (bytes.Length == 0)
            throw new NotSupportedException("Numeric type not supported by BinaryPrimitives.");
        return bytes;
    }
}
public static class ByteReader
{
    /// <summary>
    /// 0 bytes.
    /// </summary>
    public const byte x0bit = 0x00;

    /// <summary>
    /// 1 byte.
    /// </summary>
    public const byte x8bit = 0x01;
    /// <summary>
    /// 2 bytes.
    /// </summary>
    public const byte x16bit = 0x02;
    /// <summary>
    /// 4 bytes.
    /// </summary>
    public const byte x32bit = 0x04;
    /// <summary>
    /// 8 bytes.
    /// </summary>
    public const byte x64bit = 0x08;

    public const byte x128bit = 0x16;
}

