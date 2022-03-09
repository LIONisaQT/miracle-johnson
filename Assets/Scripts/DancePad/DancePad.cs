using UnityEngine;

namespace Disco
{
	public class DancePad : MonoBehaviour
	{
		private int _selectedCellIndex;
		private DancePadCell _selectedCell;

		private AudioSource _sfxPlayer;
		private uint _currentCombo;
		private uint _currentStreak;

		[SerializeField] private DancePadCell[] _cells;

		[SerializeField] private AudioClip _successSfx;
		[SerializeField] private AudioClip _failSfx;
		[SerializeField] private AudioClip _bigComboSfx;

		private void Awake()
		{
			_sfxPlayer = GetComponent<AudioSource>();
		}

		public void Start()
		{
			SelectCell(_cells.Length / 2); // Select middle cell

			foreach (var cell in _cells)
			{
				cell.OnInputCallback += OnCellInput;
				cell.OnSuccessCallback += OnSuccess;
				cell.OnFailCallback += OnFail;
			}
		}

		public void Update()
		{
			HandleInput();
		}

		public void SelectCell(int index)
		{
			if (_selectedCell != null)
			{
				_selectedCell.ToggleCell(false);
			}

			_selectedCellIndex = index;
			_selectedCell = _cells[index];
			_selectedCell.ToggleCell(true);
			_selectedCell.CurrentCombo = _currentCombo;
		}

		public void SendNotedata(NoteData[] data)
		{
			foreach (NoteData note in data)
			{
				var cellAtLocation = _cells[note.location];
				cellAtLocation.Enqueue(note);
				cellAtLocation.BeginPlaying();
			}
		}

		#region Input
		private void HandleInput()
		{
			if (Input.GetKeyDown(KeyCode.W)
				|| Input.GetKeyDown(KeyCode.A)
				|| Input.GetKeyDown(KeyCode.S)
				|| Input.GetKeyDown(KeyCode.D))
			{
				HandleMovementInput();
			}
		}

		private void HandleMovementInput()
		{
			if (Input.GetKeyDown(KeyCode.W)) Move(KeyCode.W);
			else if (Input.GetKeyDown(KeyCode.A)) Move(KeyCode.A);
			else if (Input.GetKeyDown(KeyCode.S)) Move(KeyCode.S);
			else if (Input.GetKeyDown(KeyCode.D)) Move(KeyCode.D);
		}

		private void Move(KeyCode keycode)
		{
			switch (keycode)
			{
				case KeyCode.W:
					if (_selectedCellIndex < 4) return;
					_selectedCellIndex -= 5;
					break;
				case KeyCode.A:
					if (_selectedCellIndex % 5 == 0) return;
					_selectedCellIndex--;
					break;
				case KeyCode.S:
					if (_selectedCellIndex > 19) return;
					_selectedCellIndex += 5;
					break;
				case KeyCode.D:
					_selectedCellIndex++;
					if (_selectedCellIndex % 5 == 0) _selectedCellIndex--;
					break;
				default:
					break;
			}

			_currentCombo++;
			SelectCell(_selectedCellIndex);
		}
		#endregion

		public void OnCellInput()
		{
			_currentCombo = 0;
		}

		public void OnSuccess()
		{
			var clip = _currentCombo > 1 ? _bigComboSfx : _successSfx;
			_sfxPlayer.PlayOneShot(clip);

			_currentStreak++;
		}

		public void OnFail()
		{
			_currentStreak = 0;
		}
	}
}
