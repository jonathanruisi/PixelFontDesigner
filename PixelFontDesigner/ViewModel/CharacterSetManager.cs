#region Header
// ┏━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
// ┃  FILE:       CharacterSetManager.cs
// ┃  PROJECT:    PixelFontDesigner
// ┃  SOLUTION:   PixelFontDesigner
// ┃  CREATED:    2016-01-04 @ 6:25 AM
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

using JonathanRuisi.UtilityLibrary.Xml;

namespace JonathanRuisi.PixelFontDesigner.ViewModel
{
	public sealed class CharacterSetManager : XmlViewModel<CharacterSet>
	{
		#region Fields
		private ColorScheme _colorScheme;
		#endregion

		#region Properties
		public override string NodeName { get { return "CharacterSets"; } }

		public ColorScheme ColorScheme
		{
			get { return _colorScheme; }
			set
			{
				SetProperty(value, () => _colorScheme, newValue =>
				{
					if (newValue == null) return;
					_colorScheme = newValue;
					foreach (var characterSet in this)
					{
						characterSet.MainColor = newValue.GetNextAvailableColor();
					}
				});
			}
		}
		#endregion

		#region Constructors
		internal CharacterSetManager(ColorScheme colorScheme)
		{
			SuspendChildCollectionChangeNotification = true;
			ColorScheme = colorScheme;
		}
		#endregion

		#region Protected Methods
		protected override void OnItemAdded(CharacterSet item)
		{
			base.OnItemAdded(item);
			item.MainColor = ColorScheme.GetNextAvailableColor();
		}

		protected override void OnItemRemoved(CharacterSet item)
		{
			base.OnItemRemoved(item);
			ColorScheme.ReturnColor(item.MainColor);
		}
		#endregion
	}
}