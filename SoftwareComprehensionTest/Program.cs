using System.Buffers;
using System.Runtime.InteropServices;

public interface IFrameCallBack
{
    void FrameReceived(IntPtr pFrame, int width, int height);
}

public interface IValueReporter
{
    void Report(double value);
}

public sealed class FrameAverageStreamer : IFrameCallBack
{
    private readonly IValueReporter _reporter;

    public FrameAverageStreamer(IValueReporter reporter)
    {
        ArgumentNullException.ThrowIfNull(reporter);
        _reporter = reporter;
    }

    public void FrameReceived(IntPtr pFrame, int width, int height)
    {
        if (pFrame == IntPtr.Zero || width <= 0 || height <= 0)
            return;

        long pixelCount64 = (long)width * height;
        if (pixelCount64 > int.MaxValue)
            return;

        int pixelCount = (int)pixelCount64;
        byte[] pixels = ArrayPool<byte>.Shared.Rent(pixelCount);

        try
        {
            // The native buffer is valid only until this callback returns.
            // This implementation assumes one 8-bit grayscale byte per pixel.
            Marshal.Copy(pFrame, pixels, 0, pixelCount);

            long sum = 0;
            for (int i = 0; i < pixelCount; i++)
                sum += pixels[i];

            _reporter.Report(sum / (double)pixelCount);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(pixels);
        }
    }
}

public sealed class RecordingReporter : IValueReporter
{
    public double? LastValue { get; private set; }

    public void Report(double value) => LastValue = value;
}

public static class Program
{
    public static void Main()
    {
        var reporter = new RecordingReporter();
        var streamer = new FrameAverageStreamer(reporter);
        byte[] pixels = [0, 10, 20, 30];
        IntPtr nativeFrame = Marshal.AllocHGlobal(pixels.Length);

        try
        {
            Marshal.Copy(pixels, 0, nativeFrame, pixels.Length);
            streamer.FrameReceived(nativeFrame, width: 2, height: 2);

            if (reporter.LastValue != 15.0)
                throw new InvalidOperationException($"Expected 15.0, got {reporter.LastValue}.");

            Console.WriteLine($"Average pixel value: {reporter.LastValue:0.0}");
        }
        finally
        {
            Marshal.FreeHGlobal(nativeFrame);
        }
    }
}
