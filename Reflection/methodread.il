IL_0000: nop  ||  OpCodeBytes: 0 0 
IL_0001: newobj instance void Gen<T>::.ctor() ||  OpCodeBytes: 115 0  :: Token: 167772195  TokenBytes: 35 0 0 10 
IL_0006: starg.s Gen`1[T] gen ||  OpCodeBytes: 16 0 
IL_0008: ldc.i4 1402105491 ||  OpCodeBytes: 32 0  OperandBytes: 147 110 146 83 
IL_000d: starg.s Int32 num ||  OpCodeBytes: 16 0 
IL_000f: ldloca.s System.ValueTuple`2[System.Int32,System.Char] (0) ||  OpCodeBytes: 18 0 
IL_0011: ldc.i4.1  ||  OpCodeBytes: 23 0 
IL_0012: ldc.i4.s 97 ||  OpCodeBytes: 31 0 
IL_0014: call instance void ValueTuple<Int32, Char>::.ctor(Int32, Char) ||  OpCodeBytes: 40 0  :: Token: 167772196  TokenBytes: 36 0 0 10 
IL_0019: ldstr "Test:_field" ||  OpCodeBytes: 114 0  :: Token: 1879048219  TokenBytes: 27 0 0 112 
IL_001e: call static fieldinfo AccessTools::Field(String) ||  OpCodeBytes: 40 0  :: Token: 167772197  TokenBytes: 37 0 0 10 
IL_0023: stloc.1  ||  OpCodeBytes: 11 0 
IL_0024: ldloca.s BonesClassLibrary.Reflection.AccessModifiers (2) ||  OpCodeBytes: 18 0 
IL_0026: ldloc.1  ||  OpCodeBytes: 7 0 
IL_0027: call instance void AccessModifiers::.ctor(FieldInfo) ||  OpCodeBytes: 40 0  :: Token: 100663314  TokenBytes: 18 0 0 6 
IL_002c: ldloca.s BonesClassLibrary.Reflection.AccessModifiers (3) ||  OpCodeBytes: 18 0 
IL_002e: ldloc.1  ||  OpCodeBytes: 7 0 
IL_002f: call instance void AccessModifiers::.ctor(FieldInfo) ||  OpCodeBytes: 40 0  :: Token: 100663314  TokenBytes: 18 0 0 6 
IL_0034: ldloc.3  ||  OpCodeBytes: 9 0 
IL_0035: stloc.2  ||  OpCodeBytes: 12 0 
IL_0036: newobj instance void List<Object>::.ctor() ||  OpCodeBytes: 115 0  :: Token: 167772198  TokenBytes: 38 0 0 10 
IL_003b: stloc.s System.Collections.Generic.List`1[System.Object] (4) ||  OpCodeBytes: 19 0 
IL_003d: newobj instance void List<Object>::.ctor() ||  OpCodeBytes: 115 0  :: Token: 167772198  TokenBytes: 38 0 0 10 
IL_0042: stloc.s System.Collections.Generic.List`1[System.Object] (5) ||  OpCodeBytes: 19 0 
IL_0044: ldloc.s System.Collections.Generic.List`1[System.Object] (5) ||  OpCodeBytes: 17 0 
IL_0046: stloc.s System.Collections.Generic.List`1[System.Object] (4) ||  OpCodeBytes: 19 0 
IL_0048: ret  ||  OpCodeBytes: 42 0 
