using System;
using Xunit;

namespace XChip8.UnitTests.Emulators
{
    public class LDTest : Base
    {
        [Fact]
        public void InlineLD()
        {
            var x = rand.Next(0x0, 0xE);
            var _byte = (byte) rand.Next();
            var instr = (ushort) ((6 << 12) | (x << 8) | _byte);
            _chip8.LD(instr);
            Assert.Equal(_chip8.V[x], _byte);
        }

        [Fact]
        public void LD()
        {
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var instr = (ushort) ((8 << 12) | (x << 8) | (y << 4));
            _chip8.LD(instr);
            Assert.Equal(_chip8.V[x], _chip8.V[y]);
        }
    }
}