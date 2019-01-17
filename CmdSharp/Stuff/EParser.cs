/*
 ESCRIPT Project - powerful command interpreter
 File:        EParser.cs
 Description: Command parser (part of command processor)
 Author:      osdever (Nikita Ivanov)
*/

using escript.Stuff.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace escript
{
    enum EVariableType
    {
        E_STRING = 0,
        E_BOOL = 1,
    };

    class BaseToken
    {
        //abstract public bool Read(Stream s);
    };

    class EVariableToken : BaseToken
    {
        public string name;
        public object value;
        public EVariableType type;
    }

    class EBool : EVariableToken
    {
        public void Read(Stream s)
        {
            char c = '\0';

            string stringValue = "";
            while (c != ';') //читаем до ;
            {
                c = s.ReadChar();

                if (c == '\0')
                    return;
                if (c == ';')
                {
                    s.Seek(-1, SeekOrigin.Current); //return the semicolon in the stream
                    break;
                }
                stringValue += c;
            }

            if (stringValue == "true")
                value = true;
            else if (stringValue == "false")
                value = false;
            else throw new Exception("Boolean parsing failed: unknown value '" + stringValue + "'");
        }
    }
    class EVariableDef : EVariableToken
    {
        public EVariableDef(EVariableType t)
        {
            type = t;
        }
        public void Read(Stream s)
        {
            char c = s.ReadChar();

            if (c == '\0')
                return;

            while (c == ' ' || c == '\t')
            { //ищем имя
                c = s.ReadChar();

                if (c == '\0')
                    return;
            }

            s.Seek(-1, SeekOrigin.Current);

            c = s.ReadChar();
            s.Seek(-1, SeekOrigin.Current);

            while (c != ' ' && c != '\t' && c != '=') //читаем имя
            {
                c = s.ReadChar();

                if (c == '\0')
                    return;
                if (c != ' ' && c != '\t' && c != '=')
                    name += c;
            }

            if (c != '=')
            {
                while (c == ' ' || c == '\t') //читаем имя
                {
                    c = s.ReadChar();

                    if (c == '\0')
                        throw new Exception("Variable definition parsing failed: EOF while looking for '='");
                }
            }
            else s.Seek(-1, SeekOrigin.Current);
            if (c != '=')
                throw new Exception("Variable definition parsing failed: expected '=', got '" + c + "'");

            c = s.ReadChar();

            if (c == ' ' || c == '\t')
            {
                while (c == ' ' || c == '\t')
                { //ищем начало значения
                    c = s.ReadChar();

                    if (c == '\0')
                        throw new Exception("Variable definition parsing failed: EOF while looking for the value");
                }
                s.Seek(-1, SeekOrigin.Current);
            }

            //s.Seek(-2, SeekOrigin.Current);
            switch (type)
            {
                case EVariableType.E_STRING:
                    {
                        var str = new EString();
                        str.Read(s);

                        value = str.value;

                        break;
                    }
                case EVariableType.E_BOOL:
                    {
                        var str = new EBool();
                        str.Read(s);

                        value = str.value;

                        break;
                    }
                default:
                    throw new Exception("Variable definition parsing failed: unknown variable type");
            }

        }
    }
    class EString : EVariableToken
    {
        public void Read(Stream s)
        {
            value = "";

            char c = s.ReadChar();

            if (c == '\0')
                return;

            while (c != '\"') //ищем открывающую кавычку
            {
                c = s.ReadChar();

                if (c == '\0')
                    return;
            }

            //c = s.ReadChar();

            while (true) //читаем строку до закрывающей кавычки
            {
                c = s.ReadChar();

                if (c == '\0' || c == '\"')
                    return;

                if (c != '"')
                    value += c.ToString();
            }
        }
    }
    class EParser
    {
        public static bool ReadToken(Stream s, ESCode code)
        {
            string str = "";

            while (true)
            {
                char c = s.ReadChar();

                if (c == '\0')
                    return false;

                if (c == ' ')
                    break;

                if (c == ';')
                    return true; //прочитано сука

                str += c;
            }

            if (str == "string")
            {
                var token = new EVariableDef(EVariableType.E_STRING);
                token.Read(s);
                Variables.SetVariableObject(token.name, (string)token.value);
            }
            if (str == "bool")
            {
                var token = new EVariableDef(EVariableType.E_BOOL);
                token.Read(s);
                Variables.SetVariableObject(token.name, (bool)token.value);
            }
            return ReadToken(s, code);
        }
    }
}
