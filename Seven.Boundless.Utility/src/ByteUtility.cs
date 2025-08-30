using System;

namespace Seven.Boundless.Utility;

public static class ByteUtility {
	public static class PaddingBoundaries {
		public const uint CPUWord = 4;
		public const uint CacheLine = 64;
		public const uint AESBlock = 16;
		public const uint DiskSector = 512;
		public const uint FileSystemBlock = 4096;
		public const uint GLSL = 16;
	}

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

	/// <summary>
	/// Pads <paramref name="size"/> up to the next multiple of <paramref name="alignment"/>.
	/// Works with arbitrary (non power-of-two) boundary sizes.
	/// </summary>
	/// <param name="size">The value to pad.</param>
	/// <param name="alignment">The alignment boundary (in bytes).</param>
	/// <returns>
	/// The smallest multiple of <paramref name="alignment"/> greater than or equal to <paramref name="size"/>.
	/// </returns>
	public static uint AlignUpArbitrary(uint size, uint alignment) {
		uint remainder = size % alignment;
		return size + (remainder == 0 ? 0 : alignment - remainder);
	}

	/// <summary>
	/// Faster implementation of <see cref="AlignUpArbitrary(uint, uint)"/> for boundary sizes
	/// that are powers of two. Uses bit manipulation instead of division/modulo.
	/// </summary>
	/// <param name="size">The value to pad.</param>
	/// <param name="alignment">The alignment boundary (must be a power of two).</param>
	/// <returns>
	/// The smallest multiple of <paramref name="alignment"/> greater than or equal to <paramref name="size"/>.
	/// </returns>
	public static uint AlignUp(uint size, uint alignment) {
		return (size + alignment - 1) & ~(alignment - 1);
	}

	/// <inheritdoc cref="AlignUpArbitrary(uint, uint)"/>
	public static ulong AlignUpArbitrary(ulong size, ulong alignment) {
		ulong remainder = size % alignment;
		return size + (remainder == 0 ? 0 : alignment - remainder);
	}

	/// <inheritdoc cref="AlignUp(uint, uint)"/>
	public static ulong AlignUp(ulong size, ulong alignment) {
		return (size + alignment - 1) & ~(alignment - 1);
	}
}