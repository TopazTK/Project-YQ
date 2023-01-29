using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

public static class Extensions
{
	public static byte ToByte(this string Input) => Input.Contains("0x") ? Convert.ToByte(Input, 16) : Convert.ToByte(Input);
	public static ushort ToShort(this string Input) => Input.Contains("0x") ? Convert.ToUInt16(Input, 16) : Convert.ToUInt16(Input);
	
	public static byte[] GetBytes<T>(this T Input)
	{
		var _dynoMethod = new DynamicMethod("SizeOfType", typeof(int), new Type[] { });
		ILGenerator _ilGen = _dynoMethod.GetILGenerator();
		
		_ilGen.Emit(OpCodes.Sizeof, typeof(T));
		_ilGen.Emit(OpCodes.Ret);
		
		var _inSize = (int)_dynoMethod.Invoke(null, null);
		int _inWrite = 0;
		
		if (_inSize > 1)
		{
			var _inArray = (byte[])typeof(BitConverter).GetMethod("GetBytes", new[] { typeof(T) }) .Invoke(null, new object[] { Input });
			return _inArray;
		}
		
		else
		{
			var _inArray = new byte[] { (byte)Convert.ChangeType(Input, typeof(byte)) };
			return _inArray;
		}
	}
}
