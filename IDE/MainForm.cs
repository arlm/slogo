using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using TurtleRuntime;
using SLogoRuntime;
using System.Drawing.Imaging;

namespace SLogo
{
    public partial class MainForm : Form
    {
        private Runtime logoRuntime = null;

        // The drawing surface is the canvas on which the
        // turtle draws.
        private Graphics drawingSurface = null;

        // We save the image of the turtle's tracks just
        // before we draw the turtle icon.  That way, when
        // the turtle moves, we don't have to worry show_help erasing
        // the icon.
        Image savedImage = null;

        private string currentFilename = String.Empty;
        
        private bool currentFileModified = false;

        private string defaultLogoContent = @"
to square :length
  repeat 4 [ fd :length rt 90 ]
end

to main
   clearscreen
   repeat 36 [ square 50 rt 10 ]
end
";

        public MainForm(string[] cmd)
        {
            logoRuntime = new Runtime(cmd);

            InitializeComponent();
            InitializeCanvas();
            LoadLogoSourceFile();
        }


        // This brings our application back to a clean state.
        // We create a fresh new canvas the same size as the current
        // form, create a new turtle to reference that canvas,
        // and draw the turtle.
        private void InitializeCanvas()
        {
            this.pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            this.pictureBox.Image = new Bitmap(logoRuntime.width, logoRuntime.height);
            drawingSurface = Graphics.FromImage(this.pictureBox.Image);
            drawingSurface.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            System.Diagnostics.Debug.WriteLine("turtle = new Turtle(drawingSurface);");
            logoRuntime.turtle = new Turtle(drawingSurface);
            logoRuntime.turtle.Reset();

            savedImage = new Bitmap(this.pictureBox.Image);
            logoRuntime.turtle.Draw();
        }

        private void LoadLogoSourceFile()
        {
            if (!String.IsNullOrEmpty(logoRuntime.source_file))
            {
                currentFilename = logoRuntime.source_file;
                lgoCode.Text = logoRuntime.source_code.Replace("\n", "\n\r");
                return;

            }

            lgoCode.Text = defaultLogoContent;  // No file ? OK.. Set default .. 
            currentFileModified = false;
            updateUI();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            splitContainer1.Dock = DockStyle.Fill;
            tabControl1.Dock = DockStyle.Fill;
            lgoCode.Dock = DockStyle.Fill;
            csCode.Dock = DockStyle.Fill;
            pictureBox.Dock = DockStyle.Fill;


            if (logoRuntime.cs_source_file == null)
                tabControl1.TabPages.Remove(tpCSharp);

            if (!String.IsNullOrEmpty(logoRuntime.code_2_run))
            {
                this.compileAndRunLogoCode();
                Close();
            }
        }

        private void justCompileLogoCode()
        {
            string errors;
            string code = logoRuntime.CompileToCs(lgoCode.Text, out errors);
            if (String.IsNullOrEmpty(errors))
            {
                csCode.Text = code.Replace("\n", "\r\n");
                MessageBox.Show("Compiled OK");
            }
            else
                MessageBox.Show(errors);
        }

        private void compileAndRunLogoCode()
        {
            string errors;
            string code = logoRuntime.CompileToCs(lgoCode.Text, out errors);
            if (String.IsNullOrEmpty(errors))
            {
                csCode.Text = code.Replace("\n", "\r\n");
                runCompiledLogoCode();
            }
            else
                MessageBox.Show(errors);
        }

        private void runCompiledLogoCode()
        {
            InitializeCanvas();
            drawingSurface.DrawImage(savedImage, 0, 0);
            string err = logoRuntime.RunCs(csCode.Text);
            if (err != null)
            {
                MessageBox.Show(err);
            }
            else
            {
                savedImage = new Bitmap(this.pictureBox.Image);
                logoRuntime.turtle.Draw();
                this.pictureBox.Refresh();
            }
        }


        private void updateUI()
        {
            tpLogo.Text = !String.IsNullOrEmpty(currentFilename) 
                                    ? Path.GetFileName(currentFilename)
                                    : "untitled.lgo";

            if (!String.IsNullOrEmpty(currentFilename))
            {
                this.Text = "SLogo - " + currentFilename;
                if (currentFileModified)
                    this.Text += "*";
            }
            else
                this.Text = "SLogo" + currentFilename;

            if (currentFileModified)
                tpLogo.Text += "*";
        }

        private void not_yet_supported()
        {
            MessageBox.Show("This feature is not yet implemented", "Sorry...", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentFilename = String.Empty;
            lgoCode.Text = String.Empty;
            currentFileModified = false;
            updateUI();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "LOGO files (*.lgo)|*.lgo|All files (*.*)|*.*";
            ofd.Multiselect = false;
            ofd.Title = "Please select logo file to load";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currentFilename = ofd.FileName;
                lgoCode.Text = File.ReadAllText(ofd.FileName);
                currentFileModified = false;
                updateUI();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(currentFilename))
            {
                File.WriteAllText(currentFilename, lgoCode.Text);
                currentFileModified = false;
                updateUI();
            }
            else
                saveAsToolStripMenuItem_Click(sender, e);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            ofd.Filter = "LOGO files (*.lgo)|*.lgo|All files (*.*)|*.*";
            ofd.Title = "Please select logo file to save";
            if (!String.IsNullOrEmpty(currentFilename))
                ofd.FileName = currentFilename;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                currentFilename = ofd.FileName;
                File.WriteAllText(ofd.FileName, lgoCode.Text);
                currentFileModified = false;
                updateUI();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFileModified)
            {
                if (MessageBox.Show("There are unsaved changes..\nAre you sure you want to exit ?", 
                                    "Exit !? ",
                                    MessageBoxButtons.YesNo, 
                                    MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            Close();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compileAndRunLogoCode();
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            not_yet_supported();
        }

        private void lgoCode_TextChanged(object sender, EventArgs e)
        {
            currentFileModified = true;
            updateUI();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            justCompileLogoCode();    
        }

        private void compileToEXEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            ofd.Filter = "EXE files (*.exe)|*.exe|All files (*.*)|*.*";
            ofd.Title = "Please select exe file to save";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string errors;
                string code = logoRuntime.CompileToCs(lgoCode.Text, out errors);

                if (String.IsNullOrEmpty(errors))
                {
                    errors = logoRuntime.MakeExe(code, ofd.FileName);

                    if (String.IsNullOrEmpty(errors))
                        MessageBox.Show("Compiled OK");
                    else
                        MessageBox.Show(errors);
                }
                else
                    MessageBox.Show(errors);


            }
        }

        private void goToWebSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://slogo.codeplex.com");  
        }

        private void aboutSLogoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            not_yet_supported(); 
        }

        private void saveOutputImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();

            ofd.Filter = @"BMP files (*.bmp)|*.bmp|"+
                         @"PNG files (*.png)|*.png|"+
                         @"JPG files (*.jpg)|*.jpg|"+
                         @"GIF files (*.gif)|*.gif|" +
                         @"All files (*.*)|*.*";

            ofd.Title = "Please select image file to save";
            ofd.FileName = Path.GetFileNameWithoutExtension( currentFilename );

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(ofd.FileName);
                ImageFormat imgf = ImageFormat.Bmp;

                if (!extension.ToLower().Equals(".bmp"))
                {
                    if (extension.ToLower().Equals(".png"))
                        imgf = System.Drawing.Imaging.ImageFormat.Png;
                    else if (extension.ToLower().Equals(".jpg"))
                        imgf = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (extension.ToLower().Equals(".jpeg"))
                        imgf = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (extension.ToLower().Equals(".gif"))
                        imgf = System.Drawing.Imaging.ImageFormat.Gif;
                }
                savedImage.Save(ofd.FileName, imgf);
            }
        }
    }
}