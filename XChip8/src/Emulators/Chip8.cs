using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SDL2;
using XChip8.Input;
using XChip8.Renderers;

namespace XChip8.Emulators
{
    public class Chip8
    {
        public byte[] Memory { get; private set; }
        public byte[] V { get; private set; }
        public ushort I { get; set; }
        public byte ST { get; set; }
        public byte DT { get; set; }
        public ushort PC { get; private set; }
        public bool[, ] ScreenState { get; private set; }
        public short SP { get; private set; }
        public ushort[] Stack { get; private set; }

        public bool ScreenStateChanged = false;
        Random rand;

        private Renderer renderer;
        public Input.Input input;

        public Chip8(Renderer renderer, Input.Input input)
        {
            Memory = new byte[5000];
            V = new byte[16];
            I = 0x0000;
            ST = 0;
            DT = 0;
            PC = 0x200;
            SP = -1;
            ScreenState = new bool[64, 32];
            Stack = new ushort[16];
            rand = new Random();
            initFonts();
            this.renderer = renderer;
            this.input = input;
        }

        private void initFonts()
        {
            var fontSet = new byte[][]
            {
                new byte[] { 0xF0, 0x90, 0x90, 0x90, 0xF0 }, // 0
                new byte[] { 0x20, 0x60, 0x20, 0x20, 0x70 }, // 1
                new byte[] { 0xF0, 0x10, 0xF0, 0x80, 0xF0 }, // 2
                new byte[] { 0xF0, 0x10, 0xF0, 0x10, 0xF0 }, // 3
                new byte[] { 0x90, 0x90, 0xF0, 0x10, 0x10 }, // 4
                new byte[] { 0xF0, 0x80, 0xF0, 0x10, 0xF0 }, // 5
                new byte[] { 0xF0, 0x80, 0xF0, 0x90, 0xF0 }, // 6
                new byte[] { 0xF0, 0x10, 0x20, 0x40, 0x40 }, // 7
                new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0xF0 }, // 8
                new byte[] { 0xF0, 0x90, 0xF0, 0x10, 0x10 }, // 9
                new byte[] { 0xF0, 0x90, 0xF0, 0x90, 0x90 }, // A
                new byte[] { 0xE0, 0x90, 0xE0, 0x90, 0xE0 }, // B
                new byte[] { 0xF0, 0x80, 0x80, 0x80, 0xF0 }, // C
                new byte[] { 0xE0, 0x90, 0x90, 0x90, 0xE0 }, // D
                new byte[] { 0xF0, 0x80, 0xE0, 0x80, 0xF0 }, // E
                new byte[] { 0xF0, 0x80, 0xE0, 0x80, 0x80 } // F
            };
            int j = 0;
            for (int i = 0; i < 76; i += 5)
            {
                loadFont(i, fontSet[j]);
                j++;
            }
        }

        private int getMemoryLocationForChar(byte char_)
        {
            return char_ * 5;
        }

        private void loadFont(int start, byte[] sprite)
        {
            for (int i = 0; i < 5; i++)
            {
                Memory[start + i] = sprite[i];
            }
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
                .Select(x => (ushort) (((x.ElementAt(0).Value << 8) | x.ElementAt(1).Value) & 0xFFFF))
                //Converts to array
                .ToArray();
        }

        public void AdvancePC()
        {
            PC += 2;
        }

        public ushort GetOpcode()
        {
            return (ushort) (Memory[PC] << 8 | Memory[PC + 1]);
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

        // Get second byte
        private int getKk(ushort opcode)
        {
            return (opcode & 0x00FF);
        }

        // Get Last 3 nibbles i,e Xnnn this method gets nnn
        private int getNnn(ushort opcode)
        {
            return (opcode & 0x0FFF);
        }

        public void DrawSprite(byte[] sprite, int col, int row)
        {
            for (int i = 0; i < sprite.Length; i++)
            {
                for (int j = 7; j >= 0; j--)
                {
                    var old_pixel = ScreenState[(col + (7 - j)) % 64, (row + i) % 32] ? 1 : 0;
                    var sprite_pixel = sprite[i] >> j & 1;
                    if ((old_pixel ^ sprite_pixel) == 0 && old_pixel == 1)
                        V[0xF] = 1;
                    ScreenState[(col + (7 - j)) % 64, (row + i) % 32] = (old_pixel ^ sprite_pixel) == 1 ? true : false;
                }
            }
            ScreenStateChanged = true;
        }

        private byte[] loadSprite(int n)
        {
            return Memory.Skip(I).Take(n).ToArray();
        }

        public void CLS() 
        {
            ScreenState = new bool[64, 32];
            ScreenStateChanged = true;
        }

        public void JP(ushort opcode)
        {
            switch ((opcode & 0xF000) >> 12)
            {
                case 1:
                    PC = (ushort) getNnn(opcode);
                    break;
                case 0xB:
                    var addr = getNnn(opcode) + V[0];
                    PC = (ushort) addr;
                    break;
            }
        }

        public void RET()
        {
            PC = Stack[SP];
            SP--;
        }

        public void Call(ushort opcode)
        {
            SP += 1;
            Stack[SP] = PC;
            PC = (ushort) (opcode & 0x0FFF);
        }

        public void SE(ushort opcode)
        {
            var x = getVx(opcode);
            switch ((opcode & 0xF000) >> 12)
            {
                case 3:
                    var kk = getKk(opcode);
                    if (V[x] == (byte) kk)
                        AdvancePC();
                    break;
                case 5:
                    var y = getVy(opcode);
                    if (V[x] == V[y])
                        AdvancePC();
                    break;
            }

        }

        public void SNE(ushort opcode)
        {
            var x = getVx(opcode);
            switch ((opcode & 0xF000) >> 12)
            {
                case 4:
                    var kk = getKk(opcode);
                    if (V[x] != (byte) kk)
                        AdvancePC();
                    break;
                case 9:
                    var y = getVy(opcode);
                    if (V[x] != V[y])
                        AdvancePC();
                    break;
            }
        }

        public void LD(ushort opcode)
        {
            int x = getVx(opcode);
            switch ((opcode & 0xF000) >> 12)
            {
                case 6:
                    int kk = getKk(opcode);
                    V[x] = (byte) kk;
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
            switch ((opcode & 0xF000) >> 12)
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
            V[0xF] = (byte) (sum > 255 ? 1 : 0);
            V[x] = (byte) (sum);
        }

        public void Or(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte) (V[x] | V[y]);
        }

        public void And(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte) (V[x] & V[y]);
        }

        public void Xor(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            V[x] = (byte) (V[x] ^ V[y]);
        }

        public void Sub(ushort opcode)
        {
            var x = getVx(opcode);
            var y = getVy(opcode);
            var diff = V[x] - V[y];
            switch (opcode & 0x000F)
            {
                case 5:
                    V[0xF] = (byte) (V[y] > V[x] ? 1 : 0);
                    break;
                case 7:
                    V[0xF] = (byte) (V[x] > V[y] ? 1 : 0);
                    break;
            }
            V[x] = (byte) diff;
        }

        public void SHR(ushort opcode)
        {
            var x = getVx(opcode);
            V[0xF] = (byte) ((V[x] & 0x0001) == 1 ? 1 : 0);
            V[x] = (byte) (V[x] >> 1);
        }

        public void SHL(ushort opcode)
        {
            var x = getVx(opcode);
            V[0xF] = (byte) (((V[x] & 0x8000) >> 12) == 1 ? 1 : 0);
            V[x] = (byte) (V[x] << 1);
        }

        public void LDI(ushort opcode)
        {
            var nnn = getNnn(opcode);
            I = (ushort) nnn;
        }
        public void RND(ushort opcode)
        {
            var b = (byte) rand.Next(255);
            var kk = getKk(opcode);
            var x = getVx(opcode);
            Console.WriteLine("Random Byte {0}", b);
            V[x] = (byte) (b & kk);
        }

        public void LDDT(ushort opcode)
        {
            var x = getVx(opcode);
            V[x] = DT;
        }

        public void STDT(ushort opcode)
        {
            var x = getVx(opcode);
            DT = V[x];
        }

        public void STST(ushort opcode)
        {
            var x = getVx(opcode);
            ST = V[x];
        }

        public void ADDI(ushort opcode)
        {
            var x = getVx(opcode);
            I = (ushort) (I + V[x]);
        }

        public void STRegs(ushort opcode)
        {
            var x = getVx(opcode);
            for (int i = 0; i <= x; i++)
            {
                Memory[I + i] = V[i];
            }
        }

        public void LDRegs(ushort opcode)
        {
            var x = getVx(opcode);
            for (int i = 0; i <= x; i++)
            {
                V[i] = Memory[I + i];
            }
        }

        public void LDBCD(ushort opcode)
        {
            var x = getVx(opcode);
            var num = (int) V[x];
            var div = 100;
            var i = 0;
            while (i < 3)
            {
                Memory[I + i] = (byte) (num / div);
                num %= div;
                div /= 10;
                i++;
            }
        }

        public void LDFI(ushort opcode)
        {
            var x = getVx(opcode);
            I = (ushort) getMemoryLocationForChar(V[x]);
        }

        public void DRW(ushort opcode)
        {   
            V[0xF] = 0;
            var x = getVx(opcode);
            var y = getVy(opcode);
            var n = opcode & 0x000F;
            var sprite = loadSprite(n);
            DrawSprite(sprite, V[x], V[y]);
            ScreenStateChanged = true;
        }

        public void SKP(ushort opcode)
        {
            var x = getVx(opcode);
            if (input.IsKeyPressed(V[x]))
                AdvancePC();
        }

        public void SKNP(ushort opcode)
        {
            var x = getVx(opcode);
            if (!input.IsKeyPressed(V[x]))
                AdvancePC();
        }

        public void LDK(ushort opcode)
        {
            var x = getVx(opcode);
            var waiting = true;
            while (waiting)
            {
                SDL.SDL_Event e;
                if (SDL.SDL_PollEvent(out e) == 1)
                {
                    if (e.type == SDL.SDL_EventType.SDL_KEYDOWN)
                    {
                        if (input.KeyMap.ContainsKey(e.key.keysym.sym))
                        {
                            V[x] = (byte) input.KeyMap[e.key.keysym.sym];
                            waiting = false;
                        }
                    }
                }
            }
        }

        public void RenderScreen()
        {
            //renderer.BlankWindow();
            renderer.RenderScreen(ScreenState);
            ScreenStateChanged = false;
        }

        public void BlankScreen() => renderer.BlankWindow();

    }
}