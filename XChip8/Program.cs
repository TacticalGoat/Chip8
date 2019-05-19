using System;
using XChip8.Systems;
using XChip8.Emulators;
using XChip8.Renderers;
using XChip8.Input;

namespace XChip8
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var rend = new Renderer();
            var inp = new Input.Input();
            var emu = new Chip8(rend, inp);
            var system = new XChip8.Systems.System(emu);
            system.Start();
            // Console.ReadLine();
        }
    }
}
