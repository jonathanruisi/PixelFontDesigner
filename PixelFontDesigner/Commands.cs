#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       Commands.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:36 AM
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
using System.Windows;
using System.Windows.Input;

using JonathanRuisi.PixelFontDesigner.Properties;
using JonathanRuisi.PixelFontDesigner.ViewModel;
using JonathanRuisi.PixelFontDesigner.Windows;

namespace JonathanRuisi.PixelFontDesigner
{
	public static class Commands
	{
		public static class FileNewCommand
		{
			public static RoutedUICommand FileNew { get; private set; }

			static FileNewCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.N, ModifierKeys.Control, "Ctrl + N")};
				FileNew = new RoutedUICommand("New...", "FileNew",
					typeof(FileNewCommand), gestures);
			}
		}

		public static class FileOpenCommand
		{
			public static RoutedUICommand FileOpen { get; private set; }

			static FileOpenCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.O, ModifierKeys.Control, "Ctrl + O")};
				FileOpen = new RoutedUICommand("Open...", "FileOpen",
					typeof(FileOpenCommand), gestures);
			}
		}

		public static class FileSaveCommand
		{
			public static RoutedUICommand FileSave { get; private set; }

			static FileSaveCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.S, ModifierKeys.Control, "Ctrl + S")};
				FileSave = new RoutedUICommand("Save...", "FileSave",
					typeof(FileSaveCommand), gestures);
			}
		}

		public static class FileSaveAsCommand
		{
			public static RoutedUICommand FileSaveAs { get; private set; }

			static FileSaveAsCommand()
			{
				var gestures = new InputGestureCollection
				{
					new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift, "Ctrl + Shift + S")
				};
				FileSaveAs = new RoutedUICommand("Save As...", "FileSaveAs",
					typeof(FileSaveAsCommand), gestures);
			}
		}

		public static class FileCloseCommand
		{
			public static RoutedUICommand FileClose { get; private set; }

			static FileCloseCommand()
			{
				var gestures = new InputGestureCollection
				{
					new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl + W")
				};
				FileClose = new RoutedUICommand("Close", "FileClose",
					typeof(FileCloseCommand), gestures);
			}
		}

		public static class FileProjectSettingsCommand
		{
			public static RoutedUICommand FileProjectSettings { get; private set; }

			static FileProjectSettingsCommand()
			{
				FileProjectSettings = new RoutedUICommand("Project Settings...", "FileProjectSettings",
					typeof(FileExportCommand));
			}
		}

		public static class FileExportCommand
		{
			public static RoutedUICommand FileExport { get; private set; }

			static FileExportCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.E, ModifierKeys.Control, "Ctrl + E")};
				FileExport = new RoutedUICommand("Export Character Data...", "FileExport",
					typeof(FileExportCommand), gestures);
			}
		}

		public static class FileExitCommand
		{
			public static RoutedUICommand FileExit { get; private set; }

			static FileExitCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.Q, ModifierKeys.Control, "Ctrl + Q")};
				FileExit = new RoutedUICommand("Exit", "FileExit",
					typeof(FileExitCommand), gestures);
			}
		}

		public static class ToolsPreferencesCommand
		{
			public static RoutedUICommand ToolsPreferences { get; private set; }

			static ToolsPreferencesCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.K, ModifierKeys.Control, "Ctrl + K")};
				ToolsPreferences = new RoutedUICommand("Preferences...", "ToolsPreferences",
					typeof(ToolsPreferencesCommand), gestures);
			}
		}

		public static class HelpAboutCommand
		{
			public static RoutedUICommand HelpAbout { get; private set; }

			static HelpAboutCommand()
			{
				var gestures = new InputGestureCollection {new KeyGesture(Key.F1, ModifierKeys.None, "F1")};
				HelpAbout = new RoutedUICommand("About...", "HelpAbout",
					typeof(HelpAboutCommand), gestures);
			}
		}
	}

	public partial class MainWindow
	{
		#region Private Methods
		private void InitializeCommands()
		{
			// Associate controls with respective commands
			MenuItemFileNew.Command = Commands.FileNewCommand.FileNew;
			MenuItemFileOpen.Command = Commands.FileOpenCommand.FileOpen;
			MenuItemFileSave.Command = Commands.FileSaveCommand.FileSave;
			MenuItemFileSaveAs.Command = Commands.FileSaveAsCommand.FileSaveAs;
			MenuItemFileClose.Command = Commands.FileCloseCommand.FileClose;
			MenuItemFileProjectSettings.Command = Commands.FileProjectSettingsCommand.FileProjectSettings;
			MenuItemFileExport.Command = Commands.FileExportCommand.FileExport;
			MenuItemFileExit.Command = Commands.FileExitCommand.FileExit;
			MenuItemToolsPreferences.Command =
				Commands.ToolsPreferencesCommand.ToolsPreferences;
			MenuItemHelpAbout.Command = Commands.HelpAboutCommand.HelpAbout;

			// Add command bindings to the command system
			var binding = new CommandBinding(Commands.FileNewCommand.FileNew);
			binding.CanExecute += FileNew_CanExecute;
			binding.Executed += FileNew_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileOpenCommand.FileOpen);
			binding.CanExecute += FileOpen_CanExecute;
			binding.Executed += FileOpen_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileSaveCommand.FileSave);
			binding.CanExecute += FileSave_CanExecute;
			binding.Executed += FileSave_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileSaveAsCommand.FileSaveAs);
			binding.CanExecute += FileSaveAs_CanExecute;
			binding.Executed += FileSaveAs_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileCloseCommand.FileClose);
			binding.CanExecute += FileClose_CanExecute;
			binding.Executed += FileClose_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileProjectSettingsCommand.FileProjectSettings);
			binding.CanExecute += FileProjectSettings_CanExecute;
			binding.Executed += FileProjectSettings_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileExportCommand.FileExport);
			binding.CanExecute += FileExport_CanExecute;
			binding.Executed += FileExport_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.FileExitCommand.FileExit);
			binding.CanExecute += FileExit_CanExecute;
			binding.Executed += FileExit_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.ToolsPreferencesCommand.ToolsPreferences);
			binding.CanExecute += ToolsPreferences_CanExecute;
			binding.Executed += ToolsPreferences_Executed;
			CommandBindings.Add(binding);
			binding = new CommandBinding(Commands.HelpAboutCommand.HelpAbout);
			binding.CanExecute += HelpAbout_CanExecute;
			binding.Executed += HelpAbout_Executed;
			CommandBindings.Add(binding);
		}
		#endregion

		#region Event Handlers (Command CanExecute)
		private void FileNew_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void FileOpen_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void FileSave_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = (Project != null && _projectChanged);
		}

		private void FileSaveAs_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Project != null;
		}

		private void FileClose_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Project != null;
		}

		private void FileProjectSettings_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Project != null;
		}

		private void FileExport_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = Project != null;
		}

		private void FileExit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void ToolsPreferences_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}

		private void HelpAbout_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
		#endregion

		#region Event Handlers (Command Executed)
		private void FileNew_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			EditProjectSettings(true);
			e.Handled = true;
		}

		private void FileOpen_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (PromptSaveProjectChangesAction())
			{
				string fileName;
				if (ShowOpenFileDialog("Open Project", out fileName))
				{
					Settings.Default.CurrentProjectFile = fileName;
				}

				if (Project == null)
					Project = new ProjectManager();
				if (!Project.Load(fileName))
				{
					MessageBox.Show("Unable to load specified project file", "Unrecognized Format",
						MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
			e.Handled = true;
		}

		private void FileSave_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (String.IsNullOrEmpty(Settings.Default.CurrentProjectFile))
			{
				string fileName;
				if (ShowSaveFileDialog("Save Project", "*.pfdp", "Pixel Font Designer Project Files (*.pfdp)|*.pfdp", out fileName))
				{
					Settings.Default.CurrentProjectFile = fileName;
				}
			}
			Project.Save(Settings.Default.CurrentProjectFile);
			_projectChanged = false;
			e.Handled = true;
		}

		private void FileSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string fileName;
			if (ShowSaveFileDialog("Save Project As", "*.pfdp", "Pixel Font Designer Project Files (*.pfdp)|*.pfdp", out fileName))
			{
				Settings.Default.CurrentProjectFile = fileName;
			}
			Project.Save(Settings.Default.CurrentProjectFile);
			_projectChanged = false;
			e.Handled = true;
		}

		private void FileClose_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (PromptSaveProjectChangesAction())
			{
				Project = null;
			}
			e.Handled = true;
		}

		private void FileProjectSettings_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			ListBoxProjectData.SelectedItem = null;
			EditProjectSettings(false);
			e.Handled = true;
		}

		private void FileExport_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			string fileName;
			if (ShowSaveFileDialog("Export Project", "*.txt", "Text Files (*.txt)|*.txt", out fileName))
				ExportProject(fileName);
			e.Handled = true;
		}

		private void FileExit_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			Close();
			e.Handled = true;
		}

		private void ToolsPreferences_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var preferencesWindow = new PreferencesWindow(_csManager);
			preferencesWindow.ShowDialog();
			e.Handled = true;
		}

		private void HelpAbout_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			var about = new AboutWindow();
			about.ShowDialog();
			e.Handled = true;
		}
		#endregion
	}
}