using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Trains.SearchCriteria;

namespace Trains.Routing {

  #region [Router class definition]

  /// <summary>
  /// Implements routes graph traversal routines.
  /// </summary>
  public static class Router {

    #region [RouteEvaluationResult enumeration definition]

    /// <summary>
    /// Defines route evaluation result.
    /// </summary>
    [Flags]
    public enum RouteEvaluationResult {

      /// <summary>
      /// Reject current route and stop further processing.
      /// </summary>
      Reject = 0,

      /// <summary>
      /// Continue search.
      /// </summary>
      Continue = 1,

      /// <summary>
      /// Accept current route.
      /// </summary>
      Accept = 2,

      /// <summary>
      /// Accept current route and continue search.
      /// </summary>
      AcceptAndContinue = Accept | Continue

    }

    #endregion [RouteEvaluationResult enumeration definition]

    #region Constants

    /// <summary>
    /// Matches single route entry definition.
    /// </summary>
    private static readonly Regex RoteEntryRegEx = new Regex(@"^\W*(?<start>[A-Z])(?<end>[A-Z])(?<dist>[0-9]+)\W*$", 
      RegexOptions.Compiled);

    /// <summary>
    /// Matches trip path string.
    /// </summary>
    private static readonly Regex TripRouteRegEx = new Regex(@"^\s*(?<src>[A-Z])(?<trip>(?<path>-|\.\.)(?<dest>[A-Z]))+\s*$", 
      RegexOptions.Compiled);

    /// <summary>
    /// Route depth limit during graph traverse.
    /// </summary>
    private const UInt32 RouteDepthLimit = 200;

    #endregion Constants
    
    #region Traverse

    /// <summary>
    /// Calculates possible graph routes based on the specified filter conditions.
    /// </summary>
    /// <param name="routes">Graph routes configuration.</param>
    /// <param name="path">Preferred route path configuration.</param>
    /// <param name="filter">(optional) Routes evaluation criteria.</param>
    /// <returns>List of possible routes that match specified filter conditions.</returns>
    public static IEnumerable<Tuple<Stack<Int32>, Int32>> Traverse(Int16[,] routes, String path, String filter) {
      if (routes == null) throw new ArgumentNullException("routes");
      if (path == null) throw new ArgumentNullException("path");

      //
      // Parse filter conditions
      //
      var limits = SearchCriteria.SearchCriteria.Parse(filter);

      //
      // Parse requested path string
      //
      var match = Router.TripRouteRegEx.Match(path);
      if (!match.Success) throw new ArgumentException("Invalid trip path format.", "path");

      //
      // Starting point
      //
      var dist = (Int32)0;
      var src = (Int32)(match.Groups["src"].Value[0] - 'A');
      var route = new Stack<Int32>();
      route.Push(src);

      //
      // Initial route
      //
      var res = new List<Tuple<Stack<Int32>, Int32>>();
      res.Add(new Tuple<Stack<Int32>, Int32>(new Stack<Int32>(route.ToArray().Reverse()), dist));

      //
      // Route evaluation routine
      //
      Func<Int32, Int32, Stack<Int32>, Int32, RouteEvaluationResult> eval = (_src, _dest, _path, _dist) => {
        var _curr = _path.Peek();
        var _stops = _path.Count - 1;

        // Do not allow loops unless there are other limits in place
        var _stop = (_path.Count(i => (i == _curr)) > (1 + (_curr == _src ? 1 : 0)));
        if (limits != null && limits.Count > 0) {
          _stop = false;
          foreach (var _limit in limits)
            _stop |= !(_limit.Eval(_dest, _path, _dist));
        }

        if (_stop)
          return RouteEvaluationResult.Reject;

        var _eval = (_curr == _dest);
        if (_eval)
          return RouteEvaluationResult.AcceptAndContinue;

        return RouteEvaluationResult.Continue;
      };

      //
      // Iterate through requested path routes
      //
      for (var i = 0; i < match.Groups["trip"].Captures.Count && res.Count > 0; i++) {
        var mode = match.Groups["path"].Captures[i].Value;
        var dest = (Int32)(match.Groups["dest"].Captures[i].Value[0] - 'A');

        //
        // For all active routes - calculate new trip possibilities
        //
        var options = new List<Tuple<Stack<Int32>, Int32>>();
        foreach (var option in res) {
          route = option.Item1;
          dist = option.Item2;

          switch (mode) {
            //
            // Direct trip to the destination
            //
            case "-": {
                var maxDepth = route.Count + 1;
                var newOptions = Router.Traverse(routes, src, dest, route, dist,
                  (_src, _dest, _path, _dist) => {
                    if (_path.Count > maxDepth) return RouteEvaluationResult.Reject;
                    return eval(_src, _dest, _path, _dist);
                  });
                options.AddRange(newOptions);
              }
              break;

            //
            // Use any possible route to the destination
            //
            case "..": {
                var newOptions = Router.Traverse(routes, src, dest, route, dist, eval);
                options.AddRange(newOptions);
              }
              break;

            default:
              throw new NotSupportedException(String.Format("Unexpected trip path element: '{0}'.", mode));
          }
        }

        //
        // Update active routes list
        //
        res.Clear();
        res.AddRange(options);
      }

      //
      // Verify routes if strict filter was set
      //
      res = res.Where(trip => {
        var fail = false;
        fail |= limits.Where(limit => (limit.Test(trip.Item1.Peek(), trip.Item1, trip.Item2) == false)).Any();
        return !fail;
      }).ToList();

      //
      // Done
      //
      return res;
    }

    /// <summary>
    /// Traverses graph (Depth-first search) based on provided evaluation criteria.
    /// </summary>
    /// <param name="routes">Graph routes configuration.</param>
    /// <param name="src">Starting node index.</param>
    /// <param name="dest">Destination node index.</param>
    /// <param name="eval">Route evaluation routine.</param>
    /// <returns>List of possible routes that match evaluation criteria.</returns>
    public static IEnumerable<Tuple<Stack<Int32>, Int32>> Traverse(Int16[,] routes, Int32 src, Int32 dest,
      Func<Int32, Int32, Stack<Int32>, Int32, RouteEvaluationResult> eval) {
      if (routes == null) throw new ArgumentNullException("routes");

      //
      // Starting point
      //
      Int32 dist = 0;
      var route = new Stack<Int32>();
      route.Push(src);

      //
      // Calculate possible routes
      //
      return Router.Traverse(routes, src, dest, route, dist, eval);
    }

    /// <summary>
    /// Traverses graph (Depth-first search) based on provided evaluation criteria.
    /// </summary>
    /// <param name="routes">Graph routes configuration.</param>
    /// <param name="src">Starting node index.</param>
    /// <param name="dest">Destination node index.</param>
    /// <param name="route">Route traversed so far.</param>
    /// <param name="dist">Calculated route distance.</param>
    /// <param name="eval">Route evaluation routine.</param>
    /// <param name="maxStops">Maximum route depth (infinite loop protection).</param>
    /// <returns>List of possible routes that match evaluation criteria.</returns>
    private static IList<Tuple<Stack<Int32>, Int32>> Traverse(Int16[,] routes, Int32 src, Int32 dest, Stack<Int32> route, Int32 dist,
      Func<Int32, Int32, Stack<Int32>, Int32, RouteEvaluationResult> eval, UInt32 maxStops = Router.RouteDepthLimit) {
      if (routes == null) throw new ArgumentNullException("routes");
      if (route == null) throw new ArgumentNullException("route");

      var res = new List<Tuple<Stack<Int32>, Int32>>();

      //
      // Prevent infinite loops (sanity check).
      //
      if (route.Count > maxStops) return res;

      //
      // Evaluate current route
      //
      var test = eval(src, dest, route, dist);
      if ((test & RouteEvaluationResult.Accept) == RouteEvaluationResult.Accept)
        res.Add(new Tuple<Stack<Int32>, Int32>(new Stack<Int32>(route.ToArray().Reverse()), dist));
      if ((test & RouteEvaluationResult.Continue) != RouteEvaluationResult.Continue)
        return res;

      //
      // Calculate all possible routes starting from current node
      //
      var curr = route.Peek();
      for (var j = 0; j < routes.GetLength(1); j++) {
        if (routes[curr, j] < 0) continue;              // Route is not possible

        route.Push(j);
        var options = Router.Traverse(routes, src, dest, route, dist + routes[curr, j], eval);
        res.AddRange(options);
        route.Pop();
      }

      return res;
    }

    #endregion Traverse

    #region LoadRoutes

    /// <summary>
    /// Loads routes graph from file.
    /// </summary>
    /// <exception cref="ConfigurationErrorsException">
    /// Throws if routes table contains invalid entries.
    /// </exception>
    /// <param name="file">Data file.</param>
    /// <returns>Routes graph.</returns>
    public static async Task<Int16[,]> LoadRoutesAsync(String file) {

      Int16[,] res;
      using (var src = new StreamReader(File.OpenRead(file))) {
        try {
          res = await Router.ParseRoutesAsync(src);
        } catch (ConfigurationErrorsException cEx) {
          throw new ConfigurationErrorsException(cEx.Message, file, cEx.Line);
        }
      }

      return res;
    }

    #endregion LoadRoutes

    #region ParseRoutes

    /// <summary>
    /// Parses routes graph from the data stream provided.
    /// </summary>
    /// <exception cref="ConfigurationErrorsException">
    /// Throws if routes table contains invalid entries.
    /// </exception>
    /// <param name="stream">Data stream.</param>
    /// <returns>Routes graph.</returns>
    public static async Task<Int16[,]> ParseRoutesAsync(TextReader stream) {
      if (stream == null) throw new ArgumentException("stream");

      var res = new Int16['Z' - 'A' + 1, 'Z' - 'A' + 1];
      for (var i = 0; i < res.GetLength(0); i++)
        for (var j = 0; j < res.GetLength(1); j++)
          res[i, j] = -1;

      var line = (String)null;
      var lineNo = 0;
      while ((line = await stream.ReadLineAsync()) != null) {
        lineNo++;

        var ignoreAfter = line.IndexOf('#');
        if (ignoreAfter >= 0) line = line.Substring(0, ignoreAfter);

        line = line.Trim();
        if (line.Length <= 0) continue;

        var entries = line.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var str in entries) {
          var routeRegEx = Router.RoteEntryRegEx;
          var mathch = routeRegEx.Match(str);
          if (!mathch.Success)
            throw new ConfigurationErrorsException(String.Format(
              "Invalid route entry: '{0}'.", str), String.Empty, lineNo);

          var start = mathch.Groups["start"].Value[0];
          var end = mathch.Groups["end"].Value[0];
          if (start == end) continue;

          var dist = Int16.Parse(mathch.Groups["dist"].Value);
          res[start - 'A', end - 'A'] = dist;
        }
      }

      return res;
    }

    #endregion ParseRoutes
  }

  #endregion [Router class definition]

}
