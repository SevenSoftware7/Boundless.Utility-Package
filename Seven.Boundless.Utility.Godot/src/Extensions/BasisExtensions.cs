namespace Seven.Boundless.Utility;

using Godot;

public static class Bases {
	extension(Basis basis) {
		public Vector3 Right => -basis.X;
		public Vector3 Up => basis.Y;
		public Vector3 Forward => -basis.Z;
		public Vector3 Left => basis.X;
		public Vector3 Down => -basis.Y;
		public Vector3 Back => basis.Z;

		public Basis SafeSlerp(Basis to, float weight) {
			if (basis.IsEqualApprox(to)) {
				return basis;
			}
			return basis.Orthonormalized().Slerp(to.Orthonormalized(), weight);
		}

		/// <summary>
		/// Creates a Godot.Basis that can be easily interpolated to, from the given Basis, without gimble lock.
		/// The forward axis (+Y) points towards the target position. The up axis (-Z) points as close to the up vector
		/// as possible while staying perpendicular to the up axis. The resulting Basis
		/// is orthonormalized. The target and forward vectors cannot be zero, and cannot be parallel
		/// to each other.
		/// </summary>
		/// <param name="forward">The target forward direction.</param>
		/// <param name="up">The target up direction.</param>
		/// <param name="useModelFront">
		/// If true, then the model is oriented in reverse, towards the model front axis
		/// (+Z, Vector3.ModelFront), which is more useful for orienting 3D models.
		/// </param>
		/// <returns>
		/// The resulting basis matrix, which can be easily interpolated to, from the given Basis.
		/// </returns>
		public Basis WarpTowards(Vector3 forward, Vector3? up = null, bool useModelFront = false) {
			if (!up.HasValue) {
				up = basis.Up;
			}

			// Theoretical Rotation, with its -Z = forward and +Y ≈ up
			Basis targetRotation = Basis.LookingAt(forward, up, useModelFront);
			// Practical rotation, can be smoothly interpolated to, from the current rotation
			return FromToBasis(basis, targetRotation) * basis;
		}

		/// <summary>
		/// Creates a Godot.Basis that can be easily interpolated to, from the given Basis, without gimble lock.
		/// The up axis (+Y) points towards the target up. The forward axis (-Z) points as close to the forward vector
		/// as possible while staying perpendicular to the up axis. The resulting Basis
		/// is orthonormalized. The target and forward vectors cannot be zero, and cannot be parallel
		/// to each other.
		/// </summary>
		/// <param name="up">The target up direction.</param>
		/// <param name="forward">The target forward direction.</param>
		/// <param name="useModelFront">
		/// If true, then the model is oriented in reverse, towards the model front axis
		/// (+Z, Vector3.ModelFront), which is more useful for orienting 3D models.
		/// </param>
		/// <returns>
		/// The resulting basis matrix, which can be easily interpolated to, from the given Basis.
		/// </returns>
		public Basis WarpUpTowards(Vector3 up, Vector3? forward = null, bool useModelFront = false) {
			if (!forward.HasValue) {
				forward = basis.Forward;
			}

			// Theoretical Rotation, with its +Y = up and -Z ≈ forward
			Basis targetRotation = Vector3s.UpTowards(up, forward, useModelFront);
			// Practical rotation, can be smoothly interpolated to, from the current rotation
			return basis.To(targetRotation) * basis;
		}

		public static Basis FromToBasis(Basis from, Basis to) {
			return to * from.Inverse();
		}
		public Basis To(Basis to) {
			return FromToBasis(basis, to);
		}
	}

	extension(Vector3 vector) {
		public static Basis FromToBasis(Vector3 from, Vector3 to) {
			return new(Quaternions.FromToQuaternion(from, to));
		}
		public Basis ToBasis(Vector3 to) {
			return FromToBasis(vector, to);
		}
	}
}