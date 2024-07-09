using DeSteam.PeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.Unpackers {
    abstract internal class Unpacker64 : Unpacker {
        protected Pe64 pe;
        protected Unpacker64(Pe64 pe) : base(pe) {
            this.pe = pe;
        }
        public abstract Boolean Unpack();
        protected Boolean Is64() {
            if (pe.coffHeader.machine == PeHeaders.Machine.AMD64) return true;
            else return false;
        }

    }
}
