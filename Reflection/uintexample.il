.method /* 06000003 */ public hidebysig 
	instance void Method () cil managed 
{
	// Method begins at RVA 0x2080
	// Header size: 12
	// Code size: 40 (0x28)
	.maxstack 1
	.locals /* 11000001 */ init (
		[0] uint32,
		[1] uint32,
		[2] uint32,
		[3] uint32,
		[4] uint32,
		[5] uint32,
		[6] int32,
		[7] int32
	)

	IL_0000: nop
	IL_0001: ldc.i4.m1
	IL_0002: stloc.0
	IL_0003: ldc.i4.m1
	IL_0004: stloc.1
	IL_0005: ldc.i4.s -2
	IL_0007: stloc.2
	IL_0008: ldc.i4.s -3
	IL_000a: stloc.3
	IL_000b: ldc.i4 -999990002
	IL_0010: stloc.s 4
	IL_0012: ldc.i4 999999999
	IL_0017: stloc.s 5
	IL_0019: ldc.i4 2147483647
	IL_001e: stloc.s 6
	IL_0020: ldc.i4 2147483647
	IL_0025: stloc.s 7
	IL_0027: ret
} // end of method Test::Method

//uint plan: so i think the issue is that im casting the byte to an int and maybe i should use a byteconverter, but i should only use that if i am seeing
//specific opcodes? idk ldci4s and stloc are normal opcodes lol


//fix - i made sure all 8 bit operands are casted as bytes, well see if that works
//speaking of which, i am pretty sure i fixed the jump label bug by casting to object, but i think that the bug may reppaear
//for 8 bit jumps, so i should probably make a separate operand result for an 8 bit jump that casts to byte - i need to test that and see how that works