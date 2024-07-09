using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeSteam.PeHandlers;

namespace DeSteam.Unpackers {
    internal class Unpacker {
        protected Pe pe;
        protected Unpacker(Pe pe) {
            this.pe = pe;
        }

        protected Boolean IsPacked() {
            if(!pe.sections.Any(s => s.header.strName == ".bind")) {
                Console.WriteLine("[-] Not packed! (no .bind section)");
                return false;
            }
            //search steamStub pattern

            ulong offset = FileHelper.FindPattern(pe.sections.First(s => s.header.strName == ".bind").data, 
                "E8 00 00 00 00 50 53 51 52");
            if (offset == ulong.MaxValue) return false;
            return true;
        }

        protected Boolean IsPe() {
            if(IsValidCoffHeader() && IsValidDosHeader()) {
                return true;
            }
            Console.WriteLine("[-] Not a valid PE file!");
            return false;
        }

        private Boolean IsValidDosHeader() {
            return pe.dosHeader.e_magic == 0x5a4d;
        }

        private Boolean IsValidCoffHeader() {
            return pe.coffHeader.magic == 0x4550;
        }
    }
}
