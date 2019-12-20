using System;
using System.Numerics;

public static class ExtensionMethods
{
	public static bool Between(this int num, int lower, int upper, bool inclusive = false)
	{
		return inclusive
			? lower <= num && num <= upper
			: lower < num && num < upper;
	}

	public static void Shuffle<T>(this Random rng, T[] array)
	{
		int n = array.Length;
		while (n > 1)
		{
			int k    = rng.Next(n--);
			T   temp = array[n];
			array[n] = array[k];
			array[k] = temp;
		}
	}	
}