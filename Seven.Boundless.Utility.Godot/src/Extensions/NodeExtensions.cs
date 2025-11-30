namespace Seven.Boundless.Utility;

using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Godot;

public static class NodeExtensions {
	extension<T>(T node) where T : Node {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsEnabled() => node.ProcessMode == Node.ProcessModeEnum.Inherit || node.ProcessMode == Node.ProcessModeEnum.Always;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Enabled(Node.ProcessModeEnum mode = Node.ProcessModeEnum.Inherit) {
			if (node.ProcessMode == Node.ProcessModeEnum.Disabled) {
				node.ProcessMode = mode;
			}
			return node;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Disabled() {
			node.ProcessMode = Node.ProcessModeEnum.Disabled;
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SetEnabled(bool enabled, Node.ProcessModeEnum mode = Node.ProcessModeEnum.Inherit) {
			if (enabled) return node.Enabled(mode);
			else return node.Disabled();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ToggleEnabled() {
			if (node.IsEnabled()) return node.Disabled();
			else return node.Enabled();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T RenameTo(StringName name) {
			node.Name = name;
			return node;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ReparentTo(Node parent, bool forceReadableName = false) {
			parent.AddChild(node, forceReadableName);
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SafeReparentTo(Node? newParent, bool keepGlobalTransform = true) {
			if (node.GetParent() == newParent) return node;

			if (!node.IsInsideTree()) {
				node.Unparent();
			}

			if (node.GetParent() is null) {
				newParent?.AddChild(node, true);
			}
			else if (newParent is not null) {
				node.Reparent(newParent, keepGlobalTransform);
			}

			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SafeReparentRecursive(Node? newParent, bool keepGlobalTransform = true) {
			node.SafeReparentTo(newParent, keepGlobalTransform);

			if (!Engine.IsEditorHint()) return node;
			if (node.Owner == newParent?.Owner) return node;

			ReownRecursive(node, newParent?.Owner ?? newParent);

			static void ReownRecursive(Node childNode, Node? newOwner) {
				childNode.Reown(newOwner);
				foreach (Node child in childNode.GetChildren()) {
					ReownRecursive(child, newOwner);
				}
			}

			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SetSiblingIndex(Node parent, int index) {
			parent.MoveChild(node, index);
			return node;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Unparent() {
			node.Owner = null;
			node.GetParent()?.RemoveChild(node);
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void UnparentAndQueueFree() {
			node.QueueFree();
			node.GetParent()?.RemoveChild(node);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ReparentToSceneInstance(Node parent, bool forceReadableName = false) {
			parent.AddSceneInstanceChild(node, forceReadableName);
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T AddSceneInstanceChild(Node child, bool forceReadableName = false) {
			node.AddChild(child, forceReadableName);
			child.Owner = node.SceneFilePath.Length != 0 ? node : node.Owner ?? node;
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Reown(Node? newOwner) {
			node.Owner = newOwner;
			return node;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ShareOwnerWith(Node? newParent) =>
			node.Reown(newParent?.Owner ?? newParent);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T? GetNodeByTypeName() {
			return node.GetNodeOrNull<T>(typeof(T).Name);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetNode(NodePath nodePath, out T outNode) {
			outNode = default!;
			if (!node.HasNode(nodePath))
				return false;

			if (node.GetNodeOrNull(nodePath) is T tNode) {
				outNode = tNode;
				return true;
			}

			return false;
		}

		public void ReconnectSignal(StringName signalName, Callable method, GodotObject.ConnectFlags flags) {
			if (node is null) return;

			if (node.IsConnected(signalName, method)) {
				node.Disconnect(signalName, method);
			}

			node?.Connect(signalName, method, (uint)flags);
		}

		public static void SwapSignalEmitter(ref T? emitter, T? newEmitter, StringName signalName, Callable method, GodotObject.ConnectFlags flags = 0) {
			if (emitter is not null && emitter.IsConnected(signalName, method)) {
				emitter.Disconnect(signalName, method);
			}

			emitter = newEmitter;

			if (emitter is not null && !emitter.IsConnected(signalName, method)) { // IsConnected() sometimes does not work, not my fault https://github.com/godotengine/godot/issues/76690


				emitter.Connect(signalName, method, (uint)flags);
			}
		}

		public void PropagateActionToChildren(Action<T>? action, bool parentFirst = false) {
			if (parentFirst && node is T tParent1) {
				action?.Invoke(tParent1);
			}

			foreach (T child in node.GetChildren().OfType<T>()) {
				action?.Invoke(child);
			}

			if (!parentFirst && node is T tParent2) {
				action?.Invoke(tParent2);
			}
		}

		public void PropagateAction<TIn>(Action<TIn>? action, bool parentFirst = false) where TIn : class {
			TIn? tNode = node as TIn;
			if (parentFirst && tNode is not null) {
				action?.Invoke(tNode);
			}

			foreach (Node child in node.GetChildren()) {
				child.PropagateAction(action, parentFirst);
			}

			if (!parentFirst && tNode is not null) {
				action?.Invoke(tNode);
			}
		}


		public void MakeLocal(Node owner) {
			node.SceneFilePath = string.Empty;
			node.Owner = owner;
			foreach (Node childNode in node.GetChildren()) {
				MakeLocal(childNode, owner);
			}
		}
	}
}

public static class Node3DExtensions {
	extension<T>(T node) where T : Node3D {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Visible() {
			node.Visible = true;
			return node;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Invisible() {
			node.Visible = false;
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SetVisible(bool visible) {
			if (visible) return node.Visible();
			else return node.Invisible();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ToggleVisible() {
			if (node.Visible) return node.Invisible();
			else return node.Visible();
		}
	}
}

public static class CanvasItemExtensions {
	extension<T>(T node) where T : CanvasItem {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Visible() {
			node.Visible = true;
			return node;
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T Invisible() {
			node.Visible = false;
			return node;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T SetVisible(bool visible) {
			if (visible) return node.Visible();
			else return node.Invisible();
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public T ToggleVisible() {
			if (node.Visible) return node.Invisible();
			else return node.Visible();
		}
	}
}