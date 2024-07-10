using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.PeHandlers {
    internal class Pe {
        public readonly FileStream stream;

        internal Section[] sections;
        internal PeHeaders.IMAGE_DOS_HEADER dosHeader;
        internal PeHeaders.COFF_HEADER coffHeader;

        protected Pe(string filePath) {
            if (!File.Exists(filePath)) {
                throw new FileNotFoundException();
            }
            stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            //dos header
            byte[] dosData = new byte[Marshal.SizeOf(typeof(PeHeaders.IMAGE_DOS_HEADER))];
            stream.Read(dosData, 0, dosData.Length);
            dosHeader = FileHelper.GetStructure<PeHeaders.IMAGE_DOS_HEADER>(dosData);

            //coff header
            byte[] coffData = new byte[Marshal.SizeOf(typeof(PeHeaders.COFF_HEADER))];
            stream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);
            stream.Read(coffData, 0, coffData.Length);
            coffHeader = FileHelper.GetStructure<PeHeaders.COFF_HEADER>(coffData);

            //section header
            int sectionTablePosition = dosHeader.e_lfanew + Marshal.SizeOf(typeof(PeHeaders.COFF_HEADER))
                + coffHeader.sizeOfOptionalHeader;
            int nSections = coffHeader.numberOfSections;
            sections = new Section[nSections];
            int sectionSize = Marshal.SizeOf(typeof(PeHeaders.SECTION_HEADER));

            for (int i = 0; i < nSections; i++) {
                byte[] sectionData = new byte[sectionSize];
                stream.Seek(sectionTablePosition + (i * sectionSize), SeekOrigin.Begin);
                stream.Read(sectionData, 0, sectionData.Length);
                PeHeaders.SECTION_HEADER s = FileHelper.GetStructure<PeHeaders.SECTION_HEADER>(sectionData);
                Section section = new(s);
                sections[i] = section;
            }
        }

        public ulong GetFileOffsetFromRva(ulong rva) {
            Section? s = GetOwnerSection(rva);
            if (s != null) {
                return s.header.ptrToRawData + (rva - s.header.virtualAddress);
            }
            return default;
        }

        public Section? GetOwnerSection(ulong rva) {
            foreach (var s in sections) {
                var size = s.header.virtualSize;
                if (size == 0) {
                    size = s.header.sizeOfRawData;
                }
                if (rva >= s.header.virtualAddress && rva < s.header.virtualAddress + size) return s;
            }
            return null;
        }

        public byte[] GetAllData() {
            stream.Seek(0, SeekOrigin.Begin);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            return data;
        }
    }
}
