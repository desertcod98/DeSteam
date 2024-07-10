using DeSteam.PeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unicorn.X86;
using Unicorn;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace DeSteam.Unpackers.Variant0x64
{
    internal class Variant0x64 : Unpacker64
    {
        private StubHeader stubHeader;
        private byte[] stubHeaderData;
        private byte[] payload;
        private byte[] textDecryptedData;
        private string outputFileName;
        public Variant0x64(Pe64 pe, string outputFileName) : base(pe)
        {
            this.outputFileName = outputFileName;
        }

        public override bool Unpack()
        {
            Logger.Info("Checking if file is valid...");
            if (!IsPe()) {
                Logger.Error("File does not appear to be a PE (.exe)!");
                return false;
            };
            if (!Is64()) {
                Logger.Error("File does not appear to be x64!");
                return false;
            }
            if (!IsPacked()) {
                Logger.Error("File does not appear to be packed with Steam DRM!");
                return false;
            }

            Logger.Info("Starting unpacking...");
            if (!DecryptStubHeader()) {
                Logger.Error("Failed to decrypt stub header!");
                return false;
            }
            //DecryptPayload();
            if (!DecryptTextSection()) {
                Logger.Error("Failed to decrypt .text section!");
                return false;
            }
            if (!FixAndSaveFile()) {
                Logger.Error("Failed to create and save unpacked file!");
                return false;
            }
            return true;
        }
        internal bool DecryptStubHeader()
        {
            Logger.Verbose("Grabbing stub header...");
            ulong headerSize = 0xF0;
            ulong entryPointRva = pe.optionalHeader.addressOfEntryPoint;
            ulong entryPointAddr = pe.GetFileOffsetFromRva(entryPointRva);
            ulong headerStartAddr = entryPointAddr - headerSize;
            pe.stream.Seek((long)headerStartAddr, SeekOrigin.Begin);
            byte[] cipheredHeader = new byte[headerSize];
            pe.stream.Read(cipheredHeader, 0, (int)headerSize);

            Logger.Verbose("Decrypting stub header...");
            byte[] stubHeaderData = SteamHelper.SteamHeaderDecrypt(cipheredHeader);
            this.stubHeaderData = stubHeaderData;
            stubHeader = FileHelper.GetStructure<StubHeader>(stubHeaderData);
            Logger.Verbose("Stub header decrypted!");
            return true;
        }

        internal bool DecryptPayload() {
            ulong payloadOffset = pe.sections.First(s => s.header.strName == ".bind").header.ptrToRawData + stubHeader.payloadOffsetFromBind;
            byte[] cipheredPayloadData = new byte[stubHeader.payloadSize];
            pe.stream.Seek((long)payloadOffset, SeekOrigin.Begin);
            pe.stream.Read(cipheredPayloadData, 0, (int)stubHeader.payloadSize);
            byte[] payloadData = SteamHelper.SteamHeaderDecrypt(cipheredPayloadData, true);
            return true;
        }

        internal bool DecryptTextSection() {
            using (var emulator = new X86Emulator(X86Mode.b64)) {
                Logger.Verbose("Created emulator, preparing to emulate the .text section decryption function...");
                Section textSection = pe.sections.First(s => s.header.strName == ".text");
                uint textRawSize = textSection.header.sizeOfRawData;
                byte[] textSectionData = textSection.data;
                ulong textSizeAligned = textSection.sizeAligned;

                Logger.Verbose("Loading decryption function...");
                //dumped memory of steamdll while running
                byte[] steamDllData = FileHelper.ReadAllBytesFileInCWD("Variant0x64DecryptionCode.bin");
                ulong baseAddr = 0x0000000180000000;
                int size = 0x300000;
                emulator.Memory.Map(baseAddr, FileHelper.AlignTo(size,emulator.Memory.PageSize*2), MemoryPermissions.All);
                emulator.Memory.Write(baseAddr, steamDllData, steamDllData.Length);

                Logger.Verbose("Preparing stack and writing memory needed for the function...");
                ulong stackBaseAddr = 0x0000007EB7EF6000;
                int stackSize = 0xA000;
                emulator.Memory.Map(stackBaseAddr, FileHelper.AlignTo(stackSize, emulator.Memory.PageSize * 2), MemoryPermissions.All);
                emulator.Registers.RSP = 0x0000007EB7EFDBB8;
                emulator.Registers.RBP = 0x0000007EB7EFDCC0;
                //TODO!!!  without stack dump, it throws an exception but the result is the same!
                //byte[] stackData = File.ReadAllBytes("E:\\Dev\\zzzRETools\\SteamDRM\\stackDump.bin");
                //emulator.Memory.Write(stackBaseAddr, stackData, stackData.Length);

                //this is a copy of the .text with the IV and the stolen data on top 
                //it moves 32 bytes from header + 0x78 = header + 0x50 + 0x28 on the top of the text section (it seems to be 0x50 + 0x20 for some reason)
                byte[] dataOnTopOfText = new byte[32];
                Array.Copy(stubHeader.textDecryptionData, 0x20, dataOnTopOfText,0, dataOnTopOfText.Length);

                byte[] completeTextData = new byte[dataOnTopOfText.Length + textRawSize];
                
                Array.Copy(dataOnTopOfText, completeTextData, dataOnTopOfText.Length);
                Array.Copy(textSectionData, 0, completeTextData, dataOnTopOfText.Length, textSectionData.Length);

                ulong rcxAreaBase = 0x000002BDE8F10000;

                ulong rcxValue = 0x000002BDE8F42FD0;
                emulator.Memory.Map(rcxAreaBase, 
                    FileHelper.AlignTo((int)(textSizeAligned + rcxValue-rcxAreaBase), emulator.Memory.PageSize*2), //idk why it works if align to pagesize * 2
                    MemoryPermissions.All);
                emulator.Registers.RCX = (long)rcxValue;
                emulator.Memory.Write(rcxValue, completeTextData, completeTextData.Length);

                emulator.Registers.RDX = textRawSize + 0x20;

                ulong r8value = 0x0000007EB7EFFAA8;
                emulator.Registers.R8 = (long)r8value; //in the stack
                byte[] textDecryptionData = stubHeader.textDecryptionData;
                emulator.Memory.Write(r8value, textDecryptionData, textDecryptionData.Length);

                //if i remember this is keysize / 8
                //!!! seems to be like this for all games (with this drm version)
                emulator.Registers.R9 = 0x20;

                byte[] firstStackArg = { 0x10 };
                //it is +0x28 and not +0x20 (like before the call) because after the call the ret addr gets pushed on the stack 
                emulator.Memory.Write((ulong)emulator.Registers.RSP + 0x28, firstStackArg, firstStackArg.Length);

                //this is where .text gets decrypted
                ulong textSectionBase = 0x00007FF74BBE0000;
                emulator.Memory.Map(textSectionBase, (int)FileHelper.AlignTo(textRawSize, (ulong)emulator.Memory.PageSize *2), MemoryPermissions.All);
                
                emulator.Memory.Write(textSectionBase, textSectionData, textSectionData.Length);

                //it is +0x30 and not +0x28 (like before the call) because after the call the ret addr gets pushed on the stack
                byte[] secondStackArg = BitConverter.GetBytes(textSectionBase);
                emulator.Memory.Write((ulong)emulator.Registers.RSP + 0x30, secondStackArg, secondStackArg.Length);

                
                byte[] thirdStackArg = BitConverter.GetBytes(textRawSize);
                emulator.Memory.Write((ulong)emulator.Registers.RSP + 0x38, thirdStackArg, thirdStackArg.Length);

                //RDI (header)
                ulong rdiAreaBase = 0x0010007EB7EF6000;
                int rdiAreaSize = 0xA000;
                emulator.Memory.Map(rdiAreaBase, (int)FileHelper.AlignTo((ulong)rdiAreaSize, (ulong)emulator.Memory.PageSize*2), MemoryPermissions.All);
                emulator.Registers.RDI = 0x0010007EB7EFFA50;
                emulator.Memory.Write((ulong)emulator.Registers.RDI, stubHeaderData, stubHeaderData.Length);

                emulator.Hooks.Code.Add(SkipAllocaProbeHook, null);

                Logger.Info("Starting emulation (this takes a while, do not panic)...");
                try {
                    emulator.Start(0x0000000180001040, 0x00000001800040DB);
                }
                catch (Exception) { }
                Logger.Verbose("Emulation complete! Saving decrypted .text section...");

                textDecryptedData = new byte[textRawSize];
                emulator.Memory.Read(textSectionBase, textDecryptedData, textDecryptedData.Length);
            }
            return true;
        }

        internal bool FixAndSaveFile() {
            Logger.Verbose("Replacing .text section with the decrypted one and changing entry point...");
            byte[] fileData = pe.GetAllData();
            Section textSection = pe.sections.First(s => s.header.strName == ".text");
            uint textDataOffset = textSection.header.ptrToRawData;
            Array.Copy(textDecryptedData, 0, fileData, textDataOffset, textSection.header.sizeOfRawData);
            var optionalHeaderOffset = pe.dosHeader.e_lfanew + Marshal.SizeOf(typeof(PeHeaders.COFF_HEADER));
            var entryPointRvaOffset = optionalHeaderOffset + 0x10;
            uint originalEntryPoint = stubHeader.originalEntryPointRva;
            byte[] OEPBytes = BitConverter.GetBytes(originalEntryPoint);
            Array.Copy(OEPBytes, 0, fileData, entryPointRvaOffset, OEPBytes.Length);
            Logger.Verbose("Writing unpacked file...");
            FileHelper.WriteAllBytesFileInCWD(outputFileName, fileData);
            return true;
        }

        //TODO: FIX!!!! ALLOCA PROBE CRASHES EMU FOR SOME REASON
        private void SkipAllocaProbeHook(Emulator emulator, ulong address, int size, object userData) {
            //registers:
            //41 = RIP
            //44 = RSP
            byte[] opcode = new byte[5];
            //these are opcodes to two calls of _alloca_probe, they fail for some reason.
            //the temp fix is to just skip the calls, we check if the instruction is calling _alloca_probe and 
            //if it is set RIP to next instruction
            byte[] callTo = { 0xE8, 0x54, 0x12, 0x00, 0x00 };
            byte[] callTo2 = { 0xE8, 0xD4, 0x0f, 0x00, 0x00 };

            //TODO ((X86Emulator)emulator).Registers.EIP instead of using IDs
            emulator.Memory.Read((ulong)emulator.Registers.Read(41), opcode, opcode.Length);
            bool isCallToAllocaProbe = opcode.SequenceEqual(callTo) || opcode.SequenceEqual(callTo2);
            if (isCallToAllocaProbe) {
                emulator.Registers.Write(41, emulator.Registers.Read(41) + 0x5); //set RIP to next instruction
            }
        }
    }
}
