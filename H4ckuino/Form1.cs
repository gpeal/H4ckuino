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
        string tagName;
        string serverName;

        public Form1()
        {
            InitializeComponent();
            string[] ports = SerialPort.GetPortNames();
            serialPort = new SerialPort();
            foreach (string port in ports)
            {
                comComboBox.Items.Add(port);
            }
            comComboBox.SelectedIndex = 0;
            serverTextBox.Text = "piserver1";
            tagTextBox.Text = "HackathonRampData";
        }

        private void writePI(PIPoint piPoint, int data)
        {
            if (piPoint != null)
            {
                piPoint.Data.UpdateValue(data, 0, DataMergeConstants.dmReplaceDuplicates);
                Console.WriteLine("Sent " + data + " over PI System");
            }
        }

        private PIPoint readPIPoint(PISDK.Server piServer, string tagName)
        {
            return piSDK.GetPoint("\\\\" + piServer.Path.ToString() + "\\" + tagName);
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            serialPort.Close();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string data = serialPort.ReadExisting();
            char c = data[0];
            //foreach (char c in data)
            //{
                if(c == 10)
                    return;
                int intData = (int)c;
                Console.Write("Data Received:");
                Console.WriteLine(intData);
                writePI(piPoint, intData);
            //}
        }

        private void gameTimeButton_click(object sender, EventArgs e)
        {
            // Allow the user to set the appropriate properties.
            if (serialPort.IsOpen)
                serialPort.Close();
            serialPort.PortName = comComboBox.SelectedItem.ToString();
            serialPort.BaudRate = 9600;
            serialPort.Open();
            //send the Arduino an 'A' to get it going :)
            serialPort.Write("A");
            if (serialPort.IsOpen)
                Console.WriteLine(serialPort.PortName + " is open");
            serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            serverName = serverTextBox.Text;
            tagName = tagTextBox.Text;
            piSDK = new PISDK.PISDKClass();
            piServer = piSDK.Servers[serverName];
            if (piServer.Connected)
                Console.WriteLine("Connected to Server");
            piPoint = readPIPoint(piServer, tagName);
        }
    }
}
