using System.Collections.Generic;

namespace Compression.Files
{
    internal class ByteBlockComparer : IEqualityComparer<ByteBlock>
    {
        public bool Equals(ByteBlock x, ByteBlock y)
        {
            if (x is null && y is null)
                return true;
            if (x is null | y is null)
                return false;
            if (ReferenceEquals(x, y))
                return true;
            return x.Id == y.Id;
        }

        public int GetHashCode(ByteBlock obj)
        {
            return 2108858624 + obj.Id.GetHashCode();
        }
    }
}
