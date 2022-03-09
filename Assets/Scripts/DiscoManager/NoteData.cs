using System;
using UnityEngine;

namespace Disco
{
	[Serializable]
	public class NoteData
	{
		public enum KeyColors
		{
			Blue, Green, Orange, Pink
		}

		public enum ValidKeys
		{
			I, J, K, L
		}

		public int location;
		public ValidKeys key;
		public float timeInSong;

		public Color GetNoteColor(ValidKeys key)
		{
			switch (key)
			{
				case ValidKeys.I:
					return Color.blue;
				case ValidKeys.J:
					return Color.green;
				case ValidKeys.K:
					return Color.red;
				case ValidKeys.L:
					return Color.magenta;
				default:
					break;
			}
			return Color.white;
		}
	}
}
