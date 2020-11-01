using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GraphicsTests
{
    public partial class Form1 : Form
    {
        
        dynamic GetDc()
        {
            var bmp = new Bitmap(1, 1);
            var g = Graphics.FromImage(bmp);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("Drawing")).ToList();
            var type = assemblies.SelectMany(x => x.DefinedTypes.Where(t => t.FullName == "System.Drawing.Internal.DeviceContext")).First();

            var contextTypes = assemblies.SelectMany(x => x.DefinedTypes.Where(t => t.FullName == "System.Drawing.Internal.DeviceContexts")).First();
            var contextsField = contextTypes.GetField("activeDeviceContexts", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            var contexts = contextsField.GetValue(null);


            var frm = this;
            var hwnd = frm.Handle;
            var m = type.GetMethod("FromHwnd");
            var m2 = type.GetField("hCurrentBmp", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
         
       
            dynamic result = m.Invoke(null, new object[] { hwnd });
            var Hdc = result.Hdc;
            var bmpHwnd = m2.GetValue(result);
            var ip = new IntPtr(0x0005004a);
            var bm2 = Bitmap.FromHbitmap(bmpHwnd);
            this.pictureBox1.Image = bm2;
            return result;





        }
        public Form1()
        {
            InitializeComponent();
            GetDc();
            //processList.SelectedValueChanged += ProcessList_SelectedValueChanged;
            //bindProcesses();

        }


        private void bindProcesses()
        {

            this.processList.Items.Clear();
            processList.Items.Add(new ProcessInfo());
            foreach (var process in Process.GetProcesses().OrderBy(x => x.ProcessName))
            {

                try
                {
                    var hnd = process.Handle;
                    this.processList.Items.Add(new ProcessInfo(process));
                }
                catch { }
            }

            //processList.DisplayMember = "Name";
            var currentId = Process.GetCurrentProcess().Id;
            this.pictureBox1.Image = new Bitmap(1024, 768);
            this.DrawToBitmap((Bitmap)this.pictureBox1.Image, new Rectangle(0, 0, 1024, 768));
            foreach (ProcessInfo processInfo in this.processList.Items)
            {
                if (processInfo.Id == currentId)
                {
                    // processList.SelectedItem = processInfo;
                }
            }
        }

        private void ProcessList_SelectedValueChanged(object sender, EventArgs e)
        {
            ProcessInfo processInfo = (ProcessInfo)this.processList.SelectedItem;
            if (processInfo != null)
            {
                try
                {
                    var handle = processInfo.Process?.Handle;
                    if (handle != null)
                    {
                        var source = Bitmap.FromHbitmap(handle.Value);
                    
                        using (var g = this.pictureBox1.CreateGraphics())
                        {
                          
                            g.DrawImage(source, 0, 0);
                        }
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.Message);

                }


            }
        }
    }

    public class ProcessInfo
    {
        public Process Process { get; private set; }
        public int Id { get; private set; }
        public string Name { get; private set; }

        public ProcessInfo()
        {

        }
        public ProcessInfo(Process process)
        {
            this.Process = process;
            this.Id = process.Id;
            this.Name = process.ProcessName;
        }
        public override string ToString()
        {
            return Process == null ? "" : $"{Id} - {Name}";
        }
    }
}
