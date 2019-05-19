using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SDL2;
using XChip8.Emulators;
using XChip8.Input;
using XChip8.Renderers;

namespace XChip8.Systems
{
    public class System
    {
        private Chip8 emulator;
        // private Input.Input input;

        private const int TICKS_PER_SECOND = 500;
        private const int SKIP_TICKS = 1000 / TICKS_PER_SECOND;

        private const int TIMER_TICKS = 60;

        private const int TIMER_SKIP_TICKS = 1000 / TIMER_TICKS;
        private const int MAX_FRAME_SKIP = 5;

        public System(Chip8 emulator)
        {
            this.emulator = emulator;
            // this.input = new Input.Input();
            SDL.SDL_Init(SDL.SDL_INIT_TIMER);
            emulator.LoadRom("/home/arjun/Downloads/Chip8Roms/Trip8.ch8");
        }

        public void Start()
        {
            var running = true;
            var next_tick = SDL.SDL_GetTicks();
            var next_timer_tick = SDL.SDL_GetTicks();
            var loops = 0;
            var rom = emulator.DumpRomFromMemory();
            var suspect = rom[0x3BE];
            while (running)
            {

                loops = 0;
                while (SDL.SDL_GetTicks() > next_tick && loops < MAX_FRAME_SKIP)
                {
                    SDL.SDL_Event e;
                    while (SDL.SDL_PollEvent(out e) != 0)
                    {
                        switch (e.type)
                        {
                            case SDL.SDL_EventType.SDL_KEYDOWN:
                                switch (e.key.keysym.sym)
                                {
                                    case SDL.SDL_Keycode.SDLK_ESCAPE:
                                        running = false;
                                        break;
                                    default:
                                        //Console.WriteLine("KeyPressed {0}", e.key.keysym.sym.ToString());
                                        emulator.input.KeyDown(e.key.keysym.sym);
                                        break;
                                }
                                break;
                            case SDL.SDL_EventType.SDL_KEYUP:
                                emulator.input.KeyUp(e.key.keysym.sym);
                                break;
                        }
                    }
                    update();
                    next_tick += SKIP_TICKS;
                    loops++;
                }

                var timer_loops = 0;
                while (SDL.SDL_GetTicks() > next_timer_tick && timer_loops < MAX_FRAME_SKIP)
                {
                    if (emulator.DT > 0)
                        emulator.DT -= 1;
                    if (emulator.ST > 0)
                        emulator.ST -= 1;
                    timer_loops++;
                    next_timer_tick += SKIP_TICKS;
                }
                if (true)
                {
                    emulator.ScreenStateChanged = false;
                    render();
                }
            }
        }

        private void update()
        {
            var opcode = emulator.GetOpcode();
            var pcSet = false;
            switch ((opcode & 0xF000) >> 12)
            {
                case 0:
                    switch (opcode & 0x00FF)
                    {
                        case 0xE0:
                            emulator.CLS();
                            break;
                        case 0xEE:
                            emulator.RET();
                            // pcSet = true;
                            break;
                    }
                    break;
                case 1:
                    emulator.JP(opcode);
                    pcSet = true;
                    break;
                case 2:
                    emulator.Call(opcode);
                    pcSet = true;
                    break;
                case 3:
                    emulator.SE(opcode);
                    break;
                case 4:
                    emulator.SNE(opcode);
                    break;
                case 5:
                    emulator.SE(opcode);
                    break;
                case 6:
                    emulator.LD(opcode);
                    break;
                case 7:
                    emulator.Add(opcode);
                    break;
                case 8:
                    switch (opcode & 0x000F)
                    {
                        case 0:
                            emulator.LD(opcode);
                            break;
                        case 1:
                            emulator.Or(opcode);
                            break;
                        case 2:
                            emulator.And(opcode);
                            break;
                        case 3:
                            emulator.Xor(opcode);
                            break;
                        case 4:
                            emulator.Add(opcode);
                            break;
                        case 5:
                            emulator.Sub(opcode);
                            break;
                        case 6:
                            emulator.SHR(opcode);
                            break;
                        case 7:
                            emulator.Sub(opcode);
                            break;
                        case 0xE:
                            emulator.SHL(opcode);
                            break;
                    }
                    break;
                case 9:
                    emulator.SNE(opcode);
                    break;
                case 0xA:
                    emulator.LDI(opcode);
                    break;
                case 0xB:
                    emulator.JP(opcode);
                    break;
                case 0xC:
                    emulator.RND(opcode);
                    break;
                case 0xD:
                    emulator.DRW(opcode);
                    break;
                case 0xE:
                    switch (opcode & 0x00FF)
                    {
                        case 0x9E:
                            emulator.SKP(opcode);
                            break;
                        case 0xA1:
                            emulator.SKNP(opcode);
                            break;
                    }
                    break;
                case 0xF:
                    switch (opcode & 0x00FF)
                    {
                        case 0x07:
                            emulator.LDDT(opcode);
                            break;
                        case 0x0A:
                            emulator.LDK(opcode);
                            break;
                        case 0x15:
                            emulator.STDT(opcode);
                            break;
                        case 0x18:
                            emulator.STST(opcode);
                            break;
                        case 0x1E:
                            emulator.ADDI(opcode);
                            break;
                        case 0x29:
                            emulator.LDFI(opcode);
                            break;
                        case 0x33:
                            emulator.LDBCD(opcode);
                            break;
                        case 0x55:
                            emulator.STRegs(opcode);
                            break;
                        case 0x65:
                            emulator.LDRegs(opcode);
                            break;
                    }
                    break;
            }
            if (!pcSet)
            {
                emulator.AdvancePC();
            }
            pcSet = false;
            Console.WriteLine("OPCODE : {0:X4}", opcode);
            Console.WriteLine("PC {0:X4}", emulator.PC);
        }

        private void render()
        {
            var start = SDL.SDL_GetTicks();
            emulator.RenderScreen();
            var end = SDL.SDL_GetTicks();
            ////Console.WriteLine("Render Time {0}", (end - start));
        }

    }
}