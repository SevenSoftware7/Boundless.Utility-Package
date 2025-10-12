namespace Seven.Boundless.Utility;

using System;
using Godot;
using Godot.Collections;
using System.Runtime.InteropServices;

public static class CompositorExtensions {
	public static (Vector2I renderSize, uint xGroups, uint yGroups) GetRenderSize(this RenderSceneBuffersRD sceneBuffers, uint range = 8u) {
		Vector2I renderSize = sceneBuffers.GetInternalSize();
		if (renderSize.X == 0 && renderSize.Y == 0) {
			throw new ArgumentException("Render size is incorrect");
		}

		(uint xGroups, uint yGroups) = GetGroups(renderSize, range);

		return (renderSize, xGroups, yGroups);
	}

	public static (uint xGroups, uint yGroups) GetGroups(Vector2I renderSize, uint range) {
		uint xGroups = (uint)renderSize.X / range + 1;
		uint yGroups = (uint)renderSize.Y / range + 1;

		return (xGroups, yGroups);
	}


	public static Rid IndexBufferCreate(this RenderingDevice renderingDevice, ReadOnlySpan<uint> indices, uint shapeVertices = 3u) {
		if (indices.Length % shapeVertices != 0u) throw new ArgumentException($"Invalid number of values in the index buffer, there should be {shapeVertices} vertices in a shape. Total count : {indices.Length}", nameof(indices));
		return renderingDevice.IndexBufferCreate((uint)indices.Length, RenderingDevice.IndexBufferFormat.Uint32, MemoryMarshal.AsBytes(indices), false, 0L);
	}
	public static Rid IndexBufferCreate(this RenderingDevice renderingDevice, ReadOnlySpan<ushort> indices, uint shapeVertices = 3u) {
		if (indices.Length % shapeVertices != 0u) throw new ArgumentException($"Invalid number of values in the index buffer, there should be {shapeVertices} vertices in a shape. Total count : {indices.Length}", nameof(indices));
		return renderingDevice.IndexBufferCreate((uint)indices.Length, RenderingDevice.IndexBufferFormat.Uint16, MemoryMarshal.AsBytes(indices), false, 0L);
	}


	public static (Rid indexBuffer, Rid indexArray) IndexArrayCreate(this RenderingDevice renderingDevice, ReadOnlySpan<uint> indices, uint shapeVertices = 3u, uint indexOffset = 0u) {
		Rid indexBuffer = renderingDevice.IndexBufferCreate(indices, shapeVertices);
		if (!indexBuffer.IsValid) {
			throw new ArgumentException("Index Buffer is Invalid");
		}
		Rid indexArray = renderingDevice.IndexArrayCreate(indexBuffer, indexOffset, (uint)indices.Length);
		if (!indexArray.IsValid) {
			throw new ArgumentException("Index Array is Invalid");
		}

		return (indexBuffer, indexArray);
	}
	public static (Rid indexBuffer, Rid indexArray) IndexArrayCreate(this RenderingDevice renderingDevice, ReadOnlySpan<ushort> indices, uint shapeVertices = 3u, uint indexOffset = 0u) {
		Rid indexBuffer = renderingDevice.IndexBufferCreate(indices, shapeVertices);
		if (!indexBuffer.IsValid) {
			throw new ArgumentException("Index Buffer is Invalid");
		}
		Rid indexArray = renderingDevice.IndexArrayCreate(indexBuffer, indexOffset, (uint)indices.Length);
		if (!indexArray.IsValid) {
			throw new ArgumentException("Index Array is Invalid");
		}

		return (indexBuffer, indexArray);
	}

	public static Rid VertexBufferCreate(this RenderingDevice renderingDevice, ReadOnlySpan<float> vertices, uint vertexLength = 3u) {
		if (vertices.Length % vertexLength != 0) throw new ArgumentException($"Invalid number of values in the points buffer, there should be {vertexLength} float values per point. Total count : {vertices.Length}", nameof(vertices));
		var byteVertices = MemoryMarshal.AsBytes(vertices);

		return renderingDevice.VertexBufferCreate((uint)byteVertices.Length, byteVertices, 0L);
	}
	public static Rid VertexBufferCreate(this RenderingDevice renderingDevice, ReadOnlySpan<double> vertices, uint vertexLength = 3u) {
		if (vertices.Length % vertexLength != 0) throw new ArgumentException($"Invalid number of values in the points buffer, there should be {vertexLength} float values per point. Total count : {vertices.Length}", nameof(vertices));
		var byteVertices = MemoryMarshal.AsBytes(vertices);

		return renderingDevice.VertexBufferCreate((uint)byteVertices.Length, byteVertices, 0L);
	}

	public static (Rid vertexBuffer, Rid vertexArray) VertexArrayCreate(this RenderingDevice renderingDevice, ReadOnlySpan<float> points, long vertexFormat, uint vertexLength = 3u) {
		Rid vertexBuffer = renderingDevice.VertexBufferCreate(points);
		if (!vertexBuffer.IsValid) {
			throw new ArgumentException("Vertex Buffer is Invalid");
		}
		Rid vertexArray = renderingDevice.VertexArrayCreate((uint)(points.Length / vertexLength), vertexFormat, [vertexBuffer]);
		if (!vertexArray.IsValid) {
			throw new ArgumentException("Vertex Array is Invalid");
		}

		return (vertexBuffer, vertexArray);
	}
	public static (Rid vertexBuffer, Rid vertexArray) VertexArrayCreate(this RenderingDevice renderingDevice, ReadOnlySpan<double> points, long vertexFormat, uint vertexLength = 3u) {
		Rid vertexBuffer = renderingDevice.VertexBufferCreate(points);
		if (!vertexBuffer.IsValid) {
			throw new ArgumentException("Vertex Buffer is Invalid");
		}
		Rid vertexArray = renderingDevice.VertexArrayCreate((uint)(points.Length / vertexLength), vertexFormat, [vertexBuffer]);
		if (!vertexArray.IsValid) {
			throw new ArgumentException("Vertex Array is Invalid");
		}

		return (vertexBuffer, vertexArray);
	}


	public static (Rid framebufferTexture, Rid framebuffer) FramebufferCreate(this RenderingDevice renderingDevice, RDTextureFormat textureFormat, RDTextureView textureView, RenderingDevice.TextureSamples textureSamples = RenderingDevice.TextureSamples.Samples1) {
		Rid frameBufferTexture = renderingDevice.TextureCreate(textureFormat, textureView);
		if (!frameBufferTexture.IsValid) {
			throw new ArgumentException("Frame Buffer Texture is Invalid");
		}

		Array<RDAttachmentFormat> attachments = [
			new RDAttachmentFormat() {
				Format = textureFormat.Format,
				Samples = textureSamples,
				UsageFlags = (uint)textureFormat.UsageBits
			}
		];
		long frameBufferFormat = renderingDevice.FramebufferFormatCreate(attachments);
		Rid frameBuffer = renderingDevice.FramebufferCreate([frameBufferTexture], frameBufferFormat);
		if (!frameBuffer.IsValid) {
			throw new ArgumentException("Frame Buffer is Invalid");
		}

		return (frameBufferTexture, frameBuffer);
	}

	public static RDUniform SetIds(this RDUniform uniform, ReadOnlySpan<Rid> ids) {
		uniform._Ids = [.. ids];

		return uniform;
	}

	public static void ComputeListBind(this RenderingDevice device, long computeList, Rid shaderRid, ReadOnlySpan<Rid> ids, RenderingDevice.UniformType uniformType, uint setIndex, int binding = 0) {
		RDUniform uniform = new RDUniform() {
			UniformType = uniformType,
			Binding = binding,
		}.SetIds(ids);

		device.ComputeListBindUniform(computeList, uniform, shaderRid, setIndex);
	}

	public static void ComputeListBindImage(this RenderingDevice device, long computeList, Rid shaderRid, Rid image, uint setIndex, int binding = 0) =>
		device.ComputeListBind(computeList, shaderRid, [image], RenderingDevice.UniformType.Image, setIndex, binding);

	public static void ComputeListBindSampler(this RenderingDevice device, long computeList, Rid shaderRid, Rid image, Rid sampler, uint setIndex, int binding = 0) =>
		device.ComputeListBind(computeList, shaderRid, [sampler, image], RenderingDevice.UniformType.SamplerWithTexture, setIndex, binding);

	public static void ComputeListBindColor(this RenderingDevice device, long computeList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, uint setIndex, int binding = 0) =>
		device.ComputeListBindImage(computeList, shaderRid, sceneBuffers.GetColorLayer(view), setIndex, binding);

	public static void ComputeListBindDepth(this RenderingDevice device, long computeList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, Rid sampler, uint setIndex, int binding = 0) =>
		device.ComputeListBindSampler(computeList, shaderRid, sceneBuffers.GetDepthLayer(view), sampler, setIndex, binding);

	public static void ComputeListBindStorageBuffer(this RenderingDevice device, long computeList, Rid shaderRid, Rid buffer, uint setIndex, int binding = 0) =>
		device.ComputeListBind(computeList, shaderRid, [buffer], RenderingDevice.UniformType.StorageBuffer, setIndex, binding);


	public static void ComputeListBindUniform(this RenderingDevice device, long computeList, RDUniform uniform, Rid shaderRid, uint setIndex) {
		Rid set = UniformSetCacheRD.GetCache(shaderRid, setIndex, [uniform]);

		device.ComputeListBindUniformSet(computeList, set, setIndex);
	}


	public static void DrawListBind(this RenderingDevice device, long drawList, Rid shaderRid, ReadOnlySpan<Rid> ids, RenderingDevice.UniformType uniformType, uint setIndex, int binding = 0) {
		RDUniform uniform = new RDUniform() {
			UniformType = uniformType,
			Binding = binding,
		}.SetIds(ids);

		device.DrawListBindUniform(drawList, uniform, shaderRid, setIndex);
	}

	public static void DrawListBindImage(this RenderingDevice device, long drawList, Rid shaderRid, Rid image, uint setIndex, int binding = 0) =>
		device.DrawListBind(drawList, shaderRid, [image], RenderingDevice.UniformType.Image, setIndex, binding);

	public static void DrawListBindSampler(this RenderingDevice device, long drawList, Rid shaderRid, Rid image, Rid sampler, uint setIndex, int binding = 0) =>
		device.DrawListBind(drawList, shaderRid, [sampler, image], RenderingDevice.UniformType.SamplerWithTexture, setIndex, binding);

	public static void DrawListBindColor(this RenderingDevice device, long drawList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, uint setIndex, int binding = 0) =>
		device.DrawListBindImage(drawList, shaderRid, sceneBuffers.GetColorLayer(view), setIndex, binding);

	public static void DrawListBindDepth(this RenderingDevice device, long drawList, Rid shaderRid, RenderSceneBuffersRD sceneBuffers, uint view, Rid sampler, uint setIndex, int binding = 0) =>
		device.DrawListBindSampler(drawList, shaderRid, sceneBuffers.GetDepthLayer(view), sampler, setIndex, binding);

	public static void DrawListBindStorageBuffer(this RenderingDevice device, long drawList, Rid shaderRid, Rid buffer, uint setIndex, int binding = 0) =>
		device.DrawListBind(drawList, shaderRid, [buffer], RenderingDevice.UniformType.StorageBuffer, setIndex, binding);


	public static void DrawListBindUniform(this RenderingDevice device, long drawList, RDUniform uniform, Rid shaderRid, uint setIndex) {
		Rid set = UniformSetCacheRD.GetCache(shaderRid, setIndex, [uniform]);

		device.DrawListBindUniformSet(drawList, set, setIndex);
	}
}