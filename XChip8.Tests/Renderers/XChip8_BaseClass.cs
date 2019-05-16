using Xunit;
using XChip8.Emulators;
using XChip8.Renderers;
using System;

namespace XChip8.UnitTests.Renderers
{
    public abstract class Base
    {
        protected readonly Renderer _renderer;
        protected readonly Random rand; 

        public Base()
        {
            _renderer = new Renderer();
            rand = new Random();
        }
    }
} 