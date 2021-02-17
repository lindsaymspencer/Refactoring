using Newtonsoft.Json;
using Refactoring.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Refactoring.Services;

namespace Refactoring
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var tempPlays = JsonConvert.DeserializeObject<Plays>(File.ReadAllText(@"external\plays.json"));
            var invoices = JsonConvert.DeserializeObject<Invoice[]>(File.ReadAllText(@"external\invoices.json"));
            Console.Write(Statement.PlainTextStatement(invoices[0], tempPlays));
        }
    }
}
