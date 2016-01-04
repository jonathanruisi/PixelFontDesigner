#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       CharacterInfoControl.xaml.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:15 AM
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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using JonathanRuisi.PixelFontDesigner.ViewModel;

namespace JonathanRuisi.PixelFontDesigner.Controls
{
	public partial class CharacterInfoControl : UserControl
	{
		#region Properties
		public Character Character
		{
			get { return (Character)GetValue(CharacterProperty); }
			set { SetValue(CharacterProperty, value); }
		}
		public static readonly DependencyProperty CharacterProperty =
			DependencyProperty.Register("Character", typeof(Character), typeof(CharacterInfoControl));

		public Brush TextBoxBackground
		{
			get { return (Brush)GetValue(TextBoxBackgroundProperty); }
			set { SetValue(TextBoxBackgroundProperty, value); }
		}
		public static readonly DependencyProperty TextBoxBackgroundProperty =
			DependencyProperty.Register("TextBoxBackground", typeof(Brush), typeof(CharacterInfoControl),
				new FrameworkPropertyMetadata(Brushes.White));

		public Brush LabelForeground
		{
			get { return (Brush)GetValue(LabelForegroundProperty); }
			set { SetValue(LabelForegroundProperty, value); }
		}
		public static readonly DependencyProperty LabelForegroundProperty =
			DependencyProperty.Register("LabelForeground", typeof(Brush), typeof(CharacterInfoControl),
				new FrameworkPropertyMetadata(Brushes.Black));

		public double DescriptionFontSize
		{
			get { return (double)GetValue(DescriptionFontSizeProperty); }
			set { SetValue(DescriptionFontSizeProperty, value); }
		}
		public static readonly DependencyProperty DescriptionFontSizeProperty =
			DependencyProperty.Register("DescriptionFontSize", typeof(double), typeof(CharacterInfoControl),
				new FrameworkPropertyMetadata(10.0));
		#endregion

		#region Events
		public event RoutedEventHandler EntryComplete
		{
			add { AddHandler(EntryCompleteEvent, value); }
			remove { RemoveHandler(EntryCompleteEvent, value); }
		}
		public static readonly RoutedEvent EntryCompleteEvent =
			EventManager.RegisterRoutedEvent("EntryComplete", RoutingStrategy.Bubble,
				typeof(RoutedEventHandler), typeof(CharacterInfoControl));
		#endregion

		#region Constructor
		public CharacterInfoControl()
		{
			InitializeComponent();
		}
		#endregion

		#region Public Methods
		public void BeginEdit()
		{
			Keyboard.Focus(TextBoxNumber);
			TextBoxNumber.SelectAll();
		}
		#endregion

		#region Event Handlers
		private void UserControl_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				if (TextBoxNumber.IsKeyboardFocused)
				{
					Keyboard.Focus(TextBoxSymbol);
					TextBoxSymbol.SelectAll();
				}
				else if (TextBoxSymbol.IsKeyboardFocused)
				{
					Keyboard.Focus(TextBoxDescription);
					TextBoxDescription.SelectAll();
				}
				else if (TextBoxDescription.IsKeyboardFocused)
				{
					Keyboard.Focus(TextBoxNumber);
					TextBoxNumber.SelectAll();
				}

				if (Keyboard.Modifiers == ModifierKeys.Shift)
					RaiseEvent(new RoutedEventArgs(EntryCompleteEvent, this));
			}
		}
		#endregion
	}
}
