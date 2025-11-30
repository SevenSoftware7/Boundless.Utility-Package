using System;

namespace Seven.Boundless.Utility;

public readonly record struct Radians(float Value) : IComparable<Radians> {
	public Degrees ToDegrees() => RotationExtensions.RadToDeg(Value).Degrees;
	public AngleCosine ToDotProduct() => RotationExtensions.RadToCos(Value).DotProduct;


	public int CompareTo(Radians other) {
		return Value.CompareTo(other.Value);
	}

	public static implicit operator float(Radians radians) => radians.Value;
}

public readonly record struct Degrees(float Value) : IComparable<Degrees> {
	public Radians ToRadians() => RotationExtensions.DegToRad(Value).Radians;
	public AngleCosine ToDotProduct() => RotationExtensions.DegToCos(Value).DotProduct;


	public int CompareTo(Degrees other) {
		return Value.CompareTo(other.Value);
	}

	public static implicit operator float(Degrees degrees) => degrees.Value;
}

public readonly record struct AngleCosine(float Value) : IComparable<AngleCosine> {
	public Degrees ToDegrees() => RotationExtensions.CosToDeg(Value).Degrees;
	public Radians ToRadians() => RotationExtensions.CosToRad(Value).Radians;


	public int CompareTo(AngleCosine other) {
		return Value.CompareTo(other.Value);
	}

	public static implicit operator float(AngleCosine dotProduct) => dotProduct.Value;
}

public static class RotationExtensions {
	public static float DegToRad(this float degrees) => degrees * (MathF.PI / 180f);
	public static float RadToDeg(this float radians) => radians * (180f / MathF.PI);

	public static float CosToRad(this float dotProduct) => (1f - dotProduct) * MathF.PI / 2f;
	public static float RadToCos(this float radians) => 1f - radians * 2f / MathF.PI;

	public static float CosToDeg(this float dotProduct) => (1f - dotProduct) * 90f;
	public static float DegToCos(this float degrees) => 1f - degrees / 90f;


	extension (float value) {
		public Degrees Degrees => new(value);
		public Radians Radians => new(value);
		public AngleCosine DotProduct => new(value);
	}
}