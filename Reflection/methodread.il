IL_0000: nop 
IL_0001: ldc.i4.s 96
IL_0003: stloc.0 
IL_0004: ldc.i4.1 
IL_0005: stloc.s Boolean (8)
IL_0007: nop 
IL_0008: nop 
IL_0009: br.s IL_000b
IL_000b: ldc.i4.m1 
IL_000c: stloc.1 
IL_000d: ldc.i4.m1 
IL_000e: stloc.2 
IL_000f: ldc.i4.s -2
IL_0011: stloc.3 
IL_0012: ldc.i4.s -3
IL_0014: stloc.s UInt32 (4)
IL_0016: ldc.i4.s -126
IL_0018: stloc.s UInt32 (5)
IL_001a: ldc.i4 -999990002 ||  OperandBytes: 14 93 101 196 
IL_001f: stloc.s UInt32 (6)
IL_0021: ldc.i4 999999999 ||  OperandBytes: 255 201 154 59 
IL_0026: stloc.s UInt32 (7)
IL_0028: ldarg.0 
IL_0029: ldc.i4.s -3
IL_002b: call instance void Test::ShortTaker(UInt32) ||  :: Token: 100663303  TokenBytes: 7 0 0 6 
IL_0030: nop 
IL_0031: ret 
