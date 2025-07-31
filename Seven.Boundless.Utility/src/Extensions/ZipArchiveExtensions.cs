using System.IO;
using System.IO.Compression;

namespace Seven.Boundless.Utility;

public static class ZipArchiveExtensions {

	public static MemoryStream ToMemoryStream(this ZipArchiveEntry entry) {
		using Stream stream = entry.Open();
		MemoryStream memoryStream = new();
		stream.CopyTo(memoryStream);
		memoryStream.Position = 0;

		return memoryStream;
	}
}