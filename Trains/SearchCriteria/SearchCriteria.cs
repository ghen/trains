using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using Train.Utils;

namespace Trains.SearchCriteria {

  #region [SearchCriteria class definition]

  /// <summary>
  /// Implements based functionality for all search criteria.
  /// </summary>
  public abstract class SearchCriteria : ISearchCriteria {

    #region Constants

    /// <summary>
    /// Matches search criteria.
    /// </summary>
    private static readonly Regex SearchCriteriaRegEx = new Regex(@"^\s*(?<term>\S+)\s+(?<comp>\S+)\s+(?<val>\S+)\s*$", 
      RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Stores search criteria mapping.
    /// </summary>
    /// <remarks>
    /// TODO: If we are to support dynamic search criteria set in future, we could use one of the Dependency Injection patterns here.
    /// </remarks>
    private static readonly IReadOnlyDictionary<String, Type> SearchCriteriaMap = new ReadOnlyDictionary<String, Type>(new Dictionary<String, Type>() {
      { "stops", typeof(StopsCriteria) },
      { "dist", typeof(DistCriteria) }
    });

    #endregion Constants

    #region Constructor and Initialization

    /// <summary>
    /// Constructs new <see cref="SearchCriteria"/> instance.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Throws if one of the input parameters is invalid.
    /// </exception>
    /// <exception cref="NotSupportedException">
    /// Throws if <paramref name="operator"/> is not supported for this <paramref name="term"/>.
    /// </exception>
    /// <param name="term">Term to evaluate.</param>
    /// <param name="operator">Comparison operator.</param>
    /// <param name="value">Target value.</param>
    protected SearchCriteria(String term, String @operator, String value) {
      if (String.IsNullOrEmpty(term)) throw new ArgumentException("name");
      if (String.IsNullOrEmpty(@operator)) throw new ArgumentException("operator");

      this.Term = term;
      this.Operator = @operator;
      this.Value = value;

      // TODO: Refactor this code into MinMaxsSearchCriteria base class in future
      var intVal = Int32.Parse(value);
      switch (@operator) {
        
        case "==":
          this.MinVal =
            this.MaxVal = intVal;
          break;

        case "<":
          this.MaxVal = intVal - 1;
          break;

        case "<=":
          this.MaxVal = intVal;
          break;

        default:
          throw new NotSupportedException(String.Format(
            "Selected operator '{0}' is not supported for term '{1}'.", @operator, term));
      }
    }

    #endregion Constructor and Initialization

    #region Properties

    /// <summary>
    /// Term to evaluate.
    /// </summary>
    public String Term { get; protected set; }

    /// <summary>
    /// Comparison operator.
    /// </summary>
    public String Operator { get; protected set; }

    /// <summary>
    /// Target value.
    /// </summary>
    public String Value { get; protected set; }

    /// <summary>
    /// Lower bound limit.
    /// </summary>
    protected Int32? MinVal { get; set; }

    /// <summary>
    /// Upper bound limit.
    /// </summary>
    protected Int32? MaxVal { get; set; }

    #endregion Properties

    #region Eval

    /// <summary>
    /// Evaluates path eligibility for further search.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets criteria and could be used for further search. <value>False</value> otherwise.</returns>
    public abstract Boolean Eval(Int32 dest, Stack<Int32> path, Int32 dist);

    #endregion Eval

    #region Test

    /// <summary>
    /// Tests if path meets search criteria.
    /// </summary>
    /// <param name="dest">Destination node.</param>
    /// <param name="path">Path details.</param>
    /// <param name="dist">Travel distance.</param>
    /// <returns><value>True</value> if path meets search criteria. <value>False</value> otherwise.</returns>
    public abstract Boolean Test(Int32 dest, Stack<Int32> path, Int32 dist);

    #endregion Test

    #region ToString

    /// <summary>
    /// Converts current object to its string representation.
    /// </summary>
    public override String ToString() {
      return String.Format("{0} {1} {2}", this.Term, this.Operator, this.Value);
    }

    #endregion ToString

    #region Parse

    /// <summary>
    /// Parses search criteria form input string.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Throws if <paramref name="criteria"/> has invalid format.
    /// </exception>
    /// <param name="criteria">Search criteria string.</param>
    /// <returns>List of parsed search criteria.</returns>
    public static IList<ISearchCriteria> Parse(String criteria) {
      var res = new List<ISearchCriteria>();

      if (String.IsNullOrEmpty(criteria))
        return res;

      var critPattern = SearchCriteria.SearchCriteriaRegEx;
      var critMap = SearchCriteria.SearchCriteriaMap;

      // TODO: We could use dynamically-generated code (e.g. CSharpCodeProvider or Dynamic Linq Library) instead of search criteria string parsing.
      var match = critPattern.Match(criteria);
      if (!match.Success)
        throw new ArgumentException("Invalid search criteria format.", "criteria");

      var term = match.Groups["term"].Value;
      var comp = (String)match.Groups["comp"].Value;
      var val = (String)match.Groups["val"].Value;

      var critType = (Type)null;
      // TODO: If we are to support dynamic commands set in future, we could use one of the Dependency Injection patterns here.
      critMap.TryGetValue(term.ToLower(), out critType);
      if (critType == null)
        throw new NotSupportedException(String.Format("Search criteria for '{0}' is not supported.", term));

      res.Add(ReflectionUtils.CreateInstance<ISearchCriteria>(critType.FullName, new Object[] { comp, val }));
      return res;
    }

    #endregion Parse

  }

  #endregion [SearchCriteria class definition]
}