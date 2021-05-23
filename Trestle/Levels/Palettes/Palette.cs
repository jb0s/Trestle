using System;

namespace Trestle.Levels.Palettes
{
    public class Palette
    {
        public virtual void GetIdFromBlock()
        { }

        public virtual void GetBlockFromId(int id)
        { }
        
        public static Palette ChoosePalette(int bits = 4)
        {
            if (bits <= 4)
            {
                bits = 4;
                return new LinearPalette();
            }
            else if (bits <= 8)
                return new LinearPalette();
            else
                throw new NotImplementedException("Non-linear palette is not implemented.");
        }
    }
}