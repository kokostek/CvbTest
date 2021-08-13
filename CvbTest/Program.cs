using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Stemmer.Cvb;
using Stemmer.Cvb.Driver;
using Stemmer.Cvb.GenApi;

using CvbStream = Stemmer.Cvb.Driver.Stream;


namespace CvbConsoleApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            const string provider = "GenICam.vin";
            const int port = 0;
            const string settingsFile = @"CameraSettings.gcs";

            using (var cts = CancellationHelper.CancelByControlC())
            using (var device = DeviceFactory.OpenPort(provider, port))
            {
                LoadSettingsFile(device, settingsFile);

                //SuppressCancellationErrors(
                //    () => ReadEncoderValueFromNodes(device, cts.Token),
                //    cts.Token);

                // Uncomment to read encoder value using AT-specific chunk format.
                SuppressCancellationErrors(
                    () => ReadEncoderValueFromChunks(device, cts.Token),
                    cts.Token);
            }
        }

        private static void SuppressCancellationErrors(
            Action run, CancellationToken token)
        {
            try
            {
                run();
            }
            catch (IOException)
            {
                if (!token.IsCancellationRequested)
                {
                    throw;
                }
            }
            catch (InvalidOperationException)
            {
                if (!token.IsCancellationRequested)
                {
                    throw;
                }
            }
        }

        private static void LoadSettingsFile(Device device, string settingsFile)
        {
            Console.WriteLine($"Loading settings from {settingsFile}...");
            var nodeMap = device.NodeMaps[NodeMapNames.Device];
            nodeMap.LoadSettings(settingsFile);
        }

        private static void ReadEncoderValueFromNodes(Device device, CancellationToken token)
        {
            Console.WriteLine(nameof(ReadEncoderValueFromNodes));

            var deviceNodeMap = device.NodeMaps[NodeMapNames.Device];

            CvbStream stream = device.Stream;

            var previousEncoderValue = -1;

            var scanLineSelector = deviceNodeMap["ChunkScanLineSelector"] as IntegerNode;
            scanLineSelector.Value = 0;

            using (stream.StartAsDisposable())
            using (token.Register(() => stream.Abort()))
            {
                while (!token.IsCancellationRequested)
                {
                    using (StreamImage image = stream.Wait(out var status))
                    {
                        if (status != WaitStatus.Ok) continue;
                        var encoderValueNode = deviceNodeMap["ChunkEncoderValue"] as IntegerNode;
                        var currentEncoderValue = (int)encoderValueNode.Value;
                        if (previousEncoderValue >= 0)
                            PrintEncoderInfo(previousEncoderValue, currentEncoderValue);
                        previousEncoderValue = currentEncoderValue;
                    }
                }
            }
        }

        private static void ReadEncoderValueFromChunks(Device device, CancellationToken token)
        {
            Console.WriteLine(nameof(ReadEncoderValueFromChunks));

            var nodemap = device.NodeMaps[NodeMapNames.Device];

            PrintNodeValue(nodemap, "ChunkModeActive");
            PrintNodeValue(nodemap, "PayloadSize");

            var stream = device.Stream;

            var previousEncoderValue = -1;

            var payloadSize = (int) (nodemap["PayloadSize"] as IIntegerNode).Value;

            using (stream.StartAsDisposable())
            using (token.Register(() => stream.Abort()))
            {
                while (!token.IsCancellationRequested)
                {
                    using (var image = stream.Wait(out var status))
                    {
                        var chunks = GevChunkParser.Parse(image, payloadSize);
                        var info = image.Dereference<ATC5AcqInfoChunk>(chunks.First(chunk => chunk.ID == ATC5AcqInfoChunk.ID));

                        var currentEncoderValue = info.TriggerCoordinate;
                        if (previousEncoderValue >= 0)
                            PrintEncoderInfo(previousEncoderValue, currentEncoderValue);
                        previousEncoderValue = currentEncoderValue;
                    }
                }
            }
        }

        private static void PrintNodeValue(NodeMap nodeMap, string paramName)
        {
            Console.WriteLine($"Node[{paramName}] = {nodeMap[paramName]}");
        }

        private static void PrintEncoderInfo(int previous, int current)
        {
            var actualStep = current - previous;
            Console.WriteLine($"Encoder: {current} - {previous} = {actualStep}.");
        }

        private static T Dereference<T>(this Image image, GevChunkDescriptor chunk)
        {
            if (chunk.Length < Marshal.SizeOf<T>())
            {
                throw new ArgumentException(
                    $"Can not dereference {typeof(T)} from chunk of size {chunk.Length}.",
                    nameof(chunk));
            }

            var chunkPtr = new IntPtr(image.GetBufferBasePtr().ToInt64() + chunk.Offset);

            return Marshal.PtrToStructure<T>(chunkPtr);
        }
    }
}
