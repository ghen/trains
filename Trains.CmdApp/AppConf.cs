using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Trains.CmdApp {

  #region [AppConf class definition]

  /// <summary>
  /// Stores active application configuration.
  /// </summary>
  internal sealed class AppConf {

    public String RoutesFilePath { get; set; }
    public String InputFilePath { get; set; }
    public String OutputFilePath { get; set; }

    public Boolean VerboseOutput { get; set; }
    public Boolean PrintUsageHelp { get; set; }
  }

  #endregion [AppConf class definition]

  #region [AppConfExtensions class definition]

  /// <summary>
  /// Application configuration helpers.
  /// </summary>
  internal static class AppConfExtensions {

    #region Constants

    /// <summary>
    /// Matches command line parameters and their values (optional).
    /// </summary>
    private static readonly Regex CommandLineSwitchRegEx = new Regex(@"^[-/](?<switch>[0-9a-zA-Z?])([=:](?<value>\S+))?$", 
      RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion Constants

    #region Apply

    /// <summary>
    /// Loads (overrides) application settings from command line arguments.
    /// </summary>
    /// <exception cref="ConfigurationErrorsException">
    /// Throws if application configuration error detected.
    /// </exception>
    /// <param name="conf">Application configuration instance to apply changes to.</param>
    /// <param name="args">Command line arguments.</param>
    /// <returns>
    /// <value>true</value> if application configuration has been parsed successfully. 
    /// <value>false</value> if "help" was requested.
    /// </returns>
    public static AppConf Apply(this AppConf conf, String[] args) {

      foreach (var arg in args) {

        var match = AppConfExtensions.CommandLineSwitchRegEx.Match(arg);
        if (!match.Success) {
          conf.RoutesFilePath = arg;
          continue;
        }

        var param = match.Groups["switch"].Value.ToCharArray()[0];
        switch (param) {

          case 'i': {
              var val = (match.Groups["value"] != null) ? match.Groups["value"].Value : null;

              // NOTE:
              //   We deliberately allow "empty" value for this parameter
              //   to allow user to override app.config settings and 
              //   to use interactive input instead.
              //
              // if (String.IsNullOrEmpty(val))
              //   throw new ConfigurationErrorsException(String.Format("Invalid file selected: '{0}'.", val));

              conf.InputFilePath = val;
            }
            break;

          case 'o': {
              var val = (match.Groups["value"] != null) ? match.Groups["value"].Value : null;

              // NOTE:
              //   We deliberately allow "empty" value for this parameter
              //   to allow user to override app.config settings and 
              //   to use interactive input instead.
              //
              // if (String.IsNullOrEmpty(val))
              //   throw new ConfigurationErrorsException(String.Format("Invalid file selected: '{0}'.", val));

              conf.OutputFilePath = val;
            }
            break;

          case 'v':
            conf.VerboseOutput = true;
            break;

          case 'h':
          case '?':
            conf.PrintUsageHelp = true;
            break;

          default:
            // Ignore unknown parameters

            // IMPORTANT:
            //   Do not throw any errors for future versions compatibility.
            break;
        }
      }

      return conf;
    }

    #endregion Apply

    #region PrintUsage

    /// <summary>
    /// Prints application usage help.
    /// </summary>
    /// <param name="output">Output to write data to.</param>
    /// <param name="appName">Application executable file name.</param>
    public static void PrintUsage(TextWriter output, String appName) {

      if (appName == null)
        appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

      output.WriteLine();
      output.WriteLine("Usage:");
      output.WriteLine("    {0} <routes> [/i:<input>] [/o:<output>] [/v]", appName);
      output.WriteLine();
      output.WriteLine("Arguments:");
      output.WriteLine("        <routes>   Path to the routes data file");
      output.WriteLine("    /i  <input>    (optional) Path to the input data file");
      output.WriteLine("    /o  <output>   (optional) Path to the output data file");
      output.WriteLine();
      output.WriteLine("Options:");
      output.WriteLine("    /v             Enables verbose mode");
      output.WriteLine("    /h, /?         Prints usage information");
      output.WriteLine();
    }

    #endregion PrintUsage

  }

  #endregion [AppConfExtensions class definition]

}
