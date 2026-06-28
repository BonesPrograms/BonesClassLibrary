//using static System.BitConverter;
using static System.Buffers.Binary.BinaryPrimitives; //binaryprimitives is preferred to bitconverter cause it is more efficient and little endians for me
using System.Numerics;
using static BonesClassLibrary.Bytes.ByteReader;

namespace BonesClassLibrary.Extensions;

public static class NumericExtensions
{
    /// <summary>
    /// Convert a number or char to bytes - only for types supported by BinaryPrimitives.
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>

    public static byte[] AsBytes<T>(this T num) where T : INumber<T>
    {
        byte[] bytes = [];

        switch (num)
        {
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
                    throw new PlatformNotSupportedException("Must be x32 or x64");
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
                    throw new PlatformNotSupportedException("Must be x32 or x64");
                break;
        }
        if (bytes.Length == 0)
            throw new NotSupportedException("Numeric type not supported by BinaryPrimitives.");
        return bytes;
    }
}