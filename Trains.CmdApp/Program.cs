using System;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

using Trains.Routing;
using Trains.Commands;

namespace Trains.CmdApp {

  #region [Program class definition]

  /// <summary>
  /// Console application.
  /// </summary>
  internal static class Program {

    #region Main(..)

    /// <summary>
    /// Application entry point.
    /// </summary>
    internal static int Main(String[] args) {

      //
      // Default application configuration
      //
      var conf = new AppConf() {
        RoutesFilePath = ConfigurationManager.AppSettings["RoutesFile"],
        InputFilePath = ConfigurationManager.AppSettings["InputFile"],
        OutputFilePath = ConfigurationManager.AppSettings["OtputFile"],

        VerboseOutput = String.Equals("true", ConfigurationManager.AppSettings["Verbose"], StringComparison.CurrentCultureIgnoreCase),
        PrintUsageHelp = false
      };

      //
      // Parse command line arguments
      //
      try {
        conf.Apply(args);

        //
        // Print usage help (if requested)
        //
        if (conf.PrintUsageHelp) {
          Console.WriteLine("{0}", AppConfiguration.GetVersionInfo());
          AppConfExtensions.PrintUsage(Console.Out, null);
          return 0;
        }

        //
        // Configuration validation (quick sanity check)
        //
        if (String.IsNullOrEmpty(conf.RoutesFilePath) || !File.Exists(conf.RoutesFilePath))
          throw new ConfigurationErrorsException(String.Format(
            "Invalid routes data file: '{0}'.", conf.RoutesFilePath));

      } catch (Exception ex) {
        Console.WriteLine("[ERROR]: {0}", ex.Message);
        AppConfExtensions.PrintUsage(Console.Out, null);
        return 1;
      }
      
      //
      // Run application
      //
      var input = Console.In;
      var output = Console.Out;
      try {

        //
        // Setup input and output
        //
        if (!String.IsNullOrEmpty(conf.InputFilePath))
          input = new StreamReader(File.OpenRead(conf.InputFilePath));
        if (!String.IsNullOrEmpty(conf.OutputFilePath))
          output = new StreamWriter(File.OpenWrite(conf.OutputFilePath));

        //
        // Load routes
        //
        var routes = Task.Run(() => Router.LoadRoutesAsync(conf.RoutesFilePath)).Result;
        if (conf.VerboseOutput) {
          Console.WriteLine();
          Console.WriteLine("#");
          Console.WriteLine("# Routes table");
          Console.WriteLine("#");

          var list = new List<String>();
          for (var start = 0; start < routes.GetLength(0); start++) {
            list.Clear();
            for (var dest = 0; dest < routes.GetLength(1); dest++) {
              var dist = routes[start, dest];
              if (dist < 0) continue;
              list.Add(String.Format("{0}{1}{2}", (Char)('A' + start), (Char)('A' + dest), dist));
            }

            if (list.Count <= 0) continue;
            Console.WriteLine("# {0} ", String.Join<String>(", ", list));
          }
          Console.WriteLine();
        }

        //
        // Process queries
        //
        if (input == Console.In) {
          Console.WriteLine();
          Console.WriteLine("#");
          Console.WriteLine("# Enter your search command:");
          Console.WriteLine("#  * (type 'help' for help)");
          Console.WriteLine("#");
          Console.WriteLine();
        }

        var lineNo = 0;
        var line = (String)null;
        while ((line = input.ReadLine()) != null) {
          lineNo++;

          //
          // Remove comments
          //
          var commentCharIdx = line.IndexOf('#');
          if (commentCharIdx >= 0) line = line.Remove(commentCharIdx);
          if (String.IsNullOrWhiteSpace(line)) continue;

          //
          // Parse and Execute the command
          //
          try {
            var cmd = Command.Parse(line);
            if (conf.VerboseOutput || (cmd is HelpCommand)) {
              cmd.Trace += (sender, msg) => { output.WriteLine("# {0}", msg); };
            }

            var res = cmd.Execute(routes);
            output.WriteLine("{0}", res ?? "NO SUCH ROUTE");

          } catch (OperationCanceledException) {
            if (conf.VerboseOutput) {
              output.WriteLine("# EXIT");
            }
            break;
          } catch (Exception ex) {
            if (conf.VerboseOutput) {
              output.WriteLine("# line {0,5}: {1}", lineNo, line);
              output.WriteLine("#      ERROR: {0}", ex.Message);
            }
            continue;
          } finally {
            if (conf.VerboseOutput) {
              output.WriteLine();
            }
          }
        }

      } catch (Exception ex) {

        //
        // Error
        //
        Console.WriteLine();
        Console.WriteLine("# [ERROR]: {0}", ex.Message);
        while ((ex = ex.InnerException) != null)
          Console.WriteLine("# - {0}", ex.Message);
        Console.WriteLine();
        return 2;

      } finally {

        //
        // Cleanup
        //
        if (input != null && input != Console.In) input.Close();
        if (output != null && output != Console.Out) output.Close();
        input = null;
        output = null;
      }

      return 0;
    }

    #endregion Main(..)
  }

  #endregion [Program class definition]
}
