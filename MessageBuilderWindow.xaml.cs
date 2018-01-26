/*
 * Created by SharpDevelop.
 * User: ygsong
 * Date: 01/02/2018
 * Time: 12:21
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
using System.Windows.Controls.Primitives;

namespace Emulator_Controller
{
	/// <summary>
	/// Interaction logic for MessageBuilderWindow.xaml
	/// </summary>
	public partial class MessageBuilderWindow : Window
	{
		public enum WindowStatus {
			UNINITIALIZED = 0,
			OPENED,
			NORMALLY_CLOSED,
			CANCEL_CLOSED
		};
		public WindowStatus windowStatus { set; get; }
			
        EmulatorCommandComboBoxItem[] commandTypeComboBoxItemList = {
			new EmulatorCommandComboBoxItem("command1", "Battery Status Control", EmulatorCommandType.EMUL_COMMAND_BATTERY_STATUS),
			new EmulatorCommandComboBoxItem("command2", "Rack Feedback Control", EmulatorCommandType.EMUL_COMMAND_FEEDBACK_STATUS),
			new EmulatorCommandComboBoxItem("command3", "Rack Component Control", EmulatorCommandType.EMUL_COMMAND_COMPONENT_STATUS),
			new EmulatorCommandComboBoxItem("command4", "Rack Diagnosis Control", EmulatorCommandType.EMUL_COMMAND_DIAGNOSIS_STATUS),
			new EmulatorCommandComboBoxItem("command5", "Rack Feedback Check", EmulatorCommandType.EMUL_REQUEST_FEEDBACK_STATUS),
			new EmulatorCommandComboBoxItem("command6", "Rack Diagnosis Check", EmulatorCommandType.EMUL_REQUEST_DIAGNOSIS_STATUS)
		};        
        EmulatorElementComboBoxItem[] elementTypeComboBoxItemList = {
        	new EmulatorElementComboBoxItem("element1", "Rack SOC", EmulatorBatteryStatusElement.EMUL_RACK_SOC),
        	new EmulatorElementComboBoxItem("element2", "Rack SOH", EmulatorBatteryStatusElement.EMUL_RACK_SOH),
        	new EmulatorElementComboBoxItem("element3", "Rack Current", EmulatorBatteryStatusElement.EMUL_RACK_CURRENT),
        	new EmulatorElementComboBoxItem("element4", "Min/Max Cell Voltage", EmulatorBatteryStatusElement.EMUL_RACK_CV),
        	new EmulatorElementComboBoxItem("element5", "Min/Max Module Temperature", EmulatorBatteryStatusElement.EMUL_RACK_TEMPERATURE),
        	new EmulatorElementComboBoxItem("element6", "MBMS Power", EmulatorBatteryStatusElement.EMUL_RACK_MBMS_ON),
        	new EmulatorElementComboBoxItem("element7", "Cell Balancing Flag", EmulatorBatteryStatusElement.EMUL_RACK_CELL_BALANCING_ON),
        	new EmulatorElementComboBoxItem("element8", "PPS Flag", EmulatorBatteryStatusElement.EMUL_RACK_PPS)
        };
        EmulatorFeedbackRadioButton[] feedbackRadioButtonList = {
        	new EmulatorFeedbackRadioButton("feedback1", "MC+", EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_POSITIVE_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback2", "MC-", EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_NEGATIVE_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback3", "MC(single)", EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback4", "CB/DS", EmulatorFeedbackType.EMUL_CIRCUIT_BREAKER_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback5", "Fuse", EmulatorFeedbackType.EMUL_FUSE_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback6", "Module Fan", EmulatorFeedbackType.EMUL_FAN_MODULE_FEEDBACK),
        	new EmulatorFeedbackRadioButton("feedback7", "BPU Fan", EmulatorFeedbackType.EMUL_FAN_BPU_FEEDBACK)
        };        	
    	EmulatorComponentRadioButton[] componentRadioButtonList = {
        	new EmulatorComponentRadioButton("component1", "CB/DS", EmulatorComponentType.EMUL_CIRCUIT_BREAKER),
        	new EmulatorComponentRadioButton("component2", "Fans", EmulatorComponentType.EMUL_FAN)
        };        	
    	EmulatorDiagnosisCheckBox[] diagnosisCheckBoxList = {
        	new EmulatorDiagnosisCheckBox("diagnosis1", "OVF2", EmulatorDiagnosisType.EMUL_DIAG_OVF2ND),
        	new EmulatorDiagnosisCheckBox("diagnosis2", "RM LOC", EmulatorDiagnosisType.EMUL_DIAG_RMLOC),
        	new EmulatorDiagnosisCheckBox("diagnosis3", "COF", EmulatorDiagnosisType.EMUL_DIAG_COF),
        	new EmulatorDiagnosisCheckBox("diagnosis4", "MBMS Fault", EmulatorDiagnosisType.EMUL_DIAG_MBMSF),
        	new EmulatorDiagnosisCheckBox("diagnosis5", "CSE", EmulatorDiagnosisType.EMUL_DIAG_CSE),
        	new EmulatorDiagnosisCheckBox("diagnosis6", "TSE", EmulatorDiagnosisType.EMUL_DIAG_TSE),
        	new EmulatorDiagnosisCheckBox("diagnosis7", "RUF", EmulatorDiagnosisType.EMUL_DIAG_RUF),
        	new EmulatorDiagnosisCheckBox("diagnosis8", "MCFE", EmulatorDiagnosisType.EMUL_DIAG_MCFE),
        	new EmulatorDiagnosisCheckBox("diagnosis9", "CBE", EmulatorDiagnosisType.EMUL_DIAG_CBE),
        	new EmulatorDiagnosisCheckBox("diagnosis10", "FANE", EmulatorDiagnosisType.EMUL_DIAG_FANE),
        	new EmulatorDiagnosisCheckBox("diagnosis11", "BR LOC", EmulatorDiagnosisType.EMUL_DIAG_BRLOC)
        };
		
		TextBlock rackIdLabel;
		EmulatorIntegerBox rackIdBox;
		CheckBox rackIdAllFlagBox;
		TextBlock valueInputLabel1;
		EmulatorIntegerBox valueInputBox1;
		TextBlock valueInputLabel2;
		EmulatorIntegerBox valueInputBox2;
		EmulatorAlighedCheckBox valueFlagBox1;
		
		public MessageBuilderWindow()
		{
			InitializeComponent();
			
	   		foreach(ComboBoxItem item in commandTypeComboBoxItemList)
	   		{
	   			commandTypeComboBox.Items.Add(item);
	   		}
	   		
	   		foreach(ComboBoxItem item in elementTypeComboBoxItemList)
	   		{
	   			batteryStatusElementComboBox.Items.Add(item);
	   		}
	   		
	   		foreach(RadioButton item in feedbackRadioButtonList)
	   		{
	   			item.GroupName = feedbackRadioButtonPanel.Name;
	   			((RadioButton)item).Checked += feedbackRadioButton_Checked;
	   			feedbackRadioButtonPanel.Children.Add(item);
	   		}
	   		
	   		foreach(RadioButton item in componentRadioButtonList)
	   		{
	   			item.GroupName = componentRadioButtonPanel.Name;
	   			((RadioButton)item).Checked += componentRadioButton_Checked;
	   			componentRadioButtonPanel.Children.Add(item);
	   		}
	   		
	   		foreach(CheckBox item in diagnosisCheckBoxList)
	   		{
	   			diagnosisCheckBoxPanel.Children.Add(item);
	   		}
	   		
			rackIdLabel = new TextBlock();
	   		rackIdLabel.Text = "Rack ID:";
        	rackIdLabel.Margin = new Thickness(0, 0, 5, 0);
	   		rackIdPanel.Children.Add(rackIdLabel);
			rackIdBox = new EmulatorIntegerBox("");
	   		rackIdBox.Height = 20;
	   		rackIdBox.Width = 30;
	   		rackIdBox.ConfigRange(1, 26);
	   		rackIdBox.PreviewTextInput += CheckTextBox;
	   		//rackIdBox.Margin = new Thickness(rackIdLabel.Margin.Left + rackIdLabel.ActualWidth,rackIdLabel.Margin.Top,0,0);
	   		rackIdPanel.Children.Add(rackIdBox);
	   		rackIdAllFlagBox = new CheckBox();
	   		rackIdAllFlagBox.Click += rackIdAllFlagBox_Click;
	   		rackIdAllFlagBox.Content = "All Rack";
        	rackIdAllFlagBox.Margin = new Thickness(10, 0, 5, 0);
	   		rackIdPanel.Children.Add(rackIdAllFlagBox);
	   		
	   		valueInputLabel1 = new TextBlock();
	   		valueInputLabel1.Text = "Value1:";
        	valueInputLabel1.Margin = new Thickness(0, 0, 5, 0);
	   		valueInputPanel.Children.Add(valueInputLabel1);
			valueInputBox1 = new EmulatorIntegerBox("");
	   		valueInputBox1.Height = 20;
	   		valueInputBox1.Width = 50;
	   		valueInputBox1.ConfigRange(0, 10);
	   		valueInputBox1.PreviewTextInput += CheckTextBox;
			valueInputPanel.Children.Add(valueInputBox1);
			valueFlagBox1 = new EmulatorAlighedCheckBox("valueFlagBox1", "");
        	valueFlagBox1.Margin = new Thickness(-50, 0, 0, 0);
        	valueFlagBox1.AlignTextBox(valueInputBox1);
			valueInputPanel.Children.Add(valueFlagBox1);
	   		valueInputLabel2 = new TextBlock();
	   		valueInputLabel2.Text = "Value2:";
        	valueInputLabel2.Margin = new Thickness(20, 0, 5, 0);
	   		valueInputPanel.Children.Add(valueInputLabel2);
			valueInputBox2 = new EmulatorIntegerBox("");
	   		valueInputBox2.Height = 20;
	   		valueInputBox2.Width = 50;
	   		valueInputBox2.ConfigRange(0, 10);
	   		valueInputBox2.PreviewTextInput += CheckTextBox;
			valueInputPanel.Children.Add(valueInputBox2);
		}
		
		void onLoadWindow(object sender, RoutedEventArgs e)
		{
			this.windowStatus = WindowStatus.OPENED;
		}
		
		void onClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true; //Do not close but hide the window (not to lose resource allocation)
			this.Visibility = Visibility.Hidden;
			this.windowStatus = WindowStatus.CANCEL_CLOSED;
		}
		
		void rackIdAllFlagBox_Click(object sender, RoutedEventArgs e)
		{
			if(((CheckBox)sender).IsChecked.Value == true)
			{
				rackIdBox.Text = "ALL";	//FF: magic number to represent 'all rack' in emulator
				rackIdBox.IsEnabled = false;
			}
			else
			{
				rackIdBox.Clear();
				rackIdBox.IsEnabled = true;
			}
		}
		
		//Upon command selection...
		void commandTypeComboBox_SelectionChange(object sender, SelectionChangedEventArgs e)
		{
			//Redraw the contents to build message upon controlMessageComboBox selection change
			rackIdPanel.Visibility = Visibility.Visible;
			batteryStatusElementComboBox.Visibility = Visibility.Hidden;
			feedbackRadioButtonPanel.Visibility = Visibility.Hidden;
			componentRadioButtonPanel.Visibility = Visibility.Hidden;
			diagnosisCheckBoxPanel.Visibility = Visibility.Hidden;
			valueInputPanel.Visibility = Visibility.Hidden;
			generateMessageButton.IsEnabled = false;
			
			switch(GetSelectedCommandType())
			{
				case EmulatorCommandType.EMUL_COMMAND_BATTERY_STATUS:
					batteryStatusElementComboBox.Visibility = Visibility.Visible;
					break;					
				case EmulatorCommandType.EMUL_COMMAND_FEEDBACK_STATUS:
					feedbackRadioButtonPanel.Visibility = Visibility.Visible;
					break;
				case EmulatorCommandType.EMUL_COMMAND_COMPONENT_STATUS:
					componentRadioButtonPanel.Visibility = Visibility.Visible;
					break;
				case EmulatorCommandType.EMUL_COMMAND_DIAGNOSIS_STATUS:
					diagnosisCheckBoxPanel.Visibility = Visibility.Visible;
					generateMessageButton.IsEnabled = true;
					break;
				case EmulatorCommandType.EMUL_REQUEST_FEEDBACK_STATUS:
					feedbackRadioButtonPanel.Visibility = Visibility.Visible;
					break;
				case EmulatorCommandType.EMUL_REQUEST_DIAGNOSIS_STATUS:
					//diagnosisCheckBoxPanel.Visibility = Visibility.Visible; //--> No need to check diagnosis field to attain current status
					generateMessageButton.IsEnabled = true;
					break;					
				default: //Initial selection index(-1) shall clear all setting to default.
					rackIdPanel.Visibility = Visibility.Hidden;
					break;
			}
		}
		
		void batteryStatusElementComboBox_SelectionChange(object sender, SelectionChangedEventArgs e)
		{
			//Redraw the contents to build message upon batteryStatusElementComboBox selection change
			hideAndClearAllValueInputFields();
			
			switch(((EmulatorElementComboBoxItem)batteryStatusElementComboBox.SelectedItem).element)
			{
				case EmulatorBatteryStatusElement.EMUL_RACK_SOC:
					valueInputLabel1.Text = "SOC(%, *2):";
					valueInputBox1.ConfigRange(0, 255);
					valueInputLabel1.Visibility = valueInputBox1.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_SOH:
					valueInputLabel1.Text = "SOH(%, *2):";
					valueInputBox1.ConfigRange(0, 255);
					valueInputLabel1.Visibility = valueInputBox1.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_CURRENT:
					valueInputLabel1.Text = "Rack Current(A+1000, *10):";
					valueInputBox1.ConfigRange(0, 20000);
					valueInputLabel1.Visibility = valueInputBox1.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_CV:
					valueInputLabel1.Text = "CV Max(mV):";
					valueInputLabel2.Text = "CV Min(mV):";
					valueInputBox1.ConfigRange(0, 5000);
					valueInputBox2.ConfigRange(0, 5000);
					valueInputLabel1.Visibility = valueInputBox1.Visibility = Visibility.Visible;
					valueInputLabel2.Visibility = valueInputBox2.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_TEMPERATURE:
					valueInputLabel1.Text = "Temp Max(ºC+24, *2):";
					valueInputLabel2.Text = "Temp Min(ºC+24, *2):";
					valueInputBox1.ConfigRange(0, 20000);
					valueInputBox2.ConfigRange(0, 20000);
					valueInputLabel1.Visibility = valueInputBox1.Visibility = Visibility.Visible;
					valueInputLabel2.Visibility = valueInputBox2.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_MBMS_ON:
					valueFlagBox1.Content = "MBMS On";
					valueFlagBox1.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_CELL_BALANCING_ON:
					valueFlagBox1.Content = "Cell Bal On";
					valueFlagBox1.Visibility = Visibility.Visible;
					break;
				case EmulatorBatteryStatusElement.EMUL_RACK_PPS:
					valueFlagBox1.Content = "PPS On";
					valueFlagBox1.Visibility = Visibility.Visible;
					break;
				default:
					break;
			}
			if(IsCommandToSetValue())
				valueInputPanel.Visibility = Visibility.Visible;
			generateMessageButton.IsEnabled = true;
		}
				
		void feedbackRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			//Redraw the contents to build message upon batteryStatusElementComboBox selection change
			hideAndClearAllValueInputFields();
			
			EmulatorFeedbackRadioButton selectedItem = (EmulatorFeedbackRadioButton)sender;			
			switch(selectedItem.feedbackType)
			{
				case EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_POSITIVE_FEEDBACK:
				case EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_NEGATIVE_FEEDBACK:
				case EmulatorFeedbackType.EMUL_MAIN_CONTACTOR_FEEDBACK:
					valueFlagBox1.Content = "Relay Closed";
					break;
				case EmulatorFeedbackType.EMUL_CIRCUIT_BREAKER_FEEDBACK:
					valueFlagBox1.Content = "CB Closed";
					break;
				case EmulatorFeedbackType.EMUL_FUSE_FEEDBACK:
					valueFlagBox1.Content = "Fuse Closed";
					break;
				case EmulatorFeedbackType.EMUL_FAN_MODULE_FEEDBACK:
				case EmulatorFeedbackType.EMUL_FAN_BPU_FEEDBACK:
					valueFlagBox1.Content = "Fan On";
					break;
				default:
					break;
			}
			valueFlagBox1.Visibility = Visibility.Visible;
			if(IsCommandToSetValue())
				valueInputPanel.Visibility = Visibility.Visible;
			generateMessageButton.IsEnabled = true;
		}
		
		void componentRadioButton_Checked(object sender, RoutedEventArgs e)
		{
			//Redraw the contents to build message upon batteryStatusElementComboBox selection change
			hideAndClearAllValueInputFields();
			
			EmulatorComponentRadioButton selectedItem = (EmulatorComponentRadioButton)sender;
			switch(selectedItem.componentType)
			{
				case EmulatorComponentType.EMUL_CIRCUIT_BREAKER:
					valueFlagBox1.Content = "CB Trip";
					break;
				case EmulatorComponentType.EMUL_FAN:
					valueFlagBox1.Content = "Fan On";
					break;
				default:
					break;
			}
			valueFlagBox1.Visibility = Visibility.Visible;
			if(IsCommandToSetValue())
				valueInputPanel.Visibility = Visibility.Visible;
			generateMessageButton.IsEnabled = true;
		}
		
		void hideAndClearAllValueInputFields()
		{
			valueInputLabel1.Visibility = valueInputBox1.Visibility
			= valueInputLabel2.Visibility = valueInputBox2.Visibility
			= valueFlagBox1.Visibility = Visibility.Hidden;
			valueInputPanel.Visibility = Visibility.Hidden;
			valueInputLabel1.Text = valueInputLabel2.Text = "";			
			valueInputBox1.Clear();
			valueInputBox2.Clear();
			valueFlagBox1.Content = "";
			valueFlagBox1.IsChecked = false;
		}
		
		void generateMessageButton_Click(object sender, RoutedEventArgs e)
		{
			if(String.IsNullOrEmpty(rackIdBox.Text))
			{
				MessageBox.Show("Error: Rack ID should be valid");
				return;
			}
			if(valueInputBox1.IsVisible && String.IsNullOrEmpty(valueInputBox1.Text))
			{
				MessageBox.Show("Error: Value1 should not be empty");
				return;
			}
			if(valueInputBox2.IsVisible && String.IsNullOrEmpty(valueInputBox2.Text))
			{
				MessageBox.Show("Error: Value2 should not be empty");
				return;
			}
			
			this.Visibility = Visibility.Hidden;
			this.windowStatus = WindowStatus.NORMALLY_CLOSED;
		}
		
		public byte GetSelectedRackID()
		{
			if(rackIdBox.Text.Equals("ALL"))
			{
			   return 255;
			}
			
			int rackIdInt32 = Convert.ToInt32(rackIdBox.Text);
			if(!rackIdInt32.Equals(rackIdInt32 & 0xFF)) //overflow test
			{
				MessageBox.Show("Rack ID " + rackIdInt32 + " is invalid (out of range)");
				return 0;
			}
			else
			{
				return (byte)rackIdInt32;
			}
		}
		
		internal bool IsCommandToSetValue()
		{
			switch(GetSelectedCommandType())
			{
				case EmulatorCommandType.EMUL_COMMAND_BATTERY_STATUS:
				case EmulatorCommandType.EMUL_COMMAND_FEEDBACK_STATUS:
				case EmulatorCommandType.EMUL_COMMAND_COMPONENT_STATUS:
				case EmulatorCommandType.EMUL_COMMAND_DIAGNOSIS_STATUS:
					return true;
				default:
					return false;
			}
		}
		
		public EmulatorCommandType GetSelectedCommandType()
		{
			return (commandTypeComboBox.SelectedItem == null) ? EmulatorCommandType.EMUL_COMMAND_INVALID :
					((EmulatorCommandComboBoxItem)commandTypeComboBox.SelectedItem).commandType;
		}
		
		public EmulatorBatteryStatusElement GetSelectedBatteryStatusElement()
		{
			return (batteryStatusElementComboBox.SelectedItem == null) ? EmulatorBatteryStatusElement.EMUL_ELEMENT_INVALID :
					((EmulatorElementComboBoxItem)batteryStatusElementComboBox.SelectedItem).element;
		}
		
		public EmulatorFeedbackType GetSelectedFeedbackType()
		{
			foreach(EmulatorFeedbackRadioButton item in feedbackRadioButtonList)
			{
				if(item.IsChecked.Value)
					return item.feedbackType;
			}
			return EmulatorFeedbackType.EMUL_FEEDBACK_INVALID;
		}
				
		public EmulatorComponentType GetSelectedComponentType()
		{
			foreach(EmulatorComponentRadioButton item in componentRadioButtonList)
			{
				if(item.IsChecked.Value)
					return item.componentType;
			}
			return EmulatorComponentType.EMUL_COMPONENT_INVALID;
		}
		
		public int GetSelectedDiagnosisStatus()
		{
			int flagFields = 0;
			foreach(EmulatorDiagnosisCheckBox item in diagnosisCheckBoxList)
			{
				flagFields |= (item.IsChecked.Value) ? 0x1<<((int)item.diagnosisType-1) : 0;
			}
			return flagFields;
		}
		
		public int GetValueInputField(int index)
		{
			switch(index)
			{
				case 1:
					return Convert.ToInt32((valueInputBox1.Text.Length > 0) ? valueInputBox1.Text : "0");
				case 2:
					return Convert.ToInt32((valueInputBox2.Text.Length > 0) ? valueInputBox2.Text : "0");
				default:
					break;
			}
			return 0;
		}
		
		public bool GetValueToggleField()
		{
			return valueFlagBox1.IsChecked.Value;
		}
		
		/*
		* Makes sure that only positive integers can be used as input
		*/
        private void CheckTextBox(object sender, TextCompositionEventArgs e)
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

	}
}