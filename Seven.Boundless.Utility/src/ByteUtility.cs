using System;

namespace Seven.Boundless.Utility;

public static class ByteUtility {
	public static byte[] CreateByteBuffer(float[] floats) {
		byte[] bytes = new byte[floats.Length * sizeof(float)];
		Buffer.BlockCopy(floats, 0, bytes, 0, bytes.Length);
		return bytes;
	}
	public static byte[] CreateByteBuffer(double[] doubles) {
		byte[] bytes = new byte[doubles.Length * sizeof(double)];
		Buffer.BlockCopy(doubles, 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static byte[] CreateByteBuffer(int[] ints) {
		byte[] bytes = new byte[ints.Length * sizeof(int)];
		Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);
		return bytes;
	}
	public static byte[] CreateByteBuffer(uint[] uints) {
		byte[] bytes = new byte[uints.Length * sizeof(uint)];
		Buffer.BlockCopy(uints, 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static byte[] CreateByteBuffer(short[] shorts) {
		byte[] bytes = new byte[shorts.Length * sizeof(short)];
		Buffer.BlockCopy(shorts, 0, bytes, 0, bytes.Length);
		return bytes;
	}
	public static byte[] CreateByteBuffer(ushort[] ushorts) {
		byte[] bytes = new byte[ushorts.Length * sizeof(ushort)];
		Buffer.BlockCopy(ushorts, 0, bytes, 0, bytes.Length);
		return bytes;
	}

	public static byte[] CreateByteBuffer(long[] longs) {
		byte[] bytes = new byte[longs.Length * sizeof(long)];
		Buffer.BlockCopy(longs, 0, bytes, 0, bytes.Length);
		return bytes;
	}
	public static byte[] CreateByteBuffer(ulong[] ulongs) {
		byte[] bytes = new byte[ulongs.Length * sizeof(ulong)];
		Buffer.BlockCopy(ulongs, 0, bytes, 0, bytes.Length);
		return bytes;
	}
}