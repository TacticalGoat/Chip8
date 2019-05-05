using Xunit;
using XChip8.Emulators;
using System;

namespace XChip8.UnitTests.Emulators
{
    public abstract class Base
    {
        public readonly Chip8 _chip8;
        protected Random rand;

        public Base()
        {
            _chip8 = new Chip8();
            rand = new Random();
        }
    }
}