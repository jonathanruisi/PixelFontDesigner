#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       CharacterSet.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:24 AM
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
using System.Windows.Media;
using System.Xml.Linq;

using JonathanRuisi.UtilityLibrary.Color;
using JonathanRuisi.UtilityLibrary.Xml;

namespace JonathanRuisi.PixelFontDesigner.ViewModel
{
	public sealed class CharacterSet : XmlViewModelElement<Character>
	{
		#region Fields
		private string _setName;
		private ColorSpace _mainColor;
		#endregion

		#region Properties
		public override string NodeName => "CharacterSet";
		public string SetName { get { return _setName; } set { SetProperty(ref _setName, value); } }
		public ColorSpace MainColor { get { return _mainColor; } set { SetProperty(ref _mainColor, value); } }

		public SolidColorBrush MainColorBrush
		{
			get { return new SolidColorBrush((Color) MainColor); }
			set { SetProperty(value, () => MainColorBrush, x => MainColor = (ColorSpace) x.Color); }
		}

		public SolidColorBrush OverlayColorBrush => new SolidColorBrush((Color) MainColor.GetAutoDarkenOrLighten());
		#endregion

		#region Constructors
		public CharacterSet() : this(null, ColorSpace.OpaqueBlack) { }

		public CharacterSet(string name, ColorSpace mainColor)
		{
			SetName = String.IsNullOrEmpty(name) ? "Character Set" : name;
			MainColor = mainColor;
		}
		#endregion

		#region Protected Methods
		protected override void OnItemAdded(Character item)
		{
			base.OnItemAdded(item);
			item.CharacterSet = this;
		}
		#endregion

		#region Interface Implementation (IXNode<T>)
		public override XElement ToXNode()
		{
			var element = base.ToXNode();
			element.Add(new XAttribute("SetName", SetName));
			return element;
		}

		public override void FromXNode(XElement element)
		{
			base.FromXNode(element);
			SetName = element.Attribute("SetName").Value;
		}
		#endregion
	}
}