/*
 * Created by SharpDevelop.
 * User: ygsong
 * Date: 2017-11-24
 * Time: 오전 11:41
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using canlibCLSNET;
using System.ComponentModel;
using System.Threading;
using EmulatorController;

namespace Emulator_Controller
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
    {
        private int sendHandle = -1;
        private int channel = -1;
        private string channelName = "";
        private int readHandle = -1;
        private bool isSenderBusOn = false;
        private bool isReaderBusOn = false;
        private readonly BackgroundWorker messageReceiver;
        TextBox[] canDataBoxes = null;
        const int CAN_DATA_BOX_NUM = 8;
        
        StringBuilder logStrBuilder = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
            this.canDataBoxes = new TextBox[CAN_DATA_BOX_NUM];
            for(int index = 0; index < CAN_DATA_BOX_NUM; index++)
            {
            	TextBox box = this.canDataBoxes[index] = new TextBox();
            	box.Height = 23;
            	box.Width = 28;
            	box.MaxLength = 3;
            	box.VerticalAlignment = VerticalAlignment.Top;
            	box.HorizontalAlignment = HorizontalAlignment.Left;
            	box.PreviewTextInput += CheckTextBox;
            	box.Margin = new Thickness(0, 0, 10, 0);
            	box.Name = String.Format("dataBox{0}", index);
            	canDataBoxPanel.Children.Add(box);
            }

            this.Title += " v" + Utils.GetApplicationFileVersion();
            
            //outputLogTextBox.ContextMenuOpening += new ContextMenuEventHandler(outputLogTextBox_ContextMenuOpening);

            //Sets up a BackgroundWorker and adds delegates to 
            //the DumpMessageLoop and ProcessMessage methods
            messageReceiver = new BackgroundWorker();
            messageReceiver.DoWork += this.Receiver_MessageHandlerLoop;
            messageReceiver.WorkerReportsProgress = true;
            messageReceiver.ProgressChanged += new ProgressChangedEventHandler(this.Process_readerMessageProgress);
        }
        
        void onWindowLoaded(object sender, RoutedEventArgs e)
        {
            statusText.Text = "CAN channel Not initialized";
        }
        
		void onWindowClosing(object sender, CancelEventArgs e)
		{
			EmulMessageBuilder.GetBuilder().DestroyBuilder();
		}

        //Initializes Canlib
        void initButton_Click(object sender, RoutedEventArgs e)
        {
            Canlib.canInitializeLibrary();
            statusText.Text = "CAN channel initialized";
            initButton.IsEnabled = false;
            openChannelButton.IsEnabled = true;
            //channelBox.IsEnabled = true;
            channelComboBox.IsEnabled = true;
        }

        //Opens the channel selected in the "Channel" input box
        void openChannelButton_Click(object sender, RoutedEventArgs e)
        {
            //channel = Convert.ToInt32(channelBox.Text);
            channel = Convert.ToInt32(channelComboBox.SelectedIndex);
            channelName = ((ComboBoxItem)channelComboBox.SelectedItem).Content.ToString();
            
            //Get a handle for sending
            sendHandle = Canlib.canOpenChannel(channel, Canlib.canOPEN_REQUIRE_EXTENDED | Canlib.canOPEN_ACCEPT_VIRTUAL);
            //Get a handle for reading
            readHandle = Canlib.canOpenChannel(channel, Canlib.canOPEN_REQUIRE_EXTENDED | Canlib.canOPEN_ACCEPT_VIRTUAL);
            
            CheckStatus("Opening channel " + channelName, (Canlib.canStatus)sendHandle);
            if (sendHandle >= 0 && readHandle >= 0)
            {	                        
                openChannelButton.IsEnabled = false;
                //channelBox.IsEnabled = false;
                channelComboBox.IsEnabled = false;
                closeChannelButton.IsEnabled = true;
                busOnButton.IsEnabled = true;
           		bitrateComboBox.IsEnabled = true;
            }
            else
            {
            	MessageBox.Show("Opening Channel " + channelName + " failed. Please try again.");
            }
        }

        void closeChannelButton_Click(object sender, RoutedEventArgs e)
        {
        	if (sendHandle >= 0 && readHandle >= 0)
	        {
        		if(isSenderBusOn || isReaderBusOn)
        			busOffButton_Click(sender, e);
	            Canlib.canStatus senderStatus = Canlib.canClose(sendHandle);
	            Canlib.canStatus readerStatus = Canlib.canClose(readHandle);
	            CheckStatus("Closing channel " + channelName, senderStatus);
	            sendHandle = -1;
	            readHandle = -1;
				closeChannelButton.IsEnabled = false;
	            busOnButton.IsEnabled = false;
	            busOffButton.IsEnabled = false;
	           	bitrateComboBox.IsEnabled = false;
	           	sendMessageButton.IsEnabled = false;
	           	clearMessageButton.IsEnabled = false;
	       		buildMessageButton.IsEnabled = false;
				openChannelButton.IsEnabled = true;
				//channelBox.IsEnabled = true;
				channelComboBox.IsEnabled = true;
        	}
            else
            {
            	MessageBox.Show("Closing Channel " + channelName + " failed. Please try again.");
            }
        }

        //Goes on bus
        void busOnButton_Click(object sender, RoutedEventArgs e)
        {
        	//Set Bitrate
            int[] bitrates = new int[] { Canlib.canBITRATE_125K, Canlib.canBITRATE_250K, 
                                            Canlib.canBITRATE_500K, Canlib.BAUD_1M };
            Canlib.canStatus senderStatus = Canlib.canSetBusParams(sendHandle, bitrates[bitrateComboBox.SelectedIndex], 0, 0, 0, 0, 0);
            Canlib.canStatus readerStatus = Canlib.canSetBusParams(readHandle, bitrates[bitrateComboBox.SelectedIndex], 0, 0, 0, 0, 0);

            CheckStatus("Setting bitrate to " + ((ComboBoxItem)bitrateComboBox.SelectedValue).Content, senderStatus);
            if (senderStatus != Canlib.canStatus.canOK || readerStatus != Canlib.canStatus.canOK)
            {
            	return;
            }
            
            //Bus On
            senderStatus = Canlib.canBusOn(sendHandle);
            readerStatus = Canlib.canBusOn(readHandle);
            CheckStatus("Bus on with bitrate " + ((ComboBoxItem)bitrateComboBox.SelectedValue).Content, senderStatus);
            if (senderStatus == Canlib.canStatus.canOK && readerStatus == Canlib.canStatus.canOK)
            {
                isSenderBusOn = true;
	            isReaderBusOn = true;

                //This starts the DumpMessageLoop method
                if (!messageReceiver.IsBusy)
                {
                    messageReceiver.RunWorkerAsync();
                }
                
           		busOnButton.IsEnabled = false;
           		bitrateComboBox.IsEnabled = false;
           		busOffButton.IsEnabled = true;
           		sendMessageButton.IsEnabled = true;
           		clearMessageButton.IsEnabled = true;
           		buildMessageButton.IsEnabled = true;
            }
            else
            {
            	MessageBox.Show("Bus On failed. Please try again.");
            }
        }
        
        void busOffButton_Click(object sender, RoutedEventArgs e)
        {
            Canlib.canStatus senderStatus = Canlib.canBusOff(sendHandle);
            Canlib.canStatus readerStatus = Canlib.canBusOff(readHandle);
            CheckStatus("Bus off", senderStatus);
            if (senderStatus == Canlib.canStatus.canOK && readerStatus == Canlib.canStatus.canOK)
            {
	            isSenderBusOn = false;
	            isReaderBusOn = false;
	            
	           	busOffButton.IsEnabled = false;
	           	sendMessageButton.IsEnabled = false;
	           	clearMessageButton.IsEnabled = false;
	       		buildMessageButton.IsEnabled = false;
	           	busOnButton.IsEnabled = true;
	           	bitrateComboBox.IsEnabled = true;
            }
            else
            {
            	MessageBox.Show("Bus Off failed. Please try again.");
            }
        }

        /*
         * Looks for messages and sends them to the output box. 
         */
        void Receiver_MessageHandlerLoop(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Canlib.canStatus status = Canlib.canStatus.canERR_NOMSG;
            bool noError = true;
            int errorFrameCount = 0;
			
            //Create a Windows event handle
            Object winHandle = new IntPtr(-1);
            status = Canlib.canIoCtl(readHandle, Canlib.canIOCTL_GET_EVENTHANDLE, ref winHandle);
            if (status != Canlib.canStatus.canOK)
            {
            	worker.ReportProgress((int)ReceiverReport.ERROR_STOP, status);
                noError = false;
            }

            WaitHandle waitHandle = (WaitHandle) new CanlibWaitEvent(winHandle);

            while (noError && isReaderBusOn && readHandle >= 0)
            {
                //Wait for 25ms for an event to occur on the channel (BBMS CAN communication period: 25ms)
                bool eventHappened = waitHandle.WaitOne(25);
                if (!eventHappened)
                {
                    continue;
                }
                
				int id;
				byte[] data = new byte[Defined.MAX_DLC];
				int dlc;
				int flags;
				long time;
            	string msgLog;
                
                //To attain the initial msg
                status = Canlib.canRead(readHandle, out id, data, out dlc, out flags, out time);

                //This while loop is to repeat canRead() until the msg boxes for readHandle becomes exhausted.
                while (status == Canlib.canStatus.canOK)
                {
                    if ((flags & Canlib.canMSG_ERROR_FRAME) == Canlib.canMSG_ERROR_FRAME)
                    {
                    	msgLog = "***ERROR FRAME RECEIVED*** Count = " + (++errorFrameCount);
                        if(errorFrameCount > Defined.ERROR_FRAME_ENDURANCE_COUNT_LIMIT)
                        {
                        	worker.ReportProgress((int)ReceiverReport.REPORT_RUNNING, "Too many error frames received. Bus off.");
                        	break;
                        }
                    }
                    else
                    {
                        msgLog = String.Format("0x{0:x3}  {1}  {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}   @{10}",
                                                 id, dlc, data[0], data[1], data[2], data[3], data[4],
                                                 data[5], data[6], data[7], time);
                    }

                    //Sends the message to the ProcessMessage method
                    worker.ReportProgress((int)ReceiverReport.REPORT_RUNNING, "[Rx] " + msgLog);
                    
                    //Result status of the following canRead() will finish the while loop if status!=Canlib.canStatus.canOK
                    status = Canlib.canRead(readHandle, out id, data, out dlc, out flags, out time);
                }
                
                //If the reason to exit the while loop of canRead() was not canERR_NOMSG, an error is expected.
                if (status != Canlib.canStatus.canERR_NOMSG)
                {
                    //Sends the error status to the ProcessMessage method and breaks the loop
                    worker.ReportProgress((int)ReceiverReport.ERROR_STOP, status);
                    noError = false;
                }
            }
			if(isReaderBusOn)
			{
				worker.ReportProgress((int)ReceiverReport.FORCED_STOP, "Force Bus Off");
			}
        }

        /*
         * Adds the messages to the output box. This method is invoked in MainWindow process.
         */
        void Process_readerMessageProgress(object sender, ProgressChangedEventArgs e)
        {
        	ReceiverReport progress = (ReceiverReport)e.ProgressPercentage;
        	switch(progress)
        	{
				case ReceiverReport.REPORT_RUNNING:
					//LogOutput((string)e.UserState);
					//Console.WriteLine("{0}", (string)e.UserState);
					logStrBuilder.Append("\n" + DateTime.Now + " : " + (string)e.UserState);
					break;
					
				case ReceiverReport.ERROR_STOP:
					CheckStatus("Reading", (Canlib.canStatus)e.UserState);
					break;
				
				case ReceiverReport.FORCED_STOP:            	
					CheckStatus((string)e.UserState, Canlib.canStatus.canOK);
					forceAllBusOff();
					break;
					
				default:
					break;
        	}
        }

        /*
         * Makes sure that only positive integers can be used as input
         */
        void CheckTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !isInt(e.Text);
        }

        private bool isInt(String textStr)
        {
            foreach (char c in textStr)
            {
                if (!char.IsDigit(c))
                {
                    return false;
                }
            }
            return true;
        }

        /*
         * Updates the status bar, prints error message if something goes wrong
         */
        private void CheckStatus(String action, Canlib.canStatus status)
        {
            if (status != Canlib.canStatus.canOK)
            {
                String errorText = "";
                Canlib.canGetErrorText(status, out errorText);
                statusText.Text = action + " failed: " + errorText;
            }
            else
            {
                statusText.Text = action + " succeeded";
            }
        }

		private void LogOutput(string log)
		{
            outputLogTextBox.AppendText("\n" + DateTime.Now + " : " + log);
            outputLogTextBox.ScrollToEnd();
		}

		void buildMessageButton_Click(object sender, RoutedEventArgs e)
        {
            EmulMessage msg = null;
            EmulatorResult result = EmulMessageBuilder.GetBuilder().BuildMessageWithSelectedOptions(ref msg);

            if(result.Equals(EmulatorResult.EMULATOR_RESULT_SUCCESS) && (msg != null))
            {
            	updateMessageFields(msg);
            }
            else if(result.Equals(EmulatorResult.EMULATOR_RESULT_CANCELLED))
            {
            	//MessageBox.Show("Building message cancelled");
            }
            else
            {
            	MessageBox.Show("Building message failed");
            }
		}
		
		/* Copy the composed result from the MessageBuilderWindow/ */
		void updateMessageFields(EmulMessage msg)
		{
			if(msg != null)
			{
				idBox.Text = msg.ID;
				DLCBox.Text = msg.DLC.ToString();
	            for(int i = 0; i < msg.DLC; i++)
	            {
	            	canDataBoxes[i].Text = msg.DATA[i].ToString();
	            }
			}
		}
		
        //Reads message data from user input and writes a message to the channel, sending to the CAN bus.
        void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
        	if(String.IsNullOrEmpty(idBox.Text))
        	{
        		MessageBox.Show("Tx Failed: CAN message ID has not been specified.");
        		return;
        	}
        	if(String.IsNullOrEmpty(DLCBox.Text))
        	{
        		MessageBox.Show("Tx Failed: DLC value has not been specified.");
        		return;
        	}
        	
        	string idStr_decimal = Utils.ConvertHexStrToDecStr(idBox.Text);
            int id_decimal = Convert.ToInt32(idStr_decimal);

            int dlc = Convert.ToInt32(DLCBox.Text);
            
            byte[] data = new byte[Defined.MAX_DLC];
            Array.Clear(data, 0, sizeof(byte)*Defined.MAX_DLC);
            for(int i = 0; i < dlc; i++)
            {
                data[i] = canDataBoxes[i].Text == "" ? (byte) 0 : Convert.ToByte(canDataBoxes[i].Text);
            }
            
			int messageOptionFlags = 0;
			if(messageOptionCheckboxPanel.IsVisible)
            {
	            foreach(CheckBox box in messageOptionCheckboxPanel.Children)
	            {
	            	if (box.IsChecked.Value)
	                {
	                    messageOptionFlags += Convert.ToInt32(box.Tag);
	                }
	            }
            }

            string msgLog = String.Format("0x{0:x3}  {1}  {2:x2} {3:x2} {4:x2} {5:x2} {6:x2} {7:x2} {8:x2} {9:x2}   to handle {10}",
                                          idBox.Text, DLCBox.Text, data[0], data[1], data[2], data[3], data[4],
                                             data[5], data[6], data[7], sendHandle);
            Canlib.canStatus status = Canlib.canWrite(sendHandle, id_decimal, data, dlc, messageOptionFlags);
            LogOutput("[Tx] " + msgLog);
            CheckStatus("[Tx] " + msgLog, status);
        }
        
        //Erase all the fields of composed CAN message. Just for the user's convenience.
        void clearMessageButton_Click(object sender, RoutedEventArgs e)
        {
        	idBox.Clear();
        	DLCBox.Clear();
        	foreach(TextBox box in canDataBoxes)
            {
        		box.Clear();
            }
        }
        
        /* This method isn't working yet */
        void outputLogTextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
        	RichTextBox rtb = sender as RichTextBox;
        	if(rtb == null)
        		return;
        	
        	rtb.ContextMenu.PlacementTarget = rtb;
        	
        	// This uses HorizontalOffset and VerticalOffset properties to position the menu,
            // relative to the upper left corner of the parent element (RichTextBox in this case).
        	rtb.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.RelativePoint;
        	
        	// Compute horizontal and vertical offsets to place the menu relative to selection end.
        	TextPointer textPosition = rtb.Selection.End;
        	if(textPosition == null)
        		return;
        	Rect textPositionRect = textPosition.GetCharacterRect(LogicalDirection.Forward);
        	rtb.ContextMenu.HorizontalOffset = textPositionRect.X;
        	rtb.ContextMenu.VerticalOffset = textPositionRect.Y;
        	
        	// Finally, mark the event has handled.
        	rtb.ContextMenu.IsOpen = true;
        	e.Handled = true;
        }
        
        /* This method is invoked when application need to Bus off without user's click (on the 'Bus Off' button). */
        void forceAllBusOff()
        {
	    	Canlib.canStatus senderStatus = Canlib.canBusOff(sendHandle);
        	Canlib.canStatus readerStatus = Canlib.canBusOff(readHandle);
            //CheckStatus("Bus off", status);
            if (senderStatus == Canlib.canStatus.canOK && readerStatus == Canlib.canStatus.canOK)
            {
	            isSenderBusOn = false;
	            isReaderBusOn = false;
	            
	           	busOffButton.IsEnabled = false;
	           	sendMessageButton.IsEnabled = false;
	           	clearMessageButton.IsEnabled = false;
	       		buildMessageButton.IsEnabled = false;
	           	busOnButton.IsEnabled = true;
	           	bitrateComboBox.IsEnabled = true;
	           	
	           	MessageBox.Show("Forced Bus off succeeded. Go Bus on again.");
            }
            else
            {
            	MessageBox.Show("Forced Bus off failed. Restart application again.");
            	this.Close();
            }
        }
    }
}