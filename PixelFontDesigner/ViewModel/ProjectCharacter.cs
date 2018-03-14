#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       ProjectCharacter.cs
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
using System.Linq;
using System.Xml.Linq;

using JLR.Utility.NET.Collections;

namespace JonathanRuisi.PixelFontDesigner.ViewModel
{
	public sealed class ProjectCharacter : Character
	{
		#region Fields
		private PixelMap _pixels;
		private bool _isIncludedForExport;
		#endregion

		#region Properties
		public override string NodeName => "ProjectCharacter";

		public bool IsIncludedForExport
		{
			get { return _isIncludedForExport; }
			set { SetProperty(ref _isIncludedForExport, value); }
		}

		public PixelMap Pixels
		{
			get { return _pixels; }
			set { SetProperty(value, () => _pixels, newValue => _pixels = newValue.Clone() as PixelMap); }
		}
		#endregion

		#region Constructors
		public ProjectCharacter() : this(5, 7, new Character()) { }

		public ProjectCharacter(int width, int height, Character character, bool isIncludedForExport = true)
			: this(new PixelMap(width, height), character, isIncludedForExport) { }

		public ProjectCharacter(PixelMap pixelData, Character character, bool isIncludedForExport = true)
			: this(
				pixelData, character.CharacterSet, isIncludedForExport, character.Number, character.Symbol, character.Description) { }

		public ProjectCharacter(PixelMap pixelData, CharacterSet characterSet, bool isIncludedForExport = true,
			int number = 0, char symbol = Char.MinValue, string description = default(string))
			: base(characterSet, number, symbol, description)
		{
			Pixels = pixelData;
			IsIncludedForExport = isIncludedForExport;
		}
		#endregion

		#region Interface Implementation (IXNode<T>)
		public override XElement ToXNode()
		{
			var element = base.ToXNode();
			element.Add(Pixels.ToXNode());
			return element;
		}

		public override void FromXNode(XElement element)
		{
			base.FromXNode(element);
			Pixels = new PixelMap(Pixels.Width, Pixels.Height);
			Pixels.FromXNode(element.Descendants("PixelMap").Single());
		}
		#endregion
	}
}