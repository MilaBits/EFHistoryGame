using TMPro;
using UnityEngine;

namespace Timeline
{
	public class StepItem : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI text = default;

		public void Init(int value)
		{
			string result = value < 0 ? $"{-value} V.Chr" : value.ToString();
			text.text = result;
		}
	}
}