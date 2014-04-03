using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tfinfo
{
	public static class CustomFilters
	{
		public static string PadLeft(object input, int size)
		{
			var inputText = input.ToString();
			return string.IsNullOrWhiteSpace(inputText) || inputText.Length >= size
				? inputText
				: new string(' ', size - inputText.Length) + inputText;
		}

		public static string PadRight(object input, int size)
		{
			var inputText = input.ToString();
			return string.IsNullOrWhiteSpace(inputText) || inputText.Length >= size
				? inputText
				: inputText + new string(' ', size - inputText.Length);
		}
	}
}
