using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace UnitsConversionLib
{
  /// <summary>
  /// Struct for an internal Unit
  /// </summary>
  [StructLayout(LayoutKind.Sequential)]
  internal struct UnitValue
  {
    public int Code;
    public double Value;

    #region Object Overrides
    public override string ToString()
    {
      return "Unit Code: " + Code.ToString("X8") + "; Unit Value: " + Value;
    }
    public override bool Equals(object obj)
    {
      if (!(obj is UnitValue))
        return false;

      UnitValue unit = (UnitValue)obj;

      return unit.Code == Code && unit.Value == Value;
    }
    public override int GetHashCode()
    {
      return Code.GetHashCode() + Value.GetHashCode();
    }
    #endregion

    #region Creation
    public UnitValue(int code, double value)
    {
      this.Code = code;
      this.Value = value;
    }
    #endregion

    #region Converter
    public static implicit operator long(UnitValue x)
    {
      long result = 0 ;

      unsafe
      {
        byte* pX = (byte*)(void*)&x;
        byte* pResult = (byte*)(void*)&result;

        CopyBytes(pX, pResult, sizeof(UnitValue));
      }

      return result;
    }

    public static explicit operator UnitValue(long x)
    {
      UnitValue result;

      unsafe
      { 
        byte* pX = (byte*)(void*)&x;
        byte* pResult = (byte*)(void*)&result;

        CopyBytes(pX, pResult, sizeof(UnitValue));
      }

      return result;
    }

    private unsafe static void CopyBytes(byte* src, byte* dest, int count)
    {
      for (int iByte = 0; iByte < count; iByte++)
        dest[iByte] = src[iByte];
    }
    #endregion
  }

  /// <summary>
  /// Public Unit class
  /// <seealso cref="UnitsConversionLib.UnitTable"/>
  /// <seealso cref="UnitsConversionLib.XmlUnitTable"/>
  /// </summary>
  public class Unit
  {
    #region Fields & Properties
    private UnitTable tbl_UnitTable;
    /// <summary>
    /// Get the Unit's Table
    /// </summary>
    public UnitTable UnitTable
    {
      get { return tbl_UnitTable; }
    }

    /// <summary>
    /// Get the Unit's Name.
    /// </summary>
    public string UnitName
    {
      get { return tbl_UnitTable.GetUnitName(unt_Value.Code); }
    }

    /// <summary>
    /// Get the Unit's Symbol
    /// </summary>
    public string UnitSymbol
    {
      get { return tbl_UnitTable.GetUnitSymbol(unt_Value.Code); }
    }

    /// <summary>
    /// Get the Unit's Name in plural form
    /// </summary>
    public string UnitPlural
    {
      get { return tbl_UnitTable.GetUnitPlural(unt_Value.Code); }
    }

    private UnitValue unt_Value;

    /// <summary>
    /// Set or get the Unit's value
    /// </summary>
    public double Value
    {
      get { return unt_Value.Value; }
      set { unt_Value.Value = value; }
    }

    /// <summary>
    /// Set or get the unit's value and code
    /// </summary>
    public long Int64Value
    {
      get { return unt_Value; }
      set 
      { 
        UnitValue assign = (UnitValue)value;
        if (!tbl_UnitTable.IsKnownUnit(assign.Code))
          throw new UnknownUnitException();

        unt_Value = assign;
      }
    }

    /// <summary>
    /// set or get the unit's value
    /// </summary>
    public int UnitCode
    {
      get { return unt_Value.Code; }
      set
      {
        if (!tbl_UnitTable.IsKnownUnit(value))
          throw new UnknownUnitException();
        unt_Value.Code = value;
      }
    }

    /// <summary>
    /// Converts the unit to another unit
    /// </summary>
    /// <param name="destCode">destination unit code</param>
    /// <returns>Converted unit</returns>
    public Unit Convert(int destCode)
    {
      return tbl_UnitTable.Convert(this, destCode);
    }
    #endregion

    #region Creation
    /// <summary>
    /// Creates a new Unit
    /// </summary>
    /// <param name="unitCode">Unit code</param>
    /// <param name="unitValue">Unit value</param>
    /// <param name="table">Unit table</param>
    public Unit(int unitCode, double unitValue, UnitTable table)
    {
      unt_Value.Code = unitCode;
      unt_Value.Value = unitValue;
      tbl_UnitTable = table;
    }
    #endregion

    #region Object Overrides
    public override string ToString()
    {
      return Value + UnitSymbol;
    }
    public virtual string ToString(string format)
    {
      return Value.ToString(format) + UnitSymbol;
    }
    public virtual string ToString(IFormatProvider provider)
    { 
      return Value.ToString(provider) + UnitSymbol ;
    }
    public virtual string ToString(string format, IFormatProvider provider)
    {
      return Value.ToString(format, provider);
    }

    public override bool Equals(object obj)
    {
      if (!(obj is Unit))
        return false;

      return unt_Value.Equals(obj);
    }
    public override int GetHashCode()
    {
      return unt_Value.GetHashCode();
    }
    #endregion
  }
}
