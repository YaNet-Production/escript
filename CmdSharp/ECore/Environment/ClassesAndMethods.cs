using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static CmdSharp.EnvironmentManager;

namespace CmdSharp
{
    public class EClass
    {
        public string Namespace = null;
        public string Name = null;
        public bool IsStatic = false;
        public ObjectVisibility Visibility = ObjectVisibility.Private;
        private object content = null;
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                if (value == null || value.GetType() == typeof(Type) || value.GetType().Name == "RuntimeType")
                {
                    content = value;
                    return;
                }
                else
                {
                    throw new Exception($"Excpected Type or UserType, but '{value.GetType().Name}' received");
                }
            }
        }
        public object Tag = null;
        public bool IsUser = false;
        public object Instance = null;
        public bool IsCompatibleType = false;

        public EMethodNewArgument[] ConstructorArguments
        {
            get
            {
                List<EMethodNewArgument> arguments = new List<EMethodNewArgument>();

                if (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType")
                {
                    var type = (Type)Content;

                    var Constructors = type.GetConstructors();

                    if (Constructors.Length >= 1)
                    {
                        foreach (var a in Constructors[0].GetParameters())
                        {
                            var Type = a.ParameterType;
                            var DefaultValue = a.DefaultValue;
                            var Name = a.Name;

                            arguments.Add(new EMethodNewArgument(Name, Type, DefaultValue));
                        }
                    }

                    return arguments.ToArray();
                }
                else
                    throw new NotImplementedException("Only .NET types pelase");
            }
        }

        public EField[] Fields
        {
            get
            {
                List<EField> result = new List<EField>();


                if (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType")
                {
                    var c = (Type)Content;

                    foreach (var m in c.GetFields())
                    {
                        result.Add(new EField() { Content = m, Name = m.Name, FieldType = m.FieldType, ReflectedType = m.ReflectedType, ClassInstance = Instance });
                    }
                }

                return result.ToArray();
            }
        }

        public EProperty[] Properties
        {
            get
            {
                List<EProperty> result = new List<EProperty>();


                if (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType")
                {
                    var c = (Type)Content;

                    foreach (var m in c.GetProperties())
                    {
                        result.Add(new EProperty() { Content = m, Name = m.Name, FieldType = m.PropertyType, ReflectedType = m.ReflectedType, ClassInstance = Instance });
                    }
                }

                return result.ToArray();
            }
        }

        public EField[] PropertiesAndFields
        {
            get
            {
                List<EField> result = new List<EField>();

                result.AddRange(Fields);
                result.AddRange(Properties);

                return result.ToArray();
            }
        }

        public EMethodNew[] Methods
        {
            get
            {
                List<EMethodNew> result = new List<EMethodNew>();

                if(Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType")
                {
                    var c = (Type)Content;

                    foreach(var m in c.GetMethods())
                    {
                        ObjectVisibility v = ObjectVisibility.Private;

                        if (m.IsPublic)
                            v = ObjectVisibility.Public;

                        result.Add(new EMethodNew(m.Name, m, v, m.IsStatic, false) { ClassInstance = Instance });
                    }
                }

                return result.ToArray();
            }
        }

        public object InvokeMethod(string Name, object[] Arguments = null, bool NullInsteadOfExceptionIfNotFound = false)
        {
            if (Arguments == null)
                Arguments = new object[] { };

            foreach(var m in Methods)
            {
                if(m.Name == Name)
                {
                    return m.Invoke(Arguments);
                }
            }

            if (NullInsteadOfExceptionIfNotFound)
                return null;

            throw new Exceptions.InvokeMethodNotFoundException($"Method '{Name}' is not found in type '{Name}'");
        }

        public void ForceInstance(object i)
        {
            Instance = i;
        }

        public object CreateInstance(object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };


            var OriginalArguments = ConstructorArguments;

            if (Arguments.Length < OriginalArguments.Length)
            {
                for (int i = 0; i < OriginalArguments.Length; i++)
                {
                    if (i < Arguments.Length) // If argument no argument/is broken, set to default;
                    {
                        if (Arguments[i] == null)
                            Arguments[i] = OriginalArguments[i].DefaultValue;
                    }
                    else // If argument not found, append default value to the array
                    {
                        var TempList = Arguments.ToList();
                        TempList.Add(OriginalArguments[i].DefaultValue);
                        Arguments = TempList.ToArray();
                    }
                }
            }

            if (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType")
            {
                Instance = Activator.CreateInstance((Type)Content, Arguments);
                Debug.Log("Env", $"Instance of '{((Type)Content).FullName}' created: {Instance.GetType().FullName}");
            }
            else
                throw new Exception($"Can't create instance of '{Name}'");

            return Instance;
        }

        public EClass(string NullOrName, object content, ObjectVisibility visibility, bool istatic, bool isuser = false, string NullOrNamespace = null)
        {
            Content = content;
            if (NullOrName == null && (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType"))
            {
                var c = (Type)Content;
                NullOrName = c.Name;
            }
            if (NullOrNamespace == null && (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType"))
            {
                var c = (Type)Content;
                NullOrNamespace = c.Namespace;
            }

            if (Content.GetType() != typeof(Type) && Content.GetType().Name != "RuntimeType")
                throw new NotImplementedException($"Attemption to create EClass with '{Content.GetType().Name}' type. (not supported)");

            Name = NullOrName;
            Visibility = visibility;
            IsUser = isuser;
            Namespace = NullOrNamespace;
            IsStatic = istatic;
        }
    }

    public class EMethodNew
    {
        public string Name = null;
        public bool IsStatic = false;
        public ObjectVisibility Visibility = ObjectVisibility.Private;
        public object Content = null;
        public object Tag = null;
        public bool IsUser = false;
        public object ClassInstance = null;

        public EMethodNewArgument[] Parameters
        {
            get
            {
                List<EMethodNewArgument> arguments = new List<EMethodNewArgument>();

                if (Content.GetType() == typeof(MethodInfo) || Content.GetType().Name == "RuntimeMethodInfo")
                {
                    var type = (MethodInfo)Content;

                    foreach (var a in type.GetParameters())
                    {
                        var Type = a.ParameterType;
                        var DefaultValue = a.DefaultValue;
                        var Name = a.Name;

                        arguments.Add(new EMethodNewArgument(Name, Type, DefaultValue));
                    }

                    return arguments.ToArray();
                }
                else
                    throw new NotImplementedException("Only .NET types pelase");

                return null;
            }
        }

        public EMethodNewArgument[] GetParameters() { return Parameters; }

        public object Invoke(object[] Arguments = null)
        {
            if (Arguments == null)
                Arguments = new object[] { };


            var OriginalArguments = Parameters;

            if (Arguments.Length < OriginalArguments.Length)
            {
                for (int i = 0; i < OriginalArguments.Length; i++)
                {
                    if (i < Arguments.Length) // If argument no argument/is broken, set to default;
                    {
                        if (Arguments[i] == null)
                            Arguments[i] = OriginalArguments[i].DefaultValue;
                    }
                    else // If argument not found, append default value to the array
                    {
                        var TempList = Arguments.ToList();
                        TempList.Add(OriginalArguments[i].DefaultValue);
                        Arguments = TempList.ToArray();
                    }
                }
            }

            if (Content.GetType() == typeof(MethodInfo) || Content.GetType().Name == "RuntimeMethodInfo")
            {
                if (ClassInstance == null)
                    throw new NullReferenceException($"Can't invoke '{Name}' method: no class instance");



                var m = (MethodInfo)Content;
                return m.Invoke(ClassInstance, Arguments);
            }
            else
                throw new NotImplementedException("User method are not implemented");
        }

        public EMethodNew(string name, object content, ObjectVisibility visibility, bool istatic, bool isuser = false)
        {
            Name = name;
            Content = content;
            Visibility = visibility;
            IsUser = isuser;
            IsStatic = istatic;
        }
    }

    public class EProperty : EField
    {
    }

    public class EField
    {
        public string Name;
        public Type FieldType;
        public Type ReflectedType;
        public object Content = null;
        public object Tag = null;
        public bool IsUser = false;
        public object ClassInstance = null;

        public void SetValue(object value)
        {
            if (Content.GetType() == typeof(FieldInfo) || Content.GetType().Name == "RuntimeFieldInfo")
            {
                if (ClassInstance == null)
                    throw new NullReferenceException($"Can't access '{Name}' field: no class instance");

                FieldInfo field = (FieldInfo)Content;

                field.SetValue(ClassInstance, value);
            }
            else if (Content.GetType() == typeof(PropertyInfo) || Content.GetType().Name == "RuntimePropertyInfo")
            {
                if (ClassInstance == null)
                    throw new NullReferenceException($"Can't access '{Name}' field: no class instance");

                PropertyInfo field = (PropertyInfo)Content;
                
                field.SetValue(ClassInstance, value, null);
            }
        }

        public object GetValue()
        {
            if (Content.GetType() == typeof(FieldInfo) || Content.GetType().Name == "RuntimeFieldInfo")
            {
                if (ClassInstance == null)
                    throw new NullReferenceException($"Can't access '{Name}' field: no class instance");

                FieldInfo field = (FieldInfo)Content;

                return field.GetValue(ClassInstance);
            }
            else if (Content.GetType() == typeof(PropertyInfo) || Content.GetType().Name == "RuntimePropertyInfo")
            {
                if (ClassInstance == null)
                    throw new NullReferenceException($"Can't access '{Name}' field: no class instance");

                PropertyInfo field = (PropertyInfo)Content;

                return field.GetValue(ClassInstance, null);
            }
            else
                throw new NotImplementedException();
        }
    }

    public class EMethodNewArgument
    {
        public string Name;
        public Type ParameterType;
        public object DefaultValue;

        public EMethodNewArgument(string name, Type type, object defautlValue)
        {
            Name = name;
            ParameterType = type;
            DefaultValue = defautlValue;
        }
    }
}
