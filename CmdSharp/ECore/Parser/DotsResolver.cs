using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp.Parser
{
    public class DotsResolver
    {
        /// <summary>
        /// Do not use namespaces. Use only for variables.
        /// To use namespace, type <code>using NamespaceName;</code>
        /// </summary>
        /// <param name="Command">Command</param>
        /// <returns></returns>
        public static object Resolve(string Command, object ParentVariable = null)
        {
            string Name = Command;

            if (!Name.Contains('.'))
                return null;
            
            string[] Sections = Name.Split('.');
            string RealName = Sections[Sections.Length - 1].Trim(EParser.SpacesAndTabs);
            int AllSectionsLen = 0;

            for (int i = 0; i < Sections.Length - 1; i++)
                AllSectionsLen += Sections[i].Length - 1;
            
            Name = RealName;

            string section = "";
            object prevVariable = null;
            var variable = ParentVariable;
            bool LookInside = ParentVariable != null ? true : false;

            for (int i = 0; i < Sections.Length; i++)
            {
                if (i >= 1) section += '.';
                section += Sections[i];

                prevVariable = variable;
                variable = Variables.Get(section, true);

                if (variable != null)
                {
                    EClass c = EnvironmentManager.SearchForType(variable.GetType().Name);
                    c.ForceInstance(variable);

                    foreach (var field in c.PropertiesAndFields)
                    {
                        if (field.Name == RealName)
                            return field;
                    }

                    foreach (var method in c.Methods)
                    {
                        if (method.Name == RealName)
                            return method;
                    }
                    

                    throw new TypeAccessException($"Field/property/method '{RealName}' is not found in '{section}'");
                }
            }
            
            throw new TypeAccessException($"Type '{section}' is not found. If it's a namespace, do 'using' instead");
            
        }
    }
    public class NamespaceInfo
    {
        string Name = null;
        string Value = null;

        public NamespaceInfo(string Command)
        {

        }
    }
}
