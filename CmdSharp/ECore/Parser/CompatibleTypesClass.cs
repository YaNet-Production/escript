using System;
using System.Collections.Generic;

namespace CmdSharp.Parser
{
    public static class CompatibleTypesClass
    {
        public static ETypeInfo[] CompatibleTypes
        {
            get
            {
                List<ETypeInfo> l = new List<ETypeInfo>();

                l.AddRange(CompatibleTypesNative);

                for (int i = 0; i < EnvironmentManager.Classes.Count; i++)
                {
                    if(!EnvironmentManager.Classes[i].IsCompatibleType)
                    {
                        l.Add(new ETypeInfo((Type)EnvironmentManager.Classes[i].Content));
                    }
                }

                return l.ToArray();
            }
        }

        public static ETypeInfo[] CompatibleTypesNative =
        {
            // Full type, short name
            new ETypeInfo(typeof(char), "char"),
            new ETypeInfo(typeof(string), "string"),
            new ETypeInfo(typeof(bool), "bool"),
            new ETypeInfo(typeof(int), "int"),
            new ETypeInfo(typeof(long), "long"),
            new ETypeInfo(typeof(object), "object"),
            new ETypeInfo(typeof(IntPtr)),
            new ETypeInfo(typeof(UIntPtr)),
            new ETypeInfo(typeof(uint), "uint"),
            new ETypeInfo(typeof(ushort), "ushort"),
            new ETypeInfo(typeof(float), "float"),
            new ETypeInfo(typeof(double), "double"),
            new ETypeInfo(typeof(ulong), "ulong")
        };

        public static ETypeInfo GetCompatibleTypeByName(string name)
        {
            foreach (var type in CompatibleTypes)
            {
                foreach (var n in type.Names)
                {
                    if (n == name)
                    {
                        return type;
                    }
                }
            }

            return null;
        }

        public static string[] CompatibleTypesNames
        {
            get
            {
                List<string> tmp = new List<string>();
                foreach (var c in CompatibleTypes)
                {
                    foreach (var name in c.Names)
                        tmp.Add(name);
                }
                return tmp.ToArray();
            }
        }

    }

    public class ETypeInfo
    {
        public Type Type = null;
        public string ShortName = null;

        public string Name
        {
            get
            {
                if (ShortName != null)
                    return ShortName;

                return Type.Name;
            }
        }

        public string[] Names
        {
            get
            {
                List<string> temp = new List<string>();

                if (ShortName != null)
                    temp.Add(ShortName);

                if (Type != null)
                    temp.Add(Type.Name);

                return temp.ToArray();
            }
        }

        public ETypeInfo(Type type, string s = null)
        {
            Type = type;
            ShortName = s;
        }
    }
}
