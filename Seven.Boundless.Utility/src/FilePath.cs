namespace Seven.Boundless.Utility;

using System;

/// <summary>
/// Represents a file path with directory, file name, and extension components.
/// </summary>
public struct FilePath : IEquatable<FilePath> {
	/// <summary>
	/// Gets the directory path component of the file path.
	/// </summary>
	public DirectoryPath Directory { get; init; }

	/// <summary>
	/// Gets the file name component of the file path.
	/// </summary>
	public string FileName {
		readonly get => _fileName;
		init {
			_fileName = value;
			_fullFileName = null;
		}
	}
	private string _fileName = string.Empty;

	/// <summary>
	/// Gets the file extension component of the file path.
	/// </summary>
	public string Extension {
		readonly get => _extension;
		init {
			_extension = value;
			_fullFileName = null;
		}
	}
	private string _extension = string.Empty;

	/// <summary>
	/// Gets the full file name, including the extension if present.
	/// </summary>
	public string FullFileName => _fullFileName ??= string.IsNullOrEmpty(Extension) ? FileName : $"{FileName}.{Extension}";
	private string? _fullFileName;

	/// <summary>
	/// Gets the full path, including the directory and full file name.
	/// </summary>
	public string Path => _path ??= string.IsNullOrEmpty(Directory.Path) ? FullFileName : $"{Directory.Path}/{FullFileName}";
	private string? _path;

	/// <summary>
	/// Gets the URL representation of the file path.
	/// </summary>
	public string Url => _url ??= string.IsNullOrEmpty(Directory.Url) ? FullFileName : $"{Directory.Url}/{FullFileName}";
	private string? _url;


	/// <summary>
	/// Initializes a new instance of the <see cref="FilePath"/> struct with the specified directory and file name.
	/// </summary>
	/// <param name="directory">The directory path.</param>
	/// <param name="fileName">The file name.</param>
	public FilePath(DirectoryPath directory, string fileName) {
		Directory = directory;
		(FileName, Extension) = SplitFileName(fileName);
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="FilePath"/> struct with the specified directory, file name, and extension.
	/// </summary>
	/// <param name="directory">The directory path.</param>
	/// <param name="fileName">The file name.</param>
	/// <param name="extension">The file extension.</param>
	public FilePath(DirectoryPath directory, string fileName, string extension) {
		Directory = directory;
		FileName = fileName;
		Extension = extension;
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="FilePath"/> struct with the specified path.
	/// </summary>
	/// <param name="path">The full path.</param>
	public FilePath(string path) {
		int lastSeparator = Math.Max(path.LastIndexOf('/'), path.LastIndexOf('\\'));

		if (lastSeparator == -1) {
			FileName = path;
			Directory = new DirectoryPath(string.Empty);
		}
		else {
			FileName = path[(lastSeparator + 1)..];
			Directory = new DirectoryPath(path[..lastSeparator]);
		}

		(FileName, Extension) = SplitFileName(FileName);
	}
	/// <summary>
	/// Initializes a new instance of the <see cref="FilePath"/> struct with the specified path.
	/// </summary>
	/// <param name="path">The full path as a ReadOnlySpan of characters.</param>
	public FilePath(ReadOnlySpan<char> path) : this(path.ToString()) { }


	/// <summary>
	/// Splits the file name into the name and extension components.
	/// </summary>
	/// <param name="fileName">The file name to split.</param>
	/// <returns>A tuple containing the file name and extension.</returns>
	private static (string, string) SplitFileName(string fileName) {
		int lastDot = fileName.LastIndexOf('.');
		if (lastDot == -1) {
			return (fileName, string.Empty);
		}
		return (fileName[..lastDot], fileName[(lastDot + 1)..]);
	}

	/// <summary>
	/// Combines the current file path with another directory path.
	/// </summary>
	/// <param name="other">The directory path to combine with.</param>
	/// <returns>A new <see cref="FilePath"/> instance with the combined path.</returns>
	public FilePath Combine(DirectoryPath other) {
		return this with {
			Directory = Directory.Combine(other)
		};
	}
	/// <summary>
	/// Combines the current file path with another path.
	/// </summary>
	/// <param name="path">The path to combine with.</param>
	/// <returns>A new <see cref="FilePath"/> instance with the combined path.</returns>
	public FilePath Combine(ReadOnlySpan<char> path) {
		return Combine(new DirectoryPath(path));
	}


	/// <summary>
	/// Determines whether the specified <see cref="FilePath"/> is equal to the current <see cref="FilePath"/>.
	/// </summary>
	/// <param name="other">The <see cref="FilePath"/> to compare with the current <see cref="FilePath"/>.</param>
	/// <returns>true if the specified <see cref="FilePath"/> is equal to the current <see cref="FilePath"/>; otherwise, false.</returns>
	public readonly bool Equals(FilePath other) {
		return Directory == other.Directory && FileName == other.FileName && Extension == other.Extension;
	}
	/// <summary>
	/// Determines whether the specified object is equal to the current <see cref="FilePath"/>.
	/// </summary>
	/// <param name="obj">The object to compare with the current <see cref="FilePath"/>.</param>
	/// <returns>true if the specified object is equal to the current <see cref="FilePath"/>; otherwise, false.</returns>
	public override readonly bool Equals(object? obj) {
		return obj is DirectoryPath other && Equals(other);
	}
	/// <summary>
	/// Serves as the default hash function.
	/// </summary>
	/// <returns>A hash code for the current <see cref="FilePath"/>.</returns>
	public override readonly int GetHashCode() {
		return HashCode.Combine(Directory, FileName, Extension);
	}


	/// <summary>
	/// Determines whether two specified instances of <see cref="FilePath"/> are equal.
	/// </summary>
	/// <param name="left">The first <see cref="FilePath"/> to compare.</param>
	/// <param name="right">The second <see cref="FilePath"/> to compare.</param>
	/// <returns>true if the two <see cref="FilePath"/> instances are equal; otherwise, false.</returns>
	public static bool operator ==(FilePath left, FilePath right) => left.Equals(right);
	/// <summary>
	/// Determines whether two specified instances of <see cref="FilePath"/> are not equal.
	/// </summary>
	/// <param name="left">The first <see cref="FilePath"/> to compare.</param>
	/// <param name="right">The second <see cref="FilePath"/> to compare.</param>
	/// <returns>true if the two <see cref="FilePath"/> instances are not equal; otherwise, false.</returns>
	public static bool operator !=(FilePath left, FilePath right) => !left.Equals(right);


	/// <summary>
	/// Returns a string that represents the current <see cref="FilePath"/>.
	/// </summary>
	/// <returns>A string that represents the current <see cref="FilePath"/>.</returns>
	public override string ToString() => Url;

	/// <summary>
	/// Performs an implicit conversion from <see cref="FilePath"/> to <see cref="string"/>.
	/// </summary>
	/// <param name="path">The <see cref="FilePath"/> to convert.</param>
	public static implicit operator string(FilePath path) => path.ToString();
}