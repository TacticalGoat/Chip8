using System;
using System.Threading;
using SDL2;

namespace XChip8.Audio
{
    public class Buzz
    {
        private IntPtr buzz;
        public Buzz()
        {
            SDL.SDL_Init(SDL.SDL_INIT_AUDIO);
            SDL_mixer.Mix_OpenAudio(4410, SDL_mixer.MIX_DEFAULT_FORMAT, 1, 2048);
            buzz = SDL_mixer.Mix_LoadWAV("effects/beep44.wav");
        }

        public void Play()
        {
            SDL_mixer.Mix_PlayChannel(-1, buzz, 10);
        }

        public void Stop()
        {
            SDL_mixer.Mix_HaltChannel(-1);
        }

        ~Buzz()
        {
            SDL_mixer.Mix_FreeChunk(buzz);
            SDL_mixer.Mix_Quit();
        }
    }
}