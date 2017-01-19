using System;
using System.Reflection;

namespace Trains {

  #region [AppConfiguration class definition]

  /// <summary>
  /// Application configuration utility helpers.
  /// </summary>
  public static class AppConfiguration {

    #region GetVersionInfo

    /// <summary>
    /// Returns full application version information as a string.
    /// </summary>
    /// <param name="full">Set to <value>true</value> to include assembly title into the version string.</param>
    public static String GetVersionInfo(Boolean full = true) {
      Assembly assembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly() ?? Assembly.GetAssembly(typeof(AppConfiguration));
      String version = assembly.GetName().Version.ToString(3);
      String configuration = String.Empty;

#if DEBUG
            version = assembly.GetName().Version.ToString();
            configuration = String.Format("({0})",
                ((AssemblyConfigurationAttribute)assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), true)[0]).Configuration);
#endif

      var res = String.Format("v.{0} {1}", version, configuration);
      if (full) {
        String title = ((AssemblyTitleAttribute)assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), true)[0]).Title;
        res = String.Format("{0} {1}", title, res);
      }

      return res;
    }

    #endregion GetVersionInfo
  }

  #endregion [AppConfiguration class definition]
}