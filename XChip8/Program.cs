using System;
using XChip8.Systems;
using XChip8.Emulators;
using XChip8.Renderers;

namespace XChip8
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var emu = new Chip8();
            var rend = new Renderer();
            var system = new XChip8.Systems.System(emu, rend);
            system.test_font();
            Console.ReadLine();
        }
    }
}
