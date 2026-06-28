IL_0000: nop 
IL_0001: ldc.i4.0 
IL_0002: stloc.0 
IL_0003: ldarg.1 
IL_0004: ldnull 
IL_0006: cgt.un 
IL_0007: stloc.1 
IL_0008: ldloc.1 
IL_0009: brfalse.s IL_0018
IL_000b: ldarg.1 
IL_000c: ldstr "
"
IL_0011: call static string String::Concat(String, String)
IL_0016: starg.s System.String msg
IL_0018: br IL_00a4
IL_001d: nop 
IL_001e: call static void Console::Clear()
IL_0023: nop 
IL_0024: ldarg.1 
IL_0025: ldstr "SELECT OPTION (ARROW KEYS) WITH ENTER KEY:"
IL_002a: call static string String::Concat(String, String)
IL_002f: call static void Console::WriteLine(String)
IL_0034: nop 
IL_0035: ldarg.0 
IL_0036: ldloc.0 
IL_0037: ldarg.2 
IL_0038: ldarg.3 
IL_0039: call static void ConsoleHelper::Loop<T>(IList<T>, Int32, Func<T, String>, Boolean)
IL_003e: nop 
IL_003f: call static consolekeyinfo Console::ReadKey()
IL_0044: stloc.3 
IL_0045: ldloca.s System.ConsoleKeyInfo (3)
IL_0047: call instance consolekey ConsoleKeyInfo::get_Key()
IL_004c: stloc.2 
IL_004d: ldloc.2 
IL_004e: stloc.s System.ConsoleKey (5)
IL_0050: ldloc.s System.ConsoleKey (5)
IL_0052: stloc.s System.ConsoleKey (4)
IL_0054: ldloc.s System.ConsoleKey (4)
IL_0056: ldc.i4.s 13
IL_0058: beq.s IL_0078
IL_005a: br.s IL_005c
IL_005c: ldloc.s System.ConsoleKey (4)
IL_005e: ldc.i4.s 38
IL_0060: beq.s IL_006c
IL_0062: br.s IL_0064
IL_0064: ldloc.s System.ConsoleKey (4)
IL_0066: ldc.i4.s 40
IL_0068: beq.s IL_0072
IL_006a: br.s IL_007d
IL_006c: ldloc.0 
IL_006d: ldc.i4.1 
IL_006e: sub 
IL_006f: stloc.0 
IL_0070: br.s IL_007d
IL_0072: ldloc.0 
IL_0073: ldc.i4.1 
IL_0074: add 
IL_0075: stloc.0 
IL_0076: br.s IL_007d
IL_0078: ldloc.0 
IL_0079: stloc.s System.Int32 (6)
IL_007b: br.s IL_00af
IL_007d: ldloc.0 
IL_007e: ldc.i4.0 
IL_0080: clt 
IL_0081: stloc.s System.Boolean (7)
IL_0083: ldloc.s System.Boolean (7)
IL_0085: brfalse.s IL_0092
IL_0087: ldarg.0 
IL_0088: callvirt instance int32 ICollection<T>::get_Count()
IL_008d: ldc.i4.1 
IL_008e: sub 
IL_008f: stloc.0 
IL_0090: br.s IL_00a6
IL_0092: ldloc.0 
IL_0093: ldarg.0 
IL_0094: callvirt instance int32 ICollection<T>::get_Count()
IL_009a: clt 
IL_009b: ldc.i4.0 
IL_009d: ceq 
IL_009e: stloc.s System.Boolean (8)
IL_00a0: ldloc.s System.Boolean (8)
IL_00a2: brfalse.s IL_00a6
IL_00a4: ldc.i4.0 
IL_00a5: stloc.0 
IL_00a6: nop 
IL_00a7: ldc.i4.1 
IL_00a8: stloc.s System.Boolean (9)
IL_00aa: br IL_001a
IL_00af: ldloc.s System.Int32 (6)
IL_00b1: ret 
