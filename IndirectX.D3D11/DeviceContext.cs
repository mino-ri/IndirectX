using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace IndirectX.D3D11;

partial class DeviceContext
{
    public void IASetVertexBuffer(int slot, Buffer vertexBuffer, int stride, int offset)
    {
        IASetVertexBuffers(slot, new[] { vertexBuffer }, new[] { stride }, new[] { offset });
    }

    public void IASetVertexBuffers(int startSlot, params (Buffer vertexBuffer, int stride, int offset)[] vertexBufferDeclarations)
    {
        var (vertexBuffers, strides, offsets) = vertexBufferDeclarations.Unzip();
        IASetVertexBuffers(startSlot, vertexBuffers, strides, offsets);
    }

    public (Buffer vertexBuffer, int stride, int offset) IAGetVertexBuffer(int slot)
    {
        return IAGetVertexBuffers(slot, 1)[0];
    }

    public (Buffer vertexBuffer, int stride, int offset)[] IAGetVertexBuffers(int startSlot, int numBuffers)
    {
        return IAGetVertexBuffersCore(startSlot, numBuffers).Zip();
    }

    public void SOSetTargets(params (Buffer soTarget, int offset)[] targets)
    {
        var (soTargets, offsets) = targets.Unzip();
        SOSetTargets(soTargets, offsets);
    }

    public void OMSetRenderTargetsAndUnorderedAccessViews(RenderTargetView[] renderTargetViews, DepthStencilView? depthStencilView, int uavStartSlot, (UnorderedAccessView unorderedAccessView, int initialCount)[] unorderedAccessViews)
    {
        var (uavs, initialCounts) = unorderedAccessViews.Unzip();
        OMSetRenderTargetsAndUnorderedAccessViews(renderTargetViews, depthStencilView, uavStartSlot, uavs, initialCounts);
    }

    public unsafe void UpdateSubresource(void* srcData, Resource dstResource, int dstSubresource, in Box dstBox, int srcRowPitch = 0, int srcDepthPitch = 0)
    {
        UpdateSubresourceCore(dstResource, dstSubresource, in dstBox, srcData, srcRowPitch, srcDepthPitch);
    }

    public unsafe void UpdateSubresource(void* srcData, Resource dstResource, int dstSubresource = 0, int srcRowPitch = 0, int srcDepthPitch = 0)
    {
        UpdateSubresourceCore(dstResource, dstSubresource, in Unsafe.NullRef<Box>(), srcData, srcRowPitch, srcDepthPitch);
    }

    public unsafe void UpdateSubresource<T>(T[] srcData, Resource dstResource, int dstSubresource, in Box dstBox, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in dstBox, p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(T[] srcData, Resource dstResource, int dstSubresource = 0, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in Unsafe.NullRef<Box>(), p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(Span<T> srcData, Resource dstResource, int dstSubresource, in Box dstBox, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in dstBox, p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(Span<T> srcData, Resource dstResource, int dstSubresource = 0, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in Unsafe.NullRef<Box>(), p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(ReadOnlySpan<T> srcData, Resource dstResource, int dstSubresource, in Box dstBox, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in dstBox, p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(ReadOnlySpan<T> srcData, Resource dstResource, int dstSubresource = 0, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in Unsafe.NullRef<Box>(), p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(in T srcData, Resource dstResource, int dstSubresource, in Box dstBox, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = &srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in dstBox, p, srcRowPitch, srcDepthPitch);
        }
    }

    public unsafe void UpdateSubresource<T>(in T srcData, Resource dstResource, int dstSubresource = 0, int srcRowPitch = 0, int srcDepthPitch = 0)
        where T : unmanaged
    {
        fixed (T* p = &srcData)
        {
            UpdateSubresourceCore(dstResource, dstSubresource, in Unsafe.NullRef<Box>(), p, srcRowPitch, srcDepthPitch);
        }
    }
}
