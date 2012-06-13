using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PISDK;
using System.Threading;
using System.IO.Ports;

namespace H4ckuino
{
    public partial class Form1 : Form
    {

        SerialPort serialPort;
        PIPoint piPoint;
        PISDK.PISDK piSDK;
        PISDK.Server piServer;
        string tagName = "HackathonRampData";
        string serverName = "piserver1";

        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            serialPort = new SerialPort();
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            foreach (string port in ports)
            {
                comComboBox.Items.Add(port);
            }
            piSDK = new PISDK.PISDKClass();
            piServer = piSDK.Servers[serverName];
            piServer.Open("");
            if (piServer.Connected)
                Console.WriteLine("Connected to Server");
            piPoint = readPIPoint(piServer, tagName);
        }

        private void writePI(PIPoint piPoint, int index)
        {
            piPoint.Data.UpdateValue(index, 0, DataMergeConstants.dmReplaceDuplicates);
        }

        private PIPoint readPIPoint(PISDK.Server piServer, string tagName)
        {
            return piSDK.GetPoint("\\\\" + piServer.Path.ToString() + "\\" + tagName);
        }

        private void comComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Allow the user to set the appropriate properties.
            serialPort.PortName = comComboBox.SelectedItem.ToString();
            serialPort.BaudRate = 9600;
            serialPort.Open();
            if(serialPort.IsOpen)
                Console.WriteLine(serialPort.PortName + " is open");
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            serialPort.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            int intData;
            if(Int32.TryParse(data, out intData)) {
                Console.Write("Data Received:");
                Console.WriteLine(data);
                writePI(piPoint, intData);
            }
        }
    }
}
