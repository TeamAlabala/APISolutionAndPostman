﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITestsGitHub
{
    public class Issue
    {
        public int id { get; set; } 
        public int number { get; set; } 
        public string title { get; set; }   
        public string body { get; set; }
    }
}
