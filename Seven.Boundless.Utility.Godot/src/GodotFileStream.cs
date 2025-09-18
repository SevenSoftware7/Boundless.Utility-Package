namespace Seven.Boundless.Utility;

using System;
using System.IO;

public class GodotFileStream : Stream {
	private bool _disposed = false;

	private Godot.FileAccess _file;
	private readonly bool _canRead;
	private readonly bool _canWrite;

	public GodotFileStream(FilePath path, Godot.FileAccess.ModeFlags mode) {
		if (path.Directory.Protocol != "user" && path.Directory.Protocol != "res")
			throw new ArgumentException("Path must be a user or resource path.", nameof(path));
		if (string.IsNullOrEmpty(path.Path))
			throw new ArgumentException("Path cannot be empty.", nameof(path));


		_file = Godot.FileAccess.Open(path, mode);
		if (_file == null) throw new IOException($"Failed to open file: {path} ({Godot.FileAccess.GetOpenError()})");

		_canRead = mode.HasFlag(Godot.FileAccess.ModeFlags.Read);
		_canWrite = mode.HasFlag(Godot.FileAccess.ModeFlags.Write);
	}

	/// <inheritdoc/>
	public override bool CanRead => _canRead;
	/// <inheritdoc/>
	public override bool CanSeek => true;
	/// <inheritdoc/>
	public override bool CanWrite => _canWrite;
	/// <inheritdoc/>
	public override long Length => (long)_file.GetLength();

	/// <inheritdoc/>
	public override long Position {
		get => (long)_file.GetPosition();
		set => _file.Seek((ulong)value);
	}

	/// <inheritdoc/>
	public override void Flush() {
		// Godot's FileAccess does not require explicit flushing.
	}

	/// <inheritdoc/>
	public override int Read(byte[] buffer, int offset, int count) {
		if (!_canRead)
			throw new NotSupportedException("Stream does not support reading.");

		ArgumentNullException.ThrowIfNull(buffer);

		if (offset < 0 || offset >= buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds.");

		if (count < 0 || (offset + count) > buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(count), "Count is out of bounds.");

		byte[] bytes = _file.GetBuffer(count);
		int bytesRead = bytes.Length;
		bytes.CopyTo(buffer, offset);
		return bytesRead;
	}

	/// <inheritdoc/>
	public override void Write(byte[] buffer, int offset, int count) {
		if (!_canWrite)
			throw new NotSupportedException("Stream does not support writing.");

		ArgumentNullException.ThrowIfNull(buffer);

		if (offset < 0 || offset >= buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(offset), "Offset is out of bounds.");

		if (count < 0 || (offset + count) > buffer.Length)
			throw new ArgumentOutOfRangeException(nameof(count), "Count is out of bounds.");

		byte[] bytes = new byte[count];
		Array.Copy(buffer, offset, bytes, 0, count);
		_file.StoreBuffer(bytes);
	}

	/// <inheritdoc/>
	public override long Seek(long offset, SeekOrigin origin) {
		long newPos = origin switch {
			SeekOrigin.Begin => offset,
			SeekOrigin.Current => (long)_file.GetPosition() + offset,
			SeekOrigin.End => (long)_file.GetLength() + offset,
			_ => throw new ArgumentException("Invalid SeekOrigin", nameof(origin)),
		};
		_file.Seek((ulong)newPos);
		return (long)_file.GetPosition();
	}

	/// <inheritdoc/>
	public override void SetLength(long value) {
		if (!_canWrite)
			throw new NotSupportedException("Stream does not support writing.");

		if (value < 0)
			throw new ArgumentOutOfRangeException(nameof(value), "Length cannot be negative.");

		_file.Resize(value);
		if (_file.GetPosition() > (ulong)value) {
			_file.Seek((ulong)value);
		}
	}

	/// <inheritdoc/>
	protected override void Dispose(bool disposing) {
		if (_disposed) return;
		_disposed = true;

		base.Dispose(disposing);
		if (disposing) {
			_file.Dispose();
		}

		_file = null!;
	}
}