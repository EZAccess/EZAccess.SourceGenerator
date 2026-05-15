using System;
using System.Globalization;
using System.Web;

namespace EZAccess.SourceGenerator.Abstractions.Extensions;

public static class DataHelperExtensions
{
	public static string EzUrlEncode(this string value)
	{
		return HttpUtility.UrlEncode(value);
	}

	public static string EzUrlEncode(this char value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}

	public static string EzUrlEncode(this double value)
	{
		return HttpUtility.UrlEncode(value.ToString(CultureInfo.InvariantCulture));
	}

	public static string EzUrlEncode(this float value)
	{
		return HttpUtility.UrlEncode(value.ToString(CultureInfo.InvariantCulture));
	}

	public static string EzUrlEncode(this byte value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}

	public static string EzUrlEncode(this short value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}

	public static string EzUrlEncode(this int value) 
	{
		return HttpUtility.UrlEncode(value.ToString());
	}

	public static string EzUrlEncode(this long value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}


	public static string EzUrlEncode(this decimal value)
	{
		return HttpUtility.UrlEncode(value.ToString(CultureInfo.InvariantCulture));
	}

	public static string EzUrlEncode(this bool value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}

	public static string EzUrlEncode(this DateTime value, string format = "s")
	{
		if (format == "s") {
			return value.ToString(format);
		}
		return HttpUtility.UrlEncode(value.ToString(format, CultureInfo.InvariantCulture));
	}

	public static string EzUrlEncode(this Guid value)
	{
		return HttpUtility.UrlEncode(value.ToString());
	}
}
