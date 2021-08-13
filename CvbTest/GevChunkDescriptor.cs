using System;

namespace CvbConsoleApp
{
    /// <summary>
    /// Single data chunk from inside a <see cref="Stemmer.Cvb.Driver.DeviceImage"/>.
    /// </summary>
    /// <remarks>
    /// Attention: the chunk is only a view onto the image's buffer. It is only
    /// valid as long as the referenced <see cref="Stemmer.Cvb.Driver.DeviceImage"/> is valid.
    /// Source: https://forum.commonvisionblox.com/t/using-chunk-mode-data-with-cvb-net/333
    /// </remarks>
    public struct GevChunkDescriptor
    {
        /// <summary>
        /// The device dependent chunk ID.
        /// </summary>
        /// <remarks>
        /// Check either the <see cref="Stemmer.Cvb.Driver.NodeMapNames.Device"/> 
        /// <see cref="Stemmer.Cvb.GenApi.NodeMap"/> of the <see cref="Stemmer.Cvb.Device.NodeMaps"/>
        /// or the device's manual.
        /// </remarks>
        public uint ID { get; private set; }

        /// <summary>
        /// The offset from the image base pointer to this chunk.
        /// </summary>
        public long Offset { get; private set; }

        /// <summary>
        /// The size of this chunk in bytes.
        /// </summary>
        public long Length { get; private set; }

        /// <summary>
        /// Creates a new chunk.
        /// </summary>
        /// <param name="id">Device dependent chunk identifier.</param>
        /// <param name="basePtr">Base address of the chunk.</param>
        /// <param name="length">Length in bytes of the chunk.</param>
        public GevChunkDescriptor(uint id, long offset, long length)
        {
            if (offset < 0)
                throw new ArgumentOutOfRangeException(nameof(offset), "must be positive or 0");
            if (length < 1)
                throw new ArgumentOutOfRangeException(nameof(length), "must be positive non-0");

            ID = id;
            Offset = offset;
            Length = length;
        }
    }
}
