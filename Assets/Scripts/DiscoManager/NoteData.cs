using System;

namespace Disco
{
	[Serializable]
	public class NoteData
	{
		public enum ValidKeys
		{
			I, J, K, L
		}

		public int location;
		public ValidKeys key;
		public float timeInSong;
	}
}
