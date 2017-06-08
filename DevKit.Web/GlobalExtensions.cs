using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;

public static class GlobalExtensions
{
	public static Type mBool = typeof(bool);
	public static Type mDate = typeof(DateTime);
	public static Type mTimeSpan = typeof(TimeSpan);
	public static Type mNullable = typeof(Nullable<>);
	
	public static T ChangeType<T>(this object @this, T defaultValue = default(T))
	{
		try
		{
			if (@this == null)
				return defaultValue;

			if (@this is T)
				return (T)@this;

			if (typeof(T).IsEnum)
				return (T)Enum.Parse(typeof(T), @this.ToString(), true);

			Type conversionType = typeof(T);

			// Nullables
			if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(mNullable))
			{
				if (@this == null)
					return defaultValue;

				conversionType = (new NullableConverter(conversionType)).UnderlyingType;
			}

			// Bool
			if (conversionType.Equals(mBool))
				return (T)((object)Convert.ToBoolean(@this));

			// Datetime
			if (conversionType.Equals(mDate))
				return (T)((object)DateTime.Parse(@this.ToString(), CultureInfo.CurrentCulture));

			// TimeSpan
			if (conversionType.Equals(mTimeSpan))
				return (T)((object)TimeSpan.Parse(@this.ToString(), CultureInfo.CurrentCulture));

			return (T)Convert.ChangeType(@this, conversionType);
		}
		catch
		{
			return defaultValue;
		}
	}
	
	public static string GetQueryStringValue(this HttpRequestMessage @this, string key)
	{
		var items = @this.GetQueryNameValuePairs();
		if (items == null)
			return null;

		var item = items.FirstOrDefault(n => n.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase));
		if (string.IsNullOrEmpty(item.Value))
			return null;

		return item.Value;
	}

	public static T GetQueryStringValue<T>(this HttpRequestMessage @this, string key, T defaultValue = default(T))
	{
		string value = @this.GetQueryStringValue(key);

		var result = value.ChangeType(defaultValue);

		if (result == null || result.Equals(default(T)))
			return defaultValue;

		return result;
	}
}
