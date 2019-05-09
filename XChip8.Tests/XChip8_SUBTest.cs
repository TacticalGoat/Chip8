using Xunit;

namespace XChip8.UnitTests.Emulators
{
    public class XChip8_SUBTest : Base
    {
        [Fact]
        public void TestSub()
        {
            var b1 = (byte) rand.Next(255);
            var b2 = (byte) rand.Next(255);
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var diff = (byte) (b1 - b2);
            _chip8.V[x] = b1;
            _chip8.V[y] = b2;
            var instr = (ushort) ((8 << 12) | (x << 8) | (y << 4) | 5);
            _chip8.Sub(instr);
            Assert.Equal(diff, _chip8.V[x]);
            if (b2 > b1)
                Assert.Equal(_chip8.V[0xF], (byte) 1);
            else
                Assert.Equal(_chip8.V[0xF], 0);
        }

        [Fact]
        public void TestSubN()
        {
            var b1 = (byte) rand.Next(255);
            var b2 = (byte) rand.Next(255);
            var x = rand.Next(0x0, 0xE);
            var y = rand.Next(0x0, 0xE);
            var diff = (byte) (b1 - b2);
            _chip8.V[x] = b1;
            _chip8.V[y] = b2;
            var instr = (ushort) ((8 << 12) | (x << 8) | (y << 4) | 7);
            _chip8.Sub(instr);
            Assert.Equal(diff, _chip8.V[x]);
            if (b1 > b2)
                Assert.Equal(_chip8.V[0xF], (byte) 1);
            else
                Assert.Equal(_chip8.V[0xF], 0);
        }

        [Theory]
        [InlineData((byte) 0, (byte) 1)]
        [InlineData((byte) 235, (byte) 240)]
        [InlineData((byte) 254, (byte) 255)]
        public void TestFlagOneForSubBorrow(byte b1, byte b2)
        {
            _chip8.V[0x1] = b1;
            _chip8.V[0x2] = b2;
            _chip8.Sub((ushort) 0x8125);
            Assert.Equal(_chip8.V[0xF], (byte) 1);
        }

        [Theory]
        [InlineData((byte) 10, (byte) 1)]
        [InlineData((byte) 200, (byte) 140)]
        [InlineData((byte) 254, (byte) 155)]
        public void TestFlagZeroForSubNoBorrow(byte b1, byte b2)
        {
            _chip8.V[0x1] = b1;
            _chip8.V[0x2] = b2;
            _chip8.Sub((ushort) 0x8125);
            Assert.Equal(_chip8.V[0xF], (byte) 0);
        }

        [Theory]
        [InlineData((byte) 10, (byte) 1)]
        [InlineData((byte) 209, (byte) 200)]
        [InlineData((byte) 254, (byte) 190)]
        public void TestFlagOneForSubNNoBorrow(byte b1, byte b2)
        {
            _chip8.V[0x1] = b1;
            _chip8.V[0x2] = b2;
            _chip8.Sub((ushort) 0x8127);
            Assert.Equal(_chip8.V[0xF], (byte) 1);
        }

        [Theory]
        [InlineData((byte) 10, (byte) 11)]
        [InlineData((byte) 109, (byte) 200)]
        [InlineData((byte) 154, (byte) 190)]
        public void TestFlagZeroForSubNBorrow(byte b1, byte b2)
        {
            _chip8.V[0x1] = b1;
            _chip8.V[0x2] = b2;
            _chip8.Sub((ushort) 0x8127);
            Assert.Equal(_chip8.V[0xF], (byte) 0);
        }
    }
}