using System;
using System.Collections.Generic;
using System.Text;

namespace UnitsConversionLib
{
  [global::System.Serializable]
  public class NoAvailableConversionException : Exception
  {
    public NoAvailableConversionException() { }
    public NoAvailableConversionException(string message) : base(message) { }
    public NoAvailableConversionException(string message, Exception inner) : base(message, inner) { }
    protected NoAvailableConversionException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
  
  [global::System.Serializable]
  public class UnknownUnitException : Exception
  {
    public UnknownUnitException() { }
    public UnknownUnitException(string message) : base(message) { }
    public UnknownUnitException(string message, Exception inner) : base(message, inner) { }
    protected UnknownUnitException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }

  [global::System.Serializable]
  public class DuplicatedUnitException : Exception
  {
    public DuplicatedUnitException() { }
    public DuplicatedUnitException(string message) : base(message) { }
    public DuplicatedUnitException(string message, Exception inner) : base(message, inner) { }
    protected DuplicatedUnitException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
  
  [global::System.Serializable]
  public class ConversionCreationException : Exception
  {
    public ConversionCreationException() { }
    public ConversionCreationException(string message) : base(message) { }
    public ConversionCreationException(string message, Exception inner) : base(message, inner) { }
    protected ConversionCreationException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
  
  [global::System.Serializable]
  public class UnitCreationException : Exception
  {
    public UnitCreationException() { }
    public UnitCreationException(string message) : base(message) { }
    public UnitCreationException(string message, Exception inner) : base(message, inner) { }
    protected UnitCreationException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }

  
  [global::System.Serializable]
  public class CustomConversionCreationException : Exception
  {
    public CustomConversionCreationException() { }
    public CustomConversionCreationException(string message) : base(message) { }
    public CustomConversionCreationException(string message, Exception inner) : base(message, inner) { }
    protected CustomConversionCreationException(
    System.Runtime.Serialization.SerializationInfo info,
    System.Runtime.Serialization.StreamingContext context)
      : base(info, context) { }
  }
}
