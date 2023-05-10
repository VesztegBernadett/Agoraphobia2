﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agoraphobia.Rooms;
using Agoraphobia.Entity;
using Agoraphobia.Items;
using System.IO;
using System.ComponentModel;
using System.Threading;

namespace Agoraphobia
{
    class Viewport
    {
        private static void ShowSingle(string art, int[] coordinates)
        {
            //Its an universal Show method so we don't need it for each class
            //New interface IArtist contains the arts so we can now access all the showable elements by IArtist
            if (art is not null)
            {
                List<string> rows = art.Split('\n').ToList();
                for (int i = 0; i < rows.Count(); i++)
                {
                    Console.SetCursorPosition(coordinates[0], coordinates[1] + i);
                    Console.Write(rows[i]);
                }
            }
            else
            {
                Console.Write("\n");
            }
        }

        public static void Show(int roomId)
        {
            Room room = (Room)IRoom.Rooms.Find(x => x.Id == roomId);

            //NPCs
            ShowSingle(File.ReadAllText($"{IElement.PATH}Arts/Placeholder.txt"), INPC.Coordinates);
            if (room.NPC != 0)
            {
                //Id starts with 1
                NPC npc = (NPC)INPC.NPCs.Find(x => x.Id == room.NPC);
                ShowSingle(npc.Art, INPC.Coordinates);
            }
            //Enemies
            ShowSingle(File.ReadAllText($"{IElement.PATH}Arts/Placeholder.txt"), IEnemy.Coordinates);
            if (room.Enemy != 0)
            {
                //Id starts with 1
                Enemy enemy = (Enemy)IEnemy.Enemies.Find(x => x.Id == room.Enemy);
                ShowSingle(enemy.Art, IEnemy.Coordinates);
            }
            //Items
            ShowSingle(File.ReadAllText($"{IElement.PATH}Arts/Placeholder.txt"), IItem.Coordinates);
            if (room.Items.Count > 0)
            {
                //Id starts with 0
                if (room.Items.Count == 1)
                {
                    IItem item = IItem.Items.Find(x => x.Id == room.Items[0]);
                    ShowSingle(item.Art, IItem.Coordinates);
                }
                else
                {
                    string sack = File.ReadAllText($"{IElement.PATH}Arts/IArt.txt");
                    string[] lines = sack.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        Console.SetCursorPosition(IItem.Coordinates[0], IItem.Coordinates[1] + i);
                        Console.Write(lines[i]);
                    }
                }
            }
            ShowSingle(File.ReadAllText($"{IElement.PATH}Arts/Book.txt"), new int[] { 2, 14 });
        }

        public static void ShowGrid()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            for (int i = 0; i < 200; i++)
            {
                Console.SetCursorPosition(i, 23);
                Console.Write("_");
            }
            for (int i = 0; i < 45; i++)
            {
                Console.SetCursorPosition(120, i);
                Console.Write("|");
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void ShowStats()
        {
            Console.SetCursorPosition(123, 25);
            Console.Write("Statistics:");
            Console.SetCursorPosition(125, 26);
            Console.Write($"Sanity: {Player.Sanity} / 100      ");
            Console.SetCursorPosition(125, 27);
            Console.Write($"HP: {Player.HP} / {Player.MaxHP}    ");
            Console.SetCursorPosition(125, 28);
            Console.Write($"Attack: {Player.AttackDamage}      ");
            Console.SetCursorPosition(125, 29);
            Console.Write($"Defense: {Player.Defense}      ");
            Console.SetCursorPosition(125, 30);
            Console.Write($"Energy: {Player.Energy} / {Player.MAXENERGY}       ");
            Console.SetCursorPosition(125, 31);
            Console.Write($"DreamCoins: {Player.DreamCoins}       ");
        }

        public static void ShowInventory(int id)
        {
            Console.SetCursorPosition(157, 0);
            Console.Write("Inventory");
            int vOffset = 0;
            for (int i = 0; i < Player.Inventory.Count; i++)
            {
                if (id == i)
                    Console.BackgroundColor = ConsoleColor.Magenta;
                Console.SetCursorPosition(125, 2 + i);
                Console.Write(IItem.Items.Find(x => x.Id == Player.Inventory[i]).Name);
                Console.BackgroundColor = ConsoleColor.Black;
            }
            Console.SetCursorPosition(148, 22);
            Console.Write("Hover inspect | Enter use | Backslash Drop");
        }
        private static void ShowDescription(ref int vOffset, string text, int limit, int start)
        {
            string[] words = text.Split(' ');
            int current = 0;
            vOffset++;
            for (int i = 0; i < words.Length; i++)
            {
                if (current + words[i].Length >= limit)
                {
                    vOffset++;
                    current = 0;
                }
                Console.SetCursorPosition(start + current, 24 + vOffset);
                current += words[i].Length + 1;
                Console.Write($"{words[i]} ");
            }
        }
        public static void Interaction(int roomId, int id, bool isOpened, bool isTriggered)
        {
            Room room = (Room)IRoom.Rooms.Find(x => x.Id == roomId);
            int selected = 0;
            int vOffset = 0;
            ShowRoomInfo(ref vOffset, room);
            vOffset++;
            if (room.NPC != 0)
            {
                vOffset++;
                NPC npc = (NPC)INPC.NPCs.Find(x => x.Id == room.NPC);
                ShowOption(ref selected, id, 0, 0 + vOffset, npc.Art);
                Console.Write($">> Interact with: {npc.Name}        ");
                Console.BackgroundColor = ConsoleColor.Black;
                ShowDescription(ref vOffset, npc.Description, 60, 15);
            }
            if (room.Enemy != 0)
            {
                vOffset++;
                Enemy enemy = (Enemy)IEnemy.Enemies.Find(x => x.Id == room.Enemy);
                ShowOption(ref selected, id, 0, 0 + vOffset, enemy.Art);
                Console.Write($">> Fight: {enemy.Name}           ");
                Console.BackgroundColor = ConsoleColor.Black;
                ShowDescription(ref vOffset, enemy.Description, 60, 15);
            }
            else
            {
                vOffset++;
                if (isTriggered)
                {
                    Console.SetCursorPosition(10, 24 + vOffset);
                    Console.Write(">> Exits:         ");
                    for (int i = 0; i < room.Exits.Count; i++)
                    {
                        vOffset++;
                        IRoom current = IRoom.Rooms.Find(x => x.Id == room.Exits[i]);
                        ShowOption(ref selected, id, 0, 0 + vOffset, null);
                        Console.Write($"  >> Go to: {current.Name}");
                        Console.BackgroundColor = ConsoleColor.Black;
                        ShowDescription(ref vOffset, current.Description, 60, 15);
                    }
                }
                else
                {
                    ShowOption(ref selected, id, 0, 0 + vOffset, File.ReadAllText($"{IElement.PATH}Arts/Exit.txt"));
                    Console.Write($">> Exit room");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
            vOffset++;
            if (room.Items.Count > 1)
            {
                if (isOpened)
                {
                    Console.SetCursorPosition(10, 24 + vOffset);
                    Console.Write(">> Sack:                     ");
                    for (int i = 0; i < room.Items.Count(); i++)
                    {
                        vOffset++;
                        IItem item = IItem.Items.Find(x => x.Id == room.Items[i]);
                        ShowOption(ref selected, id, 0, 0 + vOffset, item.Art);
                        Console.Write($"  >> Pick up {item.Name}          ");
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                }
                else
                {
                    ShowOption(ref selected, id, 0, 0 + vOffset, File.ReadAllText($"{IElement.PATH}Arts/IArt.txt"));
                    Console.Write(">> Inspect Sack...       ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
            else if (room.Items.Count == 1)
            {
                IItem item = IItem.Items.Find(x => x.Id == room.Items[0]);
                ShowOption(ref selected, id, -2, 0 + vOffset, item.Art);
                Console.Write($"  >> Pick up {item.Name}          ");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            
        }
        public static void Shop(int id)
        {
            int current = 0;
            int selected = 0;
            NPC npc = (NPC)INPC.NPCs.Find(x => x.Id == id);
            int length = npc.Inventory.Count + 1;
            ClearInteraction();
            ChooseItem(id, current, selected);

            while (true)
            {
                ConsoleKey input = Console.ReadKey(true).Key;
                switch (input)
                {
                    case ConsoleKey.UpArrow:
                        if (selected == 0)
                            selected = length - 1;
                        else selected--;
                        ChooseItem(id, current, selected);
                        break;
                    case ConsoleKey.DownArrow:
                        if (selected == length - 1)
                            selected = 0;
                        else selected++;
                        ChooseItem(id, current, selected);
                        break;
                    case ConsoleKey.Enter:
                        if (selected == length - 1)
                            Program.MainScene();
                        else
                        {
                            IItem item = IItem.Items.Find(x => x.Id == npc.Inventory[selected]);
                            //Console.Write(selected);
                            //Console.ReadKey();
                            if (Player.ChangeCoins(-item.Price))
                            {
                                length--;
                                npc.Inventory.Remove(item.Id);
                                Player.Inventory.Add(item.Id);
                                ShowInventory(0);
                                ShowStats();
                                ClearInteraction();
                                ChooseItem(id, current, selected);
                            }
                            else
                            {
                                Console.SetCursorPosition(10, 27 + length);
                                Console.Write("Not enough DreamCoins!     ");
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private static void ChooseItem(int id, int current, int selected)
        {
            NPC npc = (NPC)INPC.NPCs.Find(x => x.Id == id);
            int length = npc.Inventory.Count;
            if (npc.Inventory.Count() > 0)
            {
                List<int> items = npc.Inventory;
                for (int i = 0; i < items.Count; i++)
                {
                    IItem item = IItem.Items.Find(x => x.Id == items[i]);
                    ShowOption(ref current, selected, 0, current + 1, item.Art);
                    Console.Write($">> Purchase: {item.Name} - {item.Price} DC           ");
                    Console.BackgroundColor = ConsoleColor.Black;
                }
            }
            else
            {
                Console.SetCursorPosition(10, 24);
                Console.WriteLine("You can't buy anything from this creature.");
            }
            ShowOption(ref current, selected, 0, current + 2, File.ReadAllText($"{IElement.PATH}Arts/Placeholder.txt"));
            Console.Write(">> Exit             ");
            Console.BackgroundColor = ConsoleColor.Black;
        }
        private static void ShowRoomInfo (ref int vOffset, Room room)
        {
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.SetCursorPosition((120 - room.Name.Length) / 2, 0);
            Console.Write(room.Name);
            Console.BackgroundColor = ConsoleColor.Black;
            ShowDescription(ref vOffset, room.Description, 110, 5);
        }

        private static void ShowOption(ref int selected, int id, int hOffset, int vOffset, string art)
        {
            if (selected == id)
            {
                int[] coordinates = new int[] { 80, 32 };
                ShowSingle(File.ReadAllText($"{IElement.PATH}Arts/Placeholder.txt"), coordinates);
                ShowSingle(art, coordinates);
                Console.BackgroundColor = ConsoleColor.Magenta;
            }
            Console.SetCursorPosition(10 + hOffset, 24 + vOffset);
            selected++;
        }
        public static void Message(string msg)
        {
            ClearInteraction();
            Console.SetCursorPosition(5, 25);
            Console.Write(msg+"\n     Press any key to dream on.");
            Console.ReadKey();
        }

        public static void ClearInteraction()
        {
            for (int i = 0; i < 110; i++)
            {
                for (int a = 0; a < 20; a++)
                {
                    Console.SetCursorPosition(5 + i, 25 + a);
                    Console.Write(" ");
                }
            }
        }

        public static void ShowItemInfo(int SelectedItemNumber)
        {
            int selecteditemnumber = SelectedItemNumber;
            Console.SetCursorPosition(5, 25);
            IItem selectedItem = IItem.Items.Find(x => x.Id == Player.Inventory[selecteditemnumber]);
            if (selectedItem.GetType().ToString() == "Agoraphobia.Items.Weapon")
            {
                Weapon selectedWeapon = (Weapon)selectedItem;
                Console.Write($"Weapon multiplier: {selectedWeapon.MinMultiplier}-{selectedWeapon.MaxMultiplier} Potential damage: {selectedWeapon.MinMultiplier*Player.AttackDamage}-{selectedWeapon.MaxMultiplier*Player.AttackDamage}");
            }
            else if (selectedItem.GetType().ToString() == "Agoraphobia.Items.Consumable")
            {
                Consumable selectedConsumable = (Consumable)selectedItem;
                Console.Write($"Consumable replenishes: Energy: {selectedConsumable.Energy}, HP: {selectedConsumable.HP} and it adds: Defense: {selectedConsumable.Armor}, Attack {selectedConsumable.Attack} and it lasts for {selectedConsumable.Duration} rounds");
            }
        }
    }
}
