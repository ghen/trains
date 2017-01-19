using System;
using System.Linq;
using System.Collections.Generic;

using Trains.Routing;

namespace Trains.SearchCriteria {

  #region [DistCriteria class definition]

  /// <summary>
  /// Applies limits to the total route distance.
  /// </summary>
  public sealed class DistCriteria : SearchCriteria {

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="DistCriteria"/> instance.
    /// </summary>
    /// <param name="operator">Comparison operator.</param>
    /// <param name="value">Target value.</param>
    private DistCriteria(String @operator, String value) :
      base("dist", @operator, value) { }

    #endregion Constructor and Initialization

    #region Eval

    /// <summary>
    /// Evaluates path eligibility for further search.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets criteria and could be used for further search. <value>False</value> otherwise.</returns>
    override public Boolean Eval(Int32 dest, Stack<Int32> path, Int32 dist) {
      if (path == null) return false;

      var res = true;
      if (this.MaxVal.HasValue) res &= (dist <= this.MaxVal);

      return res;
    }

    #endregion Eval

    #region Test

    /// <summary>
    /// Tests if path meets search criteria.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets search criteria. <value>False</value> otherwise.</returns>
    override public Boolean Test(Int32 dest, Stack<Int32> path, Int32 dist) {
      if (path == null) return false;

      var res = true;
      if (this.MinVal.HasValue) res &= (dist >= this.MinVal);
      if (this.MaxVal.HasValue) res &= (dist <= this.MaxVal);

      return res;
    }

    #endregion Test

  }

  #endregion [DistCriteria class definition]

}
