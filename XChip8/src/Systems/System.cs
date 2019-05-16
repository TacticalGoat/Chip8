using XChip8.Emulators;
using XChip8.Renderers;

namespace XChip8.Systems
{
    public class System
    {
        private Chip8 emulator;
        private Renderer renderer;

        public System(Chip8 emulator, Renderer renderer)
        {
            this.emulator = emulator;
            this.renderer = renderer;
        }

        public void test_font()
        {
            renderer.BlankWindow();
            renderer.BlankScreen();

            emulator.V[0] = 10;
            emulator.V[1] = 10;
            emulator.V[3] = 0xF;
            emulator.LDFI(0xF329);
            emulator.DRW(0xD015);

            emulator.V[0] = 15;
            emulator.V[1] = 10;
            emulator.V[3] = 0;
            emulator.LDFI(0xF329);
            emulator.DRW(0xD015);

            emulator.V[0] = 20;
            emulator.V[1] = 10;
            emulator.V[3] = 0xE;
            emulator.LDFI(0xF329);
            emulator.DRW(0xD015);
            
            renderer.RenderScreen(emulator.ScreenState);
            renderer.SetPixel(0,0);
            renderer.SetPixel(0,31);
            renderer.SetPixel(63,0);
            renderer.SetPixel(63,31);
        }
    }
}