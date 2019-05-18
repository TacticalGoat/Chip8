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
        private const int MAX_FRAME_SKIP = 5;

        public System(Chip8 emulator)
        {
            this.emulator = emulator;
            // this.input = new Input.Input();
            SDL.SDL_Init(SDL.SDL_INIT_TIMER);
        }

        public void Start()
        {
            var running = true;
            while (running)
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
                render();
            }
        }

        private void update()
        {
            var opcode = emulator.GetOpcode();
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
                            break;
                    }
                    break;
                case 1:
                    emulator.JP(opcode);
                    break;
                case 2:
                    emulator.Call(opcode);
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
                    switch(opcode & 0x000F)
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
                    switch(opcode & 0x00FF)
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
                    switch(opcode & 0x00FF)
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
            emulator.AdvancePC();
        }

        private void render()
        {
            emulator.BlankScreen();
            emulator.RenderScreen();
        }

    }
}