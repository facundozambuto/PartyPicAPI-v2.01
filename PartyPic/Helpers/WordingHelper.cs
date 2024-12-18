﻿using System;
using System.Linq;

namespace PartyPic.Helpers
{
    public static class WordingHelper
    {
        public static string FirstCharToUpper(this string input) =>
                input switch
                {
                    null => throw new ArgumentNullException(nameof(input)),
                    "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                    _ => input.First().ToString().ToUpper() + input.Substring(1)
                };
        
    }
}
