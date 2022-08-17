using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Text;



namespace SupaStuff.Util
{
    public class ConventionChecker
    {
        List<string> _problems;
        private int _problemCount = 0;
        public static Logger Logger = Logger.GetLogger("Convention Checker");
        const BindingFlags s_allFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public void Check(Assembly assembly)
        {
            _problems = new List<string>(1024);
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (type.Name.StartsWith("<")) continue;
                if (!char.IsUpper(type.Name[0]) && !type.IsInterface)
                {
                    AddProblem($"[CLASS NAME INVALID] Invalid class name: {type.FullName} starts with a lowercase letter: {type.Name}");
                }
                else if (type.IsInterface && !(type.Name[0] == 'I' && char.IsUpper(type.Name[0])))
                {
                    AddProblem($"[INTERFACE NAME INVALID] Invalid interface name: {type.FullName} should start with \"I[lower case letter]\"");

                }
                CheckMethods(type);
                CheckFields(type);
                CheckProperties(type);
            }




        }
        public void AddProblem(string problem)
        {
            //if(!problem.StartsWith("[<TYPE> NAME INVALID]")) return;
            _problemCount++;
            if (_problems.Count == _problems.Capacity - 1) return;
            _problems.Add(problem);
        }
        public void LogErrors()
        {
            foreach (string problem in _problems)
            {
                Logger.Log(problem);
            }
            Logger.Log($"Logged {_problems.Count} out of the {_problemCount} problems.");
        }
        internal void WriteErrors()
        {
            if (_problems.Count == 0) return;
            File.WriteAllLines("problems.txt", _problems);
        }
        public void CheckProperties(Type type)
        {
            PropertyInfo[] properties = type.GetProperties(s_allFlags);
            foreach (PropertyInfo property in properties)
            {
                if (property.DeclaringType != type) return;
                bool firstLetterUpper = char.IsUpper(property.Name[0]);
                bool isStatic = false;
                bool isPublic = false;
                MethodInfo setMethod = property.GetSetMethod(true);
                MethodInfo getMethod = property.GetSetMethod(true);
                if (setMethod != null)
                {
                    isStatic = setMethod.IsStatic;
                    isPublic = setMethod.IsPublic;
                }
                if (getMethod != null)
                {
                    if (getMethod.IsStatic) isStatic = true;
                    if (getMethod.IsPublic) isPublic = true;
                }
                if (isPublic)
                {
                    if (!firstLetterUpper)
                        AddProblem($"[PROPERTY NAME INVALID] Property {property.Name} in {type.FullName} does not begin with an uppercase letter like it should!");
                }
                else
                {
                    if (isStatic)
                    {
                        if (!(property.Name.StartsWith("s_") && char.IsLower(property.Name[2])))
                        {
                            AddProblem($"[PROPERTY NAME INVALID] Property {property.Name} in {type.FullName} does not begin with \"s_<lower case letter>\" like it should!");
                        }
                    }
                    else
                    {
                        if (!(property.Name[0] == '_' && char.IsLower(property.Name[1])))
                        {
                            AddProblem($"[PROPERTY NAME INVALID] Property {property.Name} in {type.FullName} does not begin with \"_<lower case letter>\" like it should!");
                        }
                    }
                }

            }
        }
        public void CheckFields(Type type)
        {
            FieldInfo[] fields = type.GetFields(s_allFlags);
            foreach (FieldInfo field in fields)
            {
                if (field.DeclaringType != type) return;
                bool firstLetterUpper = char.IsUpper(field.Name[0]);
                bool isPublic = field.IsPublic;
                bool isThreadStatic = field.GetCustomAttribute<ThreadStaticAttribute>() != null;
                bool isStatic = field.IsStatic && !isThreadStatic;
                if (isPublic)
                {
                    if (!firstLetterUpper)
                        AddProblem($"[FIELD NAME INVALID] Field {field.Name} in {type.FullName} does not begin with an uppercase letter like it should!");
                }
                else
                {
                    if (isThreadStatic)
                    {
                        if (!(field.Name.StartsWith("t_") && char.IsLower(field.Name[2])))
                        {
                            AddProblem($"[FIELD NAME INVALID] Field {field.Name} in {type.FullName} does not begin with \"t_<lower case letter>\" like it should!");
                        }
                    }
                    else if (isStatic)
                    {
                        if (!(field.Name.StartsWith("s_") && char.IsLower(field.Name[2])))
                        {
                            AddProblem($"[FIELD NAME INVALID] Field {field.Name} in {type.FullName} does not begin with \"s_<lower case letter>\" like it should!");
                        }
                    }
                    else
                    {
                        if (!(field.Name[0] == '_' && char.IsLower(field.Name[1])))
                        {
                            AddProblem($"[FIELD NAME INVALID] Field {field.Name} in {type.FullName} does not begin with \"_<lower case letter>\" like it should!");
                        }
                    }
                }
            }
        }
        public void CheckMethods(Type type)
        {
            MethodInfo[] methods = type.GetMethods(s_allFlags);
            foreach (MethodInfo method in methods)
            {
                if (method.DeclaringType != type) return;
                if (method.Name.StartsWith("get_") || method.Name.StartsWith("remove_") || method.Name.StartsWith("add_")) return;
                bool firstLetterUpper = char.IsUpper(method.Name[0]);
                bool isPublic = method.IsPublic;
                bool isStatic = method.IsStatic;
                if (!firstLetterUpper && isPublic)
                {
                    AddProblem(string.Format("[METHOD NAME INVALID] Method {0} in class {1} begins with a lower case letter even though it is public!", method.Name, type.FullName));
                }
                /*
                else if(firstLetterUpper && !isPublic) {
                  AddProblem(string.Format("[METHOD NAME INVALID] Method {0} in class {1} begins with an upper case letter even though it is private!",method.Name,type.FullName));
                }
          */
                ParameterInfo[] parameters = method.GetParameters();
                foreach (ParameterInfo parameter in parameters)
                {
                    if (!char.IsLower(parameter.Name[0]) && parameter.Name[0] != '_')
                    {
                        AddProblem(string.Format("[PARAMETER NAME INVALID] Parameter {0} in method {1} in class {2} doesn't begin with a lowercase letter!", parameter.Name, method.Name, type.FullName));
                    }
                }
            }
        }

    }
}