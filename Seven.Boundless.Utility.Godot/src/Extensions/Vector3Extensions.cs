namespace Seven.Boundless.Utility;

using System;
using Godot;

public static class Vector3s {
	extension (Vector3 vector) {
		public Vector3 SafeSlerp(Vector3 to, float weight) {
			if (!vector.IsFinite() || !to.IsFinite()) return vector;
			if (vector.IsEqualApprox(to)) return vector;

			// Avoid error on both vectors being inverses of each other, breaking a Cross Product operation in the Slerp method
			if ((vector + to).IsEqualApprox(Vector3.Zero)) {
				return vector.Lerp(to, weight);
			}


			return vector.Slerp(to, weight);
		}

		public Vector3 ClampMagnitude(float maxLength) {
			if (vector.LengthSquared() > maxLength * maxLength) {
				return vector.Normalized() * maxLength;
			}
			return vector;
		}

		public Vector3 ClampedLerp(Vector3 to, float amount) {
			Vector3 res = vector + (to - vector) * amount;
			if (amount > Mathf.Epsilon && vector.IsEqualApprox(res)) {
				return to;
			}
			return res;
		}

		/// <summary>
		/// Returns the target vector after being "flattened" against a surface defined by the given normal.
		/// </summary>
		/// <param name="normal">The normal of the surface to slide along.</param>
		/// <returns>>The resulting slid vector.</returns>
		public Vector3 SlideOnFace(Vector3 normal) =>
			vector + normal * Mathf.Max(vector.Project(-normal).Dot(-normal), 0);

		public Vector3 To(Vector3 to)
			=> new(to.X - vector.X, to.Y - vector.Y, to.Z - vector.Z);

		public Vector3 SmoothDamp(Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime) {
			smoothTime = Math.Max(0.0001f, smoothTime);
			float num1 = 2.0f / smoothTime;
			float num2 = num1 * deltaTime;
			float num3 = 1.0f / (1.0f + num2 + 0.479999989271164f * num2 * num2 + 0.234999999403954f * num2 * num2 * num2);
			Vector3 vector3_1 = vector - target;
			Vector3 vector3_2 = target;
			float max = maxSpeed * smoothTime;
			Vector3 vector3_3 = vector3_1.ClampMagnitude(max);
			target = vector - vector3_3;
			Vector3 vector3_4 = (currentVelocity + num1 * vector3_3) * deltaTime;
			currentVelocity = (currentVelocity - num1 * vector3_4) * num3;
			Vector3 vector3_5 = target + (vector3_3 + vector3_4) * num3;
			if ((vector3_2 - vector).Dot(vector3_5 - vector3_2) > 0.0) {
				vector3_5 = vector3_2;
				currentVelocity = (vector3_5 - vector3_2) / deltaTime;
			}
			return vector3_5;
		}


		public void Split(Vector3 upDirection, out Vector3 vertical, out Vector3 horizontal) {
			vertical = vector.Project(upDirection);
			horizontal = vector - vertical;
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
}