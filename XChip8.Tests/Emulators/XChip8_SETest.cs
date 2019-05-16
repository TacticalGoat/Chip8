using Xunit;
using System;

namespace XChip8.UnitTests.Emulators
{
    public class XChip8_SETest : Base
    {
        [Fact]
        public void InlineSE()
        {
            // register number
            var x = rand.Next(0x0, 0xE);
            // byte to be checked
            var kk = (byte)rand.Next(0x00, 0xFF);
            // Build instruction 3xkk
            var instr = (ushort)((3 << 12) | (x << 8) | (kk));
            _chip8.V[x] = kk;
            var curPC = _chip8.PC;
            _chip8.SE(instr);
            Assert.Equal(_chip8.PC - curPC, 2);
        }

        [Fact]
        public void InlineSEFalse()
        {
            // register number
            var x = rand.Next(0x0, 0xE);
            // byte to be checked
            var kk = (byte)rand.Next(0x00, 0xFF);
            // Build instruction 3xkk
            var instr = (ushort)((3 << 12) | (x << 8) | (kk));
            _chip8.V[x] = (byte)(kk + 1);
            var curPC = _chip8.PC;
            _chip8.SE(instr);
            Assert.Equal(_chip8.PC, curPC);
        }

        [Fact]
        public void SE()
        {
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var instr = (ushort)((5 << 12) | (x << 8) | (y << 4));
            var _byte = (byte)rand.Next();
            _chip8.V[x] = _byte;
            _chip8.V[y] = _byte;
            var curPC = _chip8.PC;
            _chip8.SE(instr);
            Assert.Equal(_chip8.PC - curPC, 2);
        }

        [Fact]
        public void SEFalse()
        {
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var instr = (ushort)((5 << 12) | (x << 8) | (y << 4));
            var _byte = (byte)rand.Next();
            _chip8.V[x] = _byte;
            _chip8.V[y] = (byte)(_byte + 1);
            var curPC = _chip8.PC;
            Assert.Equal(_chip8.PC, curPC);
        }
    }
}