namespace SevenDev.Boundless.Utility;

using Godot;

public static class BasisExtensions {
	public static Vector3 Right(this Basis basis) => -basis.X;
	public static Vector3 Up(this Basis basis) => basis.Y;
	public static Vector3 Forward(this Basis basis) => -basis.Z;
	public static Vector3 Left(this Basis basis) => basis.X;
	public static Vector3 Down(this Basis basis) => -basis.Y;
	public static Vector3 Back(this Basis basis) => basis.Z;

	public static Basis SafeSlerp(this Basis from, Basis to, float weight) {
		if (from.IsEqualApprox(to)) {
			return from;
		}
		return from.Orthonormalized().Slerp(to.Orthonormalized(), weight);
	}

	public static Basis FromToBasis(this Vector3 from, Vector3 to) {
		return new(QuaternionExtensions.FromToQuaternion(from, to));
	}

	public static Basis FromToBasis(this Basis from, Basis to) {
		return to * from.Inverse();
	}

	/// <summary>
	/// Creates a Godot.Basis with a rotation such that the up axis (+Y) points
	/// towards the target position. The forward axis (-Z) points as close to the forward vector
	/// as possible while staying perpendicular to the up axis. The resulting Basis
	/// is orthonormalized. The target and forward vectors cannot be zero, and cannot be parallel
	/// to each other.
	/// </summary>
	/// <param name="target">The position to point up towards.</param>
	/// <param name="forward">The relative forward direction.</param>
	/// <param name="useModelFront">
	/// If true, then the model is oriented in reverse, towards the model front axis
	/// (+Z, Vector3.ModelFront), which is more useful for orienting 3D models.
	/// </param>
	/// <returns>The resulting basis matrix.</returns>
	public static Basis UpTowards(Vector3 target, Vector3? forward = null, bool useModelFront = false) {
		if (!forward.HasValue) {
			forward = Vector3.Forward;
		}
		if (!useModelFront) {
			forward = -forward;
		}

		Vector3 up = target.Normalized();

		Vector3 right = up.Cross(forward.Value).Normalized();
		Vector3 back = right.Cross(up);
		return new(right, up, back);
	}
}