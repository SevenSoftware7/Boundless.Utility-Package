namespace Seven.Boundless.Utility;

using Godot;

public static class Quaternions {
	extension (Quaternion quaternion) {
		public Quaternion SafeSlerp(Quaternion to, float weight) {
			if (quaternion.IsEqualApprox(to)) {
				return quaternion;
			}
			return quaternion.Slerp(to, weight);
		}

		public Quaternion FromToQuaternion(Quaternion to) {
			return to * quaternion.Inverse();
		}
		public Quaternion To(Quaternion to) {
			return FromToQuaternion(quaternion, to);
		}
	}

	extension (Vector3 vector) {
		public static Quaternion FromToQuaternion(Vector3 from, Vector3 to) {
			Vector3 cross = from.Cross(to);
			float dot = from.Dot(to);
			float w = Mathf.Sqrt(from.LengthSquared() * to.LengthSquared()) + dot;
			return new Quaternion(cross.X, cross.Y, cross.Z, w);
		}
		public Quaternion ToQuaternion(Vector3 to) {
			return FromToQuaternion(vector, to);
		}
	}
}