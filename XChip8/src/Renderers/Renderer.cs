using System;
using System.Runtime.InteropServices;
using SDL2;

namespace XChip8.Renderers
{
    public class Renderer
    {
        public bool Collision;
        private IntPtr window;
        private IntPtr sdlRenderer;
        private int height;
        private int width;

        private SDL.SDL_Rect pixelRect;

        public Renderer(int width = 64, int height = 32)
        {
            this.height = height * 10;
            this.width = width * 10;
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            window = IntPtr.Zero;
            window = SDL.SDL_CreateWindow(
                "XChip8",
                SDL.SDL_WINDOWPOS_CENTERED,
                SDL.SDL_WINDOWPOS_CENTERED,
                this.width,
                this.height,
                SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL
                |SDL.SDL_WindowFlags.SDL_WINDOW_INPUT_FOCUS
            );
            sdlRenderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_SOFTWARE);
            Collision = false;
            pixelRect = new SDL.SDL_Rect();
            pixelRect.h = 10;
            pixelRect.w = 10;
        }
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
            SDL.SDL_RenderFillRect(sdlRenderer, ref rect);
        }

        public void ClearPixel(int col, int row)
        {
            SDL.SDL_SetRenderDrawColor(sdlRenderer, 0x00, 0x00, 0x00, 0xFF);
            SDL.SDL_Rect rect;
            rect.x = col * 10;
            rect.y = row * 10;
            rect.h = 10;
            rect.w = 10;
            SDL.SDL_RenderFillRect(sdlRenderer, ref rect);
        }

        public void RenderScreen(bool[,] ScreenState)
        {
            // BlankWindow();
            for (var col = 0; col < 64; col++)
            {
                for (var row = 0; row < 32; row++)
                {
                    if (ScreenState[col, row])
                        SetPixel(col, row);
                    else
                        ClearPixel(col, row);
                }
            }

            SDL.SDL_RenderPresent(sdlRenderer);
        }
    }
}