using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdSharp
{
    public class EVariable
    {
        public string Name = "null";
        public object Value = "null";
        public object ValueSafe
        {
            get
            {
                if (Value == null)
                    return "null";
                else
                    return Value.ToString();
            }
        }

        public List<string> Options = new List<string>();

        public EVariable(string n, object v, string[] options = null)
        {
            Name = n;
            if (options != null) Options = options.ToList();
            Edit(v);
        }

        public EVariable(string n, object v, List<string> options = null)
        {
            Name = n;
            if (options != null) Options = options;
            Edit(v);
        }

        public void Edit(object v, List<string> newOptions = null)
        {
            Value = v;
            if (newOptions != null) Options = newOptions;
        }
    }

    public class EList
    {
        public List<object> Content = new List<object>();
        public int DefaultIndex = -1;
        public override string ToString()
        {
            try
            {
                StringBuilder a = new StringBuilder();
                if (DefaultIndex >= 0)
                {
                    a.Append(Content[DefaultIndex].ToString());
                }
                else if (DefaultIndex == -1)
                {
                    a.Append(String.Format("List[{0}] {1} ", Content.Count, "{"));
                    for (int i = 0; i < Content.Count; i++)
                    {
                        a.Append(i + ":\"" + Content[i].ToString() + "\" ");
                    }
                    a.Append("}");
                }
                else if (DefaultIndex == -2)
                {
                    a.Append(Content[Content.Count-1].ToString());
                }
                return a.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }

    public class EDictionary
    {
        public Dictionary<object, object> Content = new Dictionary<object, object>();
        public override string ToString()
        {
            try
            {
                StringBuilder a = new StringBuilder();
                //if (DefaultIndex >= 0)
                //{
                //    a.Append(Content[DefaultIndex].ToString());
                //}
                //else if (DefaultIndex == -1)
                //{
                    a.Append(String.Format("Dictionary[{0}] {1} ", Content.Count, "{"));
                    foreach (var entry in Content)
                    {
                        a.Append($"\"{entry.Key.ToString()}\":\"{entry.Value.ToString()}\" ");
                    }
                    a.Append("}");
                //}
                //else if (DefaultIndex == -2)
                //{
                //    a.Append(Content[Content.Count - 1].ToString());
                //}
                return a.ToString();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
