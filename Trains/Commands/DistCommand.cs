using System;
using System.Linq;
using System.Collections.Generic;

using Trains.Routing;

namespace Trains.Commands {

  #region [DistCommand class definition]

  /// <summary>
  /// Calculates shortest distance for selected path.
  /// </summary>
  public sealed class DistCommand : Command {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="DistCommand"/> instance.
    /// </summary>
    /// <param name="path">Path to follow/calculate.</param>
    /// <param name="limit">Set limits, if any.</param>
    public DistCommand(String path, String limit) :
      base("dist", path, limit) { }

    #endregion Constructor and Initialization

    #region Execute

    /// <summary>
    /// Executes command.
    /// </summary>
    /// <returns>Shortest possible distance for selected path, or <value>null</value> if travel is not possible.</returns>
    override public Object Execute(Int16[,] routes) {
      this.OnTrace(this.ToString());

      var trips = Router.Traverse(routes, this.Path, this.Limit);
      trips = trips.Where(t => t.Item1.Count > 1);

      var trip = trips.OrderBy(t => (Int32)t.Item2).FirstOrDefault();
      if (trip != null) {
        var path = trip.Item1;
        var dist = trip.Item2;
        this.OnTrace(
          String.Format("{0} (dist: {1}; stops: {2})",
            String.Join<Char>("-", path.Select(i => (Char)(i + 'A')).ToArray().Reverse()),
            dist, path.Count - 1));
      }
      
      return (trip != null) ? (Int32?)trip.Item2 : null;
    }

    #endregion Execute

  }

  #endregion [DistCommand class definition]

}
