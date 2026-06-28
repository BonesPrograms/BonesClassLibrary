IL_0000: nop 
IL_0001: newobj instance void TokenTest::.ctor()
IL_0006: stloc.0 
IL_0007: ldloca.s System.ValueTuple`2[System.Int32,System.Int32] (1)
IL_0009: ldc.i4.1 
IL_000a: ldc.i4.1 
IL_000b: call instance void ValueTuple<Int32, Int32>::.ctor(Int32, Int32)
IL_0010: ldc.i4 11111111
IL_0015: call static uint128 UInt128::op_Implicit(UInt32)
IL_001a: stloc.2 
IL_001b: ldstr ""
IL_0020: stloc.3 
IL_0021: ldc.i4 231244
IL_0026: stloc.s System.Int32 (4)
IL_0028: ldloca.s System.DateTime (5)
IL_002b: initobj struct DateTime
IL_0030: ret 
