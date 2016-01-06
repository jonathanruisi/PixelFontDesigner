#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       ProjectManager.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:26 AM
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
using System.Xml.Linq;

using JonathanRuisi.UtilityLibrary.Xml;

namespace JonathanRuisi.PixelFontDesigner.ViewModel
{
	public sealed class ProjectManager : XmlViewModel<ProjectCharacter>
	{
		#region Fields
		private string _projectName;
		private int _characterWidth, _characterHeight;
		private int _ascentHeight, _descentHeight, _uppercaseHeight, _lowercaseHeight, _leftBearingWidth, _rightBearingWidth;
		private bool _isAscentOverlay, _isDescentOverlay, _isBearingOverlay;
		private bool _isBaselineGuide, _isUppercaseGuide, _isLowercaseGuide, _isBearingGuides;
		private bool _isExtendGuides;
		#endregion

		#region Properties
		public override string NodeName => "Project";
		public string ProjectName { get { return _projectName; } set { SetProperty(ref _projectName, value); } }
		public int CharacterWidth { get { return _characterWidth; } private set { SetProperty(ref _characterWidth, value); } }

		public int CharacterHeight
		{
			get { return _characterHeight; }
			private set { SetProperty(ref _characterHeight, value); }
		}

		public int AscentHeight
		{
			get { return _ascentHeight; }
			set
			{
				if (value >= 0 && value <= _characterHeight)
					SetProperty(ref _ascentHeight, value);
			}
		}

		public int DescentHeight
		{
			get { return _descentHeight; }
			set
			{
				if (value >= 0 && value <= _characterHeight)
					SetProperty(ref _descentHeight, value);
			}
		}

		public int UppercaseHeight
		{
			get { return _uppercaseHeight; }
			set
			{
				if (value >= 0 && value <= _characterHeight)
					SetProperty(ref _uppercaseHeight, value);
			}
		}

		public int LowercaseHeight
		{
			get { return _lowercaseHeight; }
			set
			{
				if (value >= 0 && value <= _characterHeight)
					SetProperty(ref _lowercaseHeight, value);
			}
		}

		public int LeftBearingWidth
		{
			get { return _leftBearingWidth; }
			set
			{
				if (value >= 0 && value <= _characterWidth)
					SetProperty(ref _leftBearingWidth, value);
			}
		}

		public int RightBearingWidth
		{
			get { return _rightBearingWidth; }
			set
			{
				if (value >= 0 && value <= _characterWidth)
					SetProperty(ref _rightBearingWidth, value);
			}
		}

		public bool IsAscentOverlay { get { return _isAscentOverlay; } set { SetProperty(ref _isAscentOverlay, value); } }
		public bool IsDescentOverlay { get { return _isDescentOverlay; } set { SetProperty(ref _isDescentOverlay, value); } }
		public bool IsBearingOverlay { get { return _isBearingOverlay; } set { SetProperty(ref _isBearingOverlay, value); } }
		public bool IsBaselineGuide { get { return _isBaselineGuide; } set { SetProperty(ref _isBaselineGuide, value); } }
		public bool IsUppercaseGuide { get { return _isUppercaseGuide; } set { SetProperty(ref _isUppercaseGuide, value); } }
		public bool IsLowercaseGuide { get { return _isLowercaseGuide; } set { SetProperty(ref _isLowercaseGuide, value); } }
		public bool IsBearingGuides { get { return _isBearingGuides; } set { SetProperty(ref _isBearingGuides, value); } }
		public bool IsExtendGuides { get { return _isExtendGuides; } set { SetProperty(ref _isExtendGuides, value); } }
		#endregion

		#region Constructors
		public ProjectManager() : this(8, 8, 0, 0, 0, 0, 0, 0, true, true, true, true, true, true, true, false) { }

		public ProjectManager(ProjectManager projectManager)
			: this(
				projectManager.CharacterWidth, projectManager.CharacterHeight, projectManager.AscentHeight,
				projectManager.DescentHeight, projectManager.UppercaseHeight, projectManager.LowercaseHeight,
				projectManager.LeftBearingWidth, projectManager.RightBearingWidth, projectManager.IsAscentOverlay,
				projectManager.IsDescentOverlay, projectManager.IsBearingOverlay, projectManager.IsBaselineGuide,
				projectManager.IsUppercaseGuide, projectManager.IsLowercaseGuide, projectManager.IsBearingGuides,
				projectManager.IsExtendGuides, projectManager.ProjectName)
		{
			AddRange(projectManager);
		}

		private ProjectManager(
			int characterWidth, int characterHeight,
			int ascentHeight, int descentHeight,
			int uppercaseHeight, int lowercaseHeight,
			int leftBearingWidth, int rightBearingWidth,
			bool isAscentOverlay, bool isDescentOverlay, bool isBearingOverlay,
			bool isBaselineGuide, bool isUppercaseGuide, bool isLowercaseGuide, bool isBearingGuides, bool isExtendGuides,
			string projectName = "Untitled Project")
		{
			ProjectName = projectName;
			CharacterWidth = characterWidth;
			CharacterHeight = characterHeight;
			IsAscentOverlay = isAscentOverlay;
			IsDescentOverlay = isDescentOverlay;
			IsBearingOverlay = isBearingOverlay;
			IsBaselineGuide = isBaselineGuide;
			IsUppercaseGuide = isUppercaseGuide;
			IsLowercaseGuide = isLowercaseGuide;
			IsBearingGuides = isBearingGuides;
			IsExtendGuides = isExtendGuides;
			AscentHeight = ascentHeight;
			DescentHeight = descentHeight;
			UppercaseHeight = uppercaseHeight;
			LowercaseHeight = lowercaseHeight;
			LeftBearingWidth = leftBearingWidth;
			RightBearingWidth = rightBearingWidth;
		}
		#endregion

		#region Public Methods
		public void ChangeCharacterDimensions(int newWidth, int newHeight, bool resizeExistingCharacters)
		{
			if (newWidth == CharacterWidth && newHeight == CharacterHeight)
				return;

			if (resizeExistingCharacters)
			{
				foreach (var character in this)
				{
					character.Pixels.Resize(newWidth, newHeight);
				}
			}

			CharacterWidth = newWidth;
			CharacterHeight = newHeight;
		}
		#endregion

		#region Interface Implementation (IXNode<T>)
		public override XElement ToXNode()
		{
			var element = base.ToXNode();
			element.Add(new XAttribute("ProjectName", ProjectName));
			element.Add(new XAttribute("CharacterWidth", CharacterWidth));
			element.Add(new XAttribute("CharacterHeight", CharacterHeight));
			element.Add(new XAttribute("AO", IsAscentOverlay));
			element.Add(new XAttribute("DO", IsDescentOverlay));
			element.Add(new XAttribute("BRO", IsBearingOverlay));
			element.Add(new XAttribute("BG", IsBaselineGuide));
			element.Add(new XAttribute("UG", IsUppercaseGuide));
			element.Add(new XAttribute("LG", IsLowercaseGuide));
			element.Add(new XAttribute("BRG", IsBearingGuides));
			element.Add(new XAttribute("EG", IsExtendGuides));
			element.Add(new XAttribute("AH", AscentHeight));
			element.Add(new XAttribute("DH", DescentHeight));
			element.Add(new XAttribute("UH", UppercaseHeight));
			element.Add(new XAttribute("LH", LowercaseHeight));
			element.Add(new XAttribute("LBW", LeftBearingWidth));
			element.Add(new XAttribute("RBW", RightBearingWidth));
			return element;
		}

		public override void FromXNode(XElement element)
		{
			base.FromXNode(element);
			ProjectName = element.Attribute("ProjectName").Value;
			CharacterWidth = Int32.Parse(element.Attribute("CharacterWidth").Value);
			CharacterHeight = Int32.Parse(element.Attribute("CharacterHeight").Value);
			IsAscentOverlay = Boolean.Parse(element.Attribute("AO").Value);
			IsDescentOverlay = Boolean.Parse(element.Attribute("DO").Value);
			IsBearingOverlay = Boolean.Parse(element.Attribute("BRO").Value);
			IsBaselineGuide = Boolean.Parse(element.Attribute("BG").Value);
			IsUppercaseGuide = Boolean.Parse(element.Attribute("UG").Value);
			IsLowercaseGuide = Boolean.Parse(element.Attribute("LG").Value);
			IsBearingGuides = Boolean.Parse(element.Attribute("BRG").Value);
			IsExtendGuides = Boolean.Parse(element.Attribute("EG").Value);
			AscentHeight = Int32.Parse(element.Attribute("AH").Value);
			DescentHeight = Int32.Parse(element.Attribute("DH").Value);
			UppercaseHeight = Int32.Parse(element.Attribute("UH").Value);
			LowercaseHeight = Int32.Parse(element.Attribute("LH").Value);
			LeftBearingWidth = Int32.Parse(element.Attribute("LBW").Value);
			RightBearingWidth = Int32.Parse(element.Attribute("RBW").Value);
		}
		#endregion
	}
}