using Stemmer.Cvb;
using Stemmer.Cvb.Driver;
using System;
using System.Diagnostics;
using System.Linq;

namespace CvbConsoleApp
{
    /// <summary>
    /// Extended functionality for GenICam <see cref="Device"/> 
    /// <see cref="DeviceImage"/>s . 
    /// </summary>
    /// <remarks>
    /// Source: https://forum.commonvisionblox.com/t/using-chunk-mode-data-with-cvb-net/333
    /// </remarks>
    public static class GenICamDeviceImageExtensions
    {
        /// <summary>
        /// Gets whether the device image contains chunk data.
        /// </summary>
        /// <param name="device">This device.</param>
        /// <returns><b>true</b> this <paramref name="device"/> has chunk data; 
        /// <b>false</b> otherwise.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="device"/>
        /// is not a GenICam device image.</exception>
        public static bool HasChunks(this Device device)
        {
            switch (device.GetPayloadType())
            {
                case GevPayloadType.ChunkData:
                case GevPayloadType.ExtendedChunkData:
                case GevPayloadType.ExtendedChunkWithFile:
                case GevPayloadType.ExtendedChunkWithH264:
                case GevPayloadType.ExtendedChunkWithImage:
                case GevPayloadType.ExtendedChunkWithJPEG:
                case GevPayloadType.ExtendedChunkWithJPEG2000:
                case GevPayloadType.ExtendedChunkWithMultiZoneImage:
                case GevPayloadType.ExtendedChunkWithRawData:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the buffer base pointer of this <paramref name="image"/>.
        /// </summary>
        /// <param name="image">This contiguous image.</param>
        /// <returns>Pointer to the first element of the buffer.</returns>
        public static IntPtr GetBufferBasePtr(this Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return image.Planes
              .Min(plane => plane.GetLinearAccess().BasePtr);
        }

        /// <summary>
        /// Gets the <see cref="GevPayloadType"/> of this <paramref name="device"/>.
        /// </summary>
        /// <param name="device">This device.</param>
        /// <returns>The delivered payload type.</returns>
        /// <exception cref="InvalidOperationException">If <paramref name="device"/>
        /// is not a GenICam device image.</exception>
        public static GevPayloadType GetPayloadType(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            long payloadType;
            device.DeviceControl.SendCommand(GetDeliveredPaylodType, IntPtr.Zero, out payloadType);
            return (GevPayloadType)payloadType;
        }

        /// <summary>
        /// Gets the valid size of the delivered buffer storing the image and 
        /// additional data.
        /// </summary>
        /// <param name="device">This device.</param>
        /// <returns>Number of valid bytes in the <paramref name="device"/>'s buffer.
        /// </returns>
        public static int GetPayloadSize(this Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            long payloadSize;
            try
            {
                device.DeviceControl.SendCommand(GetDeliveredPaylodSize, IntPtr.Zero, out payloadSize);
                if (payloadSize > 0)
                    return (int) payloadSize;
            }
            catch
            {
                // swallow
            }

            device.DeviceControl.SendCommand(GetPaylodSize, IntPtr.Zero, out payloadSize);
            return (int) payloadSize;
        }

        private static IDeviceControl DeviceControl(DeviceImage image)
        {
            Debug.Assert(image != null);
            if (!IsGenICamDevice(image.Parent))
                throw new InvalidOperationException("DeviceImage is not of GenICam Device");

            return image.Parent.DeviceControl;
        }

        private static bool IsGenICamDevice(Device device)
        {
            return device?.DriverGuid == DeviceFactory.GenICamGuid;
        }

        private static readonly DeviceControlCommand GetDeliveredPaylodType = new DeviceControlCommand(DeviceControlOperation.Get, 0x1000);
        private static readonly DeviceControlCommand GetPaylodSize = new DeviceControlCommand(DeviceControlOperation.Get, 0x1900);
        private static readonly DeviceControlCommand GetDeliveredPaylodSize = new DeviceControlCommand(DeviceControlOperation.Get, 0x1B00);
    }
}