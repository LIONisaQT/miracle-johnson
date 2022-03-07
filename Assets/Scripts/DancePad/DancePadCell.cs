using UnityEngine;
using UnityEngine.UI;

namespace Disco
{
	public class DancePadCell : MonoBehaviour
	{
		private Image _cellImage;

		[SerializeField] private KeyCode? _key;
		[SerializeField] private bool _isSelected = false;
		[SerializeField] private Color _selectedColor = Color.red;
		[SerializeField] private Color _deselectedColor = Color.white;

		public bool IsSelected => _isSelected;

		private void Awake()
		{
			_cellImage = GetComponent<Image>();
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
	}
}
