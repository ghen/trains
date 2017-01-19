using System;

namespace Trains.Commands {

  #region [HelpCommand class definition]

  /// <summary>
  /// Outputs supported commands details.
  /// </summary>
  public sealed class HelpCommand : Command {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="HelpCommand"/> instance.
    /// </summary>
    public HelpCommand() : 
      this(null, null) { }

    /// <summary>
    /// Constructs new <see cref="HelpCommand"/> instance.
    /// </summary>
    /// <param name="path">Ignored for this command.</param>
    /// <param name="limit">Ignored for this command.</param>
    private HelpCommand(String path, String limit) :
      base("help", path, limit) { }

    #endregion Constructor and Initialization

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>This method always returns <seealso cref="String.Empty"/>.</returns>
    override public Object Execute(Int16[,] routes) {

      this.OnTrace("Commands format:");
      this.OnTrace("");
      this.OnTrace("  <name> <path> [<limit>]");
      this.OnTrace("");
      this.OnTrace("Where:");
      this.OnTrace("");
      this.OnTrace("  name    - Name of the command (case insensitive).");
      this.OnTrace("");
      this.OnTrace("            Supported commands: ");
      this.OnTrace("");

      // TODO: Generate list of supported commands and their description dynamically when switched to DI
      this.OnTrace("              'dist'    - Calculates shortest distance");
      this.OnTrace("              'routes'  - Calculates number of possible routes");
      this.OnTrace("              'stops'   - Calculates minimal number of stops");
      this.OnTrace("              'help'    - Outputs list of supported commands");
      this.OnTrace("              'exit'    - Terminates further processing");

      this.OnTrace("");
      this.OnTrace("  path    - Graph path to travel/calculate.");
      this.OnTrace("");
      this.OnTrace("            Route is described as following: ");
      this.OnTrace("              'A-B'     - Direct travel between A and B");
      this.OnTrace("              'A..B'    - Any possible route between A and B");
      this.OnTrace("");
      this.OnTrace("  limit   - (optional) Limits to apply during search/calculation.");
      this.OnTrace("");
      this.OnTrace("            Limits can apply to 'dist' or 'stops', and are described");
      this.OnTrace("            in form of boolean condition (C# language syntax).");
      this.OnTrace("            For example: 'stops == 5', or 'dist &lt;= 20'.");
      this.OnTrace("");
      this.OnTrace("Notes:");
      this.OnTrace("          - Empty lines are ignored");
      this.OnTrace("          - Comments start with '#' character");
      this.OnTrace("");
      this.OnTrace("Commands examples:");
      this.OnTrace("");
      this.OnTrace("  dist A-B-C");
      this.OnTrace("  dist A..C");
      this.OnTrace("  routes C..C [stops &lt;= 3]");
      this.OnTrace("  stops A-B..E");
      this.OnTrace("  exit");
      this.OnTrace("");

      return String.Empty;
    }

    #endregion Execute

  }

  #endregion [HelpCommand class definition]

}
