using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class EnvironmentManager
    {
        public enum ESEditions
        {
            Standard,
            Core
        }

        public static void Initialize()
        {
            Debug.Log("ENV", "Starting...");

            Classes.Clear();

            AddClass(new EClass(null, typeof(Functions), ObjectVisibility.Public, true, false), true);

            AddClass(new EClass(null, typeof(TestClass), ObjectVisibility.Public, true, false), false);
            AddClass(new EClass(null, typeof(SuperClass), ObjectVisibility.Public, true, false), false);

            foreach(var c in Parser.CompatibleTypesClass.CompatibleTypesNative)
            {
                AddClass(new EClass(null, c.Type, ObjectVisibility.Public, true, false) { IsCompatibleType = true }, false);
            }
            
            
            Debug.Log("ENV", "Started!");
        }

        public static List<EClass> Classes = new List<EClass>();

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