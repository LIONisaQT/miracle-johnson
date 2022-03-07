using UnityEngine;

namespace Disco
{
	[CreateAssetMenu(fileName = "SongData", menuName = "ScriptableObjects/SongDataScriptableObject", order = 1)]
	public class SongData : ScriptableObject
	{
		public string Id;
		public AudioClip Song;
		public NoteData[] Notes;
	}
}
