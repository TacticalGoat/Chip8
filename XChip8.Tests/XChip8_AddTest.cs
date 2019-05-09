using System;
using XChip8.Emulators;
using Xunit;

namespace XChip8.UnitTests.Emulators
{
    public class AddTest : Base
    {
        [Fact]
        public void AddInline()
        {
            var x = rand.Next(0x0, 0xE);
            var b1 = (byte) rand.Next(255);
            _chip8.V[x] = b1;
            var b2 = (byte) rand.Next(255);
            var instr = (ushort) ((7 << 12) | (x << 8) | b2);
            var sum = b1 + b2;
            _chip8.Add(instr);
            Assert.Equal((byte) sum, _chip8.V[x]);
            if (sum > 255)
                Assert.Equal(_chip8.V[0xF], 1);
            else
                Assert.Equal(_chip8.V[0xF], 0);
        }

        [Fact]
        public void Add()
        {
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var b1 = (byte) rand.Next(255);
            var b2 = (byte) rand.Next(255);
            _chip8.V[x] = b1;
            _chip8.V[y] = b2;
            var sum = b1 + b2;
            var instr = (ushort) ((8 << 12) | (x << 8) | (y << 4) | 4);
            _chip8.Add(instr);
            Assert.Equal((byte) sum, _chip8.V[x]);
            if (sum > 255)
                Assert.Equal(_chip8.V[0xF], 1);
            else
                Assert.Equal(_chip8.V[0xF], 0);
        }

        // TODO dedicated Carry Flag tests
    }
}