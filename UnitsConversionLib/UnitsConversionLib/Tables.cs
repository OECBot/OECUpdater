using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Resources;
using System.IO;

namespace UnitsConversionLib
{
  /// <summary>
  /// An interface for UnitTable
  /// <seealso cref="UnitsConversionLib.UnitTable"/>
  /// </summary>
  public interface IUnitTable
  {
    string GetUnitName(int unitCode);
    string GetUnitName(Unit unit);
    string GetUnitSymbol(int unitCode);
    string GetUnitSymbol(Unit unit);
    string GetUnitPlural(int unitCode);
    string GetUnitPlural(Unit unit);
    /// <summary>
    /// Returns a Converter
    /// </summary>
    /// <param name="srcCode">Code for source unit</param>
    /// <param name="destCode">Code for destination unit</param>
    /// <returns>A Converter object</returns>
    IConverter GetConversion(int srcCode, int destCode);
    IConverter GetConversion(Unit srcUnit, Unit destUnit);
    /// <summary>
    /// Converts the unit
    /// </summary>
    /// <param name="srcUnit">Unit to be converted</param>
    /// <param name="destCode">Code for destination unit</param>
    /// <returns></returns>
    Unit Convert(Unit srcUnit, int destCode);
    bool IsKnownUnit(int unitCode);
  }

  public abstract class UnitTable : IUnitTable
  {
    #region IUnitTable Members

    public abstract string GetUnitName(int unitCode);
    public string GetUnitName(Unit unit)
    {
      return GetUnitName(unit.UnitCode);
    }

    public abstract string GetUnitSymbol(int unitCode);
    public string GetUnitSymbol(Unit unit)
    {
      return GetUnitSymbol(unit.UnitCode);
    }

    public abstract string GetUnitPlural(int unitCode);
    public string GetUnitPlural(Unit unit)
    {
      return GetUnitPlural(unit.UnitCode);
    }

    public abstract IConverter GetConversion(int srcCode, int destCode);
    public IConverter GetConversion(Unit srcUnit, Unit destUnit)
    {
      return GetConversion(srcUnit.UnitCode, destUnit.UnitCode);
    }

    public Unit Convert(Unit srcUnit, int destCode)
    {
      return new Unit(destCode, GetConversion(srcUnit.UnitCode, destCode).Convert(srcUnit.Value), srcUnit.UnitTable);
    }

    public abstract bool IsKnownUnit(int unitCode);
    #endregion

    #region Built-in Tables
    static XmlUnitTable tbl_Length;
    public static UnitTable LengthTable
    {
      get
      {
        if (tbl_Length == null)
          tbl_Length = new XmlUnitTable(Assembly.GetExecutingAssembly().
            GetManifestResourceStream("UnitsConversionLib.LengthUnits.xml"));
        return tbl_Length;
      }
    }
    static XmlUnitTable tbl_Weight;
    public static UnitTable WeightTable
    { 
      get
      {
        if (tbl_Weight == null)
          tbl_Weight = new XmlUnitTable(Assembly.GetExecutingAssembly().
            GetManifestResourceStream("UnitsConversionLib.WeightUnits.xml"));
        return tbl_Weight;
      }
    }
    static UnitTable tbl_Temperature;
    public static UnitTable TemperatureTable
    {
      get
      {
        if (tbl_Temperature == null)
          tbl_Temperature = new XmlUnitTable(Assembly.GetExecutingAssembly().
            GetManifestResourceStream("UnitsConversionLib.TemperatureUnits.xml"));
        return tbl_Temperature;
      }
    }
    static UnitTable tbl_Volume;
    public static UnitTable VolumeTable
    {
      get
      {
        if (tbl_Volume == null)
          tbl_Volume = new XmlUnitTable(Assembly.GetExecutingAssembly().
            GetManifestResourceStream("UnitsConversionLib.VolumeUnits.xml"));
        return tbl_Volume;
      }
    }

    #endregion
  }

  /// <summary>
  /// A table class using dictionaries
  /// </summary>
  public abstract class BasicUnitTable : UnitTable
  {
    #region Heirs Tool Memebers
    /// <summary>
    /// Specifications of the units
    /// </summary>
    protected struct UnitSpec
    {
      public int Code;
      public string Name;
      public string Symbol;
      public string Plural;
    }
    protected Dictionary<int, UnitSpec> dic_Units = new Dictionary<int,UnitSpec>();
    protected Dictionary<string, IConverter> dic_ConversionTable = new Dictionary<string,IConverter>();
    protected void AddUnit(int unitCode, string Name, string Symbol, string Plural)
    {
      if (dic_Units.ContainsKey(unitCode))
        throw new DuplicatedUnitException();

      UnitSpec spec;
      spec.Code = unitCode;
      spec.Name = Name;
      spec.Symbol = Symbol;
      spec.Plural = Plural;

      dic_Units[unitCode] = spec;
    }
    protected void AddConversion(int srcCode, int destCode, Converter Converter)
    {
      dic_ConversionTable[FormatKey(srcCode, destCode)] = Converter;
      if (!dic_ConversionTable.ContainsKey(FormatKey(destCode, srcCode)) && Converter.AllowInverse)
        dic_ConversionTable[FormatKey(destCode, srcCode)] = Converter.Inverse;
    }
    protected static string FormatKey(int srcCode, int destCode)
    {
      return srcCode + ":" + destCode;
    }
    #endregion

    #region UnitTable Overrides
    public override IConverter GetConversion(int srcCode, int destCode)
    {
      if (!dic_ConversionTable.ContainsKey(FormatKey(srcCode, destCode)))
        throw new NoAvailableConversionException();
      return dic_ConversionTable[FormatKey(srcCode, destCode)];
    }
    public override string GetUnitName(int unitCode)
    {
      if (! dic_Units.ContainsKey(unitCode))
        throw new UnknownUnitException() ;
      return dic_Units[unitCode].Name;
    }
    public override string GetUnitSymbol(int unitCode)
    {
      if (!dic_Units.ContainsKey(unitCode))
        throw new UnknownUnitException();
      return dic_Units[unitCode].Symbol;
    }
    public override string GetUnitPlural(int unitCode)
    {
      if (!dic_Units.ContainsKey(unitCode))
        throw new UnknownUnitException();
      return dic_Units[unitCode].Plural;
    }
    public override bool IsKnownUnit(int unitCode)
    {
      return dic_Units.ContainsKey(unitCode);
    }
    #endregion
  }

  /// <summary>
  /// Read in Converter and units from XML
  /// </summary>
  public class XmlUnitTable : BasicUnitTable
  {
    #region Constants
    public const string ELEMENT_UNITTABLE = "UnitTable";
    public const string ELEMENT_UNITS = "Units";
    public const string ELEMENT_UNIT = "Unit";
    public const string ELEMENT_ConversionS = "Conversions";
    public const string ELEMENT_Conversion = "Converter";
    public const string ELEMENT_LINEAR = "Linear";
    public const string ELEMENT_DECIBEL = "Decibel";
    public const string ELEMENT_CUSTOM = "Custom";

    public const string ATTRIBUTE_NAME = "name";
    public const string ATTRIBUTE_SYMBOL = "symbol";
    public const string ATTRIBUTE_PLURAL = "plural";
    public const string ATTRIBUTE_CODE = "code";
    public const string ATTRIBUTE_FACTOR = "factor";
    public const string ATTRIBUTE_DELTHA = "deltha";
    public const string ATTRIBUTE_TYPENAME = "typeName";
    public const string ATTRIBUTE_SRCCODE = "srcCode";
    public const string ATTRIBUTE_DESTCODE = "destCode";
    public const string ATTRIBUTE_REFERENCE = "reference";
    #endregion

    #region Creation & Initialization
    public XmlUnitTable(Stream stream)
    {
      XmlDocument units = new XmlDocument();
      units.Load(stream);

      Initialize(units[ELEMENT_UNITTABLE]);
    }
    public XmlUnitTable(string fileName)
    {
      XmlDocument units = new XmlDocument();
      units.Load(fileName);

      Initialize(units[ELEMENT_UNITTABLE]);
    }
    public XmlUnitTable(XmlDocument units)
    {
      Initialize(units[ELEMENT_UNITTABLE]);
    }

    private void Initialize(XmlElement unitTableElement)
    {
      foreach (XmlElement unitElement in unitTableElement[ELEMENT_UNITS].ChildNodes)
      {
        int code ;
        string name, symbol, plural;

        CreateUnit(unitElement, out code, out name, out symbol, out plural);
        AddUnit(code, name, symbol, plural);
      }

      foreach (XmlElement ConversionElement in unitTableElement[ELEMENT_ConversionS].ChildNodes)
      {
        int srcCode, destCode;

        Converter Converter = CreateConversion(ConversionElement, out srcCode, out destCode);
        AddConversion(srcCode, destCode, Converter);
      }
    }

    protected static void CreateUnit(XmlElement unitElement, out int unitCode, out string unitName, out string unitSymbol, out string unitPlural)
    {
      try
      {
        unitPlural = (unitElement.Attributes[ATTRIBUTE_PLURAL] != null)? 
          unitElement.Attributes[ATTRIBUTE_PLURAL].Value : unitElement.Attributes[ATTRIBUTE_NAME].Value;

        unitCode = int.Parse(unitElement.Attributes[ATTRIBUTE_CODE].Value);
        unitName = unitElement.Attributes[ATTRIBUTE_NAME].Value;
        unitSymbol = unitElement.Attributes[ATTRIBUTE_SYMBOL].Value;
      }
      catch (Exception ex)
      {
        throw new UnitCreationException(global::UnitsConversionLib.Resources.Errors.UnitTagError, ex);
      }
    }

    protected static Converter CreateConversion(XmlElement ConversionElement, out int srcCode, out int destCode)
    {
      Converter result = null ;
      try
      {
        srcCode = int.Parse(ConversionElement.Attributes[ATTRIBUTE_SRCCODE].Value);
        destCode = int.Parse(ConversionElement.Attributes[ATTRIBUTE_DESTCODE].Value);
      }
      catch (Exception ex)
      {
        throw new ConversionCreationException(global::UnitsConversionLib.Resources.Errors.ConversionTagError, ex);
      }

      if (ConversionElement.Name == ELEMENT_LINEAR)
      {
        try
        {
          double factor = ConversionElement.Attributes[ATTRIBUTE_FACTOR] == null ? 
            1d : double.Parse(ConversionElement.Attributes[ATTRIBUTE_FACTOR].Value);
          double deltha = ConversionElement.Attributes[ATTRIBUTE_DELTHA] == null ? 
            0d : double.Parse(ConversionElement.Attributes[ATTRIBUTE_DELTHA].Value);

          result = new LinearConverter(factor, deltha);
        }
        catch (Exception ex)
        {
          throw new ConversionCreationException(global::UnitsConversionLib.Resources.Errors.ConversionTagError, ex);
        }
      }

      if (ConversionElement.Name == ELEMENT_DECIBEL)
      {
        try
        {
          double reference = double.Parse(ConversionElement.Attributes[ATTRIBUTE_REFERENCE].Value) ;

          result = new DecibelConverter(reference);
        }
        catch (Exception ex)
        { 
          throw new ConversionCreationException(global::UnitsConversionLib.Resources.Errors.ConversionTagError, ex);
        }
      }

      if (ConversionElement.Name == ELEMENT_CUSTOM)
      {
        try
        {
          string typeName = ConversionElement.Attributes[ATTRIBUTE_TYPENAME].Value;

          result = (Converter)Utilities.CreateInstance(typeName);
          if (result is CustomConverter)
          {
            ((CustomConverter)result).XmlInitialize(ConversionElement);
          }
        }
        catch (Exception ex)
        {
          throw new ConversionCreationException(global::UnitsConversionLib.Resources.Errors.ConversionTagError, ex);
        }
      }

      if (result == null)
        throw new ConversionCreationException();

      return result;
    }
    #endregion
  }

  /// <summary>
  /// Local unit names and their plural forms
  /// </summary>
  public class LocalizedXmlUnitTable : XmlUnitTable
  {
    #region Fields & Properties
    ResourceManager man_Resources;
    #endregion

    #region Creation & Initialization
    public LocalizedXmlUnitTable(string fileName, ResourceManager manager)
      : base(fileName)
    {
      if (manager == null)
        throw new ArgumentNullException();

      man_Resources = manager;
    }

    public LocalizedXmlUnitTable(XmlDocument units, ResourceManager manager) 
      : base(units)
    {
      if (manager == null)
        throw new ArgumentNullException();

      man_Resources = manager;
    }

    public LocalizedXmlUnitTable(string fileName)
      : this(fileName, global::UnitsConversionLib.Resources.Units.ResourceManager)
    { 
    
    }

    public LocalizedXmlUnitTable(XmlDocument units)
      : this(units, global::UnitsConversionLib.Resources.Units.ResourceManager)
    { 
    
    }
    #endregion

    #region BasicUnitTable Overrides
    public override string GetUnitName(int unitCode)
    {
      return man_Resources.GetString(base.GetUnitName(unitCode));
    }
    public override string GetUnitPlural(int unitCode)
    {
      return man_Resources.GetString(base.GetUnitPlural(unitCode));
    }
    #endregion
  }
}
