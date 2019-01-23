﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using static CmdSharp.EnvironmentManager;

namespace CmdSharp
{
    public class EClass
    {
        public string Name = null;
        public bool IsStatic = false;
        public ObjectVisibility Visibility = ObjectVisibility.Private;
        public object Content = null;
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

                return null;
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

        public EClass(string NullOrName, object content, ObjectVisibility visibility, bool istatic, bool isuser = false)
        {
            Content = content;
            if (NullOrName == null && (Content.GetType() == typeof(Type) || Content.GetType().Name == "RuntimeType"))
            {
                var c = (Type)Content;
                NullOrName = c.Name;
            }

            if (Content.GetType() != typeof(Type) && Content.GetType().Name != "RuntimeType")
                throw new NotImplementedException($"Attemption to create EClass with '{Content.GetType().Name}' type. (not supported)");

            Name = NullOrName;
            Visibility = visibility;
            IsUser = isuser;
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
