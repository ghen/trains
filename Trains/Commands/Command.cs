using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using Train.Utils;

namespace Trains.Commands {

  #region [Command class definition]

  /// <summary>
  /// Implements common functionality for all commands.
  /// </summary>
  public abstract class Command : ICommand {

    #region Constants

    /// <summary>
    /// Matches command and its parameters.
    /// </summary>
    private static readonly Regex CommandRegEx = new Regex(@"^\s*(?<cmd>\S+)(\s+(?<path>\S+)(\s+[[](?<limit>[^]]+)])?)?\s*$", 
      RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Stores commands mapping.
    /// </summary>
    /// <remarks>
    /// TODO: If we are to support dynamic commands set in future, we could use one of the Dependency Injection patterns here.
    /// </remarks>
    private static readonly IReadOnlyDictionary<String, Type> CommandsMap = new ReadOnlyDictionary<String, Type>(new Dictionary<String, Type>() {
      { "dist", typeof(DistCommand) },
      { "routes", typeof(RoutesCommand) },
      { "stops", typeof(StopsCommand) },
      { "help", typeof(HelpCommand) },
      { "exit", typeof(ExitCommand) }
    });

    #endregion Constants

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="Command"/> instance.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Throws if <paramref name="name"/> is <value>null</value> or empty.
    /// </exception>
    /// <param name="name">Internal command name.</param>
    /// <param name="path">Path to follow/calculate.</param>
    /// <param name="limit">Set limits, if any.</param>
    protected Command(String name, String path, String limit) {
      if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");

      this.Name = name;
      this.Path = path;
      this.Limit = limit;
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Internal command name.
    /// </summary>
    public String Name { get; protected set; }

    /// <summary>
    /// Path to follow/calculate.
    /// </summary>
    public String Path { get; protected set; }

    /// <summary>
    /// Set limits, if any.
    /// </summary>
    public String Limit { get; protected set; }

    #endregion Properties

    #region Events

    /// <summary>
    /// This event is used to trace command execution.
    /// </summary>
    public event EventHandler<String> Trace;

    /// <summary>
    /// Triggers <see cref="Trace"/> event.
    /// </summary>
    /// <param name="msg">Message to be sent to the listeners.</param>
    protected void OnTrace(String msg) {
      if (this.Trace != null)
        try {
          this.Trace(this, msg);
        } catch (Exception) {
          // Do noting
        }
    }

    #endregion Events

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>Command execution result.</returns>
    public abstract Object Execute(Int16[,] routes);

    #endregion Execute

    #region ToString

    /// <summary>
    /// Converts current object to its string representation.
    /// </summary>
    public override String ToString() {
      var res = new System.Text.StringBuilder();
      res.AppendFormat("{0}", this.Name);

      if (!String.IsNullOrEmpty(this.Path)) {
        res.AppendFormat(" \"{0}\"", this.Path);

        if (!String.IsNullOrEmpty(this.Limit)) {
          res.AppendFormat(" [{0}]", this.Limit);
        }
      }

      return res.ToString();
    }

    #endregion ToString

    #region Parse

    /// <summary>
    /// Parses input string and creates a new instance of command.
    /// </summary>
    /// <remarks>
    /// <seealso cref="HelpCommand.Execute"/> for command format details.
    /// </remarks>
    /// <param name="str">String representation of command to parse.</param>
    /// <returns>Newly created command instance.</returns>
    public static ICommand Parse(String str) {

      var cmdPattern = Command.CommandRegEx;
      var cmdMap = Command.CommandsMap;

      var match = cmdPattern.Match(str);
      if (!match.Success)
        throw new ArgumentException("Command format is not supported.");

      var cmd = match.Groups["cmd"].Value;
      var path = (String)match.Groups["path"].Value;
      var limit = (String)match.Groups["limit"].Value;

      var cmdType = (Type)null;
      // TODO: If we are to support dynamic commands set in future, we could use one of the Dependency Injection patterns here.
      cmdMap.TryGetValue(cmd.ToLower(), out cmdType);
      if (cmdType == null) 
        throw new NotSupportedException(String.Format("Command is not supported: '{0}'.", cmd));

      var res = ReflectionUtils.CreateInstance<ICommand>(cmdType.FullName, new Object[] { path, limit });
      return res;
    }

    #endregion Parse
  }

  #endregion [Command class definition]
}