using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TButt.Utils
{
	public class TBInspectorHeader : MonoBehaviour
	{
#if UNITY_EDITOR
		public string title = "Title";
		public Color color = Color.white;
		[Range (20, 100)]
		public int fontSize = 40;

		public FontStyle fontStyle = FontStyle.Bold;
#endif
	}

#if UNITY_EDITOR
	[CustomEditor(typeof(TBInspectorHeader))]
	public class TBInspectorHeaderEditor : Editor
	{
		private bool _isEditing;
		private TBInspectorHeader _target;
		private GUIStyle _style = null;
		private int _selectedPaletteColor = 0;

		private static readonly Color[] _paletteColors = new Color[6]
		{
			Color.white,
			new Color (1f, 0.5566038f, 0.564667f),
			new Color (0.5568628f, 1f, 0.6928391f),
			new Color (0.4622642f, 0.7717907f, 1f),
			new Color (0.8612136f, 0.6132076f, 1f),
			new Color (1f, 0.9661731f, 0.5754717f)
		};

		private Texture2D[] _swatchTextures = new Texture2D[6];

		private static readonly string CONFIRM = "Confirm";
		private static readonly string EDIT = "Edit";

		private static readonly string LABEL_NAME = "Label";
		private static readonly string COLOR_NAME = "Color";
		private static readonly string SIZE_NAME = "Size";
		private static readonly string STYLE_NAME = "Style";

		private static readonly int PALETTE_SWATCH_SIZE = 10;

		private void OnEnable()
		{
			_target = target as TBInspectorHeader;
			int length = PALETTE_SWATCH_SIZE * PALETTE_SWATCH_SIZE;
			Color[] colors = new Color[length];

			for (int i = 0; i < 6; i++)
			{
				Texture2D tex = new Texture2D(PALETTE_SWATCH_SIZE, PALETTE_SWATCH_SIZE);
				for (int j = 0; j < length; j++)
				{
					colors[j] = _paletteColors[i];
				}

				tex.SetPixels(colors);
				tex.Apply(false);
				_swatchTextures[i] = tex;
			}
		}

		private void OnDisable()
		{
			for (int i = 0; i < 6; i++)
			{
				DestroyImmediate(_swatchTextures[i]);
			}
		}

		public override void OnInspectorGUI()
		{
			if (_style == null)
				_style = new GUIStyle();

			_style.normal.textColor = _target.color;
			_style.fontSize = _target.fontSize;
			_style.fontStyle = _target.fontStyle;

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(_target.title, _style, GUILayout.ExpandHeight(false));

			Color startColor = GUI.backgroundColor;
			if (_isEditing)
			{
				GUI.backgroundColor = Color.green;
			}

			if (GUILayout.Button(_isEditing ? CONFIRM : EDIT))
			{
				_isEditing = !_isEditing;
			}

			GUI.backgroundColor = startColor;

			EditorGUILayout.EndHorizontal();
			GUILayout.Space((float)_target.fontSize);

			if (!_isEditing)
				return;

			EditorGUILayout.Space();

			EditorGUI.BeginChangeCheck();
			_target.title = EditorGUILayout.TextField(LABEL_NAME, _target.title);
			_target.color = EditorGUILayout.ColorField(COLOR_NAME, _target.color);
			_target.fontSize = EditorGUILayout.IntSlider(SIZE_NAME, _target.fontSize, 20, 100); //EditorGUILayout.IntField(SIZE_NAME, _target.fontSize);
			_target.fontStyle = (FontStyle)EditorGUILayout.EnumPopup(STYLE_NAME, _target.fontStyle);

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(_target);
			}

			EditorGUILayout.Space();

			DrawColorPalette();
		}

		void DrawColorPalette()
		{
			EditorGUI.BeginChangeCheck();
			_selectedPaletteColor = GUILayout.Toolbar(_selectedPaletteColor, _swatchTextures);

			if (EditorGUI.EndChangeCheck())
			{
				_target.color = _paletteColors[_selectedPaletteColor];
				EditorUtility.SetDirty(_target);
			}
		}
	}
#endif
}