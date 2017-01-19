using System;
using System.Linq;

using Trains.Routing;

namespace Trains.Commands {

  #region [RoutesCommand class definition]

  /// <summary>
  /// Calculates number of possible routes for selected path.
  /// </summary>
  public sealed class RoutesCommand : Command {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="RoutesCommand"/> instance.
    /// </summary>
    /// <param name="path">Path to follow/calculate.</param>
    /// <param name="limit">Set limits, if any.</param>
    public RoutesCommand(String path, String limit) :
      base("routes", path, limit) { }

    #endregion Constructor and Initialization

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>Number of possible routes for selected path, or <value>null</value> if travel is not possible.</returns>
    override public Object Execute(Int16[,] routes) {
      this.OnTrace(this.ToString());

      var trips = Router.Traverse(routes, this.Path, this.Limit);
      trips = trips.Where(t => t.Item1.Count > 1);

      foreach (var trip in trips.OrderBy(t => t.Item2).ThenBy(t => t.Item1.Count)) {
        var path = trip.Item1;
        var dist = trip.Item2;
        this.OnTrace(
          String.Format("{0} (dist: {1}; stops: {2})",
            String.Join<Char>("-", path.Select(i => (Char)(i + 'A')).ToArray().Reverse()),
            dist, path.Count - 1));
      }

      var res = trips.Count();
      return res > 0 ? (Int32?)res : null;
    }

    #endregion Execute

  }

  #endregion [RoutesCommand class definition]

}
