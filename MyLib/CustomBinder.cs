using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.Serialization;

namespace MyLib
{
    public class CustomBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Assembly currentasm = Assembly.GetExecutingAssembly();
            return Type.GetType($"{currentasm.GetName().Name}.{typeName.Split('.')[1]}");
        }
    }

}
