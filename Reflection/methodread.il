IL_0000: nop 
IL_0001: ldc.i4.5 
IL_0002: stloc.0 
IL_0003: ldloc.0 
IL_0004: stloc.2 
IL_0005: ldloc.2 
IL_0006: stloc.1 
IL_0007: ldloc.1 
IL_0008: ldc.i4.2 
IL_0009: beq.s IL_0015
IL_000b: br.s IL_000d
IL_000d: ldloc.1 
IL_000e: ldc.i4.3 
IL_000f: beq.s IL_0013
IL_0011: br.s IL_0017
IL_0013: br.s IL_0017
IL_0015: br.s IL_0017
IL_0017: ldc.i4.s 55
IL_0019: stloc.s System.Int32 (4)
IL_001b: ldc.i4.1 
IL_001c: brtrue.s IL_001f
IL_001e: nop 
IL_001f: br.s IL_0021
IL_0021: ldc.i4.0 
IL_0022: stloc.3 
IL_0023: br.s IL_0025
IL_0025: ldc.i4.1 
IL_0026: brtrue.s IL_0029
IL_0028: nop 
IL_0029: ldloc.3 
IL_002a: stloc.s System.Int32 (5)
IL_002c: br.s IL_002e
IL_002e: ldloc.s System.Int32 (5)
IL_0030: ret 
