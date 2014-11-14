public class LogoProgram : TurtleRuntime.CompiledLogoProgram
{
	/*CompiledCode*/
}

static class Program
{
	static void Main()
	{
		System.Windows.Forms.Application.EnableVisualStyles();
		System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
		System.Windows.Forms.Application.Run(new LogoForm());
	}
}

public class LogoForm : System.Windows.Forms.Form
{
	private System.Windows.Forms.PictureBox pictureBox = null;
	private System.Drawing.Graphics drawingSurface = null;
	private TurtleRuntime.Turtle turtle = null;

	public LogoForm()
	{
		this.Text = "A compile logo program";
		this.pictureBox = new System.Windows.Forms.PictureBox();
		this.pictureBox.BackColor = System.Drawing.Color.Yellow;
		this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.Controls.Add(this.pictureBox);
		this.Load += new System.EventHandler(this.LogoForm_Load);

		// InitializeCanvas();
		this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.pictureBox.Image = new System.Drawing.Bitmap(/*Width*/, /*Height*/);
		drawingSurface = System.Drawing.Graphics.FromImage(this.pictureBox.Image);
		drawingSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
		turtle = new TurtleRuntime.Turtle(drawingSurface);
		turtle.Reset();
		turtle.Draw();
	}
	
	private void LogoForm_Load(object sender, System.EventArgs e)
	{
		LogoProgram lp = new LogoProgram();
		lp._initialize(turtle);
		lp.main();
	}
}