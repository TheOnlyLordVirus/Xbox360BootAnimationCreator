/// <summary>
/// Our projects namespace
/// </summary>
namespace BootAnimationCreator
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Threading;
    using DamienG.Security.Cryptography;

    /// <summary>
    /// The form container for everything.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The constructor to launch the form.
        /// </summary>
        public MainForm()
        {
            // Launches the form.
            InitializeComponent();
        }

        /// <summary>
        /// Does all of the work the user needs.
        /// </summary>
        private void loadVideoFileButton_Click(object sender, EventArgs e)
        {
            CreateXEX();
        }

        /// <summary>
        /// Calculates the Crc-32 of any file a user selects.
        /// </summary>
        private void crcButton_Click(object sender, EventArgs e)
        {
            string userFilePath = OpenFilePath("XeX", "bootanim");

            if(userFilePath != string.Empty)
            {
                // Calculate Crc-32
                crcTextBox.Text = CreateCrc32(File.ReadAllBytes(userFilePath));
            }

            else
            {
                crcTextBox.Text = "";

                // Error.
                MessageBox.Show("Error: File not found.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Loads a file.
        /// </summary>
        private string OpenFilePath(string fileType, string fileName)
        {
            openFile.InitialDirectory = @"c:\";
            openFile.Filter = fileType + " files (*." + fileType + ")|*." + fileType;
            openFile.FileName = fileName + "." + fileType;

            if (openFile.ShowDialog() == DialogResult.OK)
            {
                return openFile.FileName;
            }

            return string.Empty;
        }

        /// <summary>
        /// Saves a file.
        /// </summary>
        private string SaveFilePath(string fileType, string fileName, string startPath)
        {
            saveFile.InitialDirectory = startPath;
            saveFile.Filter = fileType + " files (*." + fileType + ")|*." + fileType;
            saveFile.FileName = fileName + "." + fileType;

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                return saveFile.FileName;
            }

            return startPath + @"\" + fileName + "." + fileType;
        }

        /// <summary>
        /// Creates a bootanim.xex file.
        /// </summary>
        private void CreateXEX()
        {
            // File info.
            DialogResult yesno = MessageBox.Show("Your boot animation must follow these constraints:\nIt must be a WMV 9 Pro video.\nIt must have a 1250 x 720p resolution.\nThe end bootanim.xex must be smaller than 61342345 bytes's.\nCurrently this only works with big block nands.\n\nDo you want to continue?", "Notice!", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

            // Very simple data validation.
            if (yesno == DialogResult.Yes)
            {
                string videoFilePath = OpenFilePath("wmv", "bootlogo");
                if (!videoFilePath.Equals(string.Empty))
                {
                    // Files as byte arrays.
                    byte[] templateBinary = Properties.Resources.template;
                    byte[] videoBinary = File.ReadAllBytes(videoFilePath);
                    byte[] nullBytes = Enumerable.Repeat<byte>(0x00, 16743).ToArray<byte>();

                    // The size of the bootanimXEX
                    int bootanimXEXsize = templateBinary.Length + videoBinary.Length + nullBytes.Length;

                    // Our final file as an byte array.
                    byte[] bootanimXEX = new byte[bootanimXEXsize];
                    byte[] videoSizeBigEndian = BitConverter.GetBytes(videoBinary.Length);

                    // Convert int to big endian.
                    // For some reason videoSizeBigEndian.Reverse(); was not working here???
                    Array.Reverse(videoSizeBigEndian, 0, videoSizeBigEndian.Length);

                    // Create the file.
                    templateBinary.CopyTo(bootanimXEX, 0);
                    videoSizeBigEndian.CopyTo(bootanimXEX, 0xf2c);
                    videoBinary.CopyTo(bootanimXEX, 0x237000);
                    nullBytes.CopyTo(bootanimXEX, 0x237000 + videoBinary.Length);

                    // Convert to Big Endian.
                    bootanimXEX.Reverse();

                    // Write file to user selected directory.
                    string bootAnimFilePath = SaveFilePath("xex", "bootanim", Path.GetDirectoryName(videoFilePath));
                    File.WriteAllBytes(bootAnimFilePath, bootanimXEX);

                    // Encrypt and compress for the user.
                    EncryptAndCompressXEX(Path.GetDirectoryName(bootAnimFilePath), Path.GetFileName(bootAnimFilePath));

                    // Calculate Crc-32
                    crcTextBox.Text = CreateCrc32(File.ReadAllBytes(bootAnimFilePath));

                    // Done!
                    MessageBox.Show("Your bootanim.xex has been created!\nXex Encryption thanks to: xorloser.", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                else
                {
                    // Error.
                    MessageBox.Show("Error: File not found.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Enrypt and Compress bootanim.xex
        /// Uses xextool.exe which is stored as a local resource,
        /// We then write, run, and delete these files in the current working directory.
        /// </summary>
        private void EncryptAndCompressXEX(string bootAnimFilePath, string bootaAnimFileName)
        {
            // Create xextool for encryption.
            File.WriteAllBytes(bootAnimFilePath + @"\xextool.exe", Properties.Resources.xextool);
            File.WriteAllBytes(bootAnimFilePath + @"\x360_imports.idc", Properties.Resources.x360_imports);

            // Where to start the xextool.exe, and what command to run.
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = bootAnimFilePath,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = bootAnimFilePath + @"\xextool.exe",
                RedirectStandardInput = true,
                UseShellExecute = false,
                Arguments = "-e e -c c " + bootaAnimFileName
            };

            // Run command.
            Process.Start(startInfo);

            // Delete Encryption files.
            // Add a sleep command to make sure the encryption proccess finishes before we delete the programs.
            Thread.Sleep(5000);
            File.Delete(bootAnimFilePath + @"\xextool.exe");
            File.Delete(bootAnimFilePath + @"\x360_imports.idc");
        }

        /// <summary>
        /// Creates a crc-32 as a string of a file of your choosing.
        /// </summary>
        /// <param name="file">The file you want to check for a Crc-32</param>
        /// <returns>Crc32 as a sting.</returns>
        private string CreateCrc32(byte[] file)
        {
            Crc32 crc32 = new Crc32();
            String hash = String.Empty;

            foreach (byte b in crc32.ComputeHash(file))
            {
                hash += b.ToString("x2").ToLower();
            }

            return hash;
        }
    }
}
