namespace Strawberry.Sound
{
    public enum SoundFormat
    {
        Wave,
        OggVorbis
    }

    public enum FallOffMode
    {
        None,
        InverseDistanceClamped,
        InverseDistance,
        LinearDistance,
        LinearDistanceClamped,
        ExponentDistance,
        ExponentDistanceClamped
    }
}
