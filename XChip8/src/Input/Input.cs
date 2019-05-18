using System;
using System.Collections.Generic;
using SDL2;

namespace XChip8.Input
{
    public class Input
    {
        public Dictionary<SDL.SDL_Keycode, int> KeyMap;
        public Dictionary<int, bool> IsPressed;

        public const int TICKS_PER_SECOND = 50;

        public Input()
        {
            IsPressed = new Dictionary<int, bool>{
                [0] = false,
                [1] = false,
                [2] = false,
                [3] = false,
                [4] = false,
                [5] = false,
                [6] = false,
                [7] = false,
                [8] = false,
                [9] = false,
                [0xA] = false,
                [0xB] = false,
                [0xC] = false,
                [0xD] = false,
                [0xE] = false,
                [0xF] = false
            };

            KeyMap = new Dictionary<SDL.SDL_Keycode, int>{
                [SDL.SDL_Keycode.SDLK_1] = 1,
                [SDL.SDL_Keycode.SDLK_2] = 2,
                [SDL.SDL_Keycode.SDLK_3] = 3,
                [SDL.SDL_Keycode.SDLK_q] = 4,
                [SDL.SDL_Keycode.SDLK_w] = 5,
                [SDL.SDL_Keycode.SDLK_e] = 6,
                [SDL.SDL_Keycode.SDLK_a] = 7,
                [SDL.SDL_Keycode.SDLK_s] = 8,
                [SDL.SDL_Keycode.SDLK_d] = 9,
                [SDL.SDL_Keycode.SDLK_z] = 0xA,
                [SDL.SDL_Keycode.SDLK_x] = 0,
                [SDL.SDL_Keycode.SDLK_c] = 0xB,
                [SDL.SDL_Keycode.SDLK_4] = 0xC,
                [SDL.SDL_Keycode.SDLK_r] = 0xD,
                [SDL.SDL_Keycode.SDLK_f] = 0xE,
                [SDL.SDL_Keycode.SDLK_v] = 0xF
            };

            SDL.SDL_Init(SDL.SDL_INIT_EVENTS);
        }

        public void KeyDown(SDL.SDL_Keycode key)
        {
            if(KeyMap.ContainsKey(key))
                IsPressed[KeyMap[key]] = true;
        }

        public void KeyUp(SDL.SDL_Keycode key)
        {
            if(KeyMap.ContainsKey(key))
                IsPressed[KeyMap[key]] = false;
        }

        public bool IsKeyPressed(int key)
        {
            if(key < 16)
                return IsPressed[key];
            else
                return false;
        } 
    }
}