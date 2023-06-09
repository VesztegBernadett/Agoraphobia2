﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Agoraphobia.Entity;

namespace Agoraphobia.Rooms
{
    internal class Room : IRoom
    {
        private readonly int id;
        public int Id { get => id; }
        private readonly string name;
        public string Name { get => name; }
        private readonly string description;
        public string Description { get => description; }
        public int NPC { get; private set; }
        public int Enemy { get; private set; }
        public List<int> Items { get; private set; }
        public List<int> Exits { get; private set; } = new List<int>(3);
        public int ItemsNum { get; set; }
        public IRoom.Orientation Orientation { get; private set; }
        public bool IsQuest { get; private set; }
        public string View()
        {
            return $"";
        }
        public Room (int id, string name, string desc, bool type, int orientation, int npc, int enemy, List<int> items)
        {
            this.id = id;
            this.name = name;
            description = desc;
            IsQuest = type;
            Orientation = (IRoom.Orientation)orientation;
            NPC = npc;
            Items = items;
            Enemy = enemy;
            ItemsNum = items.Count;
            IRoom.Rooms.Add(this);
        }

        public void RemoveItem(int item)
        {
            Items.Remove(item);
        }

        public void AddItem(int item)
        {
            Items.Add(item);
        }

        public void RemoveEnemy()
        {
            Enemy = 0;
        }

        public void AddEnemy(int enemy)
        {
            Enemy = enemy;
        }

        public void RemoveNPC()
        {
            NPC = 0;
        }

        public void AddNPC(int npc)
        {
            NPC = npc;
        }
    }
}
