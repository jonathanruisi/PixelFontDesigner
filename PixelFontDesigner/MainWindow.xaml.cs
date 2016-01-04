#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       MainWindow.xaml.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:29 AM
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

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

using JonathanRuisi.PixelFontDesigner.Properties;
using JonathanRuisi.PixelFontDesigner.ViewModel;
using JonathanRuisi.UtilityLibrary.IO;
using JonathanRuisi.WpfControlLibrary;

namespace JonathanRuisi.PixelFontDesigner
{
	public partial class MainWindow
	{
		#region Fields
		private bool _projectChanged, _gridChanged, _isSwitchingDisplayedCharacter;
		private CharacterSetManager _csManager;
		#endregion

		#region Properties
		public static readonly DependencyProperty ProjectProperty =
			DependencyProperty.Register("Project", typeof(ProjectManager), typeof(MainWindow),
				new FrameworkPropertyMetadata(null, OnDependencyPropertyChanged));

		public ProjectManager Project
		{
			get { return (ProjectManager) GetValue(ProjectProperty); }
			set { SetValue(ProjectProperty, value); }
		}
		#endregion

		#region Constructor
		public MainWindow()
		{
			DataContext = this;
			InitializeComponent();
			InitializeCommands();
		}
		#endregion

		#region Property Changed Callbacks
		private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var mainWnd = d as MainWindow;
			if (mainWnd == null) return;

			if (e.Property == ProjectProperty)
			{
				var project = e.NewValue as ProjectManager;
				if (project == null)
				{
					mainWnd.StatusBarItemSeparator1.Visibility = Visibility.Collapsed;
					mainWnd.StatusBarItemSeparator2.Visibility = Visibility.Collapsed;
					mainWnd.StatusBarItemCharacterDimensions.Visibility = Visibility.Collapsed;
					mainWnd.StatusBarItemCharacterCount.Visibility = Visibility.Collapsed;
					mainWnd.StatusBarItemProjectName.Text = "NO PROJECT LOADED";
				}
				else
				{
					project.Loaded += mainWnd.Project_Loaded;
					project.PropertyChanged += mainWnd.Project_PropertyChanged;
					mainWnd.StatusBarItemSeparator1.Visibility = Visibility.Visible;
					mainWnd.StatusBarItemSeparator2.Visibility = Visibility.Visible;
					mainWnd.StatusBarItemCharacterCount.Visibility = Visibility.Visible;
					mainWnd.StatusBarItemCharacterDimensions.Visibility = Visibility.Visible;
					mainWnd.SyncProjectSettings();
				}
			}
		}
		#endregion

		#region Event Handlers (Main Window)
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			// Configure bindings
			var binding = new Binding("SelectedItems.Count")
			{
				Source = ListBoxProjectData,
				Converter = new IntegerToVisibilityConverter(),
				ConverterParameter = "=,1,Hidden"
			};
			MainPixelGrid.SetBinding(VisibilityProperty, binding);
			GuidePanel.SetBinding(VisibilityProperty, binding);

			binding = new Binding("IsAscentOverlayVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonAscentOverlay.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsDescentOverlayVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonDescentOverlay.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsBearingOverlayVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonBearingOverlay.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsBaselineGuideVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonBaselineGuide.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsUppercaseGuideVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonUppercaseGuide.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsLowercaseGuideVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonLowercaseGuide.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("IsBearingGuideVisible") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonBearingGuide.SetBinding(ToggleButton.IsCheckedProperty, binding);
			binding = new Binding("ExtendGuideLines") {Source = MainPixelGrid, Mode = BindingMode.TwoWay};
			ButtonExtendGuides.SetBinding(ToggleButton.IsCheckedProperty, binding);

			// Load the character set color scheme
			ColorScheme scheme;
			if (Settings.Default.Preferences_Appearance_CharacterSetColors == null ||
			    Settings.Default.Preferences_Appearance_CharacterSetColors.Count == 0)
			{
				scheme = ColorScheme.BlackOnlyScheme;
			}
			else
			{
				scheme = new ColorScheme(Settings.Default.Preferences_Appearance_CharacterSetColors);
			}

			// Load the character set database
			_csManager = new CharacterSetManager(scheme);
			if (!_csManager.Load(Settings.Default.Database_Filename))
			{
				_csManager.Clear();
				_csManager.Add(new CharacterSet());
				_csManager.Save(Settings.Default.Database_Filename);
				MessageBox.Show(
					"The character set database is either missing or corrupted.\nA new empty database has been created.",
					"Database missing",
					MessageBoxButton.OK,
					MessageBoxImage.Exclamation);
			}

			// Subscribe to events
			MainPixelGrid.GridUpdated += MainPixelGrid_GridUpdated;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			e.Cancel = !PromptSaveProjectChangesAction();
		}
		#endregion

		#region Event Handlers (Pixel Grid)
		private void MainPixelGrid_GridUpdated(object sender, RoutedEventArgs e)
		{
			if (ListBoxProjectData.SelectedItems.Count != 1 || _isSwitchingDisplayedCharacter)
				return;
			_gridChanged = true;
		}

		private void PixelGridBorder_MouseLeave(object sender, MouseEventArgs e)
		{
			CommitCurrentCharacterPixels(ListBoxProjectData.SelectedItem as ProjectCharacter);
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			CommitCurrentCharacterPixels(ListBoxProjectData.SelectedItem as ProjectCharacter);
		}
		#endregion

		#region Event Handlers (Project Controls)
		private void ButtonProjectInclude_Click(object sender, RoutedEventArgs e)
		{
			foreach (var character in ListBoxProjectData.SelectedItems.OfType<ProjectCharacter>())
			{
				character.IsIncludedForExport = true;
			}
			UpdateProjectButtonStatus();
			e.Handled = true;
		}

		private void ButtonProjectExclude_Click(object sender, RoutedEventArgs e)
		{
			foreach (var character in ListBoxProjectData.SelectedItems.OfType<ProjectCharacter>())
			{
				character.IsIncludedForExport = false;
			}
			UpdateProjectButtonStatus();
			e.Handled = true;
		}

		private void ListBoxProjectData_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.RemovedItems.Count == 1 && _gridChanged)
			{
				CommitCurrentCharacterPixels(e.RemovedItems[0] as ProjectCharacter);
			}
			if (e.AddedItems.Count == 1)
			{
				var projectCharacter = e.AddedItems[0] as ProjectCharacter;
				if (projectCharacter != null)
				{
					_isSwitchingDisplayedCharacter = true;
					MainPixelGrid.SetGridData(projectCharacter.Pixels);
					_isSwitchingDisplayedCharacter = false;
				}
			}
			UpdateProjectButtonStatus();
			e.Handled = true;
		}

		private void ButtonMoveUp_Click(object sender, RoutedEventArgs e)
		{
			MainPixelGrid.Transform(-1, 0, 0, 0);
		}

		private void ButtonMoveDown_Click(object sender, RoutedEventArgs e)
		{
			MainPixelGrid.Transform(0, 1, 0, 0);
		}

		private void ButtonMoveLeft_Click(object sender, RoutedEventArgs e)
		{
			MainPixelGrid.Transform(0, 0, -1, 0);
		}

		private void ButtonMoveRight_Click(object sender, RoutedEventArgs e)
		{
			MainPixelGrid.Transform(0, 0, 0, 1);
		}
		#endregion

		#region Event Handlers (Project)
		private void Project_Loaded(object sender, FileActionEventArgs e)
		{
			SyncProjectSettings();
		}

		private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Count" || e.PropertyName == "Item[]" || e.PropertyName == "IsIncludedForExport")
				UpdateProjectCharacterCount();
			_projectChanged = true;
		}
		#endregion
	}
}