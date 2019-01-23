/*
 ESCRIPT Project - powerful command interpreter
 File:        EParser.cs
 Description: Command parser (part of command processor)
 Author:      Dz3n (Yaroslav Kibysh)
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static CmdSharp.Parser.CommandNormalizer;
using static CmdSharp.Parser.CommandTypesClass;
using static CmdSharp.Parser.CompatibleTypesClass;

namespace CmdSharp.Parser
{
    public class ParseResult
    {
        public enum ParserNativeResults // What should return variable initialization process??
        {
            ParserNativeUnknown,
            ParserNativeOK,
            ParserNativeError
        }

        public ParserNativeResults NativeResult = ParserNativeResults.ParserNativeUnknown;
        public string CustomMessage = null;

        public ParseResult(ParserNativeResults result, string text = null)
        {
            NativeResult = result;
            CustomMessage = text;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            b.Append("Parser.ParseResult (");
            b.Append(NativeResult.ToString());
            b.Append(CustomMessage != null ? ", " + CustomMessage : "");
            b.Append(")");

            return b.ToString();
        }
    }
    public class EParser
    {
       
        public static char[] SpacesAndTabs = { ' ', '\t' };

        // Single-command parser
        // Rules:
        // Ends with ; - is a signal where one command ends. 
        public static object Process(string Command)
        {
            /* Input examples:
               string a = "b";
               bool x = false;
               int ttt = 5555; 
               Test("a", b(5, c(true)));
               TODO:
               string b = "test";
               string a = b;
               */

            Command = Command.Trim(SpacesAndTabs); // Remove spaces and tabs at the end and start

            if (!Command.EndsWith(";"))
                throw new Exceptions.ParserException("Ending character is not found.");

            var CommandType = GetCommandType(Command);

            if (CommandType == CommandTypes.Unknown)
            {
                throw new Exceptions.ParserException("Parser can't understand the type of this command (Unknown)");
            }
            else if (CommandType == CommandTypes.VariableInitializer)
            {
                // something like: string a = "b";
                /* Rules:
                 * space must split variable type and variable name
                 * = is the signal to start write variable's value and ; is the signal to stop parsing
                 */
                var CommandSplit = Command.Split(' ');

                if (CommandSplit.Length < 1)
                    throw new Exceptions.VariableInitializerException("Variable type or variable name is not found");

                string VariableType = CommandSplit[0];
                string VariableName = "";

                string ParsingName = CommandSplit[1];
                for (int i = 0; i < ParsingName.Length; i++)
                {
                    char c = ParsingName[i];

                    if (c == ';' || c == '=')
                        break;

                    foreach (var bad in SpacesAndTabs)
                        if (c == bad)
                            break;

                    VariableName += c;
                }

                if (VariableType.Length <= 0)
                    throw new Exceptions.VariableInitializerException("Invalid variable type");

                if (VariableName.Length <= 0)
                    throw new Exceptions.VariableInitializerException("Invalid variable name");

                // Moving on, now we must check for value somehow.
                // It can be: "bool a;" or "bool a = null" which is null or bool a = true;
                // So, simply read everything to end. By the rules, there are no more spaces/tabes at the start or between var name/value
                // so we can get length and start from that point
                int PositionAfterVarInit = (VariableType + " " + VariableName).Length;

                if (PositionAfterVarInit > Command.Length)
                    throw new Exceptions.VariableInitializerException("Something wrong");

                string valueString = "";

                bool writeValue = false;
                for (int i = PositionAfterVarInit; i < Command.Length; i++)
                {
                    char c = Command[i];

                    if (c == ';')
                        break; // stop at ;


                    if (writeValue)
                    {
                        valueString += c;
                    }

                    if (c == '=')
                    {
                        if (!writeValue)
                            writeValue = true;
                        else
                            throw new Exceptions.VariableInitializerException("Unexpected '='");
                    }
                }

                valueString = valueString.Trim(SpacesAndTabs);

                var type = GetCommandType(valueString);

                if (IsCommandTypeCompatibleWith(type, CommandTypes.Method))
                {
                    // something like: string a = Return("a");
                    var invokeType = GetCompatibleTypeByName(VariableType);
                    if (VariableType != "var" && invokeType == null)
                        throw new Exceptions.VariableSetException($"Type {VariableType} is not compatible with ESCRIPT");

                    var invokeResult = Cmd.Process(valueString + ";", false);
                    if (invokeResult != null && VariableType != "var" && invokeResult.GetType() != invokeType.Type)
                        throw new Exceptions.VariableSetException($"{invokeType.Name} expected, but {invokeResult.GetType().Name} received");

                    Variables.SetVariableObject(VariableName, invokeResult);

                    return new ParseResult(ParseResult.ParserNativeResults.ParserNativeOK, "VariableInitialzer, value is Method-compatible");
                }
                else if (type == CommandTypes.CastValue)
                {
                    // something like: IntPtr x = (IntPtr)5;
                    var invokeType = GetCompatibleTypeByName(VariableType);
                    if (VariableType != "var" && invokeType == null)
                        throw new Exceptions.VariableSetException($"Type {VariableType} is not compatible with ESCRIPT");

                    var invokeResult = ConvertStringToVar(valueString);
                    if (invokeResult != null && VariableType != "var" && invokeResult.GetType() != invokeType.Type)
                        throw new Exceptions.VariableSetException($"{invokeType.Name} expected, but {invokeResult.GetType().Name} received");

                    Variables.SetVariableObject(VariableName, invokeResult);

                    return new ParseResult(ParseResult.ParserNativeResults.ParserNativeOK, "VariableInitialzer, value is cast");
                }
                else
                {
                    // something like: string a = "a";
                    return VariableSetValueHandler(VariableType, VariableName, valueString);
                }
            }
            else if (CommandType == CommandTypes.VariableSet)
            {
                // something like: a = true;
                /* Rules:
                 * = must split variable name and variable value
                 */

                // I'm lazy, so temporary solution is to add "var" to the start:
                return Process("var " + Command);

#if NOT_LAZY_AUTHOR // TODO
                string VariableName = Command.Split('=')[0].Trim(SpacesAndTabs);

                string valueString = "";

                bool writeValue = false;
                for (int i = VariableName.Length; i < Command.Length; i++)
                {
                    char c = Command[i];

                    if (c == ';')
                        break; // stop at ;


                    if (writeValue)
                    {
                        valueString += c;
                    }

                    if (c == '=')
                    {
                        if (!writeValue)
                            writeValue = true;
                        else
                            throw new Exceptions.VariableInitializerException("Unexpected '='");
                    }
                }

                valueString = valueString.Trim(SpacesAndTabs);

                if (valueString.Length == 0)
                    throw new Exceptions.VariableSetException("Value is not found");

                for (int i = 0; i < Variables.VarList.Count; i++)
                {
                    var var = Variables.VarList[i];
                    if (var.Name == VariableName)
                    {
                        return VariableSetValueHandler(var.Value.GetType().Name, VariableName, valueString);
                    }
                }

                throw new Exceptions.VariableSetException($"Variable {VariableName} is not found");
#endif
            }
            else if (CommandType == CommandTypes.Typeof)
            {
                // rules:
                /*
                 * arguments container with only one value which is Class name
                 * returns Type of value
                 */
                string ClassName = "";

                // Check for CAST
                string CastTo = CheckForCast(Command);

                var Pair = ConvertStringToArgumentsContainer(Command);
                ClassName = Pair.Value;

                foreach(var t in CompatibleTypes)
                {
                    if (t.Names.Contains(ClassName))
                        return t.Type;
                }

                throw new Exception($"Class '{ClassName}' not found.");
            }
            else if (CommandType == CommandTypes.Method)
            {
                // something like: Test(); Test(true); Test("a", 2); Test("a", 2, GetTrue());
                /* Rules:
                 * Arguments container must be started with ( and ended with )
                 */
                // First of all, we need to split arguments containt and method name:

                string MethodName = "";
                string ArgumentsContainer = "";

                // Check for CAST
                string CastTo = CheckForCast(Command);

                var Pair = ConvertStringToArgumentsContainer(Command);
                MethodName = Pair.Key;
                ArgumentsContainer = Pair.Value;

                if (MethodName.Length <= 0)
                    throw new Exceptions.ParserException("Invalid method name");
                

                // This method must resolve invokes inside of a container and convert it to an array
                object[] Args = StaticArgsToArray(ArgumentsContainer);

                List<object> FutureArguments = new List<object>();

                foreach (var argument in Args)
                {
                    // unknown argument type is string by default
                    if (argument != null)
                    {
                        if(argument.GetType() == typeof(string))
                            FutureArguments.Add(ConvertStringToVar((string)argument));
                        else
                            FutureArguments.Add(argument);
                    }
                    else
                        throw new NullReferenceException("Argument is null for some reason :/");
                }

                var invokeResult = Cmd.InvokeMethod(MethodName, FutureArguments.ToArray());
                if (CastTo.Length >= 1)
                {
                    if (invokeResult == null)
                        throw new NullReferenceException($"Trying to cast method's result to {CastTo}, but result is null");

                    invokeResult = ConvertStringToVar($"({CastTo}){CommandNormalizer.ConvertObjectToStringCode(invokeResult)}");
                }
                return invokeResult;
            }
            else if (CommandType == CommandTypes.UsingHeader)
            {
                string Namespace = "";
                string NamespaceOriginal = Command.Split(' ')[1].Trim(SpacesAndTabs);
                for(int i = 0; i < NamespaceOriginal.Length; i++)
                {
                    char c = NamespaceOriginal[i];

                    if (c == ';')
                        break;

                    Namespace += c;
                }

                Namespace = Namespace.Trim(SpacesAndTabs);

                if (Namespace.Length <= 0)
                    throw new Exceptions.ParserException($"Namespace is invalid");

                EnvironmentManager.AddNamespace(Namespace);

                return new ParseResult(ParseResult.ParserNativeResults.ParserNativeOK, $"Namespace '{Namespace}' imported");
            }
            else if(CommandType == CommandTypes.ObjectInitializer)
            {
                /* RULES:
                   Type name is after "new " (4 len)
                   Contains argument container!
                */

                string TypeName = "";
                string ArgumentsContainer = "";

                // Check for CAST
                string CastTo = CheckForCast(Command);

                var Pair = ConvertStringToArgumentsContainer(Command);
                TypeName = Pair.Key.Remove(0, 4); // remove from string starting "new " (4 chars)
                ArgumentsContainer = Pair.Value;

                if (TypeName.Length <= 0)
                    throw new Exceptions.ParserException("Invalid type name");

                // This method must resolve invokes inside of a container and convert it to an array
                object[] Args = StaticArgsToArray(ArgumentsContainer);

                List<object> FutureArguments = new List<object>();

                foreach (var argument in Args)
                {
                    // unknown argument type is string by default
                    if (argument != null)
                    {
                        if (argument.GetType() == typeof(string))
                            FutureArguments.Add(ConvertStringToVar((string)argument));
                        else
                            FutureArguments.Add(argument);
                    }
                    else
                        throw new NullReferenceException("Argument is null for some reason :/");
                }

                var invokeResult = Cmd.CreateInstance(TypeName, FutureArguments.ToArray());

                if (CastTo.Length >= 1)
                {
                    if (invokeResult == null)
                        throw new NullReferenceException($"Trying to cast {invokeResult.GetType().Name} to {CastTo}, but result is null");

                    invokeResult = ConvertStringToVar($"({CastTo}){CommandNormalizer.ConvertObjectToStringCode(invokeResult)}");
                }
                return invokeResult;
            }
            else
                throw new NotImplementedException("Type " + CommandType.GetType().Name + "." + CommandType.ToString() + " is not implemented!!!");

        }
        
        public static string CheckForCast(string Command)
        {
            // Check for CAST
            string CastTo = "";

            if (Command.StartsWith("("))
            {
                int EndCastPosition = -1;
                for (int i = 1; i < Command.Length; i++)
                {
                    char c = Command[i];

                    if (c == ')')
                    {
                        EndCastPosition = i;
                        break;
                    }

                    CastTo += c;
                }

                if (EndCastPosition == -1)
                    throw new Exceptions.ParserException("The cast in this command is invalid");

                CastTo = CastTo.Trim(SpacesAndTabs);

                Command = Command.Remove(0, EndCastPosition + 1);
                Command = Command.Trim(SpacesAndTabs);
            }

            return CastTo;
        }

        public static KeyValuePair<string, string> ConvertStringToArgumentsContainer(string Command, int startPosition = 0)
        {
            string TypeName = "";
            string ArgumentsContainer = "";

            bool ParsingArgs = false;
            CodeIgnoreType IgnoreType = CodeIgnoreType.None;

            for (int i = 0; i < Command.Length; i++)
            {
                char c = Command[i];

                if (!ParsingArgs)
                {
                    if (c == '(')
                    {
                        ParsingArgs = true;
                        IgnoreType = CodeIgnoreType.None;
                        continue;
                    }

                    if (c == ';')
                        break;

                    TypeName += c;
                }
                else
                {
                    if (c == ';' && IgnoreType == CodeIgnoreType.None)
                    {
                        break;
                    }

                    IgnoreType = CheckForIgnoreType(IgnoreType, c, i);

                    ArgumentsContainer += c;
                }
            }

            TypeName = TypeName.Trim(SpacesAndTabs);
            ArgumentsContainer = ArgumentsContainer.Trim(SpacesAndTabs);


            if (ArgumentsContainer[ArgumentsContainer.Length - 1] != ')')
                throw new Exceptions.ParserException("Arguments container is invalid. Ending symbol is not found.");
            else
                ArgumentsContainer = ArgumentsContainer.Remove(ArgumentsContainer.Length - 1, 1);

            return new KeyValuePair<string, string>(TypeName, ArgumentsContainer);
        }
        
        public static object ConvertStringToNumber(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Remove(0, 2);

                if (int.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out int r))
                    return r;

                if (long.TryParse(str, System.Globalization.NumberStyles.HexNumber, null, out long l))
                    return l;
            }
            else
            {
                if (int.TryParse(str, out int r))
                    return r;

                if (long.TryParse(str, out long l))
                    return l;
            }
            return null;
        }

        public static object ConvertStringToVar(ETypeInfo FutureType, string VariableValue)
        {
            object FutureValue = null;
            if (FutureType.Type == typeof(char))
            {
                VariableValue = VariableValue.Trim(new char[] { "'"[0] });

                FutureValue = VariableValue[0];
            }
            else if (FutureType.Type == typeof(string))
            {
                VariableValue = VariableValue.Trim(new char[] { '"' });

                FutureValue = VariableValue;
            }
            else if (FutureType.Type == typeof(object))
            {
                FutureValue = ConvertStringToVar(VariableValue);
            }
            else if (FutureType.Type == typeof(bool))
            {
                FutureValue = bool.Parse(VariableValue);
            }
            else if (FutureType.Type == typeof(int))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = int.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(long))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = long.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(ulong))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = ulong.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(uint))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = uint.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(ushort))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = ushort.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(float))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = float.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(double))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = double.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(IntPtr))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = (IntPtr)int.Parse(VariableValue, NumberStyle);
            }
            else if (FutureType.Type == typeof(UIntPtr))
            {
                var NumberStyle = System.Globalization.NumberStyles.Any;
                if (VariableValue.StartsWith("0x"))
                {
                    VariableValue = VariableValue.Remove(0, 2);
                    NumberStyle = System.Globalization.NumberStyles.HexNumber;
                }
                FutureValue = (UIntPtr)uint.Parse(VariableValue, NumberStyle);
            }
            else
            {
                throw new NotImplementedException($"Type {FutureType.Type.Name} is not implemented in ESCRIPT");
            }
            return FutureValue;
        }

        public static object ConvertStringToVar(string str)
        {
            if(str.StartsWith("(")) // it's a cast
            {
                string CastTo = "";
                string Value = "";
                bool IsCastNow = true;
                for (int i = 1; i < str.Length; i++)
                {
                    if (str[i] == ')')
                    {
                        IsCastNow = false;
                        continue;
                    }

                    if(IsCastNow)
                        CastTo += str[i];
                    else
                        Value += str[i];
                }

                ETypeInfo FutureType = GetCompatibleTypeByName(CastTo);
                
                if (FutureType == null)
                    throw new Exceptions.ParserException($"Unknown variable type: {CastTo}");

                var castResult = ConvertStringToVar(FutureType, Value);

                if (castResult == null)
                    throw new Exceptions.ParserException($"Can't cast '{Value}' to {CastTo})");
                else
                    return castResult;

            }

            if (str == "null")
                return null;

            var var = Variables.Get(str);
            if (var == null || var.GetType() != typeof(VNull))
                return var;

            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                // FIX ME: it will remove all " chars even like """"test"""" will become just test
                return str.Trim(new char[] { '"' });
            }
            else if (str.StartsWith("'") && str.EndsWith("'"))
            {
                // FIX ME too
                return str.Trim(new char[] { "'"[0] })[0];
            }
            else if (str == "false" || str == "true")
            {
                return bool.Parse(str);
            }
            else if (str.StartsWith("0x") || str.Any(char.IsDigit))
            {
                var r = ConvertStringToNumber(str);
                if (r != null)
                    return r;
            }
            else
            {
            }

            // well i tried everything but...
            throw new NotImplementedException("Parser can't detect variable's type");
        }

        private static object[] StaticArgsToArray(string ArgumentsContainer)
        {
            // RULE: Ignore parser symbols in strings
            // Input: a,bc,";c:", d, Kek()
            // Output: [0] a
            //         [1] bc
            //         [2] ";c:"
            //         ...

            List<object> result = new List<object>();
            bool InsideInvoke = false;
            CodeIgnoreType IgnoreType = CodeIgnoreType.None;
            string CurrentArg = "";

            for (int i = 0; i < ArgumentsContainer.Length; i++)
            {
                char c = ArgumentsContainer[i];

                IgnoreType = CheckForIgnoreType(IgnoreType, c, i);

                if (c == '(' && IgnoreType == CodeIgnoreType.None)
                    InsideInvoke = true;

                if (c == ')' && IgnoreType == CodeIgnoreType.None)
                    InsideInvoke = false;

                CurrentArg += c;

                if (IgnoreType == CodeIgnoreType.None && !InsideInvoke)
                {
                    if (c == ',' || i == ArgumentsContainer.Length - 1) // command end/split thing
                    {
                        result.Add(CurrentArg.TrimEnd(new char[] { ',' }).Trim(SpacesAndTabs).TrimEnd(new char[] { ',' }));
                        CurrentArg = ""; // new command;
                    }
                }

            }

            for(int i = 0; i < result.Count; i++)
            {
                var a = result[i];
                var type = GetCommandType((string)a);
                if (IsCommandTypeCompatibleWith(type, CommandTypes.Method))
                {
                    var invokeResult = Cmd.Process(((string)a) + ";", false);

                    if (invokeResult != null)
                    {
                        if (invokeResult.GetType() == typeof(string))
                            invokeResult = "\"" + invokeResult.ToString() + "\"";
                    }

                    result[i] = invokeResult;
                    /*
                    string varName = GlobalVars.RandomString(15);

                    Variables.SetVariableObject(varName, invokeResult, new List<string>() { "Hidden" });
                    result[i] = varName;
                    
                    ParserDebug.Log("Arg-invoke variable: " + varName);
                    if (invokeResult != null)
                        ParserDebug.Log($"              Value: ({invokeResult.GetType().Name}) " + invokeResult.ToString());
                        */
                }
                else if (type == CommandTypes.CastValue)
                {
                    result[i] = ConvertStringToVar((string)a);
                }
            }

            return result.ToArray();
        }

        private static ParseResult VariableSetValueHandler(string VariableType, string VariableName, string VariableValue)
        {
            object FutureValue = null;
            ETypeInfo FutureType = GetCompatibleTypeByName(VariableType);

            if (VariableType == "var")
            {
                Variables.SetVariableObject(VariableName, ConvertStringToVar(VariableValue));
                return new ParseResult(ParseResult.ParserNativeResults.ParserNativeOK, "VariableSetValueHandler, variable is auto");
            }

            if (FutureType == null)
                throw new Exceptions.ParserException($"Unknown variable type ({VariableName})");

            if (VariableValue.Length == 0)
            {
                // It's something like "bool a;", so the value is null
                FutureValue = null;

                // But what if the variable is non nullable!??!?!?!?!
                // Well, C# should throw an exception, yes?
                Variables.SetVariableObject(VariableName, FutureValue);

                // NICE>
            }
            else
            {
                for (int i = 0; i < Variables.VarList.Count; i++)
                {
                    var var = Variables.VarList[i];
                    if (var.Name == VariableValue)
                    {
                        if (var.Value != null && var.Value.GetType() != FutureType.Type && FutureType.Type != typeof(object))
                            throw new Exceptions.VariableSetException($"{var.Name}'s type is {var.Value.GetType().Name}, but you are trying to set it to {FutureType.Type.Name}");

                        FutureValue = var.Value;
                        goto finishWithValue;
                    }
                }

                // Check for values
                if (VariableValue == "null") // it should think like command was: bool a;
                    return VariableSetValueHandler(VariableType, VariableName, "");

                FutureValue = ConvertStringToVar(FutureType, VariableValue);

                finishWithValue:
                Variables.SetVariableObject(VariableName, FutureValue);
            }

            return new ParseResult(ParseResult.ParserNativeResults.ParserNativeOK, "VariableSetValueHandler done");
        }

    }
}
