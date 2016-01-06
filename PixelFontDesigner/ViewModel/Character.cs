#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       Character.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:21 AM
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
	public class Character : XmlViewModelElement<Character>
	{
		#region Fields
		private CharacterSet _characterSet;
		private int _number;
		private char _symbol;
		private string _description;
		#endregion

		#region Properties
		public override string NodeName => "Character";
		public CharacterSet CharacterSet { get { return _characterSet; } set { SetProperty(ref _characterSet, value); } }
		public int Number { get { return _number; } set { SetProperty(ref _number, value); } }
		public char Symbol { get { return _symbol; } set { SetProperty(ref _symbol, value); } }
		public string Description { get { return _description; } set { SetProperty(ref _description, value); } }
		#endregion

		#region Constructors
		public Character() : this(null) { }

		public Character(CharacterSet characterSet, int number = 0, char symbol = Char.MinValue,
			string description = default(string))
		{
			CharacterSet = characterSet ?? new CharacterSet();
			Number = number;
			Symbol = symbol;
			Description = description;
		}
		#endregion

		#region Interface Implementation (IXNode<T>)
		public override XElement ToXNode()
		{
			var element = base.ToXNode();
			element.Add(new XAttribute("Number", Number));
			element.Add(new XAttribute("Symbol", Symbol));
			element.Add(new XAttribute("Description", Description));
			return element;
		}

		public override void FromXNode(XElement element)
		{
			base.FromXNode(element);
			Number = Int32.Parse(element.Attribute("Number").Value);
			Symbol = Char.Parse(element.Attribute("Symbol").Value);
			Description = element.Attribute("Description").Value;
		}
		#endregion
	}
}