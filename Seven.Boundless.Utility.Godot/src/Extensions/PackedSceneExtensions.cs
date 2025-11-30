namespace Seven.Boundless.Utility;

using System.Collections.Generic;
using Godot;

public static class PackedSceneExtensions {
	extension(PackedScene scene) {
		public void PackWithSubnodes(Node path) {
			Dictionary<Node, Node> originalOwners = [];
			ReownChildren(path);

			void ReownChildren(Node node, uint layer = 0) {
				foreach (Node item in node.GetChildren()) {
					originalOwners[item] = item.Owner;

					item.Owner = path;
					ReownChildren(item, layer + 1);
				}
			}

			scene.Pack(path);
			foreach (KeyValuePair<Node, Node> original in originalOwners) {
				original.Key.Owner = original.Value;
			}
		}

		public T GetNodeProperty<[MustBeVariant] T>(StringName propertyName, int nodeIndex = 0) {
			return scene.GetNodeProperty(propertyName, nodeIndex).As<T>();
		}
		public T GetNodePropertyRecursive<[MustBeVariant] T>(StringName propertyName) {
			return scene.GetNodePropertyRecursive(propertyName).As<T>();
		}
		public Variant GetNodeProperty(StringName propertyName, int nodeIndex = 0) {
			SceneState state = scene.GetState();

			for (int propIndex = 0; propIndex < state.GetNodePropertyCount(nodeIndex); propIndex++) {
				StringName propName = state.GetNodePropertyName(nodeIndex, propIndex);

				if (propName == propertyName) return state.GetNodePropertyValue(nodeIndex, propIndex);
			}

			return default;
		}
		public Variant GetNodePropertyRecursive(StringName propertyName) {
			SceneState state = scene.GetState();
			for (int nodeIndex = 0; nodeIndex < state.GetNodeCount(); nodeIndex++) {
				for (int propIndex = 0; propIndex < state.GetNodePropertyCount(nodeIndex); propIndex++) {
					StringName propName = state.GetNodePropertyName(nodeIndex, propIndex);

					if (propName == propertyName) return state.GetNodePropertyValue(nodeIndex, propIndex);
				}

			}

			return default;
		}
	}
}