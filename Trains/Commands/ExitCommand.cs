using System;

namespace Trains.Commands {

  #region [ExitCommand class definition]

  /// <summary>
  /// Terminates commands sequence.
  /// </summary>
  public sealed class ExitCommand : Command {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="ExitCommand"/> instance.
    /// </summary>
    public ExitCommand() : 
      this(null, null) { }

    /// <summary>
    /// Constructs new <see cref="ExitCommand"/> instance.
    /// </summary>
    /// <param name="path">Ignored for this command.</param>
    /// <param name="limit">Ignored for this command.</param>
    private ExitCommand(String path, String limit) :
      base("exit", path, limit) { }

    #endregion Constructor and Initialization

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <exception cref="OperationCanceledException">
    /// Always thrown whenever <see cref="ExitCommand.Execute"/> is called.
    /// </exception>
    /// <returns><see cref="ExitCommand.Execute"/> always throws <seealso cref="OperationCanceledException"/>.</returns>
    override public Object Execute(Int16[,] routes) {
      throw new OperationCanceledException(this.Name);
    }

    #endregion Execute

  }

  #endregion [ExitCommand class definition]

}
