using System.Runtime.InteropServices;
using static DeSteam.PeHandlers.PeHeaders;

namespace DeSteam.PeHandlers {
    internal class Pe64Headers {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class IMAGE_DOS_HEADER64 : IMAGE_DOS_HEADER { }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class COFF_HEADER64 : COFF_HEADER { }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class OPTIONAL_HEADER64 {
            internal ushort magic;
            internal byte majorLinkerVersion;
            internal byte minorLinkerVersion;
            internal uint sizeOfCode;
            internal uint sizeOfInitializedData;
            internal uint sizeOfUninitializedData;
            internal uint addressOfEntryPoint;
            internal uint baseOfCode;
            internal uint imageBase;
            internal uint sectionAlignment;
            internal uint fileAlignment;
            internal ushort majorOperatingSystemVersion;
            internal ushort minorOperatingSystemVersion;
            internal ushort majorImageVersion;
            internal ushort minorImageVersion;
            internal ushort majorSubsystemVersion;
            internal ushort minorSubsystemVersion;
            internal uint win32VersionValue;
            internal uint sizeOfImage;
            internal uint sizeOfHeaders;
            internal uint checkSum;
            internal ushort subsystem;
            internal ushort dllCharacteristics;
            internal ulong sizeOfStackReserve;
            internal ulong sizeOfStackCommit;
            internal ulong sizeOfHeapReserve;
            internal ulong sizeOfHeapCommit;
            internal uint loaderFlags;
            internal uint numberOfRvaAndSizes;
            //TODO: add image directories ecc.
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class SECTION_HEADER64 : SECTION_HEADER { }
    }
}
