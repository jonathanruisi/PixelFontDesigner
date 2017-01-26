#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       PrivateMethods.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:37 AM
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
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

using JonathanRuisi.PixelFontDesigner.Properties;
using JonathanRuisi.PixelFontDesigner.ViewModel;
using JonathanRuisi.PixelFontDesigner.Windows;
using JonathanRuisi.UtilityLibrary;
using JonathanRuisi.WpfControlLibrary;

using Microsoft.Win32;

namespace JonathanRuisi.PixelFontDesigner
{
	public partial class MainWindow
	{
		#region Private Methods (File Dialogs)
		private bool ShowOpenFileDialog(string title, out string fileName)
		{
			fileName = null;
			var ofd = new OpenFileDialog
			{
				AddExtension = true,
				CheckFileExists = true,
				CheckPathExists = true,
				DefaultExt = ".pfdp",
				Filter = "Pixel Font Designer Project Files (*.pfdp)|*.pfdp",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				Title = title
			};

			var result = ofd.ShowDialog();
			if (result == true)
			{
				fileName = ofd.FileName;
				return true;
			}
			return false;
		}

		private bool ShowSaveFileDialog(string title, string defaultExtension, string filter, out string fileName)
		{
			fileName = null;
			var sfd = new SaveFileDialog
			{
				AddExtension = true,
				CheckFileExists = false,
				CheckPathExists = true,
				DefaultExt = defaultExtension,
				Filter = filter,
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				OverwritePrompt = true,
				Title = title
			};

			var result = sfd.ShowDialog();
			if (result == true)
			{
				fileName = sfd.FileName;
				return true;
			}
			return false;
		}
		#endregion

		#region Private Methods (User Prompts)
		/// <summary>
		/// Prompts the user whether or not to save any unsaved project changes.
		/// If the user chooses "Yes", the project is saved.
		/// </summary>
		/// <returns>
		/// <code>true</code> indicating that the application can continue to unload the current project,
		/// <code>false</code> indicating that the application should abort the command about to be executed.
		/// </returns>
		private bool PromptSaveProjectChangesAction()
		{
			if (Project != null && _projectChanged)
			{
				var result = MessageBox.Show("Save changes to current project?",
					"Unsaved Changes",
					MessageBoxButton.YesNoCancel,
					MessageBoxImage.Exclamation,
					MessageBoxResult.Cancel);
				if (result == MessageBoxResult.Cancel)
					return false;
				if (result == MessageBoxResult.Yes)
					Commands.FileSaveCommand.FileSave.Execute(null, this);
				Project.Clear();
			}
			return true;
		}
		#endregion

		#region Private Methods (Project)
		private void EditProjectSettings(bool isNewProject)
		{
			ProjectSettingsWindow psw;
			if (isNewProject)
			{
				if (!PromptSaveProjectChangesAction())
					return;
				Settings.Default.CurrentProjectFile = String.Empty;
				psw = new ProjectSettingsWindow(_csManager, null);
			}
			else
			{
				psw = new ProjectSettingsWindow(_csManager, Project);
			}

			if (psw.ShowDialog() == true)
				Project = new ProjectManager(psw.Project);
		}

		private void ExportProject(string fileName)
		{
			// Generate array name string
			var formatString = Settings.Default.Preferences_Export_ArrayDeclaration_Name;
			var arrayName = new StringBuilder();
			int startIndex = 0;
			for (int i = 0; i < formatString.Length; i++)
			{
				if (formatString[i] == '%' && i < formatString.Length - 1)
				{
					arrayName.Append(formatString.Substring(startIndex, i - startIndex));
					if (formatString[i + 1] == 'w' || formatString[i + 1] == 'W')
						arrayName.Append(Project.CharacterWidth);
					else if (formatString[i + 1] == 'h' || formatString[i + 1] == 'H')
						arrayName.Append(Project.CharacterHeight);
					else if (formatString[i + 1] == 'n' || formatString[i + 1] == 'N')
						arrayName.Append(Project.ProjectName);
					i++;
					startIndex = i + 1;
				}
			}
			if (startIndex < formatString.Length)
				arrayName.Append(formatString.Substring(startIndex, formatString.Length - startIndex));

			// Calculate array dimensions
			var bytesPerItem = Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation == RowOrColumn.Row
								   ? (int)Math.Ceiling((decimal)Project.CharacterHeight / 8)
								   : (int)Math.Ceiling((decimal)Project.CharacterWidth / 8);

			// Force IsPacked=false if multiple array elements are needed to represent one row/column
			if (bytesPerItem * 8 >= Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize)
				Settings.Default.Preferences_Export_ArrayDeclaration_IsPacked = false;

			var itemsPerElement = Settings.Default.Preferences_Export_ArrayDeclaration_IsPacked
									  ? Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize / (bytesPerItem * 8)
									  : 1;
			var majorDimension = Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation == RowOrColumn.Row
									 ? (int)Math.Ceiling((decimal)Project.CharacterHeight / itemsPerElement)
									 : (int)Math.Ceiling((decimal)Project.CharacterWidth / itemsPerElement);
			var minorDimension = (int)Math.Ceiling((decimal)(bytesPerItem * itemsPerElement * 8)
									/ Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize);

			// Query characters marked for export
			var characters = (from character in Project where character.IsIncludedForExport select character).ToList();

			using (var writer = new StreamWriter(fileName, false))
			{
				// Write array declaration
				writer.Write("{0} {1} {2}[{3}][{4}]", Settings.Default.Preferences_Export_ArrayDeclaration_Keywords,
					Settings.Default.Preferences_Export_ArrayDeclaration_Type, arrayName, characters.Count, majorDimension);
				if (minorDimension > 1)
					writer.Write("[{0}]", minorDimension);
				writer.WriteLine(" =");

				// Write opening bracket for the array
				writer.WriteLine('{');

				//		{0x11, 0x12, 0x13, 0x14},

				//		{{0x11, 0x1A}, {0x12, 0x1A}, {0x13, 0x1A}, {0x14, 0x1A}},

				// Write array contents
				for (int i = 0; i < characters.Count; i++)
				{
					// Get array data for current character
					var unpackedCharData = new byte[bytesPerItem * itemsPerElement * majorDimension];
					var charData8 = new byte[majorDimension, minorDimension];
					var charData16 = new ushort[majorDimension, minorDimension];
					var charData32 = new uint[majorDimension, minorDimension];
					var charData64 = new ulong[majorDimension, minorDimension];

					if (Settings.Default.Preferences_Export_ArrayDeclaration_IsPacked)
					{
						if (bytesPerItem == 1)
						{
							var tempData = characters[i].Pixels.ToArray8(
								Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
								Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
							for (var z = 0; z < itemsPerElement * majorDimension && z < tempData.GetLength(0); z++)
							{
								unpackedCharData[z] = tempData[z, 0];
							}
						}
						else if (bytesPerItem == 2)
						{
							var tempData = characters[i].Pixels.ToArray16(
								Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
								Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
							for (var z = 0; z < itemsPerElement * majorDimension; z++)
							{
								Array.Copy(BitConverter.GetBytes(tempData[z, 0]), 0, unpackedCharData, z * 2, bytesPerItem);
							}
						}
						else if (bytesPerItem > 2 && bytesPerItem <= 4)
						{
							var tempData = characters[i].Pixels.ToArray32(
								Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
								Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
							for (var z = 0; z < itemsPerElement * majorDimension; z++)
							{
								Array.Copy(BitConverter.GetBytes(tempData[z, 0]), 0, unpackedCharData, z * 4, bytesPerItem);
							}
						}
						else
						{
							var tempData = characters[i].Pixels.ToArray64(
								Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
								Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
							for (var z = 0; z < itemsPerElement * majorDimension; z++)
							{
								Array.Copy(BitConverter.GetBytes(tempData[z, 0]), 0, unpackedCharData, z * 8, bytesPerItem);
							}
						}

						switch (Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize)
						{
							case 8:
								for (var m = 0; m < majorDimension; m++)
								{
									charData8[m, 0] = unpackedCharData[m];
								}
								break;
							case 16:
								for (int m = 0; m < majorDimension; m++)
								{
									charData16[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 2);
								}
								break;
							case 32:
								for (int m = 0; m < majorDimension; m++)
								{
									if (bytesPerItem == 1)
									{
										charData32[m, 0] = BitConverter.ToUInt32(unpackedCharData, m * 4);
									}
									else if (bytesPerItem == 2)
									{
										charData32[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 4 + 2);
										charData32[m, 0] <<= 16;
										charData32[m, 0] |= BitConverter.ToUInt16(unpackedCharData, m * 4);
									}
								}
								break;
							case 64:
								for (int m = 0; m < majorDimension; m++)
								{
									if (bytesPerItem == 1)
									{
										charData64[m, 0] = BitConverter.ToUInt64(unpackedCharData, m * 8);
									}
									else if (bytesPerItem == 2)
									{
										charData64[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 8 + 6);
										charData64[m, 0] <<= 16;
										charData64[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 8 + 4);
										charData64[m, 0] <<= 16;
										charData64[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 8 + 2);
										charData64[m, 0] <<= 16;
										charData64[m, 0] = BitConverter.ToUInt16(unpackedCharData, m * 8);
									}
									else if (bytesPerItem == 4)
									{
										charData64[m, 0] = BitConverter.ToUInt32(unpackedCharData, m * 8 + 4);
										charData64[m, 0] <<= 32;
										charData64[m, 0] |= BitConverter.ToUInt32(unpackedCharData, m * 8);
									}
								}
								break;
						}
					}
					else
					{
						switch (Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize)
						{
							case 8:
								charData8 = characters[i].Pixels.ToArray8(Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
									Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
								break;
							case 16:
								charData16 = characters[i].Pixels.ToArray16(
									Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
									Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
								break;
							case 32:
								charData32 = characters[i].Pixels.ToArray32(
									Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
									Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
								break;
							case 64:
								charData64 = characters[i].Pixels.ToArray64(
									Settings.Default.Preferences_Export_ArrayDeclaration_Interpretation,
									Settings.Default.Preferences_Export_ArrayDeclaration_Endianness);
								break;
						}
					}

					// TODO: Implement binary number formatting
					// Until binary formatting is implemented, alert the user and abort the export process
					if (Settings.Default.Preferences_Export_ArrayElement_Radix == Radix.Binary)
					{
						MessageBox.Show(
							"Binary number format is not currently supported.\n"
							+ "Please change this setting in the application preferences.\n" + "The export process will be aborted...",
							"Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}

					writer.Write("\t{");
					for (int j = 0; j < majorDimension; j++)
					{
						if (minorDimension > 1) writer.Write('{');
						for (int k = 0; k < minorDimension; k++)
						{
							writer.Write("0x");
							switch (Settings.Default.Preferences_Export_ArrayDeclaration_TypeSize)
							{
								case 8:
									if (charData8[j, k] < 0x10U)
										writer.Write('0');
									writer.Write(
										charData8[j, k].ToString(Settings.Default.Preferences_Export_ArrayElement_Radix == Radix.Hexadecimal
											? "X"
											: "D"));
									break;
								case 16:
									if (charData16[j, k] < 0x1000U)
										writer.Write('0');
									if (charData16[j, k] < 0x100U)
										writer.Write('0');
									if (charData16[j, k] < 0x10U)
										writer.Write('0');
									writer.Write(
										charData16[j, k].ToString(Settings.Default.Preferences_Export_ArrayElement_Radix == Radix.Hexadecimal
											? "X"
											: "D"));
									break;
								case 32:
									if (charData32[j, k] < 0x10000000U)
										writer.Write('0');
									if (charData32[j, k] < 0x1000000U)
										writer.Write('0');
									if (charData32[j, k] < 0x100000U)
										writer.Write('0');
									if (charData32[j, k] < 0x10000U)
										writer.Write('0');
									if (charData32[j, k] < 0x1000U)
										writer.Write('0');
									if (charData32[j, k] < 0x100U)
										writer.Write('0');
									if (charData32[j, k] < 0x10U)
										writer.Write('0');
									writer.Write(
										charData32[j, k].ToString(Settings.Default.Preferences_Export_ArrayElement_Radix == Radix.Hexadecimal
											? "X"
											: "D"));
									break;
								case 64:
									if (charData64[j, k] < 0x1000000000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x100000000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x10000000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x1000000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x100000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x10000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x1000000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x100000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x10000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x1000000U)
										writer.Write('0');
									if (charData64[j, k] < 0x100000U)
										writer.Write('0');
									if (charData64[j, k] < 0x10000U)
										writer.Write('0');
									if (charData64[j, k] < 0x1000U)
										writer.Write('0');
									if (charData64[j, k] < 0x100U)
										writer.Write('0');
									if (charData64[j, k] < 0x10U)
										writer.Write('0');
									writer.Write(
										charData64[j, k].ToString(Settings.Default.Preferences_Export_ArrayElement_Radix == Radix.Hexadecimal
											? "X"
											: "D"));
									break;
							}
							if (k < minorDimension - 1)
								writer.Write(", ");
						}
						if (minorDimension > 1) writer.Write('}');
						if (j < majorDimension - 1)
							writer.Write(", ");
					}

					// Write comment (if specified in settings)
					if (Settings.Default.Preferences_Export_Miscellaneous_IncludeComments)
					{
						if (i == characters.Count - 1) writer.Write('}');
						else writer.Write("},");
						writer.WriteLine("\t// [{0}] {1} ({2})", characters[i].Number, characters[i].Symbol, characters[i].Description);
					}
					else
					{
						if (i == characters.Count - 1) writer.WriteLine('}');
						else writer.WriteLine("},");
					}
				}

				// Write closing bracket for the array
				writer.Write("};");
			}
		}

		private void SyncProjectSettings()
		{
			UpdateProjectCharacterCount();
			_gridChanged = false;
			_projectChanged = false;

			// Configure bindings
			var binding = new Binding("CharacterWidth") {Source = Project};
			MainPixelGrid.SetBinding(PixelGrid.PixelGridWidthProperty, binding);
			binding = new Binding("CharacterHeight") {Source = Project};
			MainPixelGrid.SetBinding(PixelGrid.PixelGridHeightProperty, binding);
			binding = new Binding("AscentHeight") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.AscentHeightProperty, binding);
			binding = new Binding("DescentHeight") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.DescentHeightProperty, binding);
			binding = new Binding("UppercaseHeight") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.UppercaseHeightProperty, binding);
			binding = new Binding("LowercaseHeight") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.LowercaseHeightProperty, binding);
			binding = new Binding("LeftBearingWidth") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.LeftBearingWidthProperty, binding);
			binding = new Binding("RightBearingWidth") {Source = Project};
			MainPixelGrid.SetBinding(GlyphPixelGrid.RightBearingWidthProperty, binding);
			binding = new Binding("IsAscentOverlay") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsAscentOverlayVisibleProperty, binding);
			binding = new Binding("IsDescentOverlay") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsDescentOverlayVisibleProperty, binding);
			binding = new Binding("IsBearingOverlay") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsBearingOverlayVisibleProperty, binding);
			binding = new Binding("IsBaselineGuide") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsBaselineGuideVisibleProperty, binding);
			binding = new Binding("IsUppercaseGuide") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsUppercaseGuideVisibleProperty, binding);
			binding = new Binding("IsLowercaseGuide") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsLowercaseGuideVisibleProperty, binding);
			binding = new Binding("IsBearingGuides") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.IsBearingGuideVisibleProperty, binding);
			binding = new Binding("IsExtendGuides") {Source = Project, Mode = BindingMode.TwoWay};
			MainPixelGrid.SetBinding(GlyphPixelGrid.ExtendGuideLinesProperty, binding);
			MainPixelGrid.ChangeAllPixels(false);
		}
		#endregion

		#region Private Methods (Control Updates)
		private void UpdateProjectButtonStatus()
		{
			if (ListBoxProjectData.SelectedItems.Count == 0)
			{
				ButtonProjectInclude.IsEnabled = false;
				ButtonProjectExclude.IsEnabled = false;
				return;
			}

			var allIncluded = true;
			var allExcluded = true;
			foreach (var character in ListBoxProjectData.SelectedItems.OfType<ProjectCharacter>())
			{
				if (allIncluded && !character.IsIncludedForExport)
					allIncluded = false;
				if (allExcluded && character.IsIncludedForExport)
					allExcluded = false;
			}

			if (!allIncluded && !allExcluded)
			{
				ButtonProjectInclude.IsEnabled = true;
				ButtonProjectExclude.IsEnabled = true;
			}
			else if (allIncluded)
			{
				ButtonProjectInclude.IsEnabled = false;
				ButtonProjectExclude.IsEnabled = true;
			}
			else
			{
				ButtonProjectInclude.IsEnabled = true;
				ButtonProjectExclude.IsEnabled = false;
			}
		}

		private void UpdateProjectCharacterCount()
		{
			StatusBarItemProjectName.Text = Project.ProjectName;
			StatusBarItemCharacterDimensions.Text = $"{Project.CharacterWidth} X {Project.CharacterHeight}";
			StatusBarItemCharacterCount.Text =
				$"Total Characters: {Project.Count} ({Project.Count(character => character.IsIncludedForExport)} marked for export)";
		}

		private void CommitCurrentCharacterPixels(ProjectCharacter character)
		{
			if (character != null && _gridChanged)
				character.Pixels = MainPixelGrid.GridData;
			_gridChanged = false;
		}
		#endregion
	}
}