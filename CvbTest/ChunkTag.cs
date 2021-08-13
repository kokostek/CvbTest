using System;
using System.Runtime.InteropServices;

namespace CvbConsoleApp
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct ChunkTag
    {
        public static unsafe ChunkTag Dereference(IntPtr addr)
        {
            return *(ChunkTag*)addr;
        }

        public static unsafe int Size => sizeof(ChunkTag);

        public uint ID => SwapEndianess(_id);

        public int Length => (int) SwapEndianess(_length);

        private static uint SwapEndianess(uint value) =>
            (value & 0x000000FF) << 24
            | (value & 0x0000FF00) << 8
            | (value & 0x00FF0000) >> 8
            | (value & 0xFF000000) >> 24;

        private uint _id;
        private uint _length;
    }
}