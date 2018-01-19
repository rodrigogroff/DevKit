using System;
using System.Threading;

namespace DataModel
{
	public partial class Setup
	{
		Random random = new Random();

		int RandomNumber(int min, int max)
		{
			Thread.Sleep(1);
			return random.Next(min, max);
		}

        public string RandomString(int totalChars)
        {
            var ret = "";

            for (int i = 0; i < totalChars; i++)
                ret += RandomNumber(0, 9).ToString();

            return ret;
        }

        public int RandomInt(int totalChars)
        {
            var ret = "";

            for (int i = 0; i < totalChars; i++)
                ret += RandomNumber(0, 9).ToString();

            return Convert.ToInt32(ret);
        }

        public string GetProtocol()
		{
			var ret = "";

			if (stProtocolFormat != null)
				foreach (var i in stProtocolFormat)
					if (Char.IsDigit(i))
						ret += RandomNumber(0, 9).ToString();
					else
						ret += i;

			return ret;
		}
	}
}
