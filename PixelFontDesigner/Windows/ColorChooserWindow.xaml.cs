#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       ColorChooserWindow.xaml.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:51 AM
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

using System.Collections.ObjectModel;
using System.Windows;

using JLR.Utility.NET.Color;

namespace JonathanRuisi.PixelFontDesigner.Windows
{
	public partial class ColorChooserWindow : Window
	{
		#region Properties
		public ObservableCollection<ColorSpace> Colors { get; set; }
		public bool IsColorChooserOnly { get; set; }
		#endregion

		#region Constructors
		public ColorChooserWindow(ColorSpace currentColor) : this(true, currentColor) { }

		public ColorChooserWindow(params ColorSpace[] colors) : this(false, colors) { }

		private ColorChooserWindow(bool isColorChooserOnly, params ColorSpace[] colors)
		{
			IsColorChooserOnly = isColorChooserOnly;
			Colors = new ObservableCollection<ColorSpace>(colors);
			InitializeComponent();
		}
		#endregion

		#region Event Handlers
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ColorChooser.Loaded += ColorChooser_Loaded;
		}

		private void ColorChooser_Loaded(object sender, RoutedEventArgs e)
		{
			ColorChooser.IsColorChooserOnly = IsColorChooserOnly;
			if (IsColorChooserOnly)
				ColorChooser.CurrentColor = Colors[0];
			else
			{
				foreach (var color in Colors)
				{
					ColorChooser.Colors.Add(color);
				}
			}
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			if (IsColorChooserOnly)
			{
				Colors.Clear();
				Colors.Add(ColorChooser.CurrentColor);
			}
			else
			{
				Colors = new ObservableCollection<ColorSpace>(ColorChooser.Colors);
			}

			DialogResult = true;
			Close();
			e.Handled = true;
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
			e.Handled = true;
		}
		#endregion
	}
}