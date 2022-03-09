using UnityEngine;

namespace Disco
{
	public class DiscoManager : MonoBehaviour
	{
		public static DiscoManager Instance { get; private set; }

		private AudioSource _audioPlayer;

		[SerializeField] private DancePad _dancePad;
		[SerializeField] private SongData _songData;

		public AudioSource AudioPlayer => _audioPlayer;

		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}

			_audioPlayer = GetComponent<AudioSource>();
		}

		private void Start()
		{
			PlaySong();
			SendNotes();
		}

		private void PlaySong()
		{
			_audioPlayer.clip = _songData.Song;
			_audioPlayer.Play();
		}

		private void SendNotes()
		{
			_dancePad.SendNotedata(_songData.Notes);
		}

		//private void Update()
		//{
		//	if (_audioPlayer.isPlaying)
		//	{
		//		print($"Time in song: {Math.Floor(_audioPlayer.time)}");
		//	}
		//}
	}
}
