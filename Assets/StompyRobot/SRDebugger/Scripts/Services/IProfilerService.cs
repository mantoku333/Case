
namespace SRDebugger.Services
{
    using System;
    using Profiler;
    using SRF.Service;
    using UnityEngine.Rendering;

    public static class ProfilerServiceSelector
    {
        [ServiceSelector(typeof(IProfilerService))]
        public static Type GetProfilerServiceType()
        {
            if(GraphicsSettings.defaultRenderPipeline != null)
            {
                return typeof(SRPProfilerService);
            }

            return typeof(ProfilerServiceImpl);
        }
    }

    public struct ProfilerFrame
    {
        public double FrameTime;
        public double OtherTime;
        public double RenderTime;
        public double UpdateTime;
    }

    public interface IProfilerService
    {
        float AverageFrameTime { get; }
        float LastFrameTime { get; }
        CircularBuffer<ProfilerFrame> FrameBuffer { get; }
    }
}
