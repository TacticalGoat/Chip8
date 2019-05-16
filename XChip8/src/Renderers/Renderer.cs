using System;
using System.Runtime.InteropServices;
using SDL2;

namespace XChip8.Renderers
{
    public class Renderer
    {
        public bool[, ] ScreenState;
        public bool Collision;
        private IntPtr window;
        private IntPtr sdlRenderer;
        private int height;
        private int width;

        public Renderer(int width = 64, int height = 32)
        {
            this.height = height * 10;
            this.width = width * 10;
            SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
            window = IntPtr.Zero;
            window = SDL.SDL_CreateWindow(
                "XChip8",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                this.width,
                this.height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
            );
            sdlRenderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE);
            ScreenState = new bool[64, 32];
            Collision = false;
        }

        public void BlankScreen() => ScreenState = new bool[64, 32];

        public void BlankWindow()
        {
            SDL.SDL_RenderSetLogicalSize(sdlRenderer, width, height);
            SDL.SDL_SetRenderDrawColor(sdlRenderer, 0, 0, 0, 0x2F);
            SDL.SDL_RenderClear(sdlRenderer);
            SDL.SDL_RenderPresent(sdlRenderer);
        }
        public void SetPixel(int col, int row)
        {
            SDL.SDL_Rect rect;
            rect.x = col * 10;
            rect.y = row * 10;
            rect.h = 10;
            rect.w = 10;
            SDL.SDL_SetRenderDrawColor(sdlRenderer, 0xAA, 0xAA, 0xFF, 0xFF);
            SDL.SDL_RenderDrawRect(sdlRenderer, ref rect);
            SDL.SDL_RenderFillRect(sdlRenderer, ref rect);
            SDL.SDL_RenderPresent(sdlRenderer);
        }
        public void DrawSprite(byte[] sprite, int col, int row)
        {
            for (int i = 0; i < sprite.Length; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    var old_pixel = ScreenState[col + j, row + i] ? 1 : 0;
                    var sprite_pixel = sprite[i] >> j & 1;
                    if (old_pixel == 1 && sprite_pixel == 1)
                        Collision = true;
                    else
                        Collision = false;
                    ScreenState[col + j, row + i] = (old_pixel ^ sprite_pixel) == 1 ? true : false;
                }
            }
        }

        public void RenderScreen(bool[,] ScreenState)
        {
            for (var col = 0; col < 64; col++)
            {
                for (var row = 0; row < 32; row++)
                {
                    if (ScreenState[col, row])
                        SetPixel(col, row);
                }
            }
        }
    }
}