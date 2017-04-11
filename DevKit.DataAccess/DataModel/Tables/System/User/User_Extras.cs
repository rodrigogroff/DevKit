using System;
using System.Threading;

namespace DataModel
{
	public partial class User
	{
		Random random = new Random();

		int RandomNumber(int min, int max)
		{
			Thread.Sleep(1);
			return random.Next(min, max);
		}

		public string GetRandomString(int size)
		{
			var ret = "";

			for(int t=0;t < size;++t)
				ret += RandomNumber(0, 9).ToString();
			
			return ret;
		}
		
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
