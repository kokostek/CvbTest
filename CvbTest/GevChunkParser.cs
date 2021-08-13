using Stemmer.Cvb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace CvbConsoleApp
{
    /// <summary>
    /// Parser for GigE Vision chunk data.
    /// </summary>
    /// <remarks>
    /// Source: https://forum.commonvisionblox.com/t/using-chunk-mode-data-with-cvb-net/333
    /// </remarks>
    public static partial class GevChunkParser
    {
        /// <summary>
        /// Parses the given <paramref name="image"/> for chunks.
        /// </summary>
        /// <param name="image">Image to parse for chunks.</param>
        public static GevChunkDescriptor[] Parse(Image image, int payloadSize)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            return Iterate(image.GetBufferBasePtr(), payloadSize);
        }

        private static GevChunkDescriptor[] Iterate(IntPtr bufferPtr, long bufferSize)
        {
            Debug.Assert(bufferPtr != IntPtr.Zero);
            Debug.Assert(bufferSize > 0);

            long bufferBase = bufferPtr.ToInt64();
            int tagSize = ChunkTag.Size;

            var chunks = new List<GevChunkDescriptor>();

            var currentTag = bufferBase + bufferSize - tagSize;
            while (currentTag > bufferBase)
            {
                var tag = ChunkTag.Dereference((IntPtr)currentTag);

                var dataLength = tag.Length;
                var data = currentTag - dataLength;
                if (data < bufferBase)
                    throw new InvalidDataException("chunk length larger than available buffer size");

                chunks.Add(new GevChunkDescriptor
                (
                  tag.ID,
                  data - bufferBase,
                  dataLength
                ));

                currentTag = data - tagSize;
            }

            return chunks.ToArray();
        }
    }
}