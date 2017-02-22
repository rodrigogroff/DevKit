using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

public static class RegularExpressions
{
	public const string SpecialCharsPattern = @"[^a-zA-Z0-9]";

	public static readonly Regex SpecialChars = new Regex(SpecialCharsPattern, RegexOptions.Compiled);

	public const string MultiWhiteSpacesPattern = @"\s+";

	public static readonly Regex MultiWhiteSpaces = new Regex(MultiWhiteSpacesPattern, RegexOptions.Compiled);
}

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

	/// <summary>
	/// Ensures that null strings will become <see cref="string.Empty"/>
	/// </summary>
	/// <param name="this">The string being extended.</param>
	/// <returns>The extended string or <see cref="string.Empty"/>.</returns>
	public static string EnsureNotNull(this string @this)
	{
		return @this ?? string.Empty;
	}

	/// <summary>
	/// Compare strings ignoring culture and casing.
	/// </summary>
	/// <param name="this">The string being extended.</param>
	/// <param name="compareTo">The string to compare to.</param>
	/// <param name="compareNull">Flags whether to compar with nulls.</param>
	/// <returns></returns>
	public static bool Like(this string @this, string compareTo, bool compareNull = false)
	{
		if (@this == null || compareTo == null)
		{
			if (compareNull)
				return @this == compareTo;

			return false;
		}

		return @this.Trim().Equals(compareTo.Trim(), StringComparison.InvariantCultureIgnoreCase);
	}

	/// <summary>
	/// Return the input string as a normalized ASCII string.
	/// </summary>
	/// <param name="this">The string to be normalized.</param>
	/// <returns>Input string without accentuation.</returns>
	public static string NormalizeAccentuation(this string @this)
	{
		if (string.IsNullOrWhiteSpace(@this))
			return string.Empty;

		var stringBuilder = new StringBuilder();
		var normalizedString = @this.Normalize(NormalizationForm.FormD);

		foreach (var c in normalizedString)
		{
			var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);

			if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				stringBuilder.Append(c);
		}

		return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
	}

	/// <summary>
	/// Replaces any non alphanumeric chars by the specified on replacer.
	/// </summary>
	/// <param name="this">Thi string being extended.</param>
	/// <param name="replacer">The replacer <see cref="string"/>.</param>
	/// <returns>The input string with all non alphanumeric chars replaced.</returns>
	public static string FilterSpecialChars(this string @this, string replacer)
	{
		if (string.IsNullOrWhiteSpace(@this))
			return string.Empty;

		if (string.IsNullOrEmpty(replacer))
			replacer = string.Empty;

		return RegularExpressions.SpecialChars.Replace(@this, replacer);
	}

	/// <summary>
	/// Converts the input string to a normalized, non accented URL/SEO friendly string.
	/// </summary>
	/// <param name="this">The string to be converted.</param>
	/// <param name="wordSepparator">The words sepparator.</param>
	/// <returns></returns>
	public static string ToSlug(this string @this, string wordSepparator = "-")
	{
		if (string.IsNullOrWhiteSpace(@this))
			return @this;

		var normalized = @this.NormalizeAccentuation().ToLower();
		var filtered = normalized.FilterSpecialChars(replacer: " ");

		var slug = RegularExpressions.MultiWhiteSpaces.Replace(filtered, wordSepparator);

		return slug.Trim(wordSepparator.ToCharArray());
	}

	/// <summary>
	/// Truncates the input string on the specified <paramref name="length"/>.
	/// </summary>
	/// <param name="this">The string to be truncated.</param>
	/// <param name="length">The max length befor truncating.</param>
	/// <param name="suffix">The suffix to add when truncation occurs.</param>
	/// <returns>The truncated string or the original string in case its smaller than the specified length.</returns>
	public static string Truncate(this string @this, int length, string suffix = null)
	{
		if (string.IsNullOrEmpty(@this))
			return string.Empty;

		if (string.IsNullOrWhiteSpace(suffix))
			suffix = string.Empty;

		int inputLen = @this.Length;

		if (inputLen <= length)
			return @this;

		int breakPosition = @this.IndexOf("\n");

		if (breakPosition < 0)
			breakPosition = @this.IndexOf(".");

		if (breakPosition < 0 || breakPosition > length)
			breakPosition = length;

		int suffixSize = suffix.Length;

		if (breakPosition < 0)
			return string.Concat(@this.Substring(0, inputLen - suffixSize), suffix);

		if (breakPosition > @this.Length)
			breakPosition = @this.Length;

		breakPosition = breakPosition - suffixSize;

		return string.Concat(@this.Substring(0, breakPosition).Trim(), suffix);
	}

	/// <summary>
	/// Indicates whether a specified string is null, empty, or consists only of white-space characters.
	/// </summary>
	/// <param name="this">The string to check.</param>
	/// <returns></returns>
	public static bool IsNullOrWhiteSpace(this string @this)
	{
		return string.IsNullOrWhiteSpace(@this);
	}

	/// <summary>
	/// Returns a placeholder text when the string is null, empty, or consists only of white-space characters.
	/// </summary>
	/// <param name="this">The string to check.</param>
	/// <param name="placeholderText">The placeholder text to return.</param>
	/// <returns></returns>
	public static string IsNullOrWhiteSpaceThen(this string @this, string placeholderText)
	{
		if (@this.IsNullOrWhiteSpace())
			return placeholderText.EnsureNotNull();

		return @this;
	}

	/// <summary>
	/// Indicates whether the specified string is null or an <see cref="string.Empty"/> string.
	/// </summary>
	/// <param name="this">The string to check.</param>
	/// <returns></returns>
	public static bool IsNullOrEmpty(this string @this)
	{
		return string.IsNullOrEmpty(@this);
	}

	/// <summary>
	/// Returns a placeholder text when the string is null or an <see cref="string.Empty"/> string.
	/// </summary>
	/// <param name="this">The string to check.</param>
	/// <param name="placeholderText">The placeholder text to return.</param>
	/// <returns></returns>
	public static string IsNullOrEmptyThen(this string @this, string placeholderText)
	{
		if (@this.IsNullOrEmpty())
			return placeholderText.EnsureNotNull();

		return @this;
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
