using System.Runtime.InteropServices;


namespace CvbConsoleApp
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ATC5AcqInfoChunk
    {
        public const uint ID = 0x66669999;

        public uint TimeStampLow;
        public uint TimeStampHigh;
        public uint FrameCount;
        public int TriggerCoordinate;
        public byte TriggerStatus;
        public ushort DAC;
        public ushort ADC;
        public byte IntIdX;
        public byte AoiIdX;
        public ushort AoiYs;
        public ushort AoiDy;
        public ushort AoiXs;
        public ushort AoiThreshold;
        public byte AOIAlgorithm;
    }
}
