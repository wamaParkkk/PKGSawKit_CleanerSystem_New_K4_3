using MsSqlManagerLibrary;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PKGSawKit_CleanerSystem_New_K4_3.Squence
{
    class PM2Cylinder : TBaseThread
    {
        Thread thread;
        private new TStep step;
        Alarm_List alarm_List;  // Alarm list               
        private bool bWaitSet;

        public PM2Cylinder()
        {
            ModuleName = "PM2";
            module = (byte)MODULE._PM2;
            
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
                    if (Define.seqCylinderCtrl[module] == Define.CTRL_ABORT)
                    {
                        AlarmAction("Abort");
                    }
                    else if (Define.seqCylinderCtrl[module] == Define.CTRL_RETRY)
                    {
                        AlarmAction("Retry");
                    }
                    else if (Define.seqCylinderCtrl[module] == Define.CTRL_WAIT)
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

                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;

                if (Define.seqCtrl[module] == Define.CTRL_ALARM)
                {
                    Define.seqCtrl[module] = Define.CTRL_RUNNING;
                }
            }
            else if (sAction == "Abort")
            {
                ActionList();
                
                Define.seqCylinderMode[module] = Define.MODE_CYLINDER_IDLE;
                Define.seqCylinderCtrl[module] = Define.CTRL_IDLE;
                Define.seqCylinderSts[module] = Define.STS_CYLINDER_ABORTOK;

                step.Times = 1;                

                Global.EventLog("Cylinder movement stopped : " + sAction, ModuleName, "Event");
            }
            else if (sAction == "Wait")
            {
                if (!bWaitSet)
                {
                    bWaitSet = true;

                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);

                    Global.EventLog("Cylinder movement stopped : " + sAction, ModuleName, "Event");
                }                
            }
        }

        private void ActionList()
        {
            F_PROCESS_ALL_VALVE_CLOSE();
            
            Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
            Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
        }

        private void ShowAlarm(string almId)
        {
            ActionList();

            Define.seqCylinderCtrl[module] = Define.CTRL_ALARM;

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
            Define.seqCylinderCtrl[module] = Define.CTRL_HOLD;
        }

        public void F_INC_STEP()
        {
            step.Flag = true;
            step.Layer++;
            step.Times = 1;
        }

        // CYLINDER PROGRESS ////////////////////////////////////////////////////////////////
        #region CYLINDER_PROGRESS
        private void Run_Progress()
        {
            if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_RUN) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
                Define.seqCylinderSts[module] = Define.STS_CYLINDER_RUNING;                

                Global.EventLog("START THE CYLINDER MOVING.", ModuleName, "Event");
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_RUN) && (Define.seqCylinderCtrl[module] == Define.CTRL_HOLD))
            {
                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_RUN) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {                    
                    case 1:
                        {
                            P_CYLINDER_FwdBwd("Forward");
                        }
                        break;
                        
                    case 2:
                        {
                            P_CYLINDER_FwdBwd("Backward");
                        }
                        break;
                        
                    case 3:
                        {
                            P_CYLINDER_StepCheck(1);
                        }
                        break;                    
                }
            }
        }

        private void Home_Progress()
        {
            if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_HOME) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
                Define.seqCylinderSts[module] = Define.STS_CYLINDER_HOMEING;                

                Global.EventLog("START THE CYLINDER HOME.", ModuleName, "Event");
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_HOME) && (Define.seqCylinderCtrl[module] == Define.CTRL_HOLD))
            {
                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_HOME) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_CYLINDER_FwdBwd_Home();
                        }
                        break;

                    case 2:
                        {
                            P_CYLINDER_FwdBwd_HomeEnd();
                        }
                        break;                    
                }
            }
        }

        private void FWD_Progress()
        {
            if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_FWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
                Define.seqCylinderSts[module] = Define.STS_CYLINDER_FWDING;
                step.Times = 1;

                Global.EventLog("START THE CYLINDER FORWARD.", ModuleName, "Event");
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_FWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_HOLD))
            {
                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_FWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_CYLINDER_FwdBwd("Forward");
                        }
                        break;

                    case 2:
                        {
                            P_CYLINDER_FwdBwd_FwdEnd();
                        }
                        break;
                }
            }
        }

        private void BWD_Progress()
        {
            if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_BWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUN))
            {
                Thread.Sleep(500);
                step.Layer = 1;
                step.Times = 1;
                step.Flag = true;

                bWaitSet = false;

                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
                Define.seqCylinderSts[module] = Define.STS_CYLINDER_BWDING;
                step.Times = 1;

                Global.EventLog("START THE CYLINDER BACKWARD.", ModuleName, "Event");
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_BWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_HOLD))
            {
                Define.seqCylinderCtrl[module] = Define.CTRL_RUNNING;
            }
            else if ((Define.seqCylinderMode[module] == Define.MODE_CYLINDER_BWD) && (Define.seqCylinderCtrl[module] == Define.CTRL_RUNNING))
            {
                switch (step.Layer)
                {
                    case 1:
                        {
                            P_CYLINDER_FwdBwd("Backward");
                        }
                        break;

                    case 2:
                        {
                            P_CYLINDER_FwdBwd_BwdEnd();
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
        private void P_CYLINDER_FwdBwd(string FwdBwd)
        {
            if (step.Flag)
            {
                Global.EventLog("Cylinder : " + FwdBwd, ModuleName, "Event");                

                if (FwdBwd == "Forward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Fwd_i) == "On")
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);                        
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);

                        F_HOLD_STEP();
                    }
                }
                else if (FwdBwd == "Backward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Bwd_i) == "On")
                    {
                        F_INC_STEP();
                    }
                    else
                    {
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);
                        Thread.Sleep(500);
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                        F_HOLD_STEP();
                    }
                }
            }
            else
            {
                if (FwdBwd == "Forward")
                {
                    if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Fwd_i) == "On")
                    {
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Nozzle_FwdBwd_Timeout)
                        {
                            ShowAlarm("1020");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
                else
                {
                    if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Bwd_i) == "On")
                    {
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                        Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                        //Thread.Sleep(500);
                        Task.Delay(500);

                        F_INC_STEP();
                    }
                    else
                    {
                        if (step.Times >= Configure_List.Nozzle_FwdBwd_Timeout)
                        {
                            ShowAlarm("1021");
                        }
                        else
                        {
                            step.INC_TIMES_10();
                        }
                    }
                }
            }
        }

        private void P_CYLINDER_StepCheck(byte nStep)
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

        private void P_CYLINDER_FwdBwd_Home()
        {
            if (step.Flag)
            {
                if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Home_i) == "On")
                {
                    F_INC_STEP();
                }
                else
                {
                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.On, ModuleName);
                    Thread.Sleep(500);
                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.On, ModuleName);

                    F_HOLD_STEP();
                }
            }
            else
            {
                if (Global.GetDigValue((int)DigInputList.CH2_Nozzle_Home_i) == "On")
                {
                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_Pwr_o, (uint)DigitalOffOn.Off, ModuleName);
                    Global.SetDigValue((int)DigOutputList.CH2_Nozzle_FwdBwd_o, (uint)DigitalOffOn.Off, ModuleName);
                    //Thread.Sleep(500);
                    Task.Delay(500);

                    F_INC_STEP();
                }
                else
                {
                    if (step.Times >= Configure_List.Nozzle_FwdBwd_Timeout)
                    {
                        ShowAlarm("1022");
                    }
                    else
                    {
                        step.INC_TIMES_10();
                    }
                }
            }
        }

        private void P_CYLINDER_FwdBwd_HomeEnd()
        {
            Define.seqCylinderMode[module] = Define.MODE_CYLINDER_IDLE;
            Define.seqCylinderCtrl[module] = Define.CTRL_IDLE;
            Define.seqCylinderSts[module] = Define.STS_CYLINDER_HOMEEND;            

            Global.EventLog("COMPLETE THE CYLINDER HOME.", ModuleName, "Event");            
        }

        private void P_CYLINDER_FwdBwd_FwdEnd()
        {
            Define.seqCylinderMode[module] = Define.MODE_CYLINDER_IDLE;
            Define.seqCylinderCtrl[module] = Define.CTRL_IDLE;
            Define.seqCylinderSts[module] = Define.STS_CYLINDER_FWDEND;

            Global.EventLog("COMPLETE THE CYLINDER FORWARD.", ModuleName, "Event");
        }

        private void P_CYLINDER_FwdBwd_BwdEnd()
        {
            Define.seqCylinderMode[module] = Define.MODE_CYLINDER_IDLE;
            Define.seqCylinderCtrl[module] = Define.CTRL_IDLE;
            Define.seqCylinderSts[module] = Define.STS_CYLINDER_BWDEND;

            Global.EventLog("COMPLETE THE CYLINDER BACKWARD.", ModuleName, "Event");
        }

        private void F_PROCESS_ALL_VALVE_CLOSE()
        {
            // Air
            Global.SetDigValue((int)DigOutputList.CH2_AirValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);
            Global.SetDigValue((int)DigOutputList.CH2_AirValve_Bot_o, (uint)DigitalOffOn.Off, ModuleName);

            // Water
            Global.SetDigValue((int)DigOutputList.CH2_WaterValve_Top_o, (uint)DigitalOffOn.Off, ModuleName);            

            // Curtain air
            //Global.SetDigValue((int)DigOutputList.CH2_Curtain_AirValve_o, (uint)DigitalOffOn.Off, ModuleName);
        }
        #endregion
        /////////////////////////////////////////////////////////////////////////////////////
    }
}
