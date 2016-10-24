using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace UnitsConversionLib
{
  public static class Utilities
  {
    public static object CreateInstance(string typeName)
    {
      Type type = Type.GetType(typeName);

      if (type == null)
        type = FindType(typeName);

      return Activator.CreateInstance(type);
    }

    private static Type FindType(string name)
    {
      Type result = null;
      foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        if ((result = asm.GetType(name)) != null)
          break;
      return result;
    }
  }
}
