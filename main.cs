using System;
using SupaStuff.Net;
using SupaStuff.Net.ServerSide;
using SupaStuff.Util;
using System.Reflection;


class Program {
  public static void Main (string[] args) {
    Console.WriteLine ("Hello World");
    new SupaStuff.Net.Example.ExampleDemo();
    //ConventionChecker conventionChecker = new ConventionChecker();
    //conventionChecker.Check(Assembly.GetExecutingAssembly());
    //conventionChecker.WriteErrors();
  }
}