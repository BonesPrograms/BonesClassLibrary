 IL_0000: nop 
 IL_0001: ldstr ""
 IL_0006: stloc.0 
 IL_0007: ldloc.0 
 IL_0008: stloc.2 
 IL_0009: ldloc.2 
 IL_000a: stloc.1 
 IL_000b: ldloc.1 
 IL_000c: ldstr "a"
 IL_0011: call static void System.String::op_Equality(string, string)
 IL_0016: brtrue.s IL_0034: br.s IL_003a
 IL_0018: ldloc.1 
 IL_0019: ldstr "b"
 IL_001e: call static void System.String::op_Equality(string, string)
 IL_0023: brtrue.s IL_0036: br.s IL_003a
 IL_0025: ldloc.1 
 IL_0026: ldstr "c"
 IL_002b: call static void System.String::op_Equality(string, string)
 IL_0030: brtrue.s IL_0038: br.s IL_003a
 IL_0032: br.s IL_003a: ret
 IL_0034: br.s IL_003a: ret
 IL_0036: br.s IL_003a: ret
 IL_0038: br.s IL_003a: ret
 IL_003a: ret 
