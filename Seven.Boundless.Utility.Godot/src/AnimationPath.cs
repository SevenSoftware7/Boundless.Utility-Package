namespace Seven.Boundless.Utility;

using System;
using Godot;

[Serializable]
public readonly struct AnimationPath : IEquatable<AnimationPath>, IEquatable<StringName> {
	public readonly string LibraryName;
	public readonly string AnimationName;
	private readonly string FullName;


	public AnimationPath(string libraryName, string animationName) {
		LibraryName = libraryName;
		AnimationName = animationName;
		FullName = $"{LibraryName}/{AnimationName}";
	}
	public AnimationPath(string path) {
		ArgumentException.ThrowIfNullOrWhiteSpace(path);

		string[] split = path.Split('/');
		switch (split.Length) {
			case 1:
				LibraryName = split[0];
				AnimationName = string.Empty;
				break;
			case 2:
				LibraryName = split[0];
				AnimationName = split[1];
				break;
			default:
				throw new ArgumentException("Invalid AnimationPath format", nameof(path));
		}
	}

	public readonly bool Equals(AnimationPath other) => LibraryName == other.LibraryName && AnimationName == other.AnimationName;
	public readonly bool Equals(StringName other) => ToString() == other;


	public static bool operator ==(AnimationPath left, AnimationPath right) => left.Equals(right);
	public static bool operator !=(AnimationPath left, AnimationPath right) => !(left == right);

	public static bool operator ==(AnimationPath left, StringName right) => left.Equals(right);
	public static bool operator !=(AnimationPath left, StringName right) => !(left == right);

	public static bool operator ==(StringName left, AnimationPath right) => right.Equals(left);
	public static bool operator !=(StringName left, AnimationPath right) => !(left == right);


	public static implicit operator string(AnimationPath from) => from.ToString();
	public static implicit operator StringName(AnimationPath from) => from.ToString();


	public override readonly bool Equals(object? obj) => obj switch {
		StringName stringName => Equals(stringName),
		string stringVal => Equals(stringVal),
		AnimationPath animPath => Equals(animPath),
		_ => false,
	};

	public override readonly string ToString() => FullName;
	public override readonly int GetHashCode() => ToString().GetHashCode();
}