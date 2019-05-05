using Xunit;
using XChip8.Emulators;
using System;

namespace XChip8.UnitTests.Emulators
{
    public class CallTest : Base
    {
        [Fact]
        public void BaseTest()
        {
            var instr = (ushort)rand.Next(0x2200, 0x2FFF);
            var curSp = _chip8.SP;
            var curPC = _chip8.PC;
            var futPC = (ushort)instr & 0x0FFF;
            _chip8.Call(instr);

            // Check Stack Pointer Increment
            Assert.Equal(_chip8.SP - curSp, 1);
            // Check Old PC is on Stack Top
            Assert.Equal(_chip8.Stack[_chip8.SP], curPC);
            // Check current PC is set according to the Instruction
            Assert.Equal(_chip8.PC, futPC);
        }
    }
    
}