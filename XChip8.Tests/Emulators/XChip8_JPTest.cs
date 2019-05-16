using Xunit;
using XChip8.Emulators;
using System;

namespace XChip8.UnitTests.Emulators
{
    public class XChip8_JPTest : Base
    {

        [Fact]
        public void SetPCtoJPNNN()
        {
            var oldPc = _chip8.PC;
            var instr = rand.Next(0x1200, 0x1FFF);
            _chip8.JP((ushort)instr);
            var newPC = _chip8.PC;
            Assert.Equal(newPC, (ushort)(instr & 0x0FFF));
        }
    }
}