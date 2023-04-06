﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Agoraphobia.Rooms
{
    internal class Room : IRoom
    {
        private int id;
        public int Id { get => id; }
        private string name;
        public string Name { get => name; }
        private string description;
        public string Description { get => description; }
        public List<string> Elements { get; set; }
        public int Items { get; set; }
        public List<IRoom> Exits { get; private set; }
        public bool IsQuest { get; private set; }
        public string View()
        {
            return $"";
        }
        public Room (string filename)
        {
            foreach (var line in File.ReadAllLines(filename, Encoding.UTF8))
            {
                string[] data = line.Split('#');
                switch (data[1])
                {
                    case "Id":
                        id = int.Parse(data[0]);
                        break;
                    case "Name":
                        name = data[0];
                        break;
                    case "Description":
                        description = data[0];
                        break;
                    case "Type":
                        IsQuest = int.Parse(data[0]) == 0 ? false : true;
                        break;
                    case "Orientation":
                        break;
                    default:
                        break;
                }
            }
        }
    }
}