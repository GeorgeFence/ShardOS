using Cosmos.System.Graphics;

namespace CosmosTTF;

public class CGSSurface : ITTFSurface
{
    private Canvas canvas;

    public CGSSurface(Canvas canvas)
    {
        this.canvas = canvas;
    }
    public void DrawBitmap(Bitmap bmp, int x, int y)
    {
        canvas.DrawImageAlpha((Image)bmp, x, y);
    }
}