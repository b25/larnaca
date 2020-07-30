using System;
using System.Collections;

namespace collection.extensions
{
    public static class BitArrayExtensions
    {
        public static byte[] ToBytes(this BitArray bits)
        {
            if (bits == null) return null;
            byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
            bits.CopyTo(ret, 0);
            return ret;
        }

    }
}
