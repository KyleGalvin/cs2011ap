using OpenTK.Graphics;
using System.Diagnostics;
using System.Drawing;
using Img = System.Drawing.Imaging;

namespace AP
{

    /// <summary>
    /// Handles the text
    /// Contributors: Todd Burton, Adam Humeniuk, Mike Rioux
    /// Revision: 209
    /// </summary>
    public class TextHandler
    {
		#region Fields (2) 

        int tex;
        TextureFont texFont;

		#endregion Fields 

		#region Constructors (1) 

        /// <summary>
        /// Initializes a new instance of the <see cref="TextHandler"/> class.
        /// </summary>
        /// <param name="textPath">The text path.</param>
        public TextHandler(string textPath)
        {
            // Load a bitmap from disc, and put it in a GL texture.
            tex = TextUtil.CreateTextureFromFile(textPath);
            // Create a TextureFont object from the loaded texture.
            texFont = new TextureFont(tex);
        }

		#endregion Constructors 

		#region Methods (1) 

		// Public Methods (1) 

        /// <summary>
        /// Writes the text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="size">The size.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="angle">The angle.</param>
        public void writeText(string text, float size, float x, float y, int angle)
        {
            // Write something centered in the viewport.
            texFont.WriteStringAt(text, size, x, y, angle); // text, heightPercent, xPercent, yPercent, degreesCounterClockwise

        }

		#endregion Methods 
    }
    /// <summary>
    /// A text utility
    /// </summary>
  public static class TextUtil
  {
		#region Methods (4) 

		// Private Methods (4) 

      /// <summary>
      /// Creates the texture.
      /// </summary>
      /// <param name="width">The width.</param>
      /// <param name="height">The height.</param>
      /// <param name="alpha">if set to <c>true</c> [alpha].</param>
      /// <param name="bytes">The bytes.</param>
      /// <returns></returns>
    private static int CreateTexture(int width, int height, bool alpha, byte[] bytes)
    {
      int expectedBytes = width * height * (alpha ? 4 : 3);
      Debug.Assert(expectedBytes == bytes.Length);
      int tex = GiveMeATexture();
      Upload(width, height, alpha, bytes);
      SetParameters();
      return tex;
    }

    /// <summary>
    /// Gives me A texture.
    /// </summary>
    /// <returns></returns>
    private static int GiveMeATexture()
    {
      int tex = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, tex);
      return tex;
    }

    /// <summary>
    /// Sets the parameters.
    /// </summary>
    private static void SetParameters()
    {
      GL.TexParameter(
        TextureTarget.Texture2D,
        TextureParameterName.TextureMinFilter,
        (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D,
        TextureParameterName.TextureMagFilter,
        (int)TextureMagFilter.Linear);
    }

    /// <summary>
    /// Uploads the specified width.
    /// </summary>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="alpha">if set to <c>true</c> [alpha].</param>
    /// <param name="bytes">The bytes.</param>
    private static void Upload(int width, int height, bool alpha, byte[] bytes)
    {
      var internalFormat = alpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb;
      var format = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;
      GL.TexImage2D<byte>(
        TextureTarget.Texture2D,
        0,
        internalFormat,
        width, height,
        0,
        format,
        PixelType.UnsignedByte,
        bytes);
    }

		#endregion Methods 



    #region Public

    /// <summary>
    /// Initialize OpenGL state to enable alpha-blended texturing.
    /// Disable again with GL.Disable(EnableCap.Texture2D).
    /// Call this before drawing any texture, when you boot your
    /// application, eg. in OnLoad() of GameWindow or Form_Load()
    /// if you're building a WinForm app.
    /// </summary>
    public static void InitTexturing()
    {
      GL.Disable(EnableCap.CullFace);
      GL.Enable(EnableCap.Texture2D);
      GL.Enable(EnableCap.Blend);
      GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
      GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
    }

    /// <summary>
    /// Create an opaque OpenGL texture object from a given byte-array of r,g,b-triplets.
    /// Make sure width and height is 1, 2, .., 32, 64, 128, 256 and so on in size since all
    /// 3d graphics cards support those dimensions. Not necessarily square. Don't forget
    /// to call GL.DeleteTexture(int) when you don't need the texture anymore (eg. when switching
    /// levels in your game).
    /// </summary>
    public static int CreateRGBTexture(int width, int height, byte[] rgb)
    {
      return CreateTexture(width, height, false, rgb);
    }

    /// <summary>
    /// Create a translucent OpenGL texture object from given byte-array of r,g,b,a-triplets.
    /// See CreateRGBTexture for more info.
    /// </summary>
    public static int CreateRGBATexture(int width, int height, byte[] rgba)
    {
      return CreateTexture(width, height, true, rgba);
    }

    /// <summary>
    /// Create an OpenGL texture (translucent or opaque) from a given Bitmap.
    /// 24- and 32-bit bitmaps supported.
    /// </summary>
    public static int CreateTextureFromBitmap(Bitmap bitmap)
    {
      Img.BitmapData data = bitmap.LockBits(
        new Rectangle(0, 0, bitmap.Width, bitmap.Height),
        Img.ImageLockMode.ReadOnly,
        Img.PixelFormat.Format32bppArgb);
      var tex = GiveMeATexture();
      GL.BindTexture(TextureTarget.Texture2D, tex);
      GL.TexImage2D(
        TextureTarget.Texture2D,
        0,
        PixelInternalFormat.Rgba,
        data.Width, data.Height,
        0,
        PixelFormat.Bgra,
        PixelType.UnsignedByte,
        data.Scan0);
      bitmap.UnlockBits(data);
      SetParameters();
      return tex;
    }

    /// <summary>
    /// Create an OpenGL texture (translucent or opaque) by loading a bitmap
    /// from file. 24- and 32-bit bitmaps supported.
    /// </summary>
    public static int CreateTextureFromFile(string path)
    {
      return CreateTextureFromBitmap(new Bitmap(Bitmap.FromFile(path)));
    }

    #endregion
  }

  /// <summary>
  /// Store the font texture
  /// </summary>
  public class TextureFont
  {
		#region Fields (5) 

    /// <summary>
    /// Determines the distance from character center to adjacent character center, horizontally, in
    /// one written text string. Model space coordinates.
    /// </summary>
    public double AdvanceWidth = 0.9;
    /// <summary>
    /// Determines the height of the cut-out to do for each character when rendering. This is necessary
    /// to avoid artefacts stemming from filtering (zooming/rotating). Make sure your font contains some
    /// "white space" around each character so they won't be clipped due to this!
    /// </summary>
    public double CharacterBoundingBoxHeight = .9;
    /// <summary>
    /// Determines the width of the cut-out to do for each character when rendering. This is necessary
    /// to avoid artefacts stemming from filtering (zooming/rotating). Make sure your font contains some
    /// "white space" around each character so they won't be clipped due to this!
    /// </summary>
    public double CharacterBoundingBoxWidth = 0.8;
    private const double Sixteenth = 1.0 / 16.0;
    private int textureId;

		#endregion Fields 

		#region Constructors (1) 

    /// <summary>
    /// Create a TextureFont object. The sent-in textureId should refer to a
    /// texture bitmap containing a 16x16 grid of fixed-width characters,
    /// representing the ASCII table. A 32 bit texture is assumed, aswell as
    /// all GL state necessary to turn on texturing. The dimension of the
    /// texture bitmap may be anything from 128x128 to 512x256 or any other
    /// order-by-two-squared-dimensions.
    /// </summary>
    public TextureFont(int textureId)
    {
      this.textureId = textureId;
    }

		#endregion Constructors 

		#region Methods (5) 

		// Public Methods (3) 

//{ get { return 1.0 - borderY * 2; } set { borderY = (1.0 - value) / 2.0; } }
    /// <summary>
    /// Computes the expected width of text string given. The height is always 1.0.
    /// Model space coordinates.
    /// </summary>
    public double ComputeWidth(string text)
    {
      return text.Length * AdvanceWidth;
    }

    /// <summary>
    /// Draw an ASCII string around coordinate (0,0,0) in the XY-plane of the
    /// model space coordinate system. The height of the text is 1.0.
    /// The width may be computed by calling ComputeWidth(string).
    /// This call modifies the currently bound
    /// 2D-texture, but no other GL state.
    /// </summary>
    public void WriteString(string text)
    {
      GL.BindTexture(TextureTarget.Texture2D, textureId);
      GL.PushMatrix();
      double width = ComputeWidth(text);
      GL.Translate(-width / 2.0, -0.5, 0);
      GL.Begin(BeginMode.Quads);
      double xpos = 0;
      foreach (var ch in text)
      {
        WriteCharacter(ch, xpos);
        xpos += AdvanceWidth;
      }
      GL.End();
      GL.PopMatrix();
    }

    /// <summary>
    /// This is a convenience function to write a text string using a simple coordinate system defined to be 0..100 in x and 0..100 in y.
    /// For example, writing the text at 50,50 means it will be centered onscreen. The height is given in percent of the height of the viewport.
    /// No GL state except the currently bound texture is modified. This method is not as flexible nor as fast
    /// as the WriteString() method, but it is easier to use.
    /// </summary>
    public void WriteStringAt(
      string text,
      double heightPercent,
      double xPercent,
      double yPercent,
      double degreesCounterClockwise)
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Ortho(0, 100, 0, 100, -1, 1);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Translate(xPercent, yPercent, 0);
      double aspectRatio = ComputeAspectRatio();
      GL.Scale(aspectRatio * heightPercent, heightPercent, heightPercent);
      GL.Rotate(degreesCounterClockwise, 0, 0, 1);
      WriteString(text);
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Projection);
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Modelview);

    }
		// Private Methods (2) 

    /// <summary>
    /// Computes the aspect ratio.
    /// </summary>
    /// <returns></returns>
    private static double ComputeAspectRatio()
    {
      int[] viewport = new int[4];
      GL.GetInteger(GetPName.Viewport, viewport);
      int w = viewport[2];
      int h = viewport[3];
      double aspectRatio = (float)h / (float)w;
      return aspectRatio;
    }

    /// <summary>
    /// Writes the character.
    /// </summary>
    /// <param name="ch">The ch.</param>
    /// <param name="xpos">The xpos.</param>
    private void WriteCharacter(char ch, double xpos)
    {
      byte ascii;
      unchecked { ascii = (byte)ch; }

      int row = ascii >> 4;
      int col = ascii & 0x0F;

      double centerx = (col + 0.5) * Sixteenth;
      double centery = (row + 0.5) * Sixteenth;
      double halfHeight = CharacterBoundingBoxHeight * Sixteenth / 2.0;
      double halfWidth = CharacterBoundingBoxWidth * Sixteenth / 2.0;
      double left = centerx - halfWidth;
      double right = centerx + halfWidth;
      double top = centery - halfHeight;
      double bottom = centery + halfHeight;

      GL.TexCoord2(left, top); GL.Vertex2(xpos, 1);
      GL.TexCoord2(right, top); GL.Vertex2(xpos + 1, 1);
      GL.TexCoord2(right, bottom); GL.Vertex2(xpos + 1, 0);
      GL.TexCoord2(left, bottom); GL.Vertex2(xpos, 0);
    }

		#endregion Methods 
  }

}
