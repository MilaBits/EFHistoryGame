using System;

namespace Timeline
{
	[Serializable]
	public struct TimeRange
	{
		public int start;
		public int end;
		public int stepSize;
	}
}