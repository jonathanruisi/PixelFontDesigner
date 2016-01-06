#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       ProjectSettingsWindow.xaml.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:55 AM
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using JonathanRuisi.PixelFontDesigner.ViewModel;
using JonathanRuisi.UtilityLibrary.Collections;
using JonathanRuisi.WpfControlLibrary;
using JonathanRuisi.WpfControlLibrary.ValidationRules;

namespace JonathanRuisi.PixelFontDesigner.Windows
{
	public partial class ProjectSettingsWindow : Window
	{
		#region Fields
		private readonly CharacterSetManager _csManager;
		private readonly List<CharacterSet> _originalCharacterSets, _addedCharacterSets, _removedCharacterSets;
		private readonly bool _isNewProject;
		private bool _isLoading;
		#endregion

		#region Properties
		public ProjectManager Project
		{
			get { return (ProjectManager) GetValue(ProjectProperty); }
			private set { SetValue(ProjectProperty, value); }
		}

		public static readonly DependencyProperty ProjectProperty =
			DependencyProperty.Register("Project", typeof(ProjectManager), typeof(ProjectSettingsWindow),
				new FrameworkPropertyMetadata(null));

		private int CharacterWidth
		{
			get { return (int) GetValue(CharacterWidthProperty); }
			set { SetValue(CharacterWidthProperty, value); }
		}

		private static readonly DependencyProperty CharacterWidthProperty =
			DependencyProperty.Register("CharacterWidth", typeof(int), typeof(ProjectSettingsWindow));

		private int CharacterHeight
		{
			get { return (int) GetValue(CharacterHeightProperty); }
			set { SetValue(CharacterHeightProperty, value); }
		}

		private static readonly DependencyProperty CharacterHeightProperty =
			DependencyProperty.Register("CharacterHeight", typeof(int), typeof(ProjectSettingsWindow));
		#endregion

		#region Constructor
		public ProjectSettingsWindow(CharacterSetManager characterSetManager, ProjectManager projectManager)
		{
			if (characterSetManager == null)
				throw new ArgumentNullException(nameof(characterSetManager));

			DataContext = this;
			_isNewProject = projectManager == null;
			_csManager = characterSetManager;
			_originalCharacterSets = new List<CharacterSet>();
			_addedCharacterSets = new List<CharacterSet>();
			_removedCharacterSets = new List<CharacterSet>();
			_isLoading = true;

			if (_isNewProject)
			{
				Project = new ProjectManager();
			}
			else
			{
				Project = new ProjectManager(projectManager);
				foreach (var character in Project.Where(character => !_originalCharacterSets.Contains(character.CharacterSet)))
				{
					_originalCharacterSets.Add(character.CharacterSet);
				}
			}
			InitializeComponent();
		}
		#endregion

		#region Event Handlers (Window)
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			ListBoxCharacterSets.ItemsSource = _csManager;
			Title = _isNewProject ? "New Project" : $"Edit \"{Project.ProjectName}\" Settings";

			// Select all character sets already represented in the project
			foreach (var characterSet in _originalCharacterSets)
			{
				ListBoxCharacterSets.SelectedItems.Add(characterSet);
			}

			// Configure bindings
			var binding = new Binding("ProjectName")
			{
				Source = Project,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			binding.ValidationRules.Add(new TextLengthRule(1, 255));
			TextBoxProjectName.SetBinding(TextBox.TextProperty, binding);

			binding = new Binding("CharacterWidth")
			{
				Source = this,
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			binding.ValidationRules.Add(new NumericTextRule(1, 64, 1.0)
			{
				NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
			});
			TextBoxCharacterWidth.SetBinding(TextBox.TextProperty, binding);

			binding = new Binding("CharacterHeight")
			{
				Source = this,
				Mode = BindingMode.TwoWay,
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			binding.ValidationRules.Add(new NumericTextRule(1, 64, 1.0)
			{
				NumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign
			});
			TextBoxCharacterHeight.SetBinding(TextBox.TextProperty, binding);

			binding = new Binding("CharacterWidth") {Source = Project, Mode = BindingMode.TwoWay};
			SetBinding(CharacterWidthProperty, binding);
			binding = new Binding("CharacterHeight") {Source = Project, Mode = BindingMode.TwoWay};
			SetBinding(CharacterHeightProperty, binding);
			binding = new Binding("CharacterWidth") {Source = this};
			MarginPixelGrid.SetBinding(PixelGrid.PixelGridWidthProperty, binding);
			binding = new Binding("CharacterHeight") {Source = this};
			MarginPixelGrid.SetBinding(PixelGrid.PixelGridHeightProperty, binding);
			binding = new Binding("AscentHeight") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockAscent.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.AscentHeightProperty, binding);
			binding = new Binding("DescentHeight") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockDescent.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.DescentHeightProperty, binding);
			binding = new Binding("UppercaseHeight") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockUppercase.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.UppercaseHeightProperty, binding);
			binding = new Binding("LowercaseHeight") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockLowercase.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.LowercaseHeightProperty, binding);
			binding = new Binding("LeftBearingWidth") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockLeftBearing.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.LeftBearingWidthProperty, binding);
			binding = new Binding("RightBearingWidth") {Source = Project, Mode = BindingMode.TwoWay};
			TextBlockRightBearing.SetBinding(TextBlock.TextProperty, binding);
			MarginPixelGrid.SetBinding(GlyphPixelGrid.RightBearingWidthProperty, binding);

			_isLoading = false;
			e.Handled = true;
		}

		private void ListBoxCharacterSets_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (!_isLoading)
			{
				foreach (var characterSet in e.AddedItems.Cast<CharacterSet>())
				{
					_addedCharacterSets.Add(characterSet);
					_removedCharacterSets.Remove(characterSet);
				}
				foreach (var characterSet in e.RemovedItems.Cast<CharacterSet>())
				{
					if (_originalCharacterSets.Contains(characterSet))
						_removedCharacterSets.Add(characterSet);
					_addedCharacterSets.Remove(characterSet);
				}
			}
			e.Handled = true;
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
			e.Handled = true;
		}

		private void ButtonOk_Click(object sender, RoutedEventArgs e)
		{
			var characterSetTotal = _originalCharacterSets.Count +
			                        _addedCharacterSets.Count(characterSet => !_originalCharacterSets.Contains(characterSet)) -
			                        _originalCharacterSets.Count(characterSet => _removedCharacterSets.Contains(characterSet));

			// Validate settings
			if (String.IsNullOrEmpty(Project.ProjectName))
			{
				MessageBox.Show("The project name cannot be blank", "Invalid Settings",
					MessageBoxButton.OK, MessageBoxImage.Error);
				e.Handled = true;
				return;
			}

			if (!ValidateDimension(CharacterWidth) || !ValidateDimension(CharacterHeight))
			{
				MessageBox.Show("The character dimensions must be between 0 < X ≤ 64", "Invalid Settings",
					MessageBoxButton.OK, MessageBoxImage.Error);
				e.Handled = true;
				return;
			}

			if (characterSetTotal == 0)
			{
				var result = MessageBox.Show(
					"No character sets have been selected.\nAre you sure you want to create an empty project?",
					"Empty Project", MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.No)
				{
					e.Handled = true;
					return;
				}
			}

			if (_removedCharacterSets.Count > 0)
			{
				var result = MessageBox.Show("Character sets have been removed from the project.\n" +
				                             "This will permanently delete any project data associated with those characters.\n" +
				                             "Are you sure you want to continue?", "Character Set Removal",
					MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.No)
				{
					e.Handled = true;
					return;
				}
			}

			if (!_isNewProject &&
			    (CharacterWidth != Project.CharacterWidth || CharacterHeight != Project.CharacterHeight))
			{
				var result = MessageBox.Show("The character dimensions of the project have changed.\n" +
				                             "Each character will be resized to fit the new dimensions.\n" +
				                             "Are you sure you want to continue?", "Character Resize",
					MessageBoxButton.YesNo, MessageBoxImage.Warning);
				if (result == MessageBoxResult.No)
				{
					e.Handled = true;
					return;
				}
			}

			// Modify project character sets
			var additionalCharacters = from characterSet in _addedCharacterSets
			                           where _originalCharacterSets.Contains(characterSet)
			                           from character in characterSet
			                           where !Project.Contains(character)
			                           select character;
			Project.RemoveAny(character => _removedCharacterSets.Contains(character.CharacterSet));
			foreach (var character in _addedCharacterSets
				.Where(characterSet => !_originalCharacterSets.Contains(characterSet))
				.SelectMany(characterSet => characterSet))
			{
				Project.Add(new ProjectCharacter(new PixelMap(CharacterWidth, CharacterHeight), character));
			}
			foreach (var character in additionalCharacters)
			{
				Project.Add(new ProjectCharacter(new PixelMap(CharacterWidth, CharacterHeight), character));
			}

			// Resize project characters
			if (_isNewProject)
			{
				Project.ChangeCharacterDimensions(CharacterWidth, CharacterHeight, false);
			}
			else
			{
				if (CharacterWidth != Project.CharacterWidth || CharacterHeight != Project.CharacterHeight)
				{
					Project.ChangeCharacterDimensions(CharacterWidth, CharacterHeight, true);
				}
			}

			DialogResult = true;
			Close();
			e.Handled = true;
		}
		#endregion

		#region Event Handlers (Character Metrics)
		private void ButtonAscentMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.AscentHeight--;
			e.Handled = true;
		}

		private void ButtonAscentPlus_Click(object sender, RoutedEventArgs e)
		{
			Project.AscentHeight++;
			e.Handled = true;
		}

		private void ButtonDescentMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.DescentHeight--;
			e.Handled = true;
		}

		private void ButtonDescentPlus_Click(object sender, RoutedEventArgs e)
		{
			Project.DescentHeight++;
			e.Handled = true;
		}

		private void ButtonUppercaseMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.UppercaseHeight--;
			e.Handled = true;
		}

		private void ButtonUppercasePlus_Click(object sender, RoutedEventArgs e)
		{
			Project.UppercaseHeight++;
			e.Handled = true;
		}

		private void ButtonLowercaseMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.LowercaseHeight--;
			e.Handled = true;
		}

		private void ButtonLowercasePlus_Click(object sender, RoutedEventArgs e)
		{
			Project.LowercaseHeight++;
			e.Handled = true;
		}

		private void ButtonLeftBearingMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.LeftBearingWidth--;
			e.Handled = true;
		}

		private void ButtonLeftBearingPlus_Click(object sender, RoutedEventArgs e)
		{
			Project.LeftBearingWidth++;
			e.Handled = true;
		}

		private void ButtonRightBearingMinus_Click(object sender, RoutedEventArgs e)
		{
			Project.RightBearingWidth--;
			e.Handled = true;
		}

		private void ButtonRightBearingPlus_Click(object sender, RoutedEventArgs e)
		{
			Project.RightBearingWidth++;
			e.Handled = true;
		}
		#endregion

		#region Private Methods
		private static bool ValidateDimension(int value)
		{
			return value > 0 && value <= 64;
		}
		#endregion
	}
}