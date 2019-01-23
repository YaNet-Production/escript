using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace CmdSharp
{
    public class EnvironmentManager
    {
        public static void Initialize()
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            Debug.Log("ENV", "Starting...");

            Classes.Clear();

            AddClass(new EClass(null, typeof(Functions), ObjectVisibility.Public, true, false), true);

            foreach (var c in Parser.CompatibleTypesClass.CompatibleTypesNative)
            {
                AddClass(new EClass(null, c.Type, ObjectVisibility.Public, true, false) { IsCompatibleType = true }, false);
            }
            
            AddLibrary(new ImportedLibInfo(executingAssembly));

            Debug.Log("ENV", "Started!");
        }

        public enum ESEditions
        {
            Standard,
            Core
        }

        public static ESEditions Edition
        {
            get
            {
#if IsCore
                return ESEditions.Core; 
#else
                return ESEditions.Standard;
#endif
            }
        }


        public static List<EClass> Classes = new List<EClass>();
        public static List<ImportedLibInfo> LoadedLibs = new List<ImportedLibInfo>();
        public static List<Thread> UserThreads = new List<Thread>();

        public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
        {
            List<Type> result = new List<Type>();

            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.Namespace == nameSpace)
                    result.Add(type);
            }

            return result.ToArray();
        }

        public static bool IsLibLoaded(ImportedLibInfo lib)
        {
            for (int i = 0; i < LoadedLibs.Count; i++)
            {
                if (LoadedLibs[i].assembly.FullName == lib.assembly.FullName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Add library to environment and all it's references
        /// </summary>
        /// <param name="i">Library information</param>
        public static void AddLibrary(ImportedLibInfo i, bool addReferenced = true)
        {
            if (IsLibLoaded(i))
                return;

            if (addReferenced)
            {
                foreach (var reference in i.assembly.GetReferencedAssemblies())
                {
                    AddLibrary(new ImportedLibInfo(Assembly.Load(reference)), false);
                    //Debug.Log("ENV", $"[{i.assembly.FullName}] module: {reference.Name}");
                }
            }

            Debug.Log("ENV", " + lib \"" + i.assembly.FullName + "\"");

            LoadedLibs.Add(i);
        }

        public static EMethodNew[] AllMethods
        {
            get
            {
                List<EMethodNew> result = new List<EMethodNew>();

                for (int i = 0; i < Classes.Count; i++)
                {
                    result.AddRange(Classes[i].Methods);
                }

                return result.ToArray();
            }
        }

        public static void AddNamespace(string Namespace)
        {
            for (int i = 0; i < LoadedLibs.Count; i++)
            {
                var lib = LoadedLibs[i];

                var types = GetTypesInNamespace(lib.assembly, Namespace);

                foreach (var type in types)
                {
                    AddClass(new EClass(null, type, ObjectVisibility.Public, false, false), false);
                }
            }
        }

        public static void RemoveNamespace(string Namespace)
        {
            for(int i = 0; i < Classes.Count; i++)
            {
                if (Classes[i].Namespace == Namespace)
                    RemoveClass(Classes[i]);
            }
        }

        public static EClass SearchForType(string Name)
        {
            for (int i = 0; i < Classes.Count; i++)
            {
                if (Classes[i].Name == Name)
                    return Classes[i];
            }

            return null;
        }

        public static void AddClass(EClass Class, bool CreateInstance)
        {
            Debug.Log("ENV", " + class \"" + Class.Name + "\", content type: " + Class.Content.GetType().FullName);

            if (Classes.Contains(Class))
                Classes.Remove(Class);

            Classes.Add(Class);

            if(CreateInstance)
                Class.CreateInstance();
        }

        public static void RemoveClass(string name)
        {
            for(int i = 0; i < Classes.Count; i++)
            {
                if(Classes[i].Name == name)
                {
                    Classes.Remove(Classes[i]);
                    return;
                }
            }
        }

        public static void RemoveClass(EClass Class)
        {
            Debug.Log("ENV", " - class \"" + Class.Name + "\", content type: " + Class.Content.GetType().FullName);

            if (Classes.Contains(Class))
                Classes.Remove(Class);
        }

        //public static void AddMethod(EMethodNew Method)
        //{
        //    Debug.Log("ENV", " + method \"" + Method.Name + "\", content type: " + Method.Content.GetType().FullName);

        //    if (Methods.Contains(Method))
        //        Methods.Remove(Method);

        //    Methods.Add(Method);
        //}

        //public static void RemoveMethod(EMethodNew Method)
        //{
        //    Debug.Log("ENV", " - method \"" + Method.Name + "\", content type: " + Method.Content.GetType().FullName);

        //    if (Methods.Contains(Method))
        //        Methods.Remove(Method);
        //}

        public enum ObjectVisibility
        {
            Private,
            Public
        }
    }
}