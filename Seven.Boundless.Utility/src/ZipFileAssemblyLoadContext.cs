namespace Seven.Boundless.Utility;

using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System;

/// <summary>
/// An AssemblyLoadContext that loads assemblies and unmanaged DLLs from a Zip archive.
/// </summary>
public sealed class ZipFileAssemblyLoadContext : AssemblyLoadContext, IDisposable {
	private ZipArchive _zipArchive;
	private readonly DirectoryPath _assemblyPath;

	/// <param name="zipStream">Stream of the Zip file</param>
	/// <param name="assemblyPath">Path of the Assembly to load</param>
	public ZipFileAssemblyLoadContext(Stream zipStream, DirectoryPath assemblyPath) : base(isCollectible: true) {
		_zipArchive = new(zipStream, ZipArchiveMode.Read, true);
		_assemblyPath = assemblyPath;

		Unloading += ALC => {
			if (ALC == this) Dispose();
		};
	}

	/// <inheritdoc/>
	~ZipFileAssemblyLoadContext() {
		Dispose(false);
	}
	/// <inheritdoc/>
	public void Dispose() {
		Dispose(true);
		GC.SuppressFinalize(this);
	}
	private void Dispose(bool disposing) {
		if (disposing) {
			_zipArchive?.Dispose();
		}
		_zipArchive = null!;
	}

	/// <inheritdoc cref="ZipArchive.GetEntry(string)"/>
	public ZipArchiveEntry? GetEntry(FilePath filePath) {
		return _zipArchive.GetEntry(filePath);
	}

	/// <inheritdoc/>
	protected override Assembly? Load(AssemblyName assemblyName) {
		try {
			Assembly? defaultAssembly = Assembly.Load(assemblyName);
			return defaultAssembly;
		}
		catch {
			FilePath assemblyFilePath = _assemblyPath.CombineFile($"{assemblyName.Name}.dll");
			if (GetEntry(assemblyFilePath) is not ZipArchiveEntry entry) return null;

			using MemoryStream memoryStream = entry.ToMemoryStream();

			return LoadFromStream(memoryStream);
		}
	}

	/// <inheritdoc/>
	protected override nint LoadUnmanagedDll(string unmanagedDllName) {
		FilePath assemblyFilePath = _assemblyPath.CombineFile($"{unmanagedDllName}.dll");
		if (GetEntry(assemblyFilePath) is not ZipArchiveEntry entry) return 0;

		string tempPath = Path.GetTempFileName();

		using (Stream stream = entry.Open()) {
			using FileStream fileStream = new(tempPath, FileMode.Create);

			stream.CopyTo(fileStream);
		}

		nint loaded = LoadUnmanagedDllFromPath(tempPath);
		File.Delete(tempPath);

		return loaded;
	}
}