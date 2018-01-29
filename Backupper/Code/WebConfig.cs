using System;
using System.Configuration;

namespace Base.Configuration
{

  /// <summary>
  /// Funzioni per la gestone del web.config.
  /// </summary>
  public static class WebConfig
  {

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static string GetString(string key)
    {
      if (string.IsNullOrEmpty(key))
        throw new Exception("Specificare un valore.");

      return ConfigurationManager.AppSettings[key];
    }


    /// <summary>
    /// Gets the bool.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static bool GetBool(string key)
    {
      if (string.IsNullOrEmpty(key))
        throw new Exception("Specificare un valore.");

      return ((ConfigurationManager.AppSettings[key] != null && ConfigurationManager.AppSettings[key].Trim() == "1") ? true : false);
    }

    /// <summary>
    /// Gets the string.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static string GetString(string key, string defaultValue)
    {
      string v = GetString(key);
      if (v == null)
        return defaultValue;

      return v;
    }

    /// <summary>
    /// Gets the int32.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static int GetInt32(string key)
    {
      string v = GetString(key);
      if (v == null)
        return 0;

      return int.Parse(v);
    }


    /// <summary>
    /// Gets the int32.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns></returns>
    public static int GetInt32(string key, int defaultValue)
    {
      string v = GetString(key);
      if (v == null)
        return defaultValue;

      return int.Parse(v);
    }


    /// <summary>
    /// Gets the double.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public static double GetDouble(string key)
    {
      string v = GetString(key);
      if (v == null)
        return 0;

      return double.Parse(v);
    }

  }
}
