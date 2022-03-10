using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Disco
{
	public class DancePadCell : MonoBehaviour
	{
		public Action OnInputCallback;
		public Action OnSuccessCallback;
		public Action OnFailCallback;

		private const uint PREVIEW_DURATION = 3;
		private const float WINDOW_BEFORE = 0.5f;
		private const float WINDOW_AFTER = 0.5f;

		private const string MISS_STRING = "Bad!";
		private const string TOP_HIT_STRING = "Steps";
		private const string BOTTOM_HIT_STRING = "Combo";

		private enum CellState
		{
			Idle,           // Cell is doing nothing
			ShowingPreview, // Cell is showing visual countdown and can accept erroneous input
			InputWindow,    // Cell can accept correct timing input
		}

		private readonly List<KeyCode> _acceptableKeys = new List<KeyCode>()
		{
			KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L
		};

		[SerializeField] private Image _cellImage;
		[SerializeField] private Image _timingRing;
		[SerializeField] private TextMeshProUGUI _keyText;
		[SerializeField] private TextMeshProUGUI _topText;
		[SerializeField] private TextMeshProUGUI _bottomText;
		[SerializeField] private AudioSource _sfxPlayer;

		[SerializeField] private bool _isSelected = false;
		[SerializeField] private Color _selectedColor = Color.red;
		[SerializeField] private Color _deselectedColor = Color.white;

		private Animator _animator;
		private Queue<NoteData> _noteData;
		private NoteData _currentNote;
		private CellState _cellState;

		public bool IsSelected => _isSelected;
		public uint CurrentCombo { get; set; }
		public uint CurrentStreak { get; set; }

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_noteData = new Queue<NoteData>();

			_cellState = CellState.Idle;
		}

		private void Update()
		{
			if (_currentNote == null) return;

			HandleCellState();
			HandleInput();
		}

		public void Enqueue(NoteData data)
		{
			_noteData.Enqueue(data);
		}

		public void BeginPlaying()
		{
			if (_noteData.Count == 0) return;
			if (_currentNote != null) return;

			_currentNote = _noteData.Dequeue();
		}

		public void ToggleCell(bool isOn)
		{
			_isSelected = isOn;
			_cellImage.color = isOn ? _selectedColor : _deselectedColor;
		}

		public void HandleCellState()
		{
			switch (_cellState)
			{
				case CellState.Idle:
					if (DiscoManager.Instance.AudioPlayer.time > _currentNote.timeInSong - PREVIEW_DURATION)
					{
						_cellState = CellState.ShowingPreview;
					}

					break;
				case CellState.ShowingPreview:
					var currentKey = _currentNote.key;

					_animator.Play("NotePreview");

					_keyText.text = currentKey.ToString();

					var currentKeyColor = _currentNote.GetNoteColor(currentKey);
					_keyText.color = currentKeyColor;
					_timingRing.color = currentKeyColor;

					if (DiscoManager.Instance.AudioPlayer.time > _currentNote.timeInSong - WINDOW_BEFORE)
					{
						_cellState = CellState.InputWindow;
					}

					break;
				case CellState.InputWindow:
					if (DiscoManager.Instance.AudioPlayer.time > _currentNote.timeInSong + WINDOW_AFTER)
					{
						HandleInputResult(false);
					}
					break;
				default:
					break;
			}
		}

		public void HandleInput()
		{
			if (!_isSelected) return;
			if (!Input.anyKeyDown) return;

			switch (_cellState)
			{
				case CellState.ShowingPreview:
					foreach (var key in _acceptableKeys)
					{
						if (Input.GetKey(key))
						{
							HandleInputResult(false);
						}
					}

					break;

				case CellState.InputWindow:
					var correctKeycode = (KeyCode)Enum.Parse(typeof(KeyCode), _currentNote.key.ToString());
					var isCorrect = false;

					foreach (var key in _acceptableKeys)
					{
						if (Input.GetKeyDown(key) && key == correctKeycode)
						{
							isCorrect = true;
							break;
						}
					}

					HandleInputResult(isCorrect);
					break;
			}
		}

		// TODO: Make this accept an enum (early, hit, slightly late, miss)
		private void HandleInputResult(bool successful)
		{
			if (successful)
			{
				OnSuccessCallback?.Invoke();
			}
			else
			{
				OnFailCallback?.Invoke();
			}

			_animator.Play("NoteShowText");

			_topText.text = successful ? $"{CurrentCombo} {TOP_HIT_STRING}" : MISS_STRING;
			_bottomText.text = successful ? $"{CurrentStreak} {BOTTOM_HIT_STRING}" : String.Empty;

			_currentNote = _noteData.Count > 0 ? _noteData.Dequeue() : null;

			_cellState = CellState.Idle;

			OnInputCallback?.Invoke();
		}
	}
}
