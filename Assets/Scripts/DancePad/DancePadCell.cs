using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Disco
{
	public class DancePadCell : MonoBehaviour
	{
		[SerializeField] private Image _cellImage;
		private Animator _animator;

		private NoteData[] _noteData;

		[SerializeField] private KeyCode? _key;
		[SerializeField] private bool _isSelected = false;
		[SerializeField] private Color _selectedColor = Color.red;
		[SerializeField] private Color _deselectedColor = Color.white;

		public bool IsSelected => _isSelected;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
		}

		public void ToggleCell(bool isOn)
		{
			_isSelected = isOn;
			_cellImage.color = isOn ? _selectedColor : _deselectedColor;
		}

		public void OnInput(KeyCode key)
		{
			if (key == _key.Value)
			{
				print("Correct dance step woo");
			}
			else
			{
				print("Ya dun fucked up");
			}
		}

		public IEnumerator PlayNote(NoteData.ValidKeys expectedKey, float timeInSong)
		{
			yield return new WaitForSeconds(timeInSong - 3);

			print($"{name}: Expecting user to press {expectedKey} at {timeInSong}s");
			_animator.Play("NotePreview");
			StartCoroutine(MissNote(timeInSong));
		}

		private IEnumerator MissNote(float timeInSong)
		{
			yield return new WaitForSeconds(timeInSong);
			print($"{name}: Window missed!");
		}
	}
}
