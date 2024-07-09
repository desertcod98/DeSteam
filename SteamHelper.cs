using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeSteam {
    internal static class SteamHelper {
        private static byte[] lastKey;
        internal static byte[] SteamHeaderDecrypt(byte[] data, bool useLastKey = false) {
            byte[] results = new byte[data.Length];
            byte[] key;
            int i = 0;
            if(!useLastKey) {
                key = new byte[4];
                Array.Copy(data, key, 4);
                Array.Copy(data, results, 4);
                i = 4;
            }
            else {
                key = lastKey;
            }
            byte[] bytesToXor = new byte[4];
            for (; i < data.Length; i += 4) {
                Array.Copy(data, i, bytesToXor, 0, 4);
                for(int j = 0; j<4; j++) {
                    results[i+j] = (byte)(key[j] ^ bytesToXor[j]);
                }
                Array.Copy(bytesToXor, key, 4);
            }
            lastKey = bytesToXor;
            return results;
        }
    }
}
