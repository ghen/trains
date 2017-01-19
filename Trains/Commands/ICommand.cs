using System;

namespace Trains.Commands {

  #region [ICommand interface definition]

  /// <summary>
  /// Defines Command entity interface.
  /// </summary>
  public interface ICommand {

    /// <summary>
    /// Internal command name.
    /// </summary>
    String Name { get; }

    /// <summary>
    /// Path to follow/calculate.
    /// </summary>
    String Path { get; }

    /// <summary>
    /// Set limits, if any.
    /// </summary>
    String Limit { get; }

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>Command execution result.</returns>
    Object Execute(Int16[,] routes);

    /// <summary>
    /// This event is used to trace command execution.
    /// </summary>
    event EventHandler<String> Trace;
  }

  #endregion [ICommand interface definition]

}
