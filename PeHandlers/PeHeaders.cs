using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.PeHandlers {
    internal class PeHeaders {

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class IMAGE_DOS_HEADER {
            internal ushort e_magic;              // Magic number
            internal ushort e_cblp;               // Bytes on last page of file
            internal ushort e_cp;                 // Pages in file
            internal ushort e_crlc;               // Relocations
            internal ushort e_cparhdr;            // Size of header in paragraphs
            internal ushort e_minalloc;           // Minimum extra paragraphs needed
            internal ushort e_maxalloc;           // Maximum extra paragraphs needed
            internal ushort e_ss;                 // Initial (relative) SS value
            internal ushort e_sp;                 // Initial SP value
            internal ushort e_csum;               // Checksum
            internal ushort e_ip;                 // Initial IP value
            internal ushort e_cs;                 // Initial (relative) CS value
            internal ushort e_lfarlc;             // File address of relocation table
            internal ushort e_ovno;               // Overlay number
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            internal ushort[] e_res;              // Reserved words
            internal ushort e_oemid;              // OEM identifier (for e_oeminfo)
            internal ushort e_oeminfo;            // OEM information; e_oemid specific
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
            internal ushort[] e_res2;             // Reserved words
            internal int e_lfanew;                // File address of new exe header
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class COFF_HEADER {
            internal uint magic;
            internal Machine machine;
            internal ushort numberOfSections;
            internal uint timeDateStamp;
            internal uint pointerToSymbolTable;
            internal uint numberOfSymbols;
            internal ushort sizeOfOptionalHeader;
            internal ushort characteristics;
        }

        internal enum Machine : ushort {
            AMD64 = 0x8664,
            //TODO: add machine types...
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal class SECTION_HEADER {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            internal char[] name;
            internal uint virtualSize;
            internal uint virtualAddress;
            internal uint sizeOfRawData;
            internal uint ptrToRawData;
            internal uint ptrToRelocations;
            internal uint ptrToLinenumbers;
            internal ushort numberOfRelocations;
            internal ushort numberOfLinenumbers;
            internal uint characteristics;

            internal string strName => new string(this.name).Trim('\0');
        }
    }
}