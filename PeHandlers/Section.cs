using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.PeHandlers
{
    internal class Section
    {
        public PeHeaders.SECTION_HEADER header;
        public byte[]? data;
        public ulong sizeAligned;

        internal Section(PeHeaders.SECTION_HEADER header)
        {
            this.header = header;
        }
    }
}
