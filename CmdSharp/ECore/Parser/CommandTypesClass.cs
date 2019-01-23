using System.Collections.Generic;

namespace CmdSharp.Parser
{
    public static class CommandTypesClass
    {
        public enum CommandTypes
        {
            Unknown,
            VariableInitializer, // bool a; bool a = true;
            CastValue, // (bool)true;   or    (string)obj;
            VariableSet, // a = "something"; b = 5;
            Method, // SayHello();
            IfStructure, // if(..) {}
            IfElseStructure, // if(...) {} else {}
            ForStructure, // for(...){}
            WhileStructure, // while(...) { }
            ForeachStructure, // foreach(var x in ARRAY) { }
            ClassStructure, // class XXX { ... }
            NamespaceStructure, // namespace XXX { ... }
            UsingHeader, // using System;
            ObjectInitializer, // new ...(...); returns new instance
            Typeof, // typeof(ClassName); returns class type
            TryCatchStruct // try { } catch { } finally { }
        }

        public static CommandTypes GetCommandType(string Command)
        {
            var result = GetCommandTypeEx(Command);
            
            Debug.Store("Parser", "CmdType0", Debug.GetSafeStorage("Parser", Command, result));
            
            return result;
        }

        public static bool IsCommandTypeCompatibleWith(CommandTypes thisOne, CommandTypes withThisOne)
        {
            if (withThisOne == CommandTypes.Method)
            {
                if (thisOne == CommandTypes.Typeof 
                    || thisOne == CommandTypes.ObjectInitializer
                    || thisOne == CommandTypes.Method
                    || thisOne == CommandTypes.UsingHeader)
                    return true;
            }

            return false;
        }

        private static CommandTypes GetCommandTypeEx(string Command)
        {
            if (Command.Contains("="))
            {
                foreach (var compatibleTypeName in CompatibleTypesClass.CompatibleTypesNames)
                {
                    if (Command.StartsWith(compatibleTypeName) || Command.StartsWith("var"))
                    {
                        // It's variable initializer
                        return CommandTypes.VariableInitializer;
                    }
                }
                
                // Search for variables
                for (int i = 0; i < Variables.VarList.Count; i++)
                {
                    var var = Variables.VarList[i];
                    if (Command.StartsWith(var.Name))
                        return CommandTypes.VariableSet;
                }
            }
            else
            {
                foreach (var compatibleTypeName in CompatibleTypesClass.CompatibleTypesNames)
                {
                    if (Command.StartsWith(compatibleTypeName) || Command.StartsWith("var"))
                    {
                        // It's variable initializer
                        return CommandTypes.VariableInitializer;
                    }
                }
            }

            if (Command.StartsWith("new")) // new string()...
                return CommandTypes.ObjectInitializer;

            if (Command.StartsWith("typeof"))
            {
                if (Command.Contains("(") && Command.Contains(")"))
                {
                    return CommandTypes.Typeof;
                }
                else
                    throw new Exceptions.ParserException("Invalid typeof usage.");
            }

            if (Command.StartsWith("using"))
                return CommandTypes.UsingHeader;

            if (Command.StartsWith("(") && Command.Contains(")") && Command.Contains("new")) // (string)new string()...
            {
                int CastEndPos = -1;
                for(int i = 1; i < Command.Length; i++)
                {
                    char c = Command[i];
                    if (c == ')')
                    {
                        CastEndPos = i;
                        break;
                    }
                }
                if (Command.Remove(0, CastEndPos + 1).Trim(EParser.SpacesAndTabs).StartsWith("new"))
                    return CommandTypes.ObjectInitializer;

            }

            if(Command.StartsWith("(") && Command.Contains(")"))
            {
                string CastTo = "";
                string Value = "";

                bool CastNow = true;
                for(int i = 1; i < Command.Length; i++)
                {
                    char c = Command[i];

                    if ((c == '(' || c == ')') && !CastNow)// seems like it's a method
                        return CommandTypes.Method;

                    if (c == ')')
                    {
                        CastNow = false;
                        continue;
                    }

                    if (c == ';')
                    {
                        break;
                    }

                    if (CastNow)
                        CastTo += c;
                    else
                        Value += c;
                }

                CastTo = CastTo.Trim(EParser.SpacesAndTabs);
                Value = Value.Trim(EParser.SpacesAndTabs);

                if (CastTo.Length >= 1 && Value.Length >= 1)
                    return CommandTypes.CastValue;
            }

            if (Command.StartsWith("if") && Command.Contains("(") && Command.Contains(")"))
            {
                return CommandTypes.IfStructure;
            }

            if (Command.StartsWith("if") && Command.Contains("else") && Command.Contains("(") && Command.Contains(")"))
            {
                return CommandTypes.IfElseStructure;
            }

            if (Command.StartsWith("while") && Command.Contains("(") && Command.Contains(")"))
            {
                return CommandTypes.WhileStructure;
            }

            if (Command.StartsWith("for") && Command.Contains("(") && Command.Contains(")"))
            {
                return CommandTypes.ForStructure;
            }

            if (Command.StartsWith("foreach") && Command.Contains("(") && Command.Contains(")"))
            {
                return CommandTypes.ForeachStructure;
            }

            // TODO: Improve this
            if (Command.Contains("(") && Command.Contains(")"))
                return CommandTypes.Method;

            return CommandTypes.Unknown;
        }
    }
}
