﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using static Agoraphobia.IItem;
using System.Collections;
using Agoraphobia.Entity;
using Agoraphobia.Rooms;

namespace Agoraphobia.Items
{
    internal class Consumable : IConsumables
    {
        private static readonly Random random = new Random();
        private readonly int id;
        public int Id { get => id; }
        private readonly string name;
        public string Name { get => name; }
        private readonly string description;

        public string Art { get; private set; }
        public string Description { get => description; }
        public int Energy { get; private set; }
        public int HP { get; private set; }
        public int Armor { get; private set; }
        public int Attack { get; private set; }
        public int Duration { get; private set; }
        public ItemRarity Rarity { get; set; }
        public Consumable(int id, string name, string desc, int energy, int hp, int attack, int armor, int duration, int rarity, int price)
        {
            int r = random.Next(0, 8);
            this.id = id;
            if (Program.newGame)
            {
                if (r == 1 && duration != 100)
                {
                    this.name = "Better " + name;
                    description = $"It's a better version of {name}. " + desc;
                    Armor = armor + 1;
                    Attack = attack + 1;
                    Energy = energy + 1;
                    HP = hp + 2;
                    Duration = duration + 1;
                    if (rarity != 6)
                        Rarity = (ItemRarity)rarity + 1;
                    else
                        Rarity = (ItemRarity)rarity;
                    Price = price + 100;
                }
                else
                {
                    this.name = name;
                    description = desc;
                    Armor = armor;
                    Attack = attack;
                    Energy = energy;
                    HP = hp;
                    Duration = duration;
                    Rarity = (ItemRarity)rarity;
                    Price = price;
                }
            }
            else
            {
                this.name = name;
                description = desc;
                Armor = armor;
                Attack = attack;
                Energy = energy;
                HP = hp;
                Duration = duration;
                Rarity = (ItemRarity)rarity;
                Price = price;
            }
            IItem.Items.Add(this);
            Art = File.ReadAllText($"{Program.PATH}/Arts/IArt{id}.txt");
        }
        public int Price { get; private set; }

        public void Obtain()
        {
            IRoom.Rooms.Find(x => x.Id == Program.room.Id).Items.Remove(Id);
            Player.Inventory.Add(Id);
        }
    }
}
