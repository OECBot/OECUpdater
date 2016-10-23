using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OECLib.Interface
{
    public interface XMLWritable
    {
        void Write(XmlWriter w);
    }
}
