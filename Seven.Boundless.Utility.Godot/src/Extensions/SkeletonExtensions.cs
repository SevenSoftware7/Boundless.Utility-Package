namespace Seven.Boundless.Utility;

using Godot;

public static class SkeletonExtensions {
	extension(Skeleton3D? skeleton) {
		public Vector3 GetBonePositionOrDefault(string boneName, Vector3 defaultPosition) {
			if (skeleton is null)
				return defaultPosition;

			int boneIndex = skeleton.FindBone(boneName);
			if (boneIndex == -1)
				return defaultPosition;

			return skeleton.ToGlobal(skeleton.GetBoneGlobalPose(boneIndex).Origin);
		}
		public bool TryGetBonePosition(string boneName, out Vector3 position) {
			position = Vector3.Zero;
			if (skeleton is null)
				return false;

			int boneIndex = skeleton.FindBone(boneName);
			if (boneIndex == -1)
				return false;


			position = skeleton.ToGlobal(skeleton.GetBoneGlobalPose(boneIndex).Origin);
			return true;
		}


		public Transform3D GetBoneTransformOrDefault(string boneName, Transform3D defaultTransform) {
			if (skeleton is null)
				return defaultTransform;

			int boneIndex = skeleton.FindBone(boneName);
			if (boneIndex == -1)
				return defaultTransform;


			Transform3D pose = skeleton.GetBoneGlobalPose(boneIndex);
			return skeleton.GlobalTransform * pose;
		}
		public bool TryGetBoneTransform(string boneName, out Transform3D transform) {
			transform = Transform3D.Identity;
			if (skeleton is null)
				return false;

			int boneIndex = skeleton.FindBone(boneName);
			if (boneIndex == -1)
				return false;


			Transform3D pose = skeleton.GetBoneGlobalPose(boneIndex);
			transform = skeleton.GlobalTransform * pose;
			return true;
		}
	}
}