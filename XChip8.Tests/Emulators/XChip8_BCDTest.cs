using Xunit;

namespace XChip8.UnitTests.Emulators
{
    public class BCDTest : Base
    {
        [Fact]
        public void TestBdcRep()
        {
            _chip8.V[0] = 235;
            _chip8.LDBCD(0xF033);
            Assert.Equal(_chip8.Memory[_chip8.I], 2);
            Assert.Equal(_chip8.Memory[_chip8.I + 1], 3);
            Assert.Equal(_chip8.Memory[_chip8.I + 2], 5);
        }
    }
}