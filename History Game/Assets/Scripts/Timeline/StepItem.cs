using TMPro;
using UnityEngine;

namespace Timeline
{
	public class StepItem : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI text = default;

		public int start;
		public int end;

		public void InitMinor(int start, int end)
		{
			this.start = start;
			this.end   = end;
		}

		public void InitMajor(int value)
		{
			string result = value < 0 ? $"{-value} V.Chr" : value.ToString();
			text.text = result;
		}
	}
}