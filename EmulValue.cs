/*
 * Created by SharpDevelop.
 * User: ygsong
 * Date: 12/04/2017
 * Time: 17:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Emulator_Controller
{
	public static class Defined
	{
		public const int MAX_DLC = 8;
		public const int ERROR_FRAME_ENDURANCE_COUNT_LIMIT = 100;
	}
	
	public enum ReceiverReport
	{
		FORCED_STOP = -1,
		REPORT_RUNNING = 0,
		ERROR_STOP = 100
	}
    
	/// <summary>
	/// Types of operation command from EmulatorController to BBMS Emulator.
	/// </summary>
	public enum EmulatorCommandType
	{
		EMUL_COMMAND_BATTERY_STATUS = 0,
		EMUL_COMMAND_FEEDBACK_STATUS = 1,
		EMUL_COMMAND_COMPONENT_STATUS = 2,
		EMUL_COMMAND_DIAGNOSIS_STATUS = 3,
		EMUL_REQUEST_FEEDBACK_STATUS = 4,
		EMUL_REQUEST_DIAGNOSIS_STATUS = 5,
		EMUL_COMMAND_INVALID
	};
		
	/// <summary>
	/// Predefined types of the operation result.
	/// </summary>
	public enum EmulatorResult {
		EMULATOR_RESULT_UNKNOWN_FAIL = 255,
		EMULATOR_RESULT_INVALID_PARAM = 254,
		EMULATOR_RESULT_CONDITION_DENIED = 253,
		EMULATOR_RESULT_CANCELLED = 252,
		EMULATOR_RESULT_SUCCESS = 0
	};

	/// <summary>
	/// Predefined types of the on/off status.
	/// </summary>
	public enum ToggleStatus {
		EMUL_STATUS_OFF = 0,
		EMUL_STATUS_ON = 1
	};
	
	/// <summary>
	/// Predefined types of the closed/open status.
	/// </summary>
	public enum ConnectionStatus {
		EMUL_STATUS_OPEN = 0,
		EMUL_STATUS_CLOSED = 1
	};
		
	/// <summary>
	/// Predefined types of the CB handling command.
	/// </summary>
	public enum TripCommand {
		EMUL_CB_CLOSED = 0,
		EMUL_CB_TRIP = 1
	};
	
	/// <summary>
	/// Predefined types of the diagnosis type.
	/// </summary>
	public enum EmulatorDiagnosisType
	{
		EMUL_DIAG_NONE = 0,
		EMUL_DIAG_OVF2ND,	//0x00000001
		EMUL_DIAG_RMLOC,	//0x00000002
		EMUL_DIAG_COF,		//0x00000004
		EMUL_DIAG_MBMSF,	//0x00000008
		EMUL_DIAG_CSE,		//0x00000010
		EMUL_DIAG_TSE,		//0x00000020
		EMUL_DIAG_RUF,		//0x00000040
		EMUL_DIAG_MCFE,		//0x00000080 //This item should be set by changing feedback status.
		EMUL_DIAG_CBE,		//0x00000100 //This item should be set by changing feedback status.
		EMUL_DIAG_FANE,		//0x00000200 //This item should be set by changing feedback status.
		EMUL_DIAG_BRLOC		//0x00000400 //This item should be set by changing feedback status.
	};

	/// <summary>
	/// Predefined types of the component type.
	/// </summary>
	public enum EmulatorComponentType
	{
		EMUL_PRECHARGE_RELAY = 0,
		EMUL_MAIN_CONTACTOR,			//for BPU with single MC relay
		EMUL_MAIN_CONTACTOR_POSITIVE,	//for BPU with dual MC relays
		EMUL_MAIN_CONTACTOR_NEGATIVE,	//for BPU with dual MC relays
		EMUL_CIRCUIT_BREAKER,			//for BPU with Circuit Breaker or Disconnector Switch
		EMUL_FAN,
		EMUL_COMPONENT_INVALID
	};

	/// <summary>
	/// Predefined types of the feedback type.
	/// </summary>
	public enum EmulatorFeedbackType
	{
		EMUL_MAIN_CONTACTOR_POSITIVE_FEEDBACK = 0,	//for BPU with dual MC feedbacks
		EMUL_MAIN_CONTACTOR_NEGATIVE_FEEDBACK,		//for BPU with dual MC feedbacks
		EMUL_MAIN_CONTACTOR_FEEDBACK,				//for BPU with single MC feedback
		EMUL_CIRCUIT_BREAKER_FEEDBACK,				//for BPU with Circuit Breaker or Disconnector Switch
		EMUL_FUSE_FEEDBACK,							//for BPU with Fuse feedback
		EMUL_FAN_MODULE_FEEDBACK,					//corresponds to Module Fan
		EMUL_FAN_BPU_FEEDBACK,						//corresponds to BPU Fan
		EMUL_FEEDBACK_INVALID
	};

	/// <summary>
	/// Predefined types of the battery status element.
	/// </summary>
	public enum EmulatorBatteryStatusElement
	{
		EMUL_RACK_SOC = 0,
		EMUL_RACK_SOH = 1,
		EMUL_RACK_CURRENT = 2,
		EMUL_RACK_CV = 3,
		EMUL_RACK_TEMPERATURE = 4,
		EMUL_RACK_MBMS_ON = 5,
		EMUL_RACK_CELL_BALANCING_ON = 6,
		EMUL_RACK_PPS = 7,
		EMUL_ELEMENT_INVALID
	};
	
	public abstract class EmulatorComboBoxItem : ComboBoxItem
	{		
		protected EmulatorComboBoxItem()
		{
			this.Name = "NoName";
			this.Content = "-----";
		}
		protected EmulatorComboBoxItem(string name, string content)
		{
			this.Name = (string)name.Clone();
			this.Content = (string)content.Clone();
		}
	}
	
	public abstract class EmulatorRadioButton : RadioButton
	{
		protected EmulatorRadioButton()
		{
			this.Name = "NoName";
			this.Content = "-----";
		}
		protected EmulatorRadioButton(string name, string content)
		{
			this.Name = (string)name.Clone();
			this.Content = (string)content.Clone();
		}
	}
	
	public abstract class EmulatorCheckBox : CheckBox
	{
		protected EmulatorCheckBox()
		{
			this.Name = "NoName";
			this.Content = "-----";
		}
		protected EmulatorCheckBox(string name, string content)
		{
			this.Name = (string)name.Clone();
			this.Content = (string)content.Clone();
		}
	}
	
	public class EmulatorCommandComboBoxItem : EmulatorComboBoxItem
	{
		public EmulatorCommandType commandType;
		
		public EmulatorCommandComboBoxItem() {}
		public EmulatorCommandComboBoxItem(string name, string content, EmulatorCommandType commandType) : base(name, content)
		{
			this.commandType = commandType;
		}
	}
	
	public class EmulatorElementComboBoxItem : EmulatorComboBoxItem
	{
		public EmulatorBatteryStatusElement element;
		
		public EmulatorElementComboBoxItem() {}
		public EmulatorElementComboBoxItem(string name, string content, EmulatorBatteryStatusElement element) : base(name, content)
		{
			this.element = element;
		}
	}
	
	public class EmulatorFeedbackRadioButton : EmulatorRadioButton
	{
		public EmulatorFeedbackType feedbackType;
		
		public EmulatorFeedbackRadioButton() {}
		public EmulatorFeedbackRadioButton(string name, string content, EmulatorFeedbackType feedbackType) : base(name, content)
		{
			this.feedbackType = feedbackType;
		}
	}
	
	public class EmulatorComponentRadioButton : EmulatorRadioButton
	{
		public EmulatorComponentType componentType;
		
		public EmulatorComponentRadioButton() {}
		public EmulatorComponentRadioButton(string name, string content, EmulatorComponentType componentType) : base(name, content)
		{
			this.componentType = componentType;			
		}
	}
	
	public class EmulatorDiagnosisCheckBox : EmulatorCheckBox
	{
		public EmulatorDiagnosisType diagnosisType;
		
		public EmulatorDiagnosisCheckBox() {}
		public EmulatorDiagnosisCheckBox(string name, string content, EmulatorDiagnosisType diagnosisType) : base(name, content)
		{
			this.diagnosisType = diagnosisType;
		}
	}
	
	public class EmulatorAlighedCheckBox : EmulatorCheckBox
	{
		TextBox alignedTextBox;
		public EmulatorAlighedCheckBox(string name, string content) : base(name, content) {}
		
		// Checking and unchecking change text value of the aligned TextBox
		public bool AlignTextBox(TextBox box)
		{
			if(box == null)
				return false;
			
			alignedTextBox = box;
			this.Checked += updateAlignedTextBox;
			this.Unchecked += updateAlignedTextBox;
			return true;
		}
		void updateAlignedTextBox(object sender, RoutedEventArgs e)
		{
			alignedTextBox.Text = (this.IsChecked.Value == true) ? "1" : "0";
		}
	}
	
	public class EmulatorIntegerBox : TextBox
	{
		private int LowerBound;
		private int UpperBound;
		private int MinLength;
		public EmulatorIntegerBox(string name)
		{
			this.Name = name;
		}
		public void ConfigRange(int minValue, int maxValue)
		{
			// For the silly case of inverted inputs
			LowerBound = Math.Min(minValue, maxValue);
			UpperBound = Math.Max(minValue, maxValue);
			
			MinLength = LowerBound.ToString().Length;
			this.MaxLength = UpperBound.ToString().Length;
			this.LostFocus += checkRangeCompliance;
		}
		void checkRangeCompliance(object sender, RoutedEventArgs e)
		{
			int numericalInput = Convert.ToInt32((this.Text.Length > 0) ? this.Text : "0"); //not sure if this kind conversion from empty field to "0" is desired or not.
			if(numericalInput < LowerBound || numericalInput > UpperBound)
				MessageBox.Show("Watch your input value, it's out of valid range!");
		}
	}
}
