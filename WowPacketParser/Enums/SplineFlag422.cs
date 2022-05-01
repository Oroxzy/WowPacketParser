using System;

namespace WowPacketParser.Enums
{
    [Flags]
    public enum SplineFlag422 : uint
    {
        None             = 0x00000000,
        AnimTierSwim     = 0x00000001,
        AnimTierHover    = 0x00000002,
        AnimTierFly      = 0x00000003,
        AnimTierSubmerged= 0x00000004,
        Falling          = 0x00000040,
        NoSpline         = 0x00000080,
        Trajectory       = 0x00000100,
        Walking          = 0x00000200,
        Flying           = 0x00000400,
        FixedOrientation = 0x00000800,
        FinalPoint       = 0x00001000,
        FinalTarget      = 0x00002000,
        FinalOrientation = 0x00004000,
        CatmullRom       = 0x00008000,
        Cyclic           = 0x00010000,
        EnterCycle       = 0x00020000,
        AnimationTier    = 0x00040000,
        Frozen           = 0x00080000,
        Unknown5         = 0x00100000,
        Unknown6         = 0x00200000,
        Unknown7         = 0x00400000,
        Unknown8         = 0x00800000,
        Unknown9         = 0x01000000,
        Unknown10        = 0x02000000,
        MovingBackwards  = 0x04000000,
        UsePathSmoothing = 0x08000000,
        Animation        = 0x10000000,
        UncompressedPath = 0x20000000,
        Unknown11        = 0x40000000,
        Unknown12        = 0x80000000
    }
}
