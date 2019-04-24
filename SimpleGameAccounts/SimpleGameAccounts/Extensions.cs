using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGameAccounts
{
    internal static class Extensions
    {
        public static bool SequenceEquals(this byte[] self, byte[] target)
        {
            if (self == null || target == null) return false;
            if (self.Length != target.Length) return false;

            for (int i = 0; i < self.Length; ++i)
            {
                if (self[i] != target[i]) return false;
            }

            return true;
        }
    }
}