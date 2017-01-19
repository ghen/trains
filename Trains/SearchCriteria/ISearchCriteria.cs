using System;
using System.Collections.Generic;

namespace Trains.SearchCriteria {

  #region [ISearchCriteria interface definition]

  /// <summary>
  /// Defines Search Criteria entity interface.
  /// </summary>
  public interface ISearchCriteria {

    /// <summary>
    /// Evaluates path eligibility for further search.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets criteria and could be used for further search. <value>False</value> otherwise.</returns>
    Boolean Eval(Int32 dest, Stack<Int32> path, Int32 dist);

    /// <summary>
    /// Tests if path meets search criteria.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets search criteria. <value>False</value> otherwise.</returns>
    Boolean Test(Int32 dest, Stack<Int32> path, Int32 dist);

  }

  #endregion [ISearchCriteria interface definition]

}
