using System;

namespace Destiny
{
    public static class MathHelper2
    {
        static Random s_Random = new Random();

        public static int Random(int p_ExclusiveUpperBound)
        {
            return s_Random.Next(p_ExclusiveUpperBound);
        }
    }
}
