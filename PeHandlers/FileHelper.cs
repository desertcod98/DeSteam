using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam.PeHandlers {
    internal class FileHelper {
        //rounds to the next biggest multiple of 'align'
        public static ulong AlignTo(ulong toAlign, ulong align) {
            ulong reminder = toAlign % align;
            if (reminder == 0) {
                return toAlign;
            }
            else {
                return toAlign + (align - reminder);
            }
        }
        public static int AlignTo(int toAlign, int align) {
            ulong ulongToAlign = (ulong)toAlign;
            ulong ulongAlign = (ulong)align;
            ulong result = AlignTo(ulongToAlign, ulongAlign);
            return (int)result;
        }

        public static byte[] ReadAllBytesFileInCWD(string filename) {
            string folder = Environment.CurrentDirectory;
            return File.ReadAllBytes(folder+ "\\"+filename);
        }
        public static void WriteAllBytesFileInCWD(string filename, byte[] data) {
            string folder = Environment.CurrentDirectory;
            File.WriteAllBytes(folder + "\\" + filename, data);
        }
        public static T GetStructure<T>(byte[] data) {
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, ptr, size);
            T obj = (T)Marshal.PtrToStructure(ptr, typeof(T));
            Marshal.FreeHGlobal(ptr);
            return obj;
        }

        public static ulong FindPattern(byte[] data, string pattern) {
            string[] patternParts = pattern.Split(" ");
            for (ulong i = 0; i<(ulong)(data.Length-pattern.Length); i++) {
                bool isMatch = true;
                for (int j = 0; j < patternParts.Length; j++) {
                    if (!(patternParts[j] == "??") && Convert.ToByte(patternParts[j], 16) != data[i + (ulong)j]) {   
                        isMatch = false;
                        break;
                    }
                }
                if (isMatch) {
                    return i;
                }
            }
            return ulong.MaxValue;
        }
    }
}
