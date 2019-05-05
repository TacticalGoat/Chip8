using System;
using System.IO;
using System.Linq;

namespace XChip8.Emulators
{
    public class Chip8
    {
        public byte[] Memory { get; private set; }
        public byte[] V { get; private set; }
        public ushort I { get; private set; }
        public byte ST { get; private set; }
        public byte DT { get; private set; }
        public ushort PC { get; private set; }
        public bool[,] Screen { get; private set; }
        public short SP { get; private set; }
        public ushort[] Stack { get; private set; }
        Random rand;

        public Chip8()
        {
            Memory = new byte[4096];
            V = new byte[16];
            I = 0x0000;
            ST = 0;
            DT = 0;
            PC = 0x200;
            SP = -1;
            Screen = new bool[64, 32];
            Stack = new ushort[16];
            rand = new Random();
        }

        public bool LoadRom(string path)
        {
            try
            {
                var rom_bytes = File.ReadAllBytes(path);
                Array.Copy(rom_bytes, 0, Memory, 0x200, rom_bytes.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        public byte[] DumpMemory()
        {
            // Skip reserved section
            return Memory.Skip(0x200).ToArray();
        }

        public ushort[] DumpRomFromMemory()
        {
            return Memory
                .Skip(0x200)
                //Select with index
                .Select((x, i) => new { Index = i, Value = x })
                //Groups 2 elements at once
                .GroupBy(x => x.Index / 2)
                //Converts two bytes into an instruction
                .Select(x => (ushort)(((x.ElementAt(0).Value << 8) | x.ElementAt(1).Value) & 0xFFFF))
                //Converts to array
                .ToArray();
        }

        private ushort fetchCurrentOpcode()
        {
            return (ushort)(((Memory[PC] << 8) | Memory[PC + 1]) & 0xFFFF);
        }

        private void advancePC()
        {
            PC += 2;
        }

        /*
         * Most instructions follow the convention X Vx Vy X example 8AB3
         * The following extract Vx and Vy
         */
        private int getVx(ushort opcode)
        {
            return (opcode & 0x0F00) >> 8;
        }

        private int getVy(ushort opcode)
        {
            return (opcode & 0x00F0) >> 4;
        }

        // Get last two bytes
        private int getKk(ushort opcode)
        {
            return (opcode & 0x00FF);
        }

        public void JP(ushort opcode)
        {
            PC = (ushort)(opcode & 0x0FFF);
        }

        public void Call(ushort opcode)
        {
            SP += 1;
            Stack[SP] = PC;
            PC = (ushort)(opcode & 0x0FFF);
        }

        public void SE(ushort opcode)
        {
            var x = getVx(opcode);
            switch ((opcode & 0xF000) >> 12)
            {
                case 3:
                    var kk = getKk(opcode);
                    if (V[x] == (byte)kk)
                        advancePC();
                    break;
                case 5:
                    var y = getVy(opcode);
                    if (V[x] == V[y])
                        advancePC();
                    break;
            }

        }

        public void SNE(ushort opcode)
        {
            var x = getVx(opcode);
            var kk = getKk(opcode);
            if (V[x] != (byte)kk)
                advancePC();
        }

        public void LD(ushort opcode)
        {
            int x = getVx(opcode);
            switch((opcode & 0xF000) >> 12)
            {
                case 6:
                    int kk = getKk(opcode);
                    V[x] = (byte)kk;
                    break;
                case 8:
                    int y = getVy(opcode);
                    V[x] = V[y];
                    break;
            }
        }

        public void Add(ushort opcode)
        {
            var x = getVx(opcode);
            var sum = 0;
            switch((opcode & 0xF000) >> 12)
            {
                case 7:
                    var kk = getKk(opcode);
                    sum = V[x] + kk;
                    break;
                case 8:
                    var y = getVy(opcode);
                    sum = V[x] + V[y];
                    break;
            }
            V[0xF] = (byte)(sum > 255 ? 1 : 0);
            V[x] = (byte)(sum);
        }

        public void Or(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte)(V[x] | V[y]);
        }

        public void And(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte)(V[x] & V[y]);
        }

        public void Xor(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte)(V[x] ^ V[y]);
        }

        public void Sub(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            var diff = V[x] - V[y];
            V[0xF] = (byte)(V[x] > V[y] ? 1 : 0);
            V[x] = (byte)diff;
        }

        public void SHR(ushort opcode)
        {
            var x = getVx(opcode);
            V[0xF] = (byte)((V[x] & 0x0001) == 1 ? 1 : 0);
            V[x] = (byte)(V[x] >> 1);
        }


    }
}
