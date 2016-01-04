#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       PreferencesWindow.xaml.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:53 AM
// ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  AUTHOR:     Jonathan Ruisi
// ┃  EMAIL:      JonathanRuisi@gmail.com
// ┣━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  This file is part of PixelFontDesigner.
// ┃  PixelFontDesigner is free software: you can redistribute it and/or modify it under the terms
// ┃  of the GNU General Public License as published by the Free Software Foundation,
// ┃  either version 3 of the license, or (at your option) to any later version.
// ┃
// ┃  PixelFontDesigner is distributed in the hope that it will be useful,
// ┃  but WITHOUT ANY WARRANTY; without even the implied warranty of
// ┃  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// ┃  See the GNU General Public License for more details.
// ┃
// ┃  A copy of the GNU General Public License is included with PixelFontDesigner,
// ┃  and is also available at <http://www.gnu.org/licenses/>
// ┗━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
#endregion

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using JonathanRuisi.PixelFontDesigner.Properties;
using JonathanRuisi.PixelFontDesigner.ViewModel;
using JonathanRuisi.UtilityLibrary;
using JonathanRuisi.UtilityLibrary.Color;
using JonathanRuisi.WpfControlLibrary;

namespace JonathanRuisi.PixelFontDesigner.Windows
{
	public partial class PreferencesWindow : Window
	{
		#region Fields
		private readonly CharacterSetManager _csManager;
		#endregion

		#region Constructor
		public PreferencesWindow(CharacterSetManager csManager)
		{
			if (csManager == null)
				throw new ArgumentNullException("csManager");

			InitializeComponent();
			DataContext = this;
			_csManager = csManager;
		}
		#endregion

		#region Event Handlers (Window)
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			CharacterInfo.EntryComplete += CharacterInfo_EntryComplete;
			ListBoxCharacterSet.ItemsSource = _csManager;

			// Configure button enable/disable bindings
			var binding = new Binding("SelectedItem")
			{
				Source = ListBoxCharacterSet,
				Converter = new NullToBoolConverter()
			};
			ButtonCharacterSetRemove.SetBinding(IsEnabledProperty, binding);

			binding = new Binding("SelectedItem")
			{
				Source = ListBoxCharacterData,
				Converter = new NullToBoolConverter()
			};
			ButtonCharacterRemove.SetBinding(IsEnabledProperty, binding);

			var bindings = new[]
			{
				new Binding("ItemsSource")
				{
					Source = ListBoxCharacterData,
					Converter = new NullToBoolConverter()
				},
				new Binding("SelectedItems.Count")
				{
					Source = ListBoxCharacterData,
					Converter = new IntegerRangeToBoolConverter(),
					ConverterParameter = new Range<int>(0, 1)
				}
			};

			var multiBinding = new MultiBinding {Converter = new BooleanMultiConverter()};
			foreach (var bindingPart in bindings)
			{
				multiBinding.Bindings.Add(bindingPart);
			}
			ButtonCharacterAdd.SetBinding(IsEnabledProperty, multiBinding);

			bindings = new[]
			{
				new Binding("ItemsSource")
				{
					Source = ListBoxCharacterData,
					Converter = new NullToBoolConverter()
				},
				new Binding("SelectedItems.Count")
				{
					Source = ListBoxCharacterData,
					Converter = new IntegerRangeToBoolConverter(),
					ConverterParameter = new Range<int>(1, 1)
				}
			};

			multiBinding = new MultiBinding() {Converter = new BooleanToVisibilityMultiConverter()};
			foreach (var bindingPart in bindings)
			{
				multiBinding.Bindings.Add(bindingPart);
			}
			CharacterInfo.SetBinding(VisibilityProperty, multiBinding);

			e.Handled = true;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			if (DialogResult == true)
			{
				Settings.Default.Save();
				_csManager.Save();
			}
			else Settings.Default.Reload();
		}

		private void ButtonReset_Click(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show(
				"This will permanently discard all preferences changes.\nIt will not affect the character set database.",
				"Are you sure?",
				MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel);
			if (result == MessageBoxResult.OK)
				Settings.Default.Reset();
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
			e.Handled = true;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
			e.Handled = true;
		}
		#endregion

		#region Event Handlers (Appearance)
		private void ButtonMidgroundColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_MidgroundColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_MidgroundColor = ccw.Colors[0].ToString();
		}

		private void ButtonAccentColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_AccentColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_AccentColor = ccw.Colors[0].ToString();
		}

		private void ButtonPixelBorderColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_PixelBorderColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_PixelBorderColor = ccw.Colors[0].ToString();
		}

		private void ButtonPixelMouseOverColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_PixelMouseOverColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_PixelMouseOverColor = ccw.Colors[0].ToString();
		}

		private void ButtonPixelSetColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_PixelSetColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_PixelSetColor = ccw.Colors[0].ToString();
		}

		private void ButtonPixelClearColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_PixelClearColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_PixelClearColor = ccw.Colors[0].ToString();
		}

		private void ButtonAscentOverlayColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_AscentOverlayColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_AscentOverlayColor = ccw.Colors[0].ToString();
		}

		private void ButtonDescentOverlayColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_DescentOverlayColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_DescentOverlayColor = ccw.Colors[0].ToString();
		}

		private void ButtonBearingOverlayColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_BearingOverlayColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_BearingOverlayColor = ccw.Colors[0].ToString();
		}

		private void ButtonBaselineGuideColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_BaselineGuideColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_BaselineGuideColor = ccw.Colors[0].ToString();
		}

		private void ButtonUppercaseGuideColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_UppercaseGuideColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_UppercaseGuideColor = ccw.Colors[0].ToString();
		}

		private void ButtonLowercaseGuideColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_LowercaseGuideColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_LowercaseGuideColor = ccw.Colors[0].ToString();
		}

		private void ButtonBearingGuideColor_Click(object sender, RoutedEventArgs e)
		{
			var ccw = new ColorChooserWindow(ColorSpace.Parse(Settings.Default.Preferences_Appearance_BearingGuideColor));
			if (ccw.ShowDialog() == true)
				Settings.Default.Preferences_Appearance_BearingGuideColor = ccw.Colors[0].ToString();
		}

		private void ButtonCharacterSetColors_Click(object sender, RoutedEventArgs e)
		{
			var colors = from string colorString in Settings.Default.Preferences_Appearance_CharacterSetColors
			             select ColorSpace.Parse(colorString);
			var ccw = new ColorChooserWindow(colors.ToArray());
			if (ccw.ShowDialog() == true)
			{
				var strings = new StringCollection();
				foreach (var color in ccw.Colors)
				{
					strings.Add(color.ToString());
				}
				Settings.Default.Preferences_Appearance_CharacterSetColors = strings;
				_csManager.ColorScheme = new ColorScheme(Settings.Default.Preferences_Appearance_CharacterSetColors);
			}
		}
		#endregion

		#region Event Handlers (Export)
		private void TextBoxType_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (TextBoxType == null || TextBoxTypeSize == null)
				return;

			switch (TextBoxType.Text.ToUpper())
			{
				case "BYTE":
				case "UNSIGNED CHAR":
					TextBoxTypeSize.Text = "8";
					break;
				case "USHORT":
				case "UNSIGNED SHORT":
					TextBoxTypeSize.Text = "16";
					break;
				case "UINT":
				case "UNSIGNED INT":
					TextBoxTypeSize.Text = "32";
					break;
				case "ULONG":
				case "UNSIGNED LONG":
					TextBoxTypeSize.Text = "64";
					break;
			}
		}
		#endregion

		#region Event Handlers (Character Set Database - ListBoxes)
		private void ListBoxCharacterSet_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (ListBoxCharacterSet.SelectedItems.Count == 1)
				ListBoxCharacterData.ItemsSource = ListBoxCharacterSet.SelectedItem as CharacterSet;
			else
				ListBoxCharacterData.ItemsSource = null;
			ListBoxCharacterSet.InvalidateVisual();
			e.Handled = true;
		}

		private void ListBoxCharacterData_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ValidateMoveButtonEnableStatus(ListSortDirection.Ascending);
			if (ListBoxCharacterData.SelectedItems.Count == 1)
				CharacterInfo.Character = ListBoxCharacterData.SelectedItem as Character;
			e.Handled = true;
		}

		private void ListBoxCharacterData_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return && ListBoxCharacterData.SelectedItem != null)
			{
				CharacterInfo.Focus();
				CharacterInfo.BeginEdit();
			}
			e.Handled = true;
		}

		private void CharacterInfo_EntryComplete(object sender, RoutedEventArgs e)
		{
			ListBoxCharacterData.Focus();
			if (ListBoxCharacterData.SelectedIndex < ListBoxCharacterData.Items.Count - 1)
				ListBoxCharacterData.SelectedIndex++;
			else
				ListBoxCharacterData.SelectedIndex = 0;
			ListBoxCharacterData.ScrollIntoView(ListBoxCharacterData.SelectedItem);
			e.Handled = true;
		}
		#endregion

		#region Event Handlers (Character Set Database - Buttons)
		private void ButtonCharacterSetAdd_Click(object sender, RoutedEventArgs e)
		{
			var newCharacterSet = new CharacterSet();
			_csManager.Add(newCharacterSet);
			ListBoxCharacterSet.SelectedIndex = _csManager.IndexOf(newCharacterSet);
			ListBoxCharacterSet.ScrollIntoView(ListBoxCharacterSet.SelectedItem);
			e.Handled = true;
		}

		private void ButtonCharacterSetRemove_Click(object sender, RoutedEventArgs e)
		{
			while (ListBoxCharacterSet.SelectedItems.Count > 0)
			{
				_csManager.Remove(ListBoxCharacterSet.SelectedItems[0] as CharacterSet);
			}
			ListBoxCharacterSet.SelectedIndex = _csManager.Count - 1;
			ListBoxCharacterSet.ScrollIntoView(ListBoxCharacterSet.SelectedItem);
			e.Handled = true;
		}

		private void ButtonCharacterAdd_Click(object sender, RoutedEventArgs e)
		{
			var csIndex = _csManager.IndexOf(ListBoxCharacterData.ItemsSource as CharacterSet);
			var cIndex = _csManager[csIndex].IndexOf(ListBoxCharacterData.SelectedItem as Character);
			Character newCharacter;
			if (ListBoxCharacterData.SelectedItems.Count == 0)
			{
				var nextNumber = _csManager[csIndex].Count > 0
					                 ? _csManager[csIndex].Select(character => character.Number).Max() + 1
					                 : 0;
				newCharacter = new Character(_csManager[csIndex], nextNumber, 'X', "CHARACTER");
				_csManager[csIndex].Add(newCharacter);
			}
			else
			{
				newCharacter = _csManager[csIndex].IndexOf(character => character.Number == _csManager[csIndex][cIndex].Number + 1)
				               > -1
					               ? new Character(_csManager[csIndex],
						                 _csManager[csIndex].Select(character => character.Number).Max() + 1, 'X', "CHARACTER")
					               : new Character(_csManager[csIndex], _csManager[csIndex][cIndex].Number + 1, 'X', "CHARACTER");
				_csManager[csIndex].Insert(cIndex + 1, newCharacter);
			}
			ListBoxCharacterData.SelectedIndex = _csManager[csIndex].IndexOf(newCharacter);
			ListBoxCharacterData.ScrollIntoView(ListBoxCharacterData.SelectedItem);
			e.Handled = true;
		}

		private void ButtonCharacterRemove_Click(object sender, RoutedEventArgs e)
		{
			var csIndex = _csManager.IndexOf(ListBoxCharacterData.ItemsSource as CharacterSet);
			var cIndex = _csManager[csIndex].IndexOf(ListBoxCharacterData.SelectedItem as Character);
			while (ListBoxCharacterData.SelectedItems.Count > 0)
			{
				_csManager[csIndex].Remove(ListBoxCharacterData.SelectedItems[0] as Character);
			}
			ListBoxCharacterData.SelectedIndex = _csManager[csIndex].Count > 0
				                                     ? cIndex > 0
					                                       ? cIndex - 1
					                                       : 0
				                                     : -1;
			ListBoxCharacterData.ScrollIntoView(ListBoxCharacterData.SelectedItem);
			e.Handled = true;
		}

		private void ButtonCharacterMoveUp_Click(object sender, RoutedEventArgs e)
		{
			var csIndex = _csManager.IndexOf(ListBoxCharacterData.ItemsSource as CharacterSet);
			var indices = ListBoxCharacterData.SelectedIndices(ListSortDirection.Ascending);

			foreach (var index in indices)
			{
				_csManager[csIndex].Move(index, index - 1);
			}
			ValidateMoveButtonEnableStatus(ListSortDirection.Ascending);
		}

		private void ButtonCharacterMoveDown_Click(object sender, RoutedEventArgs e)
		{
			var csIndex = _csManager.IndexOf(ListBoxCharacterData.ItemsSource as CharacterSet);
			var indices = ListBoxCharacterData.SelectedIndices(ListSortDirection.Descending);

			foreach (var index in indices)
			{
				_csManager[csIndex].Move(index, index + 1);
			}
			ValidateMoveButtonEnableStatus(ListSortDirection.Descending);
		}
		#endregion

		#region Private Methods
		private void ValidateMoveButtonEnableStatus(ListSortDirection direction)
		{
			if (ListBoxCharacterData.SelectedItems.Count > 0)
			{
				var indices = ListBoxCharacterData.SelectedIndices(direction).ToList();
				ButtonCharacterMoveUp.IsEnabled = indices.Min() > 0;
				ButtonCharacterMoveDown.IsEnabled = indices.Max() < ListBoxCharacterData.Items.Count - 1;
			}
			else
			{
				ButtonCharacterMoveUp.IsEnabled = false;
				ButtonCharacterMoveDown.IsEnabled = false;
			}
		}
		#endregion
	}
}