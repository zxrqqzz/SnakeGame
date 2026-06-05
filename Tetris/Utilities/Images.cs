using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Tetris.Utilities
{
    public static class Images
    {
        public readonly static ImageSource Empty = LoadImage("Empty.png");
        public readonly static ImageSource Square = LoadImage("Square.png");

        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}",UriKind.Relative));
        }
    }
}
