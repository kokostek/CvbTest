namespace CvbConsoleApp
{
    /// <summary>
    /// Available GigE Vision transport payload types.
    /// </summary>
    /// <remarks>
    /// As of GigE Vision spec 2.0.
    /// Source: https://forum.commonvisionblox.com/t/using-chunk-mode-data-with-cvb-net/333
    /// </remarks>
    public enum GevPayloadType
    {
        Image = 0x0001,
        RawData = 0x0002,
        File = 0x0003,
        ChunkData = 0x0004,
        ExtendedChunkData = 0x0005,
        JPEG = 0x0006,
        JPEG2000 = 0x0007,
        H264 = 0x0008,
        MultiZoneImage = 0x0009,
        ExtendedChunkWithImage = 0x4001,
        ExtendedChunkWithRawData = 0x4002,
        ExtendedChunkWithFile = 0x4003,
        ExtendedChunkWithJPEG = 0x4006,
        ExtendedChunkWithJPEG2000 = 0x4007,
        ExtendedChunkWithH264 = 0x4008,
        ExtendedChunkWithMultiZoneImage = 0x4009
    }
}