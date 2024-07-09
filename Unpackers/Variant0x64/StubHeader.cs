using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.Unpackers.Variant0x64 {

    [StructLayout(LayoutKind.Explicit)]
    internal struct StubHeader {
        [FieldOffset(0x10)]
        internal ulong entryPointAddr;
        [FieldOffset(0x20)]
        internal uint originalEntryPointRva;
        [FieldOffset(0x28)]
        internal uint payloadOffsetFromBind;
        [FieldOffset(0x2C)]
        internal uint uncalculatedPayloadSize;
        [FieldOffset(0x30)]
        internal uint steamDllBindOffset;
        [FieldOffset(0x34)]
        internal uint steamDllSize;
        [FieldOffset(0x58)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x50)]
        internal byte[] textDecryptionData;

        internal uint payloadSize => (uncalculatedPayloadSize + 0xF) & 0xFFFFFFF0;
    }
}
