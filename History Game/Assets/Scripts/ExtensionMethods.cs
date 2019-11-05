public static class ExtensionMethods
{
	public static bool Between(this int num, int lower, int upper, bool inclusive = false)
	{
		return inclusive
			? lower <= num && num <= upper
			: lower < num && num < upper;
	}
}