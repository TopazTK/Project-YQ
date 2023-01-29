using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class Helpers
{
	public static byte[] SingleInstruction<T>(byte Opcode, T Input)
	{
		List<byte> _output = new List<byte>() { Opcode };
		
		_output.AddRange(Input.GetBytes());
		return _output.ToArray();
	}
	
	public static byte[] MultiInstruction<T>(byte Opcode, T[] Input)
	{
		List<byte> _output = new List<byte>() { Opcode };
		
		foreach (var _v in Input)
		_output.AddRange(_v.GetBytes());
		
		return _output.ToArray();
	}
}
