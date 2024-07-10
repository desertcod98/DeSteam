using System.Runtime.InteropServices;

namespace DeSteam.PeHandlers
{
    internal class Pe64 : Pe
    {
        
        internal Pe64Headers.OPTIONAL_HEADER64 optionalHeader;

        internal Pe64(string filePath) : base(filePath)
        {   
            byte[] optionalData = new byte[Marshal.SizeOf(typeof(Pe64Headers.OPTIONAL_HEADER64))];
            stream.Seek(dosHeader.e_lfanew + Marshal.SizeOf(typeof(PeHeaders.COFF_HEADER)), SeekOrigin.Begin);
            stream.Read(optionalData, 0, optionalData.Length);
            optionalHeader = FileHelper.GetStructure<Pe64Headers.OPTIONAL_HEADER64>(optionalData);

            foreach (var s in sections) {
                ulong sectionSize = FileHelper.AlignTo(s.header.sizeOfRawData, optionalHeader.fileAlignment);
                s.sizeAligned = sectionSize;
                byte[] data = new byte[s.header.sizeOfRawData];
                stream.Seek(s.header.ptrToRawData, SeekOrigin.Begin);
                stream.Read(data, 0, data.Length);
                s.data = data;
            }
        }

        public void Dispose() {
            stream.Close();
            stream.Dispose();
        }
    }
}
