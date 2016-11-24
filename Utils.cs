using System;

public static class Utils{

    public static void Swap<T>(ref T lhs, ref T rhs)
    {
        T temp;
        temp = lhs;
        lhs = rhs;
        rhs = temp;
    }
    public static void RandomShuffle<T>(this T[] target, Random random = null)
    {
        if (target.Length < 2) return;
        if (random == null) random = new Random();
        for (int i = target.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            if (i != j) Swap(ref target[i], ref target[j]);
        }
    }
}
