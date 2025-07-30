namespace Seven.Boundless.Utility;

using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using System.IO.Compression;

public sealed class ZipFileAssemblyLoadContext : AssemblyLoadContext {
	private readonly ZipArchive _zipArchive;
	private readonly DirectoryPath _assemblyPath;

	public ZipFileAssemblyLoadContext(ZipArchive zipArchive, DirectoryPath assemblyPath) : base(isCollectible: true) {
		_zipArchive = zipArchive;
		_assemblyPath = assemblyPath;
	}

	/// <inheritdoc/>
	protected override Assembly? Load(AssemblyName assemblyName) {
		try {
			Assembly? defaultAssembly = Assembly.Load(assemblyName);
			return defaultAssembly;
		}
		catch {
			FilePath assemblyFilePath = _assemblyPath.CombineFile($"{assemblyName.Name}.dll");
			if (_zipArchive.GetEntry(assemblyFilePath) is not ZipArchiveEntry entry) return null;

			using Stream stream = entry.Open();

			return LoadFromStream(stream);
		}
	}

	/// <inheritdoc/>
	protected override nint LoadUnmanagedDll(string unmanagedDllName) {
		FilePath assemblyFilePath = _assemblyPath.CombineFile($"{unmanagedDllName}.dll");
		if (_zipArchive.GetEntry(assemblyFilePath) is not ZipArchiveEntry entry) return 0;

		using Stream stream = entry.Open();

		string tempPath = Path.GetTempFileName();
		using (FileStream fileStream = new(tempPath, FileMode.Create)) {
			stream.CopyTo(fileStream);
		}

		nint loaded = LoadUnmanagedDllFromPath(tempPath);
		File.Delete(tempPath);

		return loaded;
	}
}