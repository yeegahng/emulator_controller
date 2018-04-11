/*
 * Created by SharpDevelop.
 * User: ygsong
 * Date: 12/04/2017
 * Time: 16:39
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace Emulator_Controller
{
	/// <summary>
	/// Defines CAN Message Format for Emulator.
	/// </summary>
	public class EmulMessage
	{
		public string ID_decimal = "";
		public int DLC = 0;
		private byte[] Data = null;
		
		public byte[] DATA
		{
			set{
				if(value.Length <= DLC)
				{
					value.CopyTo(Data, 0);
				}
			}
			get
			{
				return Data;
			}
		}
		public string ID	//of hexadecimal
		{
			set
			{
				ID_decimal = Utils.ConvertHexStrToDecStr(value);
			}			
			get
			{
				return Utils.ConvertDecStrToHexStr(ID_decimal);
			}
		}
		
		public EmulMessage(string msgId, int msgDLC)
		{
			this.ID = msgId;
			this.DLC = msgDLC;
			this.Data = new byte[msgDLC];
		}

	}
	
	/// <summary>
	/// Defines CAN Message Builder to build the messages for Emulator.
	/// </summary>
	public sealed class EmulMessageBuilder
	{
		private static EmulMessageBuilder msgBuilderInstance = new EmulMessageBuilder();
		
		private Dictionary<EmulatorCommandType, Func<EmulatorCommandType, EmulMessage, int>> cmdTypeFuncMap
			= new Dictionary<EmulatorCommandType, Func<EmulatorCommandType, EmulMessage, int>>();
		private EmulMessage message = null;
		private MessageBuilderWindow builderWindow = new MessageBuilderWindow();
		
		private EmulMessageBuilder()
		{
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_COMMAND_BATTERY_STATUS, HandleCmd_BatteryStatusControl);
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_COMMAND_FEEDBACK_STATUS, HandleCmd_FeedbackStatusControl);
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_COMMAND_COMPONENT_STATUS, HandleCmd_ComponentStatusControl);
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_COMMAND_DIAGNOSIS_STATUS, HandleCmd_DiagnosisStatusControl);
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_REQUEST_FEEDBACK_STATUS, HandleCmd_FeedbackStatusRequest);
			cmdTypeFuncMap.Add(EmulatorCommandType.EMUL_REQUEST_DIAGNOSIS_STATUS, HandleCmd_DiagnosisStatusRequest);
			
			message = new EmulMessage("611", 8);
		}
		
		public static EmulMessageBuilder GetBuilder()
		{
			return msgBuilderInstance;
		}
		
		public void DestroyBuilder()
		{
			message = null;
			builderWindow.Close();
			builderWindow = null;
			msgBuilderInstance = null;
		}
		
		public EmulatorResult BuildMessageWithSelectedOptions(ref EmulMessage msg)
		{
			builderWindow.ShowDialog();
			if(builderWindow.windowStatus == MessageBuilderWindow.WindowStatus.CANCEL_CLOSED)
				return EmulatorResult.EMULATOR_RESULT_CANCELLED;
			
			//Update variables from the controls
			EmulatorCommandType commandType = builderWindow.GetSelectedCommandType();
			foreach(var cmdType in cmdTypeFuncMap)
			{
				if(cmdType.Key.Equals(commandType))
				{
					cmdType.Value.Invoke((EmulatorCommandType)commandType, message);
					msg = message;
					return EmulatorResult.EMULATOR_RESULT_SUCCESS; //Success
				}
			}
			return EmulatorResult.EMULATOR_RESULT_INVALID_PARAM; //Fail: No matching command
		}
		
		private int HandleCmd_BatteryStatusControl(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: EmulatorBatteryStatusElement
			//D3: -
			//D4-D5: Element Status Value1
			//D6-D7: Element Status Value2
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = (byte)builderWindow.GetSelectedBatteryStatusElement();
			msg.DATA[3] = 0;
			msg.DATA[4] = (byte)((builderWindow.GetValueInputField(1)>>8)&0xff);
			msg.DATA[5] = (byte)((builderWindow.GetValueInputField(1))&0xff);
			msg.DATA[6] = (byte)((builderWindow.GetValueInputField(2)>>8)&0xff);
			msg.DATA[7] = (byte)((builderWindow.GetValueInputField(2))&0xff);
			
			return 0;
		}
		private int HandleCmd_FeedbackStatusControl(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: EmulatorFeedbackType
			//D3: Feedback Status Value
			//D4: -
			//D5: -
			//D6: -
			//D7: -
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = (byte)builderWindow.GetSelectedFeedbackType();
			msg.DATA[3] = (byte)Convert.ToByte(builderWindow.GetValueToggleField());
			msg.DATA[4] = 0;
			msg.DATA[5] = 0;
			msg.DATA[6] = 0;
			msg.DATA[7] = 0;
			
			return 0;
		}
		private int HandleCmd_ComponentStatusControl(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: EmulatorComponentType
			//D3: Component Status Value
			//D4: -
			//D5: -
			//D6: -
			//D7: -
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = (byte)builderWindow.GetSelectedComponentType();
			msg.DATA[3] = (byte)Convert.ToByte(builderWindow.GetValueToggleField());
			msg.DATA[4] = 0;
			msg.DATA[5] = 0;
			msg.DATA[6] = 0;
			msg.DATA[7] = 0;
			
			return 0;
		}
		private int HandleCmd_DiagnosisStatusControl(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: -
			//D3: -
			//D4-D7: EmulatorDiagnosisType
			Int32 diagnosisFlags = builderWindow.GetSelectedDiagnosisStatus();
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = 0;
			msg.DATA[3] = 0;
			msg.DATA[4] = (byte)((diagnosisFlags>>24)&0xff);
			msg.DATA[5] = (byte)((diagnosisFlags>>16)&0xff);
			msg.DATA[6] = (byte)((diagnosisFlags>>8)&0xff);
			msg.DATA[7] = (byte)(diagnosisFlags&0xff);
			
			return 0;
		}
		private int HandleCmd_FeedbackStatusRequest(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: EmulatorFeedbackType
			//D3: -
			//D4: -
			//D5: -
			//D6: -
			//D7: -
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = (byte)builderWindow.GetSelectedFeedbackType();
			msg.DATA[3] = 0;
			msg.DATA[4] = 0;
			msg.DATA[5] = 0;
			msg.DATA[6] = 0;
			msg.DATA[7] = 0;
			
			return 0;
		}
		private int HandleCmd_DiagnosisStatusRequest(EmulatorCommandType cmd, EmulMessage msg)
		{
			//D0: Rack ID
			//D1: cmd
			//D2: -
			//D3: -
			//D4: -
			//D5: -
			//D6: -
			//D7: -
			
			msg.DATA[0] = builderWindow.GetSelectedRackID();
			msg.DATA[1] = (byte)cmd;
			msg.DATA[2] = 0;
			msg.DATA[3] = 0;
			msg.DATA[4] = 0;
			msg.DATA[5] = 0;
			msg.DATA[6] = 0;
			msg.DATA[7] = 0;
			
			return 0;
		}
		

	}
	


}
