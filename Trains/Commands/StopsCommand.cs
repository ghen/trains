using System;
using System.Linq;

using Trains.Routing;

namespace Trains.Commands {

  #region [StopsCommand class definition]

  /// <summary>
  /// Calculates minimal number of stops for selected path.
  /// </summary>
  public sealed class StopsCommand : Command {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="StopsCommand"/> instance.
    /// </summary>
    /// <param name="path">Path to follow/calculate.</param>
    /// <param name="limit">Set limits, if any.</param>
    public StopsCommand(String path, String limit) :
      base("stops", path, limit) { }

    #endregion Constructor and Initialization

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>Minimal number of stops for selected path, or <value>null</value> if travel is not possible.</returns>
    override public Object Execute(Int16[,] routes) {
      this.OnTrace(this.ToString());

      var trips = Router.Traverse(routes, this.Path, this.Limit);
      trips = trips.Where(t => t.Item1.Count > 1);

      var trip = trips.OrderBy(t => t.Item1.Count).FirstOrDefault();
      if (trip != null) {
        var path = trip.Item1;
        var dist = trip.Item2;
        this.OnTrace(
          String.Format("{0} (dist: {1}; stops: {2})",
            String.Join<Char>("-", path.Select(i => (Char)(i + 'A')).ToArray().Reverse()),
            dist, path.Count - 1));
      }

      return (trip != null) ? (Int32?)(trip.Item1.Count - 1) : null;
    }

    #endregion Execute

  }

  #endregion [StopsCommand class definition]

}
