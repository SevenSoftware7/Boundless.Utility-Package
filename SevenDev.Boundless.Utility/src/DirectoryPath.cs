namespace SevenDev.Boundless.Utility;

using System;
using System.Text.RegularExpressions;

/// <summary>
/// Represents a directory path with an optional protocol.
/// </summary>
public partial struct DirectoryPath : IEquatable<DirectoryPath> {
	/// <summary>
	/// Regular expression to clean leading and trailing slashes from the path.
	/// </summary>
	/// <returns>A <see cref="Regex"/> to clean slashes.</returns>
	[GeneratedRegex(@"^[/\\]*(.*?)[/\\]*$")]
	private static partial Regex CleanSlashes();

	/// <summary>
	/// Gets or initializes the protocol of the directory path.
	/// </summary>
	public string Protocol {
		readonly get => _protocol;
		init {
			_protocol = value;
			_url = null;
		}
	}
	private string _protocol = string.Empty;

	/// <summary>
	/// Gets or initializes the cleaned path of the directory.
	/// </summary>
	public string Path {
		readonly get => _path;
		init {
			_path = CleanSlashes().Match(value).Groups[1].Value;
			_url = null;
		}
	}
	private string _path = string.Empty;

	/// <summary>
	/// Gets the full URL of the directory path, including the protocol if specified.
	/// </summary>
	public string Url => _url ??= string.IsNullOrEmpty(Protocol) ? Path : $"{Protocol}://{Path}";
	private string? _url;


	/// <summary>
	/// Initializes a new instance of the <see cref="DirectoryPath"/> struct with the specified path.
	/// </summary>
	/// <param name="path">The directory path.</param>
	public DirectoryPath(string path) {
		int contextSeparator = path.IndexOf("://");
		if (contextSeparator != -1) {
			Protocol = path[..contextSeparator];
			Path = path[(contextSeparator + 3)..];
		}
		else {
			Path = path;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DirectoryPath"/> struct with the specified path.
	/// </summary>
	/// <param name="path">The directory path as a <see cref="ReadOnlySpan{Char}"/>.</param>
	public DirectoryPath(ReadOnlySpan<char> path) : this(path.ToString()) { }


	/// <summary>
	/// Combines the current directory path with another directory path.
	/// </summary>
	/// <param name="other">The other directory path to combine with.</param>
	/// <returns>A new <see cref="DirectoryPath"/> representing the combined path.</returns>
	public DirectoryPath Combine(DirectoryPath other) {
		return this with {
			Path = string.IsNullOrEmpty(Path) ? other.Path :
				   string.IsNullOrEmpty(other.Path) ? Path :
				   $"{Path}/{other.Path}"
		};
	}
	/// <summary>
	/// Combines the current directory path with another directory path specified as a <see cref="ReadOnlySpan{Char}"/>.
	/// </summary>
	/// <param name="path">The other directory path to combine with.</param>
	/// <returns>A new <see cref="DirectoryPath"/> representing the combined path.</returns>
	public DirectoryPath CombineDirectory(ReadOnlySpan<char> path) {
		return Combine(new DirectoryPath(path));
	}

	/// <summary>
	/// Combines the current directory path with a file path.
	/// </summary>
	/// <param name="file">The file path to combine with.</param>
	/// <returns>A new <see cref="FilePath"/> representing the combined path.</returns>
	public FilePath Combine(FilePath file) {
		return file with {
			Directory = Combine(file.Directory)
		};
	}
	/// <summary>
	/// Combines the current directory path with a file path specified as a <see cref="ReadOnlySpan{Char}"/>.
	/// </summary>
	/// <param name="path">The file path to combine with.</param>
	/// <returns>A new <see cref="FilePath"/> representing the combined path.</returns>
	public FilePath CombineFile(ReadOnlySpan<char> path) {
		return Combine(new FilePath(path));
	}

	/// <summary>
	/// Determines whether the specified <see cref="DirectoryPath"/> is equal to the current <see cref="DirectoryPath"/>.
	/// </summary>
	/// <param name="other">The other <see cref="DirectoryPath"/> to compare with.</param>
	/// <returns><c>true</c> if the specified <see cref="DirectoryPath"/> is equal to the current <see cref="DirectoryPath"/>; otherwise, <c>false</c>.</returns>
	public readonly bool Equals(DirectoryPath other) {
		return Path == other.Path;
	}

	/// <summary>
	/// Determines whether the specified object is equal to the current <see cref="DirectoryPath"/>.
	/// </summary>
	/// <param name="obj">The object to compare with.</param>
	/// <returns><c>true</c> if the specified object is equal to the current <see cref="DirectoryPath"/>; otherwise, <c>false</c>.</returns>
	public override readonly bool Equals(object? obj) {
		return obj is DirectoryPath other && Equals(other);
	}

	/// <summary>
	/// Returns the hash code for the current <see cref="DirectoryPath"/>.
	/// </summary>
	/// <returns>A hash code for the current <see cref="DirectoryPath"/>.</returns>
	public override readonly int GetHashCode() {
		return Path.GetHashCode();
	}

	/// <summary>
	/// Determines whether two specified <see cref="DirectoryPath"/> instances are equal.
	/// </summary>
	/// <param name="left">The first <see cref="DirectoryPath"/> to compare.</param>
	/// <param name="right">The second <see cref="DirectoryPath"/> to compare.</param>
	/// <returns><c>true</c> if the two <see cref="DirectoryPath"/> instances are equal; otherwise, <c>false</c>.</returns>
	public static bool operator ==(DirectoryPath left, DirectoryPath right) => left.Equals(right);
	/// <summary>
	/// Determines whether two specified <see cref="DirectoryPath"/> instances are not equal.
	/// </summary>
	/// <param name="left">The first <see cref="DirectoryPath"/> to compare.</param>
	/// <param name="right">The second <see cref="DirectoryPath"/> to compare.</param>
	/// <returns><c>true</c> if the two <see cref="DirectoryPath"/> instances are not equal; otherwise, <c>false</c>.</returns>
	public static bool operator !=(DirectoryPath left, DirectoryPath right) => !left.Equals(right);


	/// <summary>
	/// Returns a string that represents the current <see cref="DirectoryPath"/>.
	/// </summary>
	/// <returns>A string that represents the current <see cref="DirectoryPath"/>.</returns>
	public override string ToString() => Url;
	/// <summary>
	/// Implicitly converts a <see cref="DirectoryPath"/> to a <see cref="string"/>.
	/// </summary>
	/// <param name="path">The <see cref="DirectoryPath"/> to convert.</param>
	public static implicit operator string(DirectoryPath path) => path.ToString();
}