using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            LoadProcesses(); 
        }

        private void button1_Click(object sender, EventArgs e)
        {
       
            string selectedProcessName = listBox1.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedProcessName))
            {
                MessageBox.Show("Please select a process to terminate.");
                return;
            }

            const uint TH32CS_SNAPPROCESS = 0x00000002;
            IntPtr snapshot = WinApiClass.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (snapshot == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create snapshot.");
                return;
            }

            WinApiClass.PROCESSENTRY32 processEntry = new WinApiClass.PROCESSENTRY32();
            processEntry.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinApiClass.PROCESSENTRY32));

            int processId = 0;
            if (WinApiClass.Process32First(snapshot, ref processEntry))
            {
                do
                {
                    if (processEntry.szExeFile == selectedProcessName)
                    {
                        processId = (int)processEntry.th32ProcessID;
                        break;
                    }
                } while (WinApiClass.Process32Next(snapshot, ref processEntry));
            }

            if (processId == 0)
            {
                MessageBox.Show("Failed to find the process.");
                return;
            }

            const int PROCESS_TERMINATE = 0x0001; 
            IntPtr processHandle = WinApiClass.OpenProcess(PROCESS_TERMINATE, false, processId);
            if (processHandle == IntPtr.Zero)
            {
                MessageBox.Show("Failed to open the process.");
                return;
            }

            bool result = WinApiClass.TerminateProcess(processHandle, 0);
            if (!result)
            {
                MessageBox.Show("Failed to terminate the process.");
            }
            else
            {
                MessageBox.Show("Process terminated successfully.");
                listBox1.Items.Remove(selectedProcessName);
            }

            WinApiClass.CloseHandle((uint)processHandle); 
        }

        private void LoadProcesses()
        {
            const uint TH32CS_SNAPPROCESS = 0x00000002;

            IntPtr snapshot = WinApiClass.CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
            if (snapshot == IntPtr.Zero)
            {
                MessageBox.Show("Failed to create snapshot.");
                return;
            }

            WinApiClass.PROCESSENTRY32 processEntry = new WinApiClass.PROCESSENTRY32();
            processEntry.dwSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(typeof(WinApiClass.PROCESSENTRY32));

            List<string> processes = new List<string>();

            if (WinApiClass.Process32First(snapshot, ref processEntry))
            {
                do
                {
                    processes.Add(processEntry.szExeFile);
                } while (WinApiClass.Process32Next(snapshot, ref processEntry));
            }
            else
            {
                MessageBox.Show("Failed to retrieve process information.");
            }

            processes.Sort(); 
            listBox1.Items.AddRange(processes.ToArray()); 
        }


        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
