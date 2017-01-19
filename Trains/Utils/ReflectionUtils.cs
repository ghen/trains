using System;
using System.Reflection;

namespace Train.Utils {

  #region [ReflectionUtils class definition]

  /// <summary>
  /// Helper class to simplify some reflection calls.
  /// </summary>
  public static class ReflectionUtils {

    #region CreateInstance

    /// <summary>
    /// Creates instance of target type and casts it to <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This method requires target type to have parameterless public constructor.
    /// </remarks>
    /// <typeparam name="T">Full name of type or interface to cast new instance to.</typeparam>
    /// <param name="type">Target type full name to instantiate.</param>
    /// <exception cref="ArgumentNullException">
    /// Throws in case if <paramref name="type"/> parameter is <value>null</value>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Throws in case if <paramref name="type"/> parameter contains invalid type name or empty string.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// Throws in case if target type can't be casted to <typeparamref name="T"/> or there is no suitable public constructor to call, etc.
    /// </exception>
    /// <returns>New type instance casted to <typeparamref name="T"/>.</returns>
    public static T CreateInstance<T>(String type) {
      return ReflectionUtils.CreateInstance<T>(type, null);
    }

    /// <summary>
    /// Creates instance of target type and casts it to <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// This method requires target type to have public constructor available 
    /// that accepts the list of arguments that was passed to this method.
    /// </remarks>
    /// <typeparam name="T">Full name of type or interface to cast new instance to.</typeparam>
    /// <param name="type">Target type full name to instantiate.</param>
    /// <param name="args">List of arguments to be passed to the constructor. Pass <value>null</value> to call default constructor.</param>
    /// <exception cref="ArgumentNullException">
    /// Throws in case if <paramref name="type"/> parameter is <value>null</value>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Throws in case if <paramref name="type"/> parameter contains invalid type name or empty string.
    /// </exception>
    /// <exception cref="ApplicationException">
    /// Throws in case if target type can't be casted to <typeparamref name="T"/> or there is no suitable public constructor to call, etc.
    /// </exception>
    /// <returns>New type instance casted to <typeparamref name="T"/>.</returns>
    public static T CreateInstance<T>(String type, Object[] args) {
      if (type == null) throw new ArgumentNullException("type");

      Type castType = typeof(T);
      Type sourceType = Type.GetType(type);

      if (sourceType == null)
        throw new ArgumentException(String.Format("Specified type '{0}' can't be found", type), "type");

      if (!castType.IsAssignableFrom(sourceType))
        throw new ApplicationException(String.Format(
            "Target type '{0}' can't be casted to specified type or interface: '{1}'",
            sourceType.FullName, castType.FullName));

      if (args == null) args = new Object[0];
      Object instance = null;

      try {
        instance = sourceType.InvokeMember(String.Empty, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, null, args);
      } catch (Exception ex) {
        throw new ApplicationException(String.Format("Target type '{0}' can't be instantiated. {1}", type, ex.Message), ex);
      }

      return (T)instance;
    }

    #endregion CreateInstance
  }

  #endregion [ReflectionUtils class definition]
}