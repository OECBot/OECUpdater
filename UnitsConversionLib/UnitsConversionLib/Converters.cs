using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace UnitsConversionLib
{
  /// <summary>
  /// A Converter interface
  /// </summary>
  public interface IConverter
  {
    /// <summary>
    /// Conversion
    /// </summary>
    /// <param name="source">Value of unit to be converted</param>
    /// <returns>Converted value</returns>
    double Convert(double source);
    /// <summary>
    /// Returns true id inverse formula exists
    /// </summary>
    bool AllowInverse { get; }
    /// <summary>
    /// Returns a inverse formula for the Converter
    /// </summary>
    IConverter Inverse { get; }
  }
  /// <summary>
  /// Abstract class for converters
  /// </summary>
  public abstract class Converter : IConverter
  {
    public abstract double Convert(double source);
    public abstract bool AllowInverse { get; }
    public abstract IConverter Inverse { get; }
  }

  /// <summary>
  /// Linear unit converter for linear formulas
  /// 
  /// DestUnit = SrcUnit * Factor + Deltha.
  /// </summary>
  public class LinearConverter : Converter
  {
    #region Fields & Properties
    private double dbl_Factor = 1d;
    public double Factor
    {
      get { return dbl_Factor; }
      set { dbl_Factor = value; }
    }

    private double dbl_Deltha = 0d;
    public double Deltha
    {
      get { return dbl_Deltha; }
      set { dbl_Deltha = value; }
    }
    #endregion

    #region Creation & Initialization
    public LinearConverter()
    {

    }

    public LinearConverter(double factor, double deltha)
    {
      if (factor == 0)
        throw new ArgumentException(global::UnitsConversionLib.Resources.Errors.FactorZeroError);
      this.dbl_Factor = factor;
      this.dbl_Deltha = deltha;
    }

    public LinearConverter(double factor)
    {
      if (factor == 0)
        throw new ArgumentException(global::UnitsConversionLib.Resources.Errors.FactorZeroError);
      this.dbl_Factor = factor;
    }
    #endregion

    #region Converter Overrides
    public override double Convert(double source)
    {
      return (double)(source * dbl_Factor + dbl_Deltha);
    }
    public override bool AllowInverse { get { return true; } }
    public override IConverter Inverse { get { return new LinearConverter(1 / dbl_Factor, -dbl_Deltha / dbl_Factor); } }
    #endregion
  }

  public class DecibelConverter : Converter
  {
    #region Fields
    private double dbl_Reference;
    public double Reference
    {
      get { return dbl_Reference; }
      set {
        if (value == 0)
          throw new ArgumentException();
        dbl_Reference = value; 
      }
    }
	
    #endregion
    
    #region Creation
    public DecibelConverter(double reference)
    {
      this.dbl_Reference = reference;
    }
    public DecibelConverter()
    {
      this.dbl_Reference = 1;
    }
    #endregion

    #region Converter Overrides
    public override double Convert(double source)
    {
      return (double)(10 * Math.Log10(source / dbl_Reference));
    }

    public override bool AllowInverse
    {
      get { return true ; }
    }

    public override IConverter Inverse
    {
      get { return new InverseDecibelConverter(dbl_Reference); }
    }
    #endregion
  }

  internal class InverseDecibelConverter : Converter
  {
    #region Fields
    private double dbl_Reference;
    public double Reference
    {
      get { return dbl_Reference; }
      set
      {
        if (value == 0)
          throw new ArgumentException();
        dbl_Reference = value;
      }
    }
    #endregion

    #region Creation
    public InverseDecibelConverter(double reference)
    {
      this.dbl_Reference = reference;
    }
    public InverseDecibelConverter()
    {
      this.dbl_Reference = 1;
    }
    #endregion

    #region Converter Overrides
    public override double Convert(double source)
    {
      return (double)(Math.Pow(10d, source / 10d) * dbl_Reference) ;
    }

    public override bool AllowInverse
    {
      get { return true; }
    }

    public override IConverter Inverse
    {
      get { return new DecibelConverter(dbl_Reference); }
    }
    #endregion
  }


  /// <summary>
  /// An abstract class for custom conversions
  /// </summary>
  public abstract class CustomConverter : Converter
  {
    public abstract void XmlInitialize(XmlElement element);
  }
}
