using System;

namespace DataModel
{
	public partial class User
	{
		public string GetMaskedValue(DevKitDB db, string stPhone)
		{
			var pref = db.Setup().stPhoneMask;
			var mask = "(99) 9999999"; // default

			if (pref != null)
				mask = pref;

			bool foundMask = false;

			foreach (var i in stPhone)
				if (!Char.IsLetterOrDigit(i))
				{
					foundMask = true;
					break;
				}

			if (!foundMask)
			{
				var result = ""; var index = 0; var maxlen = stPhone.Length;

				foreach (var i in mask)
				{
					if (Char.IsLetterOrDigit(i))
					{
						if (index < maxlen)
							result += stPhone[index++];
					}
					else
						result += i;
				}

				return result;
			}
			else
				return stPhone;			
		}
	}
}
