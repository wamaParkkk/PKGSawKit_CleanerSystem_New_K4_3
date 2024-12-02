using Ajin_motion_driver;
using MsSqlManagerLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PKGSawKit_CleanerSystem_New_K4_3.Squence
{
    class PM1BrushMoving : TBaseThread
    {
        Thread thread;
        private new TStep step;
        Alarm_List alarm_List;  // Alarm list
        private bool bWaitSet;

        public PM1BrushMoving()
        {
            ModuleName = "PM1";
            module = (byte)MODULE._PM1;
            
            thread = new Thread(new ThreadStart(Execute));
            
            alarm_List = new Alarm_List();            

            thread.Start();
        }

        public void Dispose()
        {
            thread.Abort();
        }

        private void Execute()
        {            
            try
            {
                while (true)
                {                    
                    if (Define.seqBrushFwBwCtrl == Define.CTRL_ABORT)
                    {
                        AlarmAction("Abort");
                    }
                    else if (Define.seqBrushFwBwCtrl == Define.CTRL_RETRY)
                    {
                        AlarmAction("Retry");
                    }
                    else if (Define.seqBrushFwBwCtrl == Define.CTRL_WAIT)
                    {
                        AlarmAction("Wait");
                    }

                    Run_Progress();
                    Home_Progress();
                    FWD_Progress();
                    BWD_Progress();

                    Thread.Sleep(10);
                }
            }
            catch (Exception)
            {
                
            }
        }

        private void AlarmAction(string sAction)
        {
            if (sAction == "Retry")
            {
                step.Flag = true;
                step.Times = 1;

                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;

                if (Define.seqCtrl[module] == Define.CTRL_ALARM)
                {
                    Define.seqCtrl[module] = Define.CTRL_RUNNING;
                }
            }
            else if (sAction == "Abort")
            {
                ActionList();                               

                Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_IDLE;
                Define.seqBrushFwBwCtrl = Define.CTRL_IDLE;
                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_ABORTOK;

                step.Times = 1;                

                Global.EventLog("Brush cylinder movement stopped : " + sAction, ModuleName, "Event");
            }
            else if (sAction == "Wait")
            {
                if (!bWaitSet)
                {
                    bWaitSet = true;

                    Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                    Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);

                    MotionClass.SetMotorSStop(Define.axis_r);

                    Global.EventLog("Brush Cylinder movement stopped : " + sAction, ModuleName, "Event");
                }                
            }
        }

        private void ActionList()
        {
            F_PROCESS_ALL_VALVE_CLOSE();

            Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
            Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);

            MotionClass.SetMotorSStop(Define.axis_r);            
        }

        private void ShowAlarm(string almId)
        {
            ActionList();

            Define.seqBrushFwBwCtrl = Define.CTRL_ALARM;

            // 프로세스 시퀀스 알람으로 멈춤
            Define.seqCtrl[module] = Define.CTRL_ALARM;

            // Buzzer IO On.
            Global.SetDigValue((int)DigOutputList.Buzzer_o, (uint)DigitalOffOn.On, ModuleName);

            // Alarm history.
            Define.sAlarmName = "";
            alarm_List.alarm_code = almId;
            Define.sAlarmName = alarm_List.alarm_code;

            Global.EventLog(almId + ":" + Define.sAlarmName, ModuleName, "Alarm");

            //HostConnection.Host_Set_RunStatus(Global.hostEquipmentInfo, ModuleName, "Alarm");
            //HostConnection.Host_Set_AlarmName(Global.hostEquipmentInfo, ModuleName, Define.sAlarmName);
        }

        public void F_HOLD_STEP()
        {
            step.Flag = false;
            step.Times = 1;
            Define.seqBrushFwBwCtrl = Define.CTRL_HOLD;
        }

        public void F_INC_STEP()
        {
            step.Flag = true;
            step.Layer++;
            step.Times = 1;
        }

        // BRUSH CYLINDER PROGRESS //////////////////////////////////////////////////////////
        #region BRUSH CYLINDER_PROGRESS
        private void Run_Progress()
        {
            if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_RUN) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_RUNING;                

                Global.EventLog("START THE BRUSH CYLINDER MOVING.", ModuleName, "Event");
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_RUN) && (Define.seqBrushFwBwCtrl == Define.CTRL_HOLD))
            {
                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_RUN) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_CYLINDER_FwdBwd("Forward");
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_CYLINDER_FwdBwd("Backward");
                        }
                        break;

                    case 3:
                        {
                            P_BRUSH_CYLINDER_StepCheck(1);
                        }
                        break;                    
                }
            }
        }

        private void Home_Progress()
        {
            if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_HOME) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_HOMEING;                

                Global.EventLog("START THE BRUSH CYLINDER HOME.", ModuleName, "Event");
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_HOME) && (Define.seqBrushFwBwCtrl == Define.CTRL_HOLD))
            {
                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_HOME) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_CYLINDER_FwdBwd_Home();
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_CYLINDER_FwdBwd_HomeEnd();
                        }
                        break;                    
                }
            }
        }

        private void FWD_Progress()
        {
            if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_FWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_FWDING;
                step.Times = 1;

                Global.EventLog("START THE BRUSH FORWARD.", ModuleName, "Event");
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_FWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_HOLD))
            {
                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_FWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_CYLINDER_FwdBwd("Forward");
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_CYLINDER_FwdBwd_FwdEnd();
                        }
                        break;
                }
            }
        }

        private void BWD_Progress()
        {
            if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_BWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
                Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_BWDING;
                step.Times = 1;

                Global.EventLog("START THE BRUSH BACKWARD.", ModuleName, "Event");
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_BWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_HOLD))
            {
                Define.seqBrushFwBwCtrl = Define.CTRL_RUNNING;
            }
            else if ((Define.seqBrushFwBwMode == Define.MODE_BRUSH_FWBW_BWD) && (Define.seqBrushFwBwCtrl == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_BRUSH_CYLINDER_FwdBwd("Backward");
                        }
                        break;

                    case 2:
                        {
                            P_BRUSH_CYLINDER_FwdBwd_BwdEnd();
                        }
                        break;
                }
            }
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////
        ///
        // FUNCTION /////////////////////////////////////////////////////////////////////////
        #region FUNCTION
        private void P_BRUSH_CYLINDER_FwdBwd(string FwdBwd)
        {
            if (step.Flag)
            {
                Global.EventLog("Brush cylinder : " + FwdBwd, ModuleName, "Event");                

                if (FwdBwd == "Forward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On")
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);

                        F_HOLD_STEP();
                    }                    
                }
                else if (FwdBwd == "Backward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On")
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);
                        Thread.Sleep(500);
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                        F_HOLD_STEP();
                    }                    
                }                
            }
            else
            {
                if (FwdBwd == "Forward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH1_Brush_Fwd_i) == "On")
                    {
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Brush_FwdBwd_Timeout)
                        {
                            ShowAlarm("1030");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
                else
                {
                    if (Global.GetDigValue((int)DigInputList.CH1_Brush_Bwd_i) == "On")
                    {
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                        Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Brush_FwdBwd_Timeout)
                        {
                            ShowAlarm("1031");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
            }
        }

        private void P_BRUSH_CYLINDER_StepCheck(byte nStep)
        {
            if (step.Flag)
            {
                F_HOLD_STEP();
            }
            else
            {
                step.Flag = true;
                step.Layer = nStep;
            }
        }

        private void P_BRUSH_CYLINDER_FwdBwd_Home()
        {
            if (step.Flag)
            {
                if (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "On")
                {
                    F_INC_STEP();
                }
                else
                {
                    Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);
                    Thread.Sleep(500);
                    Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                    F_HOLD_STEP();
                }                    
            }
            else
            {
                if (Global.GetDigValue((int)DigInputList.CH1_Brush_Home_i) == "On")
                {
                    Global.SetDigValue((int)DigOutputList.CH1_Brush_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                    Global.SetDigValue((int)DigOutputList.CH1_Brush_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                    //Thread.Sleep(500);
                    Task.Delay(500);

                    F_INC_STEP();
                }
                else
                {
                    if (step.Times >= Configure_List.Brush_FwdBwd_Timeout)
                    {
                        ShowAlarm("1032");
                    }
                    else
                    {
                        step.INC_TIMES_10();
                    }
                }
            }
        }

        private void P_BRUSH_CYLINDER_FwdBwd_HomeEnd()
        {
            Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_IDLE;
            Define.seqBrushFwBwCtrl = Define.CTRL_IDLE;
            Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_HOMEEND;            

            Global.EventLog("COMPLETE THE BRUSH CYLINDER HOME.", ModuleName, "Event");           
        }

        private void P_BRUSH_CYLINDER_FwdBwd_FwdEnd()
        {
            Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_IDLE;
            Define.seqBrushFwBwCtrl = Define.CTRL_IDLE;
            Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_FWDEND;

            Global.EventLog("COMPLETE THE BRUSH FORWARD.", ModuleName, "Event");
        }

        private void P_BRUSH_CYLINDER_FwdBwd_BwdEnd()
        {
            Define.seqBrushFwBwMode = Define.MODE_BRUSH_FWBW_IDLE;
            Define.seqBrushFwBwCtrl = Define.CTRL_IDLE;
            Define.seqBrushFwBwSts = Define.STS_BRUSH_FWBW_BWDEND;

            Global.EventLog("COMPLETE THE BRUSH BACKWARD.", ModuleName, "Event");
        }

        private void F_PROCESS_ALL_VALVE_CLOSE()
        {
            // Air
            Global.SetDigValue((int)DigOutputList.CH1_AirValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);
            Global.SetDigValue((int)DigOutputList.CH1_AirValve_Bot_o, (uint)DigitalOffOn.Off, ModuleName);

            // Water
            Global.SetDigValue((int)DigOutputList.CH1_WaterValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);            

            // Curtain air
            //Global.SetDigValue((int)DigOutputList.CH1_Curtain_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////
    }
}
