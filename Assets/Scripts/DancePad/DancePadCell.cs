using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Disco
{
	public class DancePadCell : MonoBehaviour
	{
		private const uint PREVIEW_DURATION = 3;
		private const float WINDOW_BEFORE = 0.5f;
		private const float WINDOW_AFTER = 0.5f;

		private const string MISS_STRING = "Bad!";
		private const string TOP_HIT_STRING = "Steps";
		private const string BOTTOM_HIT_STRING = "Combo";

		[SerializeField] private Image _cellImage;
		[SerializeField] private Image _timingRing;
		[SerializeField] private TextMeshProUGUI _keyText;
		[SerializeField] private TextMeshProUGUI _topText;
		[SerializeField] private TextMeshProUGUI _bottomText;

		private Animator _animator;

		private Queue<NoteData> _noteData;
		private NoteData _currentNote;

		private readonly List<KeyCode> _acceptableKeys = new List<KeyCode>()
		{
			KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L
		};

		private enum CellState
		{
			Idle,           // Cell is doing nothing
			ShowingPreview, // Cell is showing visual countdown and can accept erroneous input
			InputWindow,    // Cell can accept correct timing input
		}
		private CellState _cellState;

		[SerializeField] private KeyCode? _key;
		[SerializeField] private bool _isSelected = false;
		[SerializeField] private Color _selectedColor = Color.red;
		[SerializeField] private Color _deselectedColor = Color.white;

		public bool IsSelected => _isSelected;

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
			print($"{name}: {(successful ? "You got it!" : "Miss!")}");

			_animator.Play("NoteShowText");

			_topText.text = successful ? TOP_HIT_STRING : MISS_STRING;
			_bottomText.text = successful ? BOTTOM_HIT_STRING : String.Empty;

			_currentNote = _noteData.Count > 0 ? _noteData.Dequeue() : null;

			_cellState = CellState.Idle;
		}
	}
}
